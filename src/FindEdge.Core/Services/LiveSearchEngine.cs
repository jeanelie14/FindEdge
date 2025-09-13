using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;
using FindEdge.Core.Models;
using CoreFileInfo = FindEdge.Core.Interfaces.FileInfo;

namespace FindEdge.Core.Services
{
    /// <summary>
    /// Moteur de recherche en temps réel (sans indexation)
    /// </summary>
    public class LiveSearchEngine : ISearchEngine
    {
        private readonly IContentParserRegistry _parserRegistry;
        private readonly IFileScanner _fileScanner;

        public LiveSearchEngine(IContentParserRegistry parserRegistry)
        {
            _parserRegistry = parserRegistry ?? throw new ArgumentNullException(nameof(parserRegistry));
            _fileScanner = new MockFileScanner(); // Utiliser un mock pour l'instant
        }

        public event EventHandler<SearchProgressEventArgs>? SearchProgress;
        public event EventHandler<SearchResultEventArgs>? ResultFound;

        public IEnumerable<SearchResult> Search(SearchOptions options)
        {
            return SearchAsync(options, CancellationToken.None).Result;
        }

        public async Task<IEnumerable<SearchResult>> SearchAsync(SearchOptions options, CancellationToken cancellationToken = default)
        {
            var results = new List<SearchResult>();
            
            try
            {
                // Scanner les répertoires spécifiés
                var directories = options.IncludeDirectories.Any() 
                    ? options.IncludeDirectories.ToArray()
                    : new[] { Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) };

                foreach (var directory in directories)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var files = await _fileScanner.ScanDirectoryAsync(directory, options, cancellationToken);
                    
                    foreach (var file in files)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        var result = await ProcessFileAsync(file, options, cancellationToken);
                        if (result != null)
                        {
                            results.Add(result);
                            OnResultFound(new SearchResultEventArgs { Result = result });
                        }

                        // Déclencher l'événement de progression
                        OnSearchProgress(new SearchProgressEventArgs 
                        { 
                            FilesProcessed = results.Count,
                            CurrentFile = file.FullPath
                        });
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // L'opération a été annulée
            }
            catch (Exception)
            {
                // Log l'erreur
            }

            // Recherche terminée

            return results;
        }

        private async Task<SearchResult?> ProcessFileAsync(CoreFileInfo file, SearchOptions options, CancellationToken cancellationToken)
        {
            try
            {
                var result = new SearchResult
                {
                    FilePath = file.FullPath,
                    FileName = file.Name,
                    Directory = file.Directory,
                    FileSize = file.Size,
                    LastModified = file.LastModified,
                    FileExtension = file.Extension,
                    MatchType = SearchMatchType.FileName,
                    RelevanceScore = 0.0
                };

                // Vérifier la correspondance du nom de fichier
                if (options.SearchInFileName && IsFileNameMatch(file.Name, options.SearchTerm, options))
                {
                    result.MatchType = SearchMatchType.FileName;
                    result.RelevanceScore = CalculateRelevanceScore(file.Name, options.SearchTerm, options);
                    return result;
                }

                // Vérifier la correspondance du contenu
                if (options.SearchInContent)
                {
                    var parser = _parserRegistry.GetParser(file.FullPath);
                    if (parser != null)
                    {
                        var content = await parser.ExtractContentAsync(file.FullPath, cancellationToken);
                        if (IsContentMatch(content, options.SearchTerm, options))
                        {
                            result.Content = content;
                            result.MatchType = result.MatchType == SearchMatchType.FileName ? SearchMatchType.Both : SearchMatchType.Content;
                            result.RelevanceScore = Math.Max(result.RelevanceScore, CalculateRelevanceScore(content, options.SearchTerm, options));
                            return result;
                        }
                    }
                }

                return null;
            }
            catch (Exception)
            {
                // Ignorer les erreurs de traitement de fichier
                return null;
            }
        }

