using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;
using FindEdge.Core.Models;

namespace FindEdge.Infrastructure.Services
{
    /// <summary>
    /// Détecteur de doublons intelligent avec plusieurs méthodes de détection
    /// </summary>
    public class DuplicateDetector : IDuplicateDetector
    {
        private readonly IContentParserRegistry _parserRegistry;

        public DuplicateDetector(IContentParserRegistry parserRegistry)
        {
            _parserRegistry = parserRegistry ?? throw new ArgumentNullException(nameof(parserRegistry));
        }

        public event EventHandler<DuplicateDetectionProgressEventArgs>? DetectionProgress;
        public event EventHandler<DuplicateGroupFoundEventArgs>? DuplicateGroupFound;

        public async Task<IEnumerable<DuplicateGroup>> DetectDuplicatesAsync(IEnumerable<SearchResult> files, DuplicateDetectionOptions options, CancellationToken cancellationToken = default)
        {
            var duplicateGroups = new List<DuplicateGroup>();
            var processedFiles = 0;
            var totalFiles = files.Count();

            // Filtrer les fichiers selon les options
            var filteredFiles = files.Where(f => ShouldIncludeFile(f, options)).ToList();

            try
            {
                switch (options.Method)
                {
                    case DuplicateDetectionMethod.Hash:
                        duplicateGroups = await DetectByHashAsync(filteredFiles, options, cancellationToken);
                        break;
                    case DuplicateDetectionMethod.Content:
                        duplicateGroups = await DetectByContentAsync(filteredFiles, options, cancellationToken);
                        break;
                    case DuplicateDetectionMethod.Hybrid:
                        duplicateGroups = await DetectHybridAsync(filteredFiles, options, cancellationToken);
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                // L'opération a été annulée
            }
            catch (Exception ex)
            {
                // Log l'erreur
            }

            return duplicateGroups;
        }

        public async Task<string> CalculateFileHashAsync(string filePath, CancellationToken cancellationToken = default)
        {
            try
            {
                using var sha256 = SHA256.Create();
                using var stream = File.OpenRead(filePath);
                
                var buffer = new byte[8192];
                var hash = sha256.ComputeHash(stream);
                
                return Convert.ToHexString(hash);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public async Task<bool> AreFilesIdenticalAsync(string filePath1, string filePath2, CancellationToken cancellationToken = default)
        {
            try
            {
                var hash1 = await CalculateFileHashAsync(filePath1, cancellationToken);
                var hash2 = await CalculateFileHashAsync(filePath2, cancellationToken);
                
                return !string.IsNullOrEmpty(hash1) && !string.IsNullOrEmpty(hash2) && hash1 == hash2;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool ShouldIncludeFile(SearchResult file, DuplicateDetectionOptions options)
        {
            // Vérifier la taille
            if (file.FileSize < options.MinFileSize || file.FileSize > options.MaxFileSize)
                return false;

            // Vérifier les extensions exclues
            var extension = Path.GetExtension(file.FilePath).ToLowerInvariant();
            if (options.ExcludeExtensions.Contains(extension))
                return false;

            // Vérifier les répertoires exclus
            foreach (var excludeDir in options.ExcludeDirectories)
            {
                if (file.Directory.Contains(excludeDir, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }

        private async Task<List<DuplicateGroup>> DetectByHashAsync(List<SearchResult> files, DuplicateDetectionOptions options, CancellationToken cancellationToken)
        {
            var duplicateGroups = new List<DuplicateGroup>();
            var hashGroups = new Dictionary<string, List<SearchResult>>();

            foreach (var file in files)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    var hash = await CalculateFileHashAsync(file.FilePath, cancellationToken);
                    if (!string.IsNullOrEmpty(hash))
                    {
                        if (!hashGroups.ContainsKey(hash))
                            hashGroups[hash] = new List<SearchResult>();
                        
                        hashGroups[hash].Add(file);
                    }
                }
                catch (Exception)
                {
                    // Ignorer les erreurs de calcul de hash
                }

                OnDetectionProgress(new DuplicateDetectionProgressEventArgs
                {
                    FilesProcessed = files.IndexOf(file) + 1,
                    TotalFiles = files.Count,
                    CurrentFile = file.FilePath,
                    DuplicateGroupsFound = duplicateGroups.Count
                });
            }

            // Créer les groupes de doublons
            foreach (var group in hashGroups.Values.Where(g => g.Count > 1))
            {
                var duplicateFiles = new List<DuplicateFile>();
                foreach (var f in group)
                {
                    var hash = await CalculateFileHashAsync(f.FilePath, cancellationToken);
                    duplicateFiles.Add(new DuplicateFile
                    {
                        FilePath = f.FilePath,
                        FileName = f.FileName,
                        Directory = f.Directory,
                        FileSize = f.FileSize,
                        LastModified = f.LastModified,
                        FileHash = hash,
                        IsOriginal = group.IndexOf(f) == 0
                    });
                }

                var duplicateGroup = new DuplicateGroup
                {
                    Type = DuplicateGroupType.Identical,
                    Confidence = 1.0,
                    Files = duplicateFiles
                };

                duplicateGroup.TotalSize = duplicateGroup.Files.Sum(f => f.FileSize);
                duplicateGroup.SpaceWasted = duplicateGroup.TotalSize - duplicateGroup.Files.First().FileSize;

                duplicateGroups.Add(duplicateGroup);
                OnDuplicateGroupFound(new DuplicateGroupFoundEventArgs { Group = duplicateGroup });
            }

            return duplicateGroups;
        }

        private async Task<List<DuplicateGroup>> DetectByContentAsync(List<SearchResult> files, DuplicateDetectionOptions options, CancellationToken cancellationToken)
        {
            var duplicateGroups = new List<DuplicateGroup>();
            var contentGroups = new Dictionary<string, List<SearchResult>>();

            foreach (var file in files)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    var content = await ExtractFileContentAsync(file.FilePath, cancellationToken);
                    if (!string.IsNullOrEmpty(content))
                    {
                        var contentHash = CalculateContentHash(content);
                        if (!contentGroups.ContainsKey(contentHash))
                            contentGroups[contentHash] = new List<SearchResult>();
                        
                        contentGroups[contentHash].Add(file);
                    }
                }
                catch (Exception)
                {
                    // Ignorer les erreurs d'extraction de contenu
                }

                OnDetectionProgress(new DuplicateDetectionProgressEventArgs
                {
                    FilesProcessed = files.IndexOf(file) + 1,
                    TotalFiles = files.Count,
                    CurrentFile = file.FilePath,
                    DuplicateGroupsFound = duplicateGroups.Count
                });
            }

            // Créer les groupes de doublons
            foreach (var group in contentGroups.Values.Where(g => g.Count > 1))
            {
                var duplicateGroup = new DuplicateGroup
                {
                    Type = DuplicateGroupType.Similar,
                    Confidence = 0.95, // Confiance élevée pour le contenu identique
                    Files = group.Select(f => new DuplicateFile
                    {
                        FilePath = f.FilePath,
                        FileName = f.FileName,
                        Directory = f.Directory,
                        FileSize = f.FileSize,
                        LastModified = f.LastModified,
                        IsOriginal = group.IndexOf(f) == 0,
                        SimilarityScore = 1.0
                    }).ToList()
                };

                duplicateGroup.TotalSize = duplicateGroup.Files.Sum(f => f.FileSize);
                duplicateGroup.SpaceWasted = duplicateGroup.TotalSize - duplicateGroup.Files.First().FileSize;

                duplicateGroups.Add(duplicateGroup);
                OnDuplicateGroupFound(new DuplicateGroupFoundEventArgs { Group = duplicateGroup });
            }

            return duplicateGroups;
        }

        private async Task<List<DuplicateGroup>> DetectHybridAsync(List<SearchResult> files, DuplicateDetectionOptions options, CancellationToken cancellationToken)
        {
            // Détection hybride : d'abord par hash, puis par contenu pour les fichiers similaires
            var hashGroups = await DetectByHashAsync(files, options, cancellationToken);
            
            // Pour les fichiers restants, essayer la détection par contenu
            var processedFiles = hashGroups.SelectMany(g => g.Files).Select(f => f.FilePath).ToHashSet();
            var remainingFiles = files.Where(f => !processedFiles.Contains(f.FilePath)).ToList();
            
            var contentGroups = await DetectByContentAsync(remainingFiles, options, cancellationToken);
            
            return hashGroups.Concat(contentGroups).ToList();
        }

        private async Task<string> ExtractFileContentAsync(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                var parser = _parserRegistry.GetParser(filePath);
                if (parser != null)
                {
                    return await parser.ExtractContentAsync(filePath, cancellationToken);
                }
                else
                {
                    // Essayer de lire comme fichier texte
                    return await File.ReadAllTextAsync(filePath, cancellationToken);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private string CalculateContentHash(string content)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(content);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }

        private void OnDetectionProgress(DuplicateDetectionProgressEventArgs e)
        {
            DetectionProgress?.Invoke(this, e);
        }

        private void OnDuplicateGroupFound(DuplicateGroupFoundEventArgs e)
        {
            DuplicateGroupFound?.Invoke(this, e);
        }
    }
}
