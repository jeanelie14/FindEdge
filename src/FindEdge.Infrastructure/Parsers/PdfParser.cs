using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace FindEdge.Infrastructure.Parsers
{
    /// <summary>
    /// Parseur pour les fichiers PDF utilisant PdfPig
    /// </summary>
    public class PdfParser : IContentParser
    {
        public string[] SupportedExtensions { get; } = new[] { ".pdf" };

        public int Priority => 200; // Priorité élevée pour les PDF

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
                
                using var document = PdfDocument.Open(filePath);
                
                foreach (var page in document.GetPages())
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    // Extraire le texte de la page
                    var pageText = page.Text;
                    if (!string.IsNullOrEmpty(pageText))
                    {
                        content.AppendLine(pageText);
                    }

                    // Extraire les métadonnées de la page si nécessaire
                    // (tables, images, etc. peuvent être ajoutés plus tard)
                }

                return content.ToString();
            }
            catch (Exception ex)
            {
                // Log l'erreur et retourner un contenu vide
                return $"Erreur lors de l'extraction PDF: {ex.Message}";
            }
        }
    }
}
