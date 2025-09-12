using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;
using FindEdge.Core.Models;

namespace FindEdge.Presentation
{
    /// <summary>
    /// Moteur de recherche mock pour les tests et le développement
    /// </summary>
    public class MockSearchEngine : ISearchEngine
    {
        public event EventHandler<SearchProgressEventArgs>? SearchProgress;
        public event EventHandler<SearchResultEventArgs>? ResultFound;

        public async Task<IEnumerable<SearchResult>> SearchAsync(SearchOptions options, CancellationToken cancellationToken = default)
        {
            var results = new List<SearchResult>();
            var startTime = DateTime.UtcNow;

            try
            {
                // Simuler une recherche dans le répertoire Documents
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var files = Directory.GetFiles(documentsPath, "*", SearchOption.AllDirectories)
                    .Take(100) // Limiter pour les tests
                    .ToArray();

                for (int i = 0; i < files.Length; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var filePath = files[i];
                    var fileInfo = new System.IO.FileInfo(filePath);

                    // Simuler la progression
                    OnSearchProgress(new SearchProgressEventArgs
                    {
                        FilesProcessed = i + 1,
                        TotalFiles = files.Length,
                        CurrentFile = filePath,
                        ElapsedTime = DateTime.UtcNow - startTime
                    });

                    // Simuler une correspondance aléatoire
                    if (ShouldIncludeFile(fileInfo, options))
                    {
                        var result = CreateMockResult(fileInfo, options);
                        results.Add(result);
                        OnResultFound(new SearchResultEventArgs { Result = result });
                    }

                    // Simuler un délai de traitement
                    await Task.Delay(50, cancellationToken);
                }
            }
            catch (Exception)
            {
                // Ignorer les erreurs pour les tests
            }

            return results;
        }

        public IEnumerable<SearchResult> Search(SearchOptions options)
        {
            return SearchAsync(options).GetAwaiter().GetResult();
        }

        private bool ShouldIncludeFile(System.IO.FileInfo fileInfo, SearchOptions options)
        {
            // Simuler une correspondance basée sur le nom de fichier
            var fileName = fileInfo.Name.ToLowerInvariant();
            var searchTerm = options.SearchTerm.ToLowerInvariant();
            
            return fileName.Contains(searchTerm) || 
                   fileInfo.Extension.ToLowerInvariant().Contains(searchTerm);
        }

        private SearchResult CreateMockResult(System.IO.FileInfo fileInfo, SearchOptions options)
        {
            var result = new SearchResult
            {
                FilePath = fileInfo.FullName,
                FileName = fileInfo.Name,
                Directory = fileInfo.DirectoryName ?? string.Empty,
                FileSize = fileInfo.Length,
                LastModified = fileInfo.LastWriteTime,
                FileExtension = fileInfo.Extension,
                MatchType = SearchMatchType.FileName,
                RelevanceScore = new Random().NextDouble() * 100,
                MatchCount = 1
            };

            // Simuler du contenu pour les fichiers texte
            if (IsTextFile(fileInfo.Extension))
            {
                result.MatchType = SearchMatchType.Both;
                result.Content = $"Contenu simulé du fichier {fileInfo.Name}...\n" +
                               $"Recherche du terme: {options.SearchTerm}\n" +
                               "Ceci est un exemple de contenu de fichier.";
                result.MatchingLines = new[] { $"Ligne contenant {options.SearchTerm}" };
                result.MatchCount = 2;
            }

            return result;
        }

        private bool IsTextFile(string extension)
        {
            var textExtensions = new[] { ".txt", ".log", ".csv", ".json", ".xml", ".html", ".css", ".js", ".cs" };
            return textExtensions.Contains(extension.ToLowerInvariant());
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
}
