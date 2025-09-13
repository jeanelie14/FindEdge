using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;
using FindEdge.Core.Models;

namespace FindEdge.Infrastructure.Services
{
    /// <summary>
    /// Service de suggestions intelligentes et autocomplétion
    /// </summary>
    public class IntelligentSuggestionsService : IIntelligentSuggestions
    {
        private readonly ISearchEngine _searchEngine;
        private readonly IIndexManager _indexManager;
        private readonly Dictionary<string, List<SearchContext>> _searchHistory;
        private readonly Dictionary<string, List<string>> _recentFiles;
        private readonly Dictionary<string, List<SearchSuggestion>> _suggestionCache;

        public IntelligentSuggestionsService(ISearchEngine searchEngine, IIndexManager indexManager)
        {
            _searchEngine = searchEngine ?? throw new ArgumentNullException(nameof(searchEngine));
            _indexManager = indexManager ?? throw new ArgumentNullException(nameof(indexManager));
            _searchHistory = new Dictionary<string, List<SearchContext>>();
            _recentFiles = new Dictionary<string, List<string>>();
            _suggestionCache = new Dictionary<string, List<SearchSuggestion>>();
        }

        public async Task<IEnumerable<SearchSuggestion>> GetAutocompleteSuggestionsAsync(string partialQuery, int maxSuggestions = 10, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(partialQuery))
                return Enumerable.Empty<SearchSuggestion>();

            var suggestions = new List<SearchSuggestion>();

            // Suggestions basées sur l'historique
            var historySuggestions = await GetHistoryBasedSuggestionsAsync(maxSuggestions / 2, cancellationToken);
            suggestions.AddRange(historySuggestions.Where(s => s.Query.StartsWith(partialQuery, StringComparison.OrdinalIgnoreCase)));

            // Suggestions basées sur les fichiers récents
            var recentSuggestions = await GetRecentFilesSuggestionsAsync(maxSuggestions / 2, cancellationToken);
            suggestions.AddRange(recentSuggestions.Where(s => s.Query.StartsWith(partialQuery, StringComparison.OrdinalIgnoreCase)));

            // Suggestions basées sur les patterns similaires
            var patternSuggestions = await GetSimilarPatternsSuggestionsAsync(partialQuery, maxSuggestions / 2, cancellationToken);
            suggestions.AddRange(patternSuggestions);

            // Déduplication et tri par pertinence
            return suggestions
                .GroupBy(s => s.Query)
                .Select(g => g.OrderByDescending(s => s.Confidence).First())
                .OrderByDescending(s => s.Confidence)
                .Take(maxSuggestions);
        }

        public async Task<IEnumerable<SearchSuggestion>> GetPredictiveSuggestionsAsync(string context, int maxSuggestions = 5, CancellationToken cancellationToken = default)
        {
            var suggestions = new List<SearchSuggestion>();

            // Analyse du contexte pour prédire les besoins
            var contextWords = context.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            // Suggestions basées sur les patterns d'utilisation
            var patterns = await AnalyzeUsagePatternsAsync(context, cancellationToken);
            suggestions.AddRange(patterns.Take(maxSuggestions));

            return suggestions.OrderByDescending(s => s.Confidence);
        }

        public async Task<IEnumerable<SearchSuggestion>> GetRecommendationsAsync(string currentQuery, int maxSuggestions = 5, CancellationToken cancellationToken = default)
        {
            var recommendations = new List<SearchSuggestion>();

            // Analyse de la requête actuelle
            var queryWords = currentQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            // Recommandations basées sur des requêtes similaires
            var similarQueries = await FindSimilarQueriesAsync(currentQuery, cancellationToken);
            recommendations.AddRange(similarQueries.Take(maxSuggestions));

            return recommendations.OrderByDescending(s => s.Confidence);
        }

