using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;
using SharpCompress.Archives;
using SharpCompress.Common;

namespace FindEdge.Infrastructure.Parsers
{
    /// <summary>
    /// Parseur pour les fichiers d'archives (ZIP, RAR, 7Z, TAR, etc.)
    /// </summary>
    public class ArchiveParser : IContentParser
    {
        public string[] SupportedExtensions { get; } = new[]
        {
            ".zip", ".rar", ".7z", ".tar", ".gz", ".bz2", ".xz",
            ".tar.gz", ".tar.bz2", ".tar.xz"
        };

        public int Priority => 100;

        public bool CanParse(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return Array.Exists(SupportedExtensions, ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<string> ExtractContentAsync(string filePath, CancellationToken cancellationToken = default)
        {
            try
            {
                var content = new StringBuilder();
                var processedFiles = 0;
                const int maxFiles = 50; // Limiter le nombre de fichiers traités

                using var archive = ArchiveFactory.Open(filePath);
                
                foreach (var entry in archive.Entries)
                {
                    if (cancellationToken.IsCancellationRequested || processedFiles >= maxFiles)
                        break;

                    if (entry.IsDirectory)
                        continue;

                    // Extraire le contenu des fichiers texte dans l'archive
                    if (IsTextFile(entry.Key))
                    {
                        try
                        {
                            using var entryStream = entry.OpenEntryStream();
                            using var reader = new StreamReader(entryStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
                            
                            var fileContent = await reader.ReadToEndAsync();
                            if (!string.IsNullOrEmpty(fileContent))
                            {
                                content.AppendLine($"=== {entry.Key} ===");
                                content.AppendLine(fileContent);
                                content.AppendLine();
                            }
                        }
                        catch (Exception)
                        {
                            // Ignorer les erreurs de lecture de fichiers individuels
                            content.AppendLine($"=== {entry.Key} (erreur de lecture) ===");
                        }
                    }
                    else
                    {
                        // Pour les fichiers non-texte, ajouter juste le nom
                        content.AppendLine($"=== {entry.Key} (fichier binaire) ===");
                    }

                    processedFiles++;
                }

                return content.ToString();
            }
            catch (Exception ex)
            {
                return $"Erreur lors de l'extraction d'archive: {ex.Message}";
            }
        }

        private bool IsTextFile(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var textExtensions = new[]
            {
                ".txt", ".log", ".csv", ".json", ".xml", ".html", ".css", ".js",
                ".ts", ".cs", ".vb", ".cpp", ".h", ".py", ".java", ".php",
                ".rb", ".go", ".rs", ".swift", ".kt", ".scala", ".sh", ".bat",
                ".ps1", ".sql", ".yaml", ".yml", ".ini", ".cfg", ".conf",
                ".md", ".rst", ".tex", ".rtf"
            };

            return Array.Exists(textExtensions, ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }
    }
}
