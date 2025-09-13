using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour le système de suggestions intelligentes
    /// </summary>
    public interface IIntelligentSuggestions
    {
        /// <summary>
        /// Génère des suggestions d'autocomplétion basées sur l'historique et le contexte
        /// </summary>
        Task<IEnumerable<SearchSuggestion>> GetAutocompleteSuggestionsAsync(string partialQuery, int maxSuggestions = 10, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Génère des suggestions prédictives basées sur les patterns d'utilisation
        /// </summary>
        Task<IEnumerable<SearchSuggestion>> GetPredictiveSuggestionsAsync(string context, int maxSuggestions = 5, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Génère des recommandations "Vous pourriez aussi chercher..."
        /// </summary>
        Task<IEnumerable<SearchSuggestion>> GetRecommendationsAsync(string currentQuery, int maxSuggestions = 5, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Apprend des habitudes de recherche de l'utilisateur
        /// </summary>
        Task LearnFromSearchAsync(string query, SearchContext context, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les suggestions basées sur l'historique de recherche
        /// </summary>
        Task<IEnumerable<SearchSuggestion>> GetHistoryBasedSuggestionsAsync(int maxSuggestions = 10, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les suggestions basées sur les fichiers récemment ouverts
        /// </summary>
        Task<IEnumerable<SearchSuggestion>> GetRecentFilesSuggestionsAsync(int maxSuggestions = 10, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les suggestions basées sur les patterns de recherche similaires
        /// </summary>
        Task<IEnumerable<SearchSuggestion>> GetSimilarPatternsSuggestionsAsync(string query, int maxSuggestions = 5, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Efface l'historique d'apprentissage
        /// </summary>
        Task ClearLearningDataAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Suggestion de recherche
    /// </summary>
    public class SearchSuggestion
    {
        public string Query { get; set; } = string.Empty;
        public string DisplayText { get; set; } = string.Empty;
        public SuggestionType Type { get; set; }
        public double Confidence { get; set; }
        public string Category { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Type de suggestion
    /// </summary>
    public enum SuggestionType
    {
        Autocomplete,
        Predictive,
        Recommendation,
        History,
        RecentFiles,
        SimilarPattern,
        SmartQuery
    }

    /// <summary>
    /// Contexte de recherche pour l'apprentissage
    /// </summary>
    public class SearchContext
    {
        public string Query { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public Dictionary<string, object> SearchOptions { get; set; } = new();
        public int ResultCount { get; set; }
        public TimeSpan SearchDuration { get; set; }
        public bool WasSuccessful { get; set; }
    }
}
