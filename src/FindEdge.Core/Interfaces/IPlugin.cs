using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface de base pour tous les plugins FindEdge
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Nom du plugin
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Version du plugin
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Description du plugin
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Auteur du plugin
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Initialise le plugin
        /// </summary>
        void Initialize();

        /// <summary>
        /// Nettoie les ressources du plugin
        /// </summary>
        void Dispose();

        /// <summary>
        /// Vérifie si le plugin est compatible avec la version actuelle
        /// </summary>
        bool IsCompatible(string applicationVersion);
    }

    /// <summary>
    /// Interface pour les plugins de parseur de contenu
    /// </summary>
    public interface IContentParserPlugin : IPlugin, IContentParser
    {
        /// <summary>
        /// Extensions de fichiers supportées par ce plugin
        /// </summary>
        new string[] SupportedExtensions { get; }

        /// <summary>
        /// Priorité du plugin (plus élevé = plus prioritaire)
        /// </summary>
        new int Priority { get; }

        /// <summary>
        /// Configuration spécifique au plugin
        /// </summary>
        Dictionary<string, object> Configuration { get; set; }
    }

    /// <summary>
    /// Interface pour les plugins de moteur de recherche
    /// </summary>
    public interface ISearchEnginePlugin : IPlugin
    {
        /// <summary>
        /// Exécute une recherche avec le plugin
        /// </summary>
        Task<IEnumerable<SearchResult>> SearchAsync(SearchOptions options, System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Vérifie si le plugin peut traiter les options données
        /// </summary>
        bool CanHandle(SearchOptions options);
    }

    /// <summary>
    /// Interface pour les plugins d'export
    /// </summary>
    public interface IExportPlugin : IPlugin
    {
        /// <summary>
        /// Extensions de fichiers supportées pour l'export
        /// </summary>
        string[] SupportedExportFormats { get; }

        /// <summary>
        /// Exporte les résultats dans le format spécifié
        /// </summary>
        Task<byte[]> ExportAsync(IEnumerable<SearchResult> results, string format, Dictionary<string, object>? options = null);
    }
}
