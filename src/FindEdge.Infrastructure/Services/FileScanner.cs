using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;
using FindEdge.Core.Models;
using SystemFileInfo = System.IO.FileInfo;
using CoreFileInfo = FindEdge.Core.Interfaces.FileInfo;

namespace FindEdge.Infrastructure.Services
{
    /// <summary>
    /// Scanner de fichiers pour l'indexation et la recherche
    /// </summary>
    public class FileScanner : IFileScanner
    {
        public async Task<IEnumerable<CoreFileInfo>> ScanDirectoryAsync(string directoryPath, SearchOptions options, CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(directoryPath))
                return Enumerable.Empty<CoreFileInfo>();

            var files = new List<CoreFileInfo>();

            try
            {
                await ScanDirectoryRecursiveAsync(directoryPath, options, files, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // L'opération a été annulée, retourner les fichiers trouvés jusqu'à présent
            }
            catch (Exception)
            {
                // Ignorer les erreurs d'accès aux fichiers
            }

            return files;
        }

        private async Task ScanDirectoryRecursiveAsync(string directoryPath, SearchOptions options, List<CoreFileInfo> files, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            try
            {
                var directory = new DirectoryInfo(directoryPath);
                
                // Vérifier si le répertoire doit être exclu
                if (ShouldExcludeDirectory(directoryPath, options))
                    return;

                // Scanner les fichiers du répertoire
                var directoryFiles = directory.GetFiles();
                foreach (var file in directoryFiles)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    if (ShouldIncludeFile(file, options))
                    {
                        files.Add(ConvertToFileInfo(file));
                    }
                }

                // Scanner les sous-répertoires
                var subDirectories = directory.GetDirectories();
                foreach (var subDirectory in subDirectories)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    // Vérifier les fichiers cachés et système
                    if (!options.IncludeHiddenFiles && (subDirectory.Attributes & FileAttributes.Hidden) != 0)
                        continue;

                    if (!options.IncludeSystemFiles && (subDirectory.Attributes & FileAttributes.System) != 0)
                        continue;

                    await ScanDirectoryRecursiveAsync(subDirectory.FullName, options, files, cancellationToken);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Ignorer les répertoires sans accès
            }
            catch (DirectoryNotFoundException)
            {
                // Ignorer les répertoires supprimés
            }
        }

        private bool ShouldExcludeDirectory(string directoryPath, SearchOptions options)
        {
            // Vérifier les répertoires d'exclusion
            foreach (var excludeDir in options.ExcludeDirectories)
            {
                if (directoryPath.Contains(excludeDir, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            // Vérifier les répertoires d'inclusion (si spécifiés)
            if (options.IncludeDirectories.Any())
            {
                return !options.IncludeDirectories.Any(includeDir => 
                    directoryPath.Contains(includeDir, StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }

        private bool ShouldIncludeFile(SystemFileInfo file, SearchOptions options)
        {
            // Vérifier la taille du fichier
            if (options.MinFileSize.HasValue && file.Length < options.MinFileSize.Value)
                return false;

            if (options.MaxFileSize.HasValue && file.Length > options.MaxFileSize.Value)
                return false;

            // Vérifier la date de modification
            if (options.ModifiedAfter.HasValue && file.LastWriteTime < options.ModifiedAfter.Value)
                return false;

            if (options.ModifiedBefore.HasValue && file.LastWriteTime > options.ModifiedBefore.Value)
                return false;

            // Vérifier les attributs
            if (!options.IncludeHiddenFiles && (file.Attributes & FileAttributes.Hidden) != 0)
                return false;

            if (!options.IncludeSystemFiles && (file.Attributes & FileAttributes.System) != 0)
                return false;

            // Vérifier les extensions
            var extension = file.Extension.ToLowerInvariant();
            
            if (options.ExcludeExtensions.Any() && options.ExcludeExtensions.Contains(extension))
                return false;

            if (options.IncludeExtensions.Any() && !options.IncludeExtensions.Contains(extension))
                return false;

            return true;
        }

        private CoreFileInfo ConvertToFileInfo(SystemFileInfo systemFile)
        {
            return new CoreFileInfo
            {
                FullPath = systemFile.FullName,
                Name = systemFile.Name,
                Directory = systemFile.DirectoryName ?? string.Empty,
                Extension = systemFile.Extension,
                Size = systemFile.Length,
                LastModified = systemFile.LastWriteTime,
                Attributes = systemFile.Attributes
            };
        }

        public bool IsFileMatch(CoreFileInfo fileInfo, SearchOptions options)
        {
            // Vérifier la taille du fichier
            if (options.MinFileSize.HasValue && fileInfo.Size < options.MinFileSize.Value)
                return false;

            if (options.MaxFileSize.HasValue && fileInfo.Size > options.MaxFileSize.Value)
                return false;

            // Vérifier la date de modification
            if (options.ModifiedAfter.HasValue && fileInfo.LastModified < options.ModifiedAfter.Value)
                return false;

            if (options.ModifiedBefore.HasValue && fileInfo.LastModified > options.ModifiedBefore.Value)
                return false;

            // Vérifier les attributs
            if (!options.IncludeHiddenFiles && (fileInfo.Attributes & FileAttributes.Hidden) != 0)
                return false;

            if (!options.IncludeSystemFiles && (fileInfo.Attributes & FileAttributes.System) != 0)
                return false;

            // Vérifier les extensions
            var extension = fileInfo.Extension.ToLowerInvariant();
            
            if (options.ExcludeExtensions.Any() && options.ExcludeExtensions.Contains(extension))
                return false;

            if (options.IncludeExtensions.Any() && !options.IncludeExtensions.Contains(extension))
                return false;

            return true;
        }

        public event EventHandler<FileFoundEventArgs>? FileFound;
    }
}
