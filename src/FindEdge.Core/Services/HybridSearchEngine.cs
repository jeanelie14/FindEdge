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
    /// Moteur de recherche hybride combinant indexation et live scan
    /// </summary>
    public class HybridSearchEngine : IIndexedSearchEngine
    {
        private readonly ISearchEngine _liveSearchEngine;
        private readonly IIndexManager _indexManager;
        private SearchMode _currentSearchMode = SearchMode.Hybrid;
        private readonly SearchPerformanceStats _performanceStats = new();

        public HybridSearchEngine(ISearchEngine liveSearchEngine, IIndexManager indexManager)
        {
            _liveSearchEngine = liveSearchEngine ?? throw new ArgumentNullException(nameof(liveSearchEngine));
            _indexManager = indexManager ?? throw new ArgumentNullException(nameof(indexManager));

            // S'abonner aux événements du moteur live
            _liveSearchEngine.SearchProgress += OnLiveSearchProgress;
            _liveSearchEngine.ResultFound += OnLiveResultFound;
        }

        public SearchMode CurrentSearchMode
        {
            get => _currentSearchMode;
            set => _currentSearchMode = value;
        }

        public IIndexManager IndexManager => _indexManager;

        public event EventHandler<SearchProgressEventArgs>? SearchProgress;
        public event EventHandler<SearchResultEventArgs>? ResultFound;

        public async Task<IEnumerable<SearchResult>> SearchAsync(SearchOptions options, CancellationToken cancellationToken = default)
        {
            return _currentSearchMode switch
            {
                SearchMode.Hybrid => await SearchHybridAsync(options, cancellationToken),
                SearchMode.IndexOnly => await SearchIndexOnlyAsync(options, cancellationToken),
                SearchMode.LiveOnly => await SearchLiveOnlyAsync(options, cancellationToken),
                _ => await SearchHybridAsync(options, cancellationToken)
            };
        }

        public IEnumerable<SearchResult> Search(SearchOptions options)
        {
            return SearchAsync(options).GetAwaiter().GetResult();
        }

        public async Task<IEnumerable<SearchResult>> SearchHybridAsync(SearchOptions options, CancellationToken cancellationToken = default)
        {
            var startTime = DateTime.UtcNow;
            var results = new List<SearchResult>();

            try
            {
                // Recherche dans l'index en premier (plus rapide)
                if (_indexManager.IsIndexAvailable)
                {
                    var indexResults = await _indexManager.SearchIndexAsync(options, cancellationToken);
                    results.AddRange(indexResults);
                }

                // Si pas assez de résultats ou index non disponible, compléter avec live scan
                if (results.Count < options.MaxResults && _currentSearchMode != SearchMode.IndexOnly)
                {
                    var liveResults = await _liveSearchEngine.SearchAsync(options, cancellationToken);
                    
                    // Fusionner les résultats en évitant les doublons
                    var existingPaths = new HashSet<string>(results.Select(r => r.FilePath));
                    var additionalResults = liveResults.Where(r => !existingPaths.Contains(r.FilePath));
                    
                    results.AddRange(additionalResults);
                }

                // Trier par score de pertinence
                results = results.OrderByDescending(r => r.RelevanceScore).ToList();

                _performanceStats.HybridSearchCount++;
                _performanceStats.AverageHybridSearchTime = CalculateAverageTime(_performanceStats.AverageHybridSearchTime, _performanceStats.HybridSearchCount, DateTime.UtcNow - startTime);
            }
            catch (Exception ex)
            {
                // En cas d'erreur, fallback vers live scan uniquement
                results = (await SearchLiveOnlyAsync(options, cancellationToken)).ToList();
            }

            return results;
        }

        public async Task<IEnumerable<SearchResult>> SearchIndexOnlyAsync(SearchOptions options, CancellationToken cancellationToken = default)
        {
            var startTime = DateTime.UtcNow;

            if (!_indexManager.IsIndexAvailable)
            {
                throw new InvalidOperationException("L'index n'est pas disponible. Veuillez construire l'index d'abord.");
            }

            var results = await _indexManager.SearchIndexAsync(options, cancellationToken);
            
            _performanceStats.IndexSearchCount++;
            _performanceStats.AverageIndexSearchTime = CalculateAverageTime(_performanceStats.AverageIndexSearchTime, _performanceStats.IndexSearchCount, DateTime.UtcNow - startTime);

            return results;
        }

        public async Task<IEnumerable<SearchResult>> SearchLiveOnlyAsync(SearchOptions options, CancellationToken cancellationToken = default)
        {
            var startTime = DateTime.UtcNow;

            var results = await _liveSearchEngine.SearchAsync(options, cancellationToken);
            
            _performanceStats.LiveSearchCount++;
            _performanceStats.AverageLiveSearchTime = CalculateAverageTime(_performanceStats.AverageLiveSearchTime, _performanceStats.LiveSearchCount, DateTime.UtcNow - startTime);

            return results;
        }

        public void SwitchSearchMode(SearchMode mode)
        {
            _currentSearchMode = mode;
        }

        public SearchPerformanceStats GetPerformanceStats()
        {
            var status = _indexManager.GetIndexStatusAsync().GetAwaiter().GetResult();
            
            _performanceStats.TotalIndexedDocuments = status.DocumentCount;
            _performanceStats.TotalIndexSize = status.IndexSize;
            _performanceStats.LastIndexUpdate = status.LastUpdated;

            return _performanceStats;
        }

        private TimeSpan CalculateAverageTime(TimeSpan currentAverage, int count, TimeSpan newTime)
        {
            if (count == 1)
                return newTime;

            var totalTicks = (currentAverage.Ticks * (count - 1) + newTime.Ticks) / count;
            return TimeSpan.FromTicks(totalTicks);
        }

        private void OnLiveSearchProgress(object? sender, SearchProgressEventArgs e)
        {
            SearchProgress?.Invoke(this, e);
        }

        private void OnLiveResultFound(object? sender, SearchResultEventArgs e)
        {
            ResultFound?.Invoke(this, e);
        }
    }
}
