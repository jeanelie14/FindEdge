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
    /// Mock implementation of IIndexedSearchEngine for testing and development
    /// </summary>
    public class MockIndexedSearchEngine : IIndexedSearchEngine
    {
        private SearchMode _currentSearchMode = SearchMode.Hybrid;

        public SearchMode CurrentSearchMode
        {
            get => _currentSearchMode;
            set => _currentSearchMode = value;
        }

        public IIndexManager IndexManager { get; }

        public event EventHandler<SearchProgressEventArgs>? SearchProgress;
        public event EventHandler<SearchResultEventArgs>? ResultFound;

        public MockIndexedSearchEngine()
        {
            // Create a mock index manager
            IndexManager = new MockIndexManager();
        }

        public Task<IEnumerable<SearchResult>> SearchAsync(SearchOptions options, CancellationToken cancellationToken = default)
        {
            // Mock implementation - return empty results
            var mockResults = new List<SearchResult>();
            
            // Simulate some mock results for testing
            if (!string.IsNullOrWhiteSpace(options.SearchTerm))
            {
                mockResults.Add(new SearchResult
                {
                    FilePath = "C:\\Mock\\File1.txt",
                    FileName = "File1.txt",
                    Directory = "C:\\Mock",
                    FileSize = 1024,
                    LastModified = DateTime.UtcNow,
                    FileExtension = ".txt",
                    Content = $"Mock content containing '{options.SearchTerm}'",
                    MatchCount = 1,
                    MatchingLines = new[] { $"This is a mock file containing '{options.SearchTerm}' for testing purposes." },
                    MatchType = SearchMatchType.Content,
                    RelevanceScore = 0.95
                });
            }

            return Task.FromResult<IEnumerable<SearchResult>>(mockResults);
        }

        public IEnumerable<SearchResult> Search(SearchOptions options)
        {
            // Mock implementation - return empty results
            return new List<SearchResult>();
        }

        public Task<IEnumerable<SearchResult>> SearchHybridAsync(SearchOptions options, CancellationToken cancellationToken = default)
        {
            return SearchAsync(options, cancellationToken);
        }

        public Task<IEnumerable<SearchResult>> SearchIndexOnlyAsync(SearchOptions options, CancellationToken cancellationToken = default)
        {
            return SearchAsync(options, cancellationToken);
        }

        public Task<IEnumerable<SearchResult>> SearchLiveOnlyAsync(SearchOptions options, CancellationToken cancellationToken = default)
        {
            return SearchAsync(options, cancellationToken);
        }

        public void SwitchSearchMode(SearchMode mode)
        {
            CurrentSearchMode = mode;
        }

        public SearchPerformanceStats GetPerformanceStats()
        {
            return new SearchPerformanceStats
            {
                AverageIndexSearchTime = TimeSpan.FromMilliseconds(50),
                AverageLiveSearchTime = TimeSpan.FromMilliseconds(100),
                AverageHybridSearchTime = TimeSpan.FromMilliseconds(75),
                IndexSearchCount = 0,
                LiveSearchCount = 0,
                HybridSearchCount = 0,
                TotalIndexSize = 0,
                TotalIndexedDocuments = 0,
                LastIndexUpdate = DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// Mock implementation of IIndexManager for testing
    /// </summary>
    public class MockIndexManager : IIndexManager
    {
        public bool IsIndexAvailable => false;
        public IndexConfiguration? Configuration => new IndexConfiguration
        {
            MaxFileSize = 10 * 1024 * 1024, // 10MB
            IndexedExtensions = new List<string> { ".txt", ".cs", ".xml", ".json" },
            IndexContent = true,
            IndexMetadata = true,
            EnableCompression = false
        };

        public event EventHandler<IndexProgressEventArgs>? IndexProgress;
        public event EventHandler<IndexCompletedEventArgs>? IndexCompleted;
        public event EventHandler<IndexErrorEventArgs>? IndexError;

        public Task BuildIndexAsync(IndexConfiguration configuration, CancellationToken cancellationToken = default)
        {
            // Mock implementation - do nothing
            return Task.CompletedTask;
        }

        public Task<IEnumerable<SearchResult>> SearchIndexAsync(SearchOptions options, CancellationToken cancellationToken = default)
        {
            // Mock implementation - return empty results
            return Task.FromResult<IEnumerable<SearchResult>>(new List<SearchResult>());
        }

        public Task UpdateIndexAsync(CancellationToken cancellationToken = default)
        {
            // Mock implementation - do nothing
            return Task.CompletedTask;
        }

        public Task DeleteIndexAsync()
        {
            // Mock implementation - do nothing
            return Task.CompletedTask;
        }

        public Task<IndexStatus> GetIndexStatusAsync()
        {
            return Task.FromResult(new IndexStatus
            {
                IsAvailable = false,
                DocumentCount = 0,
                IndexSize = 0,
                LastUpdated = DateTime.UtcNow
            });
        }
    }
}
