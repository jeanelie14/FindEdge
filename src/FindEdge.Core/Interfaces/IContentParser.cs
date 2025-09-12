using System;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour les parseurs de contenu de fichiers
    /// </summary>
    public interface IContentParser
    {
        /// <summary>
        /// Extrait le contenu textuel d'un fichier
        /// </summary>
        Task<string> ExtractContentAsync(string filePath, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Vérifie si ce parseur peut traiter le type de fichier donné
        /// </summary>
        bool CanParse(string filePath);
        
        /// <summary>
        /// Extensions de fichiers supportées par ce parseur
        /// </summary>
        string[] SupportedExtensions { get; }
        
        /// <summary>
        /// Priorité du parseur (plus élevé = plus prioritaire)
        /// </summary>
        int Priority { get; }
    }

    /// <summary>
    /// Interface pour le registre des parseurs
    /// </summary>
    public interface IContentParserRegistry
    {
        /// <summary>
        /// Enregistre un nouveau parseur
        /// </summary>
        void RegisterParser(IContentParser parser);
        
        /// <summary>
        /// Trouve le parseur approprié pour un fichier
        /// </summary>
        IContentParser? GetParser(string filePath);
        
        /// <summary>
        /// Retourne tous les parseurs enregistrés
        /// </summary>
        IEnumerable<IContentParser> GetAllParsers();
    }
}
