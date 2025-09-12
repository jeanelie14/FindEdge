using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;
using FindEdge.Core.Models;

namespace FindEdge.Infrastructure.Services
{
    /// <summary>
    /// Gestionnaire d'index simple utilisant des fichiers JSON
    /// </summary>
    public class SimpleIndexManager : IIndexManager
    {
        private readonly IFileScanner _fileScanner;
        private readonly IContentParserRegistry _parserRegistry;
        private IndexConfiguration? _configuration;
        private List<IndexedFile> _indexedFiles = new();
        private readonly object _lockObject = new();

        public SimpleIndexManager(IFileScanner fileScanner, IContentParserRegistry parserRegistry)
        {
            _fileScanner = fileScanner ?? throw new ArgumentNullException(nameof(fileScanner));
            _parserRegistry = parserRegistry ?? throw new ArgumentNullException(nameof(parserRegistry));
        }

        public bool IsIndexAvailable => _indexedFiles.Any();

        public IndexConfiguration? Configuration => _configuration;

        public event EventHandler<IndexProgressEventArgs>? IndexProgress;
        public event EventHandler<IndexCompletedEventArgs>? IndexCompleted;
        public event EventHandler<IndexErrorEventArgs>? IndexError;

        public async Task BuildIndexAsync(IndexConfiguration configuration, CancellationToken cancellationToken = default)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            try
            {
                // Créer le répertoire d'index s'il n'existe pas
                Directory.CreateDirectory(configuration.IndexPath);

                var startTime = DateTime.UtcNow;
                var totalFiles = 0;
                var processedFiles = 0;

                _indexedFiles.Clear();

                // Indexer chaque répertoire
                foreach (var directoryPath in configuration.IndexedDirectories)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var files = await _fileScanner.ScanDirectoryAsync(directoryPath, CreateSearchOptions(configuration), cancellationToken);
                    var fileList = files.ToList();
                    totalFiles += fileList.Count;

                    foreach (var file in fileList)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        try
                        {
                            var indexedFile = await IndexFileAsync(file, configuration, cancellationToken);
                            if (indexedFile != null)
                            {
                                _indexedFiles.Add(indexedFile);
                            }

                            processedFiles++;

                            // Notifier la progression
                            OnIndexProgress(new IndexProgressEventArgs
                            {
                                DocumentsProcessed = processedFiles,
                                TotalDocuments = totalFiles,
                                CurrentFile = file.FullPath,
                                ElapsedTime = DateTime.UtcNow - startTime,
                                Speed = CalculateSpeed(processedFiles, startTime),
                                EstimatedTimeRemaining = CalculateEstimatedTime(processedFiles, totalFiles, startTime)
                            });
                        }
                        catch (Exception ex)
                        {
                            OnIndexError(new IndexErrorEventArgs
                            {
                                ErrorMessage = $"Erreur lors de l'indexation de {file.FullPath}: {ex.Message}",
                                Exception = ex,
                                FilePath = file.FullPath
                            });
                        }
                    }
                }

                // Sauvegarder l'index
                await SaveIndexAsync();

                var totalTime = DateTime.UtcNow - startTime;
                OnIndexCompleted(new IndexCompletedEventArgs
                {
                    TotalDocuments = processedFiles,
                    TotalTime = totalTime,
                    IndexSize = GetIndexSize(),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                OnIndexError(new IndexErrorEventArgs
                {
                    ErrorMessage = $"Erreur lors de la construction de l'index: {ex.Message}",
                    Exception = ex
                });
            }
        }

        public async Task<IEnumerable<SearchResult>> SearchIndexAsync(SearchOptions options, CancellationToken cancellationToken = default)
        {
            if (!IsIndexAvailable)
                return Enumerable.Empty<SearchResult>();

            var results = new List<SearchResult>();

            try
            {
                var searchTerm = options.SearchTerm.ToLowerInvariant();
                var resultsFound = 0;

                foreach (var indexedFile in _indexedFiles)
                {
                    if (cancellationToken.IsCancellationRequested || resultsFound >= options.MaxResults)
                        break;

                    var result = await SearchInIndexedFileAsync(indexedFile, options, searchTerm, cancellationToken);
                    if (result != null)
                    {
                        results.Add(result);
                        resultsFound++;
                    }
                }
            }
            catch (Exception ex)
            {
                OnIndexError(new IndexErrorEventArgs
                {
                    ErrorMessage = $"Erreur lors de la recherche dans l'index: {ex.Message}",
                    Exception = ex
                });
            }

            return results.OrderByDescending(r => r.RelevanceScore);
        }

        public async Task UpdateIndexAsync(CancellationToken cancellationToken = default)
        {
            if (_configuration == null)
                return;

            // Pour l'instant, reconstruction complète
            await BuildIndexAsync(_configuration, cancellationToken);
        }

        public async Task DeleteIndexAsync()
        {
            try
            {
                lock (_lockObject)
                {
                    _indexedFiles.Clear();
                }

                if (_configuration != null && Directory.Exists(_configuration.IndexPath))
                {
                    Directory.Delete(_configuration.IndexPath, true);
                }
            }
            catch (Exception ex)
            {
                OnIndexError(new IndexErrorEventArgs
                {
                    ErrorMessage = $"Erreur lors de la suppression de l'index: {ex.Message}",
                    Exception = ex
                });
            }
        }

