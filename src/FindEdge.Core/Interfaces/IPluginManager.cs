using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour la gestion des plugins
    /// </summary>
    public interface IPluginManager
    {
        /// <summary>
        /// Charge tous les plugins disponibles
        /// </summary>
        Task LoadPluginsAsync();

        /// <summary>
        /// Charge un plugin spécifique
        /// </summary>
        Task<IPlugin> LoadPluginAsync(string pluginPath);

        /// <summary>
        /// Décharge un plugin
        /// </summary>
        Task UnloadPluginAsync(string pluginName);

        /// <summary>
        /// Obtient tous les plugins chargés
        /// </summary>
        IEnumerable<IPlugin> GetAllPlugins();

        /// <summary>
        /// Obtient les plugins de parseur de contenu
        /// </summary>
        IEnumerable<IContentParserPlugin> GetContentParserPlugins();

        /// <summary>
        /// Obtient les plugins de moteur de recherche
        /// </summary>
        IEnumerable<ISearchEnginePlugin> GetSearchEnginePlugins();

        /// <summary>
        /// Obtient les plugins d'export
        /// </summary>
        IEnumerable<IExportPlugin> GetExportPlugins();

        /// <summary>
        /// Trouve un plugin par nom
        /// </summary>
        IPlugin? GetPlugin(string name);

        /// <summary>
        /// Vérifie si un plugin est chargé
        /// </summary>
        bool IsPluginLoaded(string name);

        /// <summary>
        /// Événement déclenché lors du chargement d'un plugin
        /// </summary>
        event EventHandler<PluginLoadedEventArgs>? PluginLoaded;

        /// <summary>
        /// Événement déclenché lors du déchargement d'un plugin
        /// </summary>
        event EventHandler<PluginUnloadedEventArgs>? PluginUnloaded;

        /// <summary>
        /// Événement déclenché lors d'une erreur de plugin
        /// </summary>
        event EventHandler<PluginErrorEventArgs>? PluginError;
    }

    /// <summary>
    /// Arguments pour l'événement de chargement de plugin
    /// </summary>
    public class PluginLoadedEventArgs : EventArgs
    {
        public IPlugin Plugin { get; set; } = null!;
        public string PluginPath { get; set; } = string.Empty;
    }

    /// <summary>
    /// Arguments pour l'événement de déchargement de plugin
    /// </summary>
    public class PluginUnloadedEventArgs : EventArgs
    {
        public string PluginName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Arguments pour l'événement d'erreur de plugin
    /// </summary>
    public class PluginErrorEventArgs : EventArgs
    {
        public string PluginName { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public Exception? Exception { get; set; }
    }
}
