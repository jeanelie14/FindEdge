using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;
using FindEdge.Core.Models;

namespace FindEdge.Core.Services
{
    /// <summary>
    /// Moteur de recherche hybride combinant index et scan en direct
    /// </summary>
    public class HybridSearchEngine : IHybridSearchEngine
    {
        private readonly ISearchEngine _searchEngine;
        private readonly IIndexManager _indexManager;
        private HybridSearchMode _currentMode = HybridSearchMode.Adaptive;
        private readonly HybridSearchStats _performanceStats = new();

        public event EventHandler<SearchProgressEventArgs>? SearchProgress;
        public event EventHandler<SearchResultEventArgs>? ResultFound;

        public HybridSearchEngine(ISearchEngine searchEngine, IIndexManager indexManager)
        {
            _searchEngine = searchEngine ?? throw new ArgumentNullException(nameof(searchEngine));
            _indexManager = indexManager ?? throw new ArgumentNullException(nameof(indexManager));
        }

        public async Task<IEnumerable<SearchResult>> SearchAsync(SearchOptions options, CancellationToken cancellationToken = default)
        {
            return await SearchHybridAsync(options, cancellationToken);
        }

        public IEnumerable<SearchResult> Search(SearchOptions options)
        {
            // Implémentation synchrone utilisant la méthode asynchrone
            return SearchAsync(options).GetAwaiter().GetResult();
        }

        public async Task<IEnumerable<SearchResult>> SearchHybridAsync(SearchOptions options, CancellationToken cancellationToken = default)
        {
            var startTime = DateTime.UtcNow;
            var results = new List<SearchResult>();

            try
            {
                switch (_currentMode)
                {
                    case HybridSearchMode.IndexFirst:
                        results = (await SearchIndexFirstAsync(options, cancellationToken)).ToList();
                        break;
                    case HybridSearchMode.LiveFirst:
                        results = (await SearchLiveFirstAsync(options, cancellationToken)).ToList();
                        break;
                    case HybridSearchMode.Parallel:
                        results = (await SearchParallelAsync(options, cancellationToken)).ToList();
                        break;
                    case HybridSearchMode.Adaptive:
                        results = (await SearchAdaptiveAsync(options, cancellationToken)).ToList();
                        break;
                }

                // Trier par score de pertinence
                results = results.OrderByDescending(r => r.RelevanceScore).ToList();

                _performanceStats.HybridSearchCount++;
                _performanceStats.AverageHybridSearchTime = CalculateAverageTime(_performanceStats.AverageHybridSearchTime, _performanceStats.HybridSearchCount, DateTime.UtcNow - startTime);
            }
            catch (Exception)
            {
                // En cas d'erreur, fallback vers live scan uniquement
                results = (await SearchLiveOnlyAsync(options, cancellationToken)).ToList();
            }

            return results;
        }

        public async Task<IEnumerable<SearchResult>> SearchIndexOnlyAsync(SearchOptions options, CancellationToken cancellationToken = default)
        {
            var startTime = DateTime.UtcNow;
            var results = new List<SearchResult>();

            try
            {
                // Utiliser le gestionnaire d'index pour la recherche
                results = (await _indexManager.SearchIndexAsync(options, cancellationToken)).ToList();

                _performanceStats.IndexSearchCount++;
                _performanceStats.AverageIndexSearchTime = CalculateAverageTime(_performanceStats.AverageIndexSearchTime, _performanceStats.IndexSearchCount, DateTime.UtcNow - startTime);
            }
            catch (Exception)
            {
                // En cas d'erreur, retourner une liste vide
                results = new List<SearchResult>();
            }

            return results;
        }

        public async Task<IEnumerable<SearchResult>> SearchLiveOnlyAsync(SearchOptions options, CancellationToken cancellationToken = default)
        {
            var startTime = DateTime.UtcNow;
            var results = new List<SearchResult>();

            try
            {
                // Utiliser le moteur de recherche en direct
                results = (await _searchEngine.SearchAsync(options, cancellationToken)).ToList();

                _performanceStats.LiveSearchCount++;
                _performanceStats.AverageLiveSearchTime = CalculateAverageTime(_performanceStats.AverageLiveSearchTime, _performanceStats.LiveSearchCount, DateTime.UtcNow - startTime);
            }
            catch (Exception)
            {
                // En cas d'erreur, retourner une liste vide
                results = new List<SearchResult>();
            }

            return results;
        }

        public async Task<HybridSearchStats> GetPerformanceStatsAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(1, cancellationToken); // Simuler un délai minimal
            return _performanceStats;
        }

        public async Task ConfigureHybridModeAsync(HybridSearchMode mode, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1, cancellationToken); // Simuler un délai minimal
            _currentMode = mode;
        }

        private async Task<IEnumerable<SearchResult>> SearchIndexFirstAsync(SearchOptions options, CancellationToken cancellationToken)
        {
            var results = await SearchIndexOnlyAsync(options, cancellationToken);
            
            // Si pas assez de résultats, compléter avec une recherche live
            if (results.Count() < options.MaxResults)
            {
                var liveResults = await SearchLiveOnlyAsync(options, cancellationToken);
                results = results.Concat(liveResults).Distinct().Take(options.MaxResults);
            }

            return results;
        }

        private async Task<IEnumerable<SearchResult>> SearchLiveFirstAsync(SearchOptions options, CancellationToken cancellationToken)
        {
            var results = await SearchLiveOnlyAsync(options, cancellationToken);
            
            // Si pas assez de résultats, compléter avec une recherche d'index
            if (results.Count() < options.MaxResults)
            {
                var indexResults = await SearchIndexOnlyAsync(options, cancellationToken);
                results = results.Concat(indexResults).Distinct().Take(options.MaxResults);
            }

            return results;
        }

        private async Task<IEnumerable<SearchResult>> SearchParallelAsync(SearchOptions options, CancellationToken cancellationToken)
        {
            var indexTask = SearchIndexOnlyAsync(options, cancellationToken);
            var liveTask = SearchLiveOnlyAsync(options, cancellationToken);

            await Task.WhenAll(indexTask, liveTask);

            var indexResults = await indexTask;
            var liveResults = await liveTask;

            // Combiner et dédupliquer les résultats
            return indexResults.Concat(liveResults)
                              .GroupBy(r => r.FilePath)
                              .Select(g => g.OrderByDescending(r => r.RelevanceScore).First())
                              .OrderByDescending(r => r.RelevanceScore)
                              .Take(options.MaxResults);
        }

        private async Task<IEnumerable<SearchResult>> SearchAdaptiveAsync(SearchOptions options, CancellationToken cancellationToken)
        {
            // Mode adaptatif : choisir la meilleure stratégie selon les statistiques
            if (_performanceStats.IndexHitRate > _performanceStats.LiveHitRate)
            {
                return await SearchIndexFirstAsync(options, cancellationToken);
            }
            else
            {
                return await SearchLiveFirstAsync(options, cancellationToken);
            }
        }

        private TimeSpan CalculateAverageTime(TimeSpan currentAverage, int count, TimeSpan newTime)
        {
            if (count <= 1) return newTime;
            
            var totalTicks = (currentAverage.Ticks * (count - 1)) + newTime.Ticks;
            return TimeSpan.FromTicks(totalTicks / count);
        }
    }
}