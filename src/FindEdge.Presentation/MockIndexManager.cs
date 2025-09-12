using System;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;
using FindEdge.Core.Models;
using FindEdge.Infrastructure.Services;

namespace FindEdge.Presentation
{
    /// <summary>
    /// Mock du gestionnaire d'index pour les tests
    /// </summary>
    public class MockIndexManager : IIndexManager
    {
        private IndexConfiguration? _configuration;
        private IndexStatus _status = new();

        public bool IsIndexAvailable => _status.IsAvailable;

        public IndexConfiguration? Configuration => _configuration;

        public event EventHandler<IndexProgressEventArgs>? IndexProgress;
        public event EventHandler<IndexCompletedEventArgs>? IndexCompleted;
        public event EventHandler<IndexErrorEventArgs>? IndexError;

        public async Task BuildIndexAsync(IndexConfiguration configuration, CancellationToken cancellationToken = default)
        {
            _configuration = configuration;
            _status.IsBuilding = true;
            _status.IsAvailable = false;

            try
            {
                // Simuler la construction de l'index
                var totalFiles = 1000; // Simulation
                var processedFiles = 0;

                for (int i = 0; i < totalFiles; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    processedFiles++;
                    
                    // Simuler la progression
                    OnIndexProgress(new IndexProgressEventArgs
                    {
                        DocumentsProcessed = processedFiles,
                        TotalDocuments = totalFiles,
                        CurrentFile = $"Fichier_{i}.txt",
                        ElapsedTime = TimeSpan.FromSeconds(i * 0.1),
                        Speed = 10.0,
                        EstimatedTimeRemaining = TimeSpan.FromSeconds((totalFiles - processedFiles) * 0.1)
                    });

                    // Simuler un délai
                    await Task.Delay(50, cancellationToken);
                }

                // Finaliser l'index
                _status.IsAvailable = true;
                _status.IsBuilding = false;
                _status.DocumentCount = processedFiles;
                _status.IndexSize = processedFiles * 1024; // 1KB par fichier
                _status.LastUpdated = DateTime.UtcNow;
                _status.Created = DateTime.UtcNow;

                OnIndexCompleted(new IndexCompletedEventArgs
                {
                    TotalDocuments = processedFiles,
                    TotalTime = TimeSpan.FromSeconds(processedFiles * 0.1),
                    IndexSize = _status.IndexSize,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                _status.IsBuilding = false;
                OnIndexError(new IndexErrorEventArgs
                {
                    ErrorMessage = ex.Message,
                    Exception = ex
                });
            }
        }

        public async Task<IEnumerable<SearchResult>> SearchIndexAsync(SearchOptions options, CancellationToken cancellationToken = default)
        {
            if (!IsIndexAvailable)
                return Enumerable.Empty<SearchResult>();

            // Simuler une recherche dans l'index
            var results = new List<SearchResult>();
            var random = new Random();

            for (int i = 0; i < Math.Min(50, options.MaxResults); i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var result = new SearchResult
                {
                    FilePath = $"C:\\Documents\\Fichier_{i}.txt",
                    FileName = $"Fichier_{i}.txt",
                    Directory = "C:\\Documents",
                    FileExtension = ".txt",
                    FileSize = random.Next(1024, 10240),
                    LastModified = DateTime.Now.AddDays(-random.Next(0, 30)),
                    Content = $"Contenu du fichier {i} contenant {options.SearchTerm}",
                    RelevanceScore = random.NextDouble() * 100,
                    MatchType = SearchMatchType.Both,
                    MatchCount = random.Next(1, 5)
                };

                results.Add(result);
            }

            return results.OrderByDescending(r => r.RelevanceScore);
        }

        public async Task UpdateIndexAsync(CancellationToken cancellationToken = default)
        {
            if (_configuration == null)
                return;

            // Simuler une mise à jour incrémentale
            await BuildIndexAsync(_configuration, cancellationToken);
        }

        public async Task DeleteIndexAsync()
        {
            _status = new IndexStatus();
            _configuration = null;
        }

        public async Task<IndexStatus> GetIndexStatusAsync()
        {
            return _status;
        }

        private void OnIndexProgress(IndexProgressEventArgs e)
        {
            IndexProgress?.Invoke(this, e);
        }

        private void OnIndexCompleted(IndexCompletedEventArgs e)
        {
            IndexCompleted?.Invoke(this, e);
        }

        private void OnIndexError(IndexErrorEventArgs e)
        {
            IndexError?.Invoke(this, e);
        }
    }
}
