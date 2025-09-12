using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;

namespace FindEdge.Infrastructure.Services
{
    /// <summary>
    /// Service de parsing de fichiers simple
    /// </summary>
    public class SimpleFileParserService : IFileParserService
    {
        private readonly string[] _supportedExtensions = { ".txt", ".log", ".csv", ".json", ".xml", ".html", ".htm", ".md" };

        /// <summary>
        /// Extrait le texte d'un fichier
        /// </summary>
        public async Task<string> ExtractTextAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return string.Empty;

            try
            {
                var extension = Path.GetExtension(filePath).ToLowerInvariant();
                
                // Pour les fichiers texte simples
                if (Array.Exists(_supportedExtensions, ext => ext == extension))
                {
                    return await File.ReadAllTextAsync(filePath, cancellationToken);
                }

                // Pour les autres types de fichiers, retourner le nom du fichier
                return Path.GetFileName(filePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de l'extraction du texte de {filePath}: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Vérifie si le fichier peut être parsé
        /// </summary>
        public bool CanParse(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return Array.Exists(_supportedExtensions, ext => ext == extension);
        }

        /// <summary>
        /// Obtient les types de fichiers supportés
        /// </summary>
        public string[] GetSupportedExtensions()
        {
            return (string[])_supportedExtensions.Clone();
        }
    }
}