        public async Task<IndexStatus> GetIndexStatusAsync()
        {
            var status = new IndexStatus
            {
                IsAvailable = IsIndexAvailable,
                IsBuilding = false,
                Version = "1.0"
            };

            if (IsIndexAvailable)
            {
                status.DocumentCount = _indexedFiles.Count;
                status.IndexSize = GetIndexSize();
                status.LastUpdated = DateTime.UtcNow; // TODO: Récupérer la vraie date
                status.Created = DateTime.UtcNow; // TODO: Récupérer la vraie date
            }

            return status;
        }

        private async Task<IndexedFile?> IndexFileAsync(FindEdge.Core.Interfaces.FileInfo fileInfo, IndexConfiguration configuration, CancellationToken cancellationToken)
        {
            try
            {
                var indexedFile = new IndexedFile
                {
                    FilePath = fileInfo.FullPath,
                    FileName = fileInfo.Name,
                    Directory = fileInfo.Directory,
                    Extension = fileInfo.Extension,
                    Size = fileInfo.Size,
                    LastModified = fileInfo.LastModified,
                    IndexedAt = DateTime.UtcNow
                };

                // Indexer le contenu si nécessaire
                if (configuration.IndexContent)
                {
                    var parser = _parserRegistry.GetParser(fileInfo.FullPath);
                    if (parser != null)
                    {
                        try
                        {
                            var content = await parser.ExtractContentAsync(fileInfo.FullPath, cancellationToken);
                            if (!string.IsNullOrEmpty(content))
                            {
                                // Limiter la taille du contenu
                                if (content.Length > configuration.MaxContentLength)
                                {
                                    content = content.Substring(0, configuration.MaxContentLength) + "...";
                                }
                                indexedFile.Content = content;
                            }
                        }
                        catch (Exception)
                        {
                            // Ignorer les erreurs de parsing
                        }
                    }
                }

                return indexedFile;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<SearchResult?> SearchInIndexedFileAsync(IndexedFile indexedFile, SearchOptions options, string searchTerm, CancellationToken cancellationToken)
        {
            var hasFileNameMatch = false;
            var hasContentMatch = false;
            var relevanceScore = 0.0;

            // Recherche dans le nom de fichier
            if (options.SearchInFileName)
            {
                var fileName = options.CaseSensitive ? indexedFile.FileName : indexedFile.FileName.ToLowerInvariant();
                if (fileName.Contains(searchTerm))
                {
                    hasFileNameMatch = true;
                    relevanceScore += 100;
                }
            }

            // Recherche dans le contenu
            if (options.SearchInContent && !string.IsNullOrEmpty(indexedFile.Content))
            {
                var content = options.CaseSensitive ? indexedFile.Content : indexedFile.Content.ToLowerInvariant();
                if (content.Contains(searchTerm))
                {
                    hasContentMatch = true;
                    relevanceScore += 50;
                    
                    // Bonus pour la fréquence
                    var frequency = content.Split(searchTerm).Length - 1;
                    relevanceScore += frequency * 10;
                }
            }

            if (!hasFileNameMatch && !hasContentMatch)
                return null;

            return new SearchResult
            {
                FilePath = indexedFile.FilePath,
                FileName = indexedFile.FileName,
                Directory = indexedFile.Directory,
                FileExtension = indexedFile.Extension,
                FileSize = indexedFile.Size,
                LastModified = indexedFile.LastModified,
                Content = indexedFile.Content ?? string.Empty,
                MatchType = hasFileNameMatch && hasContentMatch ? SearchMatchType.Both : 
                           hasFileNameMatch ? SearchMatchType.FileName : SearchMatchType.Content,
                RelevanceScore = relevanceScore
            };
        }

        private SearchOptions CreateSearchOptions(IndexConfiguration configuration)
        {
            return new SearchOptions
            {
                IncludeExtensions = configuration.IndexedExtensions,
                ExcludeExtensions = configuration.ExcludedExtensions,
                MaxFileSize = configuration.MaxFileSize,
                IncludeHiddenFiles = configuration.IncludeHiddenFiles,
                IncludeSystemFiles = configuration.IncludeSystemFiles
            };
        }

        private async Task SaveIndexAsync()
        {
            if (_configuration == null)
                return;

            var indexPath = Path.Combine(_configuration.IndexPath, "index.json");
            var json = System.Text.Json.JsonSerializer.Serialize(_indexedFiles, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(indexPath, json);
        }

        private long GetIndexSize()
        {
            if (_configuration == null || !Directory.Exists(_configuration.IndexPath))
                return 0;

            var directoryInfo = new System.IO.DirectoryInfo(_configuration.IndexPath);
            return directoryInfo.GetFiles("*", SearchOption.AllDirectories)
                .Sum(file => file.Length);
        }

        private double CalculateSpeed(int processedFiles, DateTime startTime)
        {
            var elapsed = DateTime.UtcNow - startTime;
            return elapsed.TotalSeconds > 0 ? processedFiles / elapsed.TotalSeconds : 0;
        }

        private TimeSpan CalculateEstimatedTime(int processedFiles, int totalFiles, DateTime startTime)
        {
            if (processedFiles == 0)
                return TimeSpan.Zero;

            var elapsed = DateTime.UtcNow - startTime;
            var remainingFiles = totalFiles - processedFiles;
            var averageTimePerFile = elapsed.TotalMilliseconds / processedFiles;

            return TimeSpan.FromMilliseconds(remainingFiles * averageTimePerFile);
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

    /// <summary>
    /// Fichier indexé
    /// </summary>
    public class IndexedFile
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Directory { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime IndexedAt { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