        public async Task LearnFromSearchAsync(string query, SearchContext context, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query) || context == null)
                return;

            // Ajouter à l'historique de recherche
            if (!_searchHistory.ContainsKey(context.UserId))
                _searchHistory[context.UserId] = new List<SearchContext>();

            _searchHistory[context.UserId].Add(context);

            // Limiter l'historique à 1000 entrées par utilisateur
            if (_searchHistory[context.UserId].Count > 1000)
            {
                _searchHistory[context.UserId] = _searchHistory[context.UserId]
                    .OrderByDescending(c => c.Timestamp)
                    .Take(1000)
                    .ToList();
            }

            // Mettre à jour le cache des suggestions
            await UpdateSuggestionCacheAsync(context.UserId, cancellationToken);

            await Task.CompletedTask;
        }

        public async Task<IEnumerable<SearchSuggestion>> GetHistoryBasedSuggestionsAsync(int maxSuggestions = 10, CancellationToken cancellationToken = default)
        {
            var suggestions = new List<SearchSuggestion>();

            // Récupérer l'historique de tous les utilisateurs
            var allHistory = _searchHistory.Values.SelectMany(h => h).ToList();

            // Grouper par requête et calculer la fréquence
            var queryFrequency = allHistory
                .GroupBy(c => c.Query)
                .Select(g => new { Query = g.Key, Count = g.Count(), LastUsed = g.Max(c => c.Timestamp) })
                .OrderByDescending(x => x.Count)
                .ThenByDescending(x => x.LastUsed)
                .Take(maxSuggestions);

            foreach (var item in queryFrequency)
            {
                suggestions.Add(new SearchSuggestion
                {
                    Query = item.Query,
                    DisplayText = item.Query,
                    Type = SuggestionType.History,
                    Confidence = Math.Min(1.0, item.Count / 10.0),
                    Category = "Historique",
                    Metadata = new Dictionary<string, object>
                    {
                        ["Frequency"] = item.Count,
                        ["LastUsed"] = item.LastUsed
                    }
                });
            }

            return suggestions;
        }

        public async Task<IEnumerable<SearchSuggestion>> GetRecentFilesSuggestionsAsync(int maxSuggestions = 10, CancellationToken cancellationToken = default)
        {
            var suggestions = new List<SearchSuggestion>();

            // Récupérer les fichiers récents de tous les utilisateurs
            var allRecentFiles = _recentFiles.Values.SelectMany(f => f).ToList();

            // Grouper par nom de fichier et calculer la fréquence
            var fileFrequency = allRecentFiles
                .GroupBy(f => f)
                .Select(g => new { FileName = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(maxSuggestions);

            foreach (var item in fileFrequency)
            {
                suggestions.Add(new SearchSuggestion
                {
                    Query = item.FileName,
                    DisplayText = $"Fichier récent: {item.FileName}",
                    Type = SuggestionType.RecentFiles,
                    Confidence = Math.Min(1.0, item.Count / 5.0),
                    Category = "Fichiers récents",
                    Metadata = new Dictionary<string, object>
                    {
                        ["Frequency"] = item.Count,
                        ["FileType"] = "Recent"
                    }
                });
            }

            return suggestions;
        }

        public async Task<IEnumerable<SearchSuggestion>> GetSimilarPatternsSuggestionsAsync(string query, int maxSuggestions = 5, CancellationToken cancellationToken = default)
        {
            var suggestions = new List<SearchSuggestion>();

            if (string.IsNullOrWhiteSpace(query))
                return suggestions;

            // Analyser les patterns similaires dans l'historique
            var similarPatterns = await FindSimilarPatternsAsync(query, cancellationToken);
            suggestions.AddRange(similarPatterns.Take(maxSuggestions));

            return suggestions.OrderByDescending(s => s.Confidence);
        }

        public async Task ClearLearningDataAsync(CancellationToken cancellationToken = default)
        {
            _searchHistory.Clear();
            _recentFiles.Clear();
            _suggestionCache.Clear();
            await Task.CompletedTask;
        }

        private async Task<List<SearchSuggestion>> AnalyzeUsagePatternsAsync(string context, CancellationToken cancellationToken)
        {
            var suggestions = new List<SearchSuggestion>();

            // Analyser les patterns d'utilisation basés sur le contexte
            var contextWords = context.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            // Rechercher des patterns similaires dans l'historique
            var similarContexts = _searchHistory.Values
                .SelectMany(h => h)
                .Where(c => c.Query.Contains(context, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(c => c.Timestamp)
                .Take(10);

            foreach (var similarContext in similarContexts)
            {
                suggestions.Add(new SearchSuggestion
                {
                    Query = similarContext.Query,
                    DisplayText = $"Pattern similaire: {similarContext.Query}",
                    Type = SuggestionType.SimilarPattern,
                    Confidence = CalculateSimilarity(context, similarContext.Query),
                    Category = "Patterns d'utilisation",
                    Metadata = new Dictionary<string, object>
                    {
                        ["OriginalContext"] = context,
                        ["Similarity"] = CalculateSimilarity(context, similarContext.Query)
                    }
                });
            }

            return suggestions;
        }

        private async Task<List<SearchSuggestion>> FindSimilarQueriesAsync(string query, CancellationToken cancellationToken)
        {
            var suggestions = new List<SearchSuggestion>();

            // Rechercher des requêtes similaires dans l'historique
            var similarQueries = _searchHistory.Values
                .SelectMany(h => h)
                .Where(c => !string.Equals(c.Query, query, StringComparison.OrdinalIgnoreCase))
                .Select(c => new { Query = c.Query, Similarity = CalculateSimilarity(query, c.Query) })
                .Where(x => x.Similarity > 0.3)
                .OrderByDescending(x => x.Similarity)
                .Take(10);

            foreach (var similarQuery in similarQueries)
            {
                suggestions.Add(new SearchSuggestion
                {
                    Query = similarQuery.Query,
                    DisplayText = $"Vous pourriez aussi chercher: {similarQuery.Query}",
                    Type = SuggestionType.Recommendation,
                    Confidence = similarQuery.Similarity,
                    Category = "Recommandations",
                    Metadata = new Dictionary<string, object>
                    {
                        ["OriginalQuery"] = query,
                        ["Similarity"] = similarQuery.Similarity
                    }
                });
            }

            return suggestions;
        }

        private async Task<List<SearchSuggestion>> FindSimilarPatternsAsync(string query, CancellationToken cancellationToken)
        {
            var suggestions = new List<SearchSuggestion>();

            // Analyser les patterns de mots dans la requête
            var queryWords = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            // Rechercher des patterns similaires
            var similarPatterns = _searchHistory.Values
                .SelectMany(h => h)
                .Select(c => new { Query = c.Query, Similarity = CalculatePatternSimilarity(query, c.Query) })
                .Where(x => x.Similarity > 0.4)
                .OrderByDescending(x => x.Similarity)
                .Take(5);

            foreach (var pattern in similarPatterns)
            {
                suggestions.Add(new SearchSuggestion
                {
                    Query = pattern.Query,
                    DisplayText = $"Pattern similaire: {pattern.Query}",
                    Type = SuggestionType.SimilarPattern,
                    Confidence = pattern.Similarity,
                    Category = "Patterns similaires",
                    Metadata = new Dictionary<string, object>
                    {
                        ["OriginalQuery"] = query,
                        ["PatternSimilarity"] = pattern.Similarity
                    }
                });
            }

            return suggestions;
        }

        private async Task UpdateSuggestionCacheAsync(string userId, CancellationToken cancellationToken)
        {
            // Mettre à jour le cache des suggestions pour l'utilisateur
            var userSuggestions = new List<SearchSuggestion>();

            // Ajouter les suggestions d'historique
            var historySuggestions = await GetHistoryBasedSuggestionsAsync(5, cancellationToken);
            userSuggestions.AddRange(historySuggestions);

            // Ajouter les suggestions de fichiers récents
            var recentSuggestions = await GetRecentFilesSuggestionsAsync(5, cancellationToken);
            userSuggestions.AddRange(recentSuggestions);

            _suggestionCache[userId] = userSuggestions;
        }

        private double CalculateSimilarity(string text1, string text2)
        {
            if (string.IsNullOrEmpty(text1) || string.IsNullOrEmpty(text2))
                return 0.0;

            // Implémentation simple de similarité basée sur la distance de Levenshtein
            var distance = LevenshteinDistance(text1, text2);
            var maxLength = Math.Max(text1.Length, text2.Length);
            
            return maxLength == 0 ? 1.0 : 1.0 - (double)distance / maxLength;
        }

        private double CalculatePatternSimilarity(string query1, string query2)
        {
            var words1 = query1.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var words2 = query2.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var commonWords = words1.Intersect(words2, StringComparer.OrdinalIgnoreCase).Count();
            var totalWords = words1.Union(words2, StringComparer.OrdinalIgnoreCase).Count();

            return totalWords == 0 ? 0.0 : (double)commonWords / totalWords;
        }

        private int LevenshteinDistance(string s1, string s2)
        {
            var matrix = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
                matrix[i, 0] = i;

            for (int j = 0; j <= s2.Length; j++)
                matrix[0, j] = j;

            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    var cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }

            return matrix[s1.Length, s2.Length];
        }
    }
}
