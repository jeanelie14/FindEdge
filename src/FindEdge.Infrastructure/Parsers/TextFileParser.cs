using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;

namespace FindEdge.Infrastructure.Parsers
{
    /// <summary>
    /// Parseur pour les fichiers texte simples
    /// </summary>
    public class TextFileParser : IContentParser
    {
        public string[] SupportedExtensions { get; } = new[]
        {
            ".txt", ".log", ".csv", ".json", ".xml", ".html", ".css", ".js",
            ".ts", ".cs", ".vb", ".cpp", ".h", ".py", ".java", ".php",
            ".rb", ".go", ".rs", ".swift", ".kt", ".scala", ".sh", ".bat",
            ".ps1", ".sql", ".yaml", ".yml", ".ini", ".cfg", ".conf",
            ".md", ".rst", ".tex", ".rtf"
        };

        public int Priority => 50; // Priorité faible pour les fichiers texte

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
                // Essayer de lire le fichier avec différents encodages
                var encodings = new[] { Encoding.UTF8, Encoding.ASCII, Encoding.Unicode, Encoding.BigEndianUnicode };
                
                foreach (var encoding in encodings)
                {
                    try
                    {
                        var content = await File.ReadAllTextAsync(filePath, encoding, cancellationToken);
                        return content;
                    }
                    catch (DecoderFallbackException)
                    {
                        // Essayer l'encodage suivant
                        continue;
                    }
                }

                // Si tous les encodages échouent, essayer de lire en binaire et convertir
                var bytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception ex)
            {
                return $"Erreur lors de la lecture du fichier: {ex.Message}";
            }
        }
    }
}
