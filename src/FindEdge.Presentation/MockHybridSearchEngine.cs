using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;
using FindEdge.Core.Models;

namespace FindEdge.Presentation
{
    /// <summary>
    /// Mock du moteur de recherche hybride pour les tests
    /// </summary>
    public class MockHybridSearchEngine : IIndexedSearchEngine
    {
        private readonly MockSearchEngine _liveSearchEngine;
        private readonly MockIndexManager _indexManager;
        private SearchMode _currentSearchMode = SearchMode.Hybrid;

        public MockHybridSearchEngine()
        {
            _liveSearchEngine = new MockSearchEngine();
            _indexManager = new MockIndexManager();
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
            var results = new List<SearchResult>();

            // Simuler la recherche hybride
            if (_indexManager.IsIndexAvailable)
            {
                var indexResults = await _indexManager.SearchIndexAsync(options, cancellationToken);
                results.AddRange(indexResults);
            }

            // Compléter avec live scan si nécessaire
            if (results.Count < options.MaxResults)
            {
                var liveResults = await _liveSearchEngine.SearchAsync(options, cancellationToken);
                var existingPaths = new HashSet<string>(results.Select(r => r.FilePath));
                var additionalResults = liveResults.Where(r => !existingPaths.Contains(r.FilePath));
                results.AddRange(additionalResults);
            }

            return results.OrderByDescending(r => r.RelevanceScore);
        }

        public async Task<IEnumerable<SearchResult>> SearchIndexOnlyAsync(SearchOptions options, CancellationToken cancellationToken = default)
        {
            if (!_indexManager.IsIndexAvailable)
            {
                throw new InvalidOperationException("L'index n'est pas disponible");
            }

            return await _indexManager.SearchIndexAsync(options, cancellationToken);
        }

        public async Task<IEnumerable<SearchResult>> SearchLiveOnlyAsync(SearchOptions options, CancellationToken cancellationToken = default)
        {
            return await _liveSearchEngine.SearchAsync(options, cancellationToken);
        }

        public void SwitchSearchMode(SearchMode mode)
        {
            _currentSearchMode = mode;
        }

        public SearchPerformanceStats GetPerformanceStats()
        {
            return new SearchPerformanceStats
            {
                AverageIndexSearchTime = TimeSpan.FromMilliseconds(50),
                AverageLiveSearchTime = TimeSpan.FromMilliseconds(200),
                AverageHybridSearchTime = TimeSpan.FromMilliseconds(100),
                IndexSearchCount = 10,
                LiveSearchCount = 5,
                HybridSearchCount = 15,
                TotalIndexedDocuments = _indexManager.IsIndexAvailable ? 1000 : 0,
                LastIndexUpdate = DateTime.Now.AddHours(-1)
            };
        }
    }
}