        private bool IsFileNameMatch(string fileName, string searchTerm, SearchOptions options)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(searchTerm))
                return false;

            var fileNameToSearch = options.CaseSensitive ? fileName : fileName.ToLowerInvariant();
            var termToSearch = options.CaseSensitive ? searchTerm : searchTerm.ToLowerInvariant();

            if (options.UseRegex)
            {
                try
                {
                    var regex = new System.Text.RegularExpressions.Regex(termToSearch, 
                        options.CaseSensitive ? System.Text.RegularExpressions.RegexOptions.None : System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    return regex.IsMatch(fileNameToSearch);
                }
                catch
                {
                    return false;
                }
            }

            if (options.WholeWord)
            {
                return fileNameToSearch.Split(' ', '.', '-', '_').Contains(termToSearch);
            }

            return fileNameToSearch.Contains(termToSearch);
        }

        private bool IsContentMatch(string content, string searchTerm, SearchOptions options)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(searchTerm))
                return false;

            var contentToSearch = options.CaseSensitive ? content : content.ToLowerInvariant();
            var termToSearch = options.CaseSensitive ? searchTerm : searchTerm.ToLowerInvariant();

            if (options.UseRegex)
            {
                try
                {
                    var regex = new System.Text.RegularExpressions.Regex(termToSearch, 
                        options.CaseSensitive ? System.Text.RegularExpressions.RegexOptions.None : System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    return regex.IsMatch(contentToSearch);
                }
                catch
                {
                    return false;
                }
            }

            if (options.WholeWord)
            {
                return contentToSearch.Split(' ', '\n', '\r', '\t', '.', ',', ';', ':', '!', '?').Contains(termToSearch);
            }

            return contentToSearch.Contains(termToSearch);
        }

        private double CalculateRelevanceScore(string text, string searchTerm, SearchOptions options)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(searchTerm))
                return 0.0;

            var textToSearch = options.CaseSensitive ? text : text.ToLowerInvariant();
            var termToSearch = options.CaseSensitive ? searchTerm : searchTerm.ToLowerInvariant();

            // Score basé sur la fréquence et la position
            var occurrences = textToSearch.Split(new[] { termToSearch }, StringSplitOptions.None).Length - 1;
            var position = textToSearch.IndexOf(termToSearch);
            
            var frequencyScore = Math.Min(occurrences * 0.1, 1.0);
            var positionScore = position == 0 ? 1.0 : Math.Max(0.1, 1.0 - (position / (double)textToSearch.Length));
            
            return (frequencyScore + positionScore) / 2.0;
        }

        private void OnSearchProgress(SearchProgressEventArgs e)
        {
            SearchProgress?.Invoke(this, e);
        }

        private void OnResultFound(SearchResultEventArgs e)
        {
            ResultFound?.Invoke(this, e);
        }

    }

    /// <summary>
    /// Mock du scanner de fichiers pour LiveSearchEngine
    /// </summary>
    public class MockFileScanner : IFileScanner
    {
        public event EventHandler<FileFoundEventArgs>? FileFound
        {
            add { /* Mock implementation - event not used */ }
            remove { /* Mock implementation - event not used */ }
        }

        public async Task<IEnumerable<CoreFileInfo>> ScanDirectoryAsync(string directoryPath, SearchOptions options, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken); // Simuler un délai
            
            var files = new List<CoreFileInfo>();
            
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    var directoryFiles = Directory.GetFiles(directoryPath, "*", SearchOption.TopDirectoryOnly);
                    
                    foreach (var filePath in directoryFiles.Take(100)) // Limiter à 100 fichiers pour les tests
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        var fileInfo = new System.IO.FileInfo(filePath);
                        var coreFileInfo = new CoreFileInfo
                        {
                            FullPath = fileInfo.FullName,
                            Name = fileInfo.Name,
                            Directory = fileInfo.DirectoryName ?? string.Empty,
                            Extension = fileInfo.Extension,
                            Size = fileInfo.Length,
                            LastModified = fileInfo.LastWriteTime,
                            Attributes = fileInfo.Attributes
                        };

                        files.Add(coreFileInfo);
                    }
                }
            }
            catch (Exception)
            {
                // Ignorer les erreurs
            }

            return files;
        }

        public bool IsFileMatch(CoreFileInfo fileInfo, SearchOptions options)
        {
            return true; // Accepter tous les fichiers pour les tests
        }
    }
}
