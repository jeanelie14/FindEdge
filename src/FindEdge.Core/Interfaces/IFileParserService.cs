using System;
using System.Threading;
using System.Threading.Tasks;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour le service de parsing de fichiers
    /// </summary>
    public interface IFileParserService
    {
        /// <summary>
        /// Extrait le texte d'un fichier
        /// </summary>
        Task<string> ExtractTextAsync(string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Vérifie si le fichier peut être parsé
        /// </summary>
        bool CanParse(string filePath);

        /// <summary>
        /// Obtient les types de fichiers supportés
        /// </summary>
        string[] GetSupportedExtensions();
    }
}
