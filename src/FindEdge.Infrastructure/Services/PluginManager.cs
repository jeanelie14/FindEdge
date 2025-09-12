using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;

namespace FindEdge.Infrastructure.Services
{
    /// <summary>
    /// Gestionnaire de plugins pour FindEdge
    /// </summary>
    public class PluginManager : IPluginManager
    {
        private readonly Dictionary<string, IPlugin> _loadedPlugins = new();
        private readonly List<Assembly> _loadedAssemblies = new();
        private readonly string _pluginsDirectory;

        public PluginManager()
        {
            _pluginsDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FindEdge",
                "Plugins"
            );
        }

        public event EventHandler<PluginLoadedEventArgs>? PluginLoaded;
        public event EventHandler<PluginUnloadedEventArgs>? PluginUnloaded;
        public event EventHandler<PluginErrorEventArgs>? PluginError;

        public async Task LoadPluginsAsync()
        {
            try
            {
                // Créer le répertoire des plugins s'il n'existe pas
                if (!Directory.Exists(_pluginsDirectory))
                {
                    Directory.CreateDirectory(_pluginsDirectory);
                    return;
                }

                // Charger tous les plugins du répertoire
                var pluginFiles = Directory.GetFiles(_pluginsDirectory, "*.dll", SearchOption.AllDirectories);
                
                foreach (var pluginFile in pluginFiles)
                {
                    try
                    {
                        await LoadPluginAsync(pluginFile);
                    }
                    catch (Exception ex)
                    {
                        OnPluginError(new PluginErrorEventArgs
                        {
                            PluginName = Path.GetFileNameWithoutExtension(pluginFile),
                            ErrorMessage = $"Erreur lors du chargement du plugin: {ex.Message}",
                            Exception = ex
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                OnPluginError(new PluginErrorEventArgs
                {
                    PluginName = "PluginManager",
                    ErrorMessage = $"Erreur lors du chargement des plugins: {ex.Message}",
                    Exception = ex
                });
            }
        }

        public async Task<IPlugin> LoadPluginAsync(string pluginPath)
        {
            if (!File.Exists(pluginPath))
                throw new FileNotFoundException($"Plugin non trouvé: {pluginPath}");

            // Charger l'assembly
            var assembly = Assembly.LoadFrom(pluginPath);
            _loadedAssemblies.Add(assembly);

            // Trouver les types qui implémentent IPlugin
            var pluginTypes = assembly.GetTypes()
                .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToArray();

            if (pluginTypes.Length == 0)
                throw new InvalidOperationException($"Aucun plugin trouvé dans {pluginPath}");

            // Instancier le premier plugin trouvé
            var pluginType = pluginTypes[0];
            var plugin = (IPlugin)Activator.CreateInstance(pluginType)!;

            // Initialiser le plugin
            plugin.Initialize();

            // Enregistrer le plugin
            _loadedPlugins[plugin.Name] = plugin;

            OnPluginLoaded(new PluginLoadedEventArgs
            {
                Plugin = plugin,
                PluginPath = pluginPath
            });

            return plugin;
        }

        public async Task UnloadPluginAsync(string pluginName)
        {
            if (_loadedPlugins.TryGetValue(pluginName, out var plugin))
            {
                try
                {
                    plugin.Dispose();
                    _loadedPlugins.Remove(pluginName);

                    OnPluginUnloaded(new PluginUnloadedEventArgs
                    {
                        PluginName = pluginName
                    });
                }
                catch (Exception ex)
                {
                    OnPluginError(new PluginErrorEventArgs
                    {
                        PluginName = pluginName,
                        ErrorMessage = $"Erreur lors du déchargement du plugin: {ex.Message}",
                        Exception = ex
                    });
                }
            }
        }

        public IEnumerable<IPlugin> GetAllPlugins()
        {
            return _loadedPlugins.Values.ToList();
        }

        public IEnumerable<IContentParserPlugin> GetContentParserPlugins()
        {
            return _loadedPlugins.Values
                .OfType<IContentParserPlugin>()
                .ToList();
        }

        public IEnumerable<ISearchEnginePlugin> GetSearchEnginePlugins()
        {
            return _loadedPlugins.Values
                .OfType<ISearchEnginePlugin>()
                .ToList();
        }

        public IEnumerable<IExportPlugin> GetExportPlugins()
        {
            return _loadedPlugins.Values
                .OfType<IExportPlugin>()
                .ToList();
        }

        public IPlugin? GetPlugin(string name)
        {
            _loadedPlugins.TryGetValue(name, out var plugin);
            return plugin;
        }

        public bool IsPluginLoaded(string name)
        {
            return _loadedPlugins.ContainsKey(name);
        }

        private void OnPluginLoaded(PluginLoadedEventArgs e)
        {
            PluginLoaded?.Invoke(this, e);
        }

        private void OnPluginUnloaded(PluginUnloadedEventArgs e)
        {
            PluginUnloaded?.Invoke(this, e);
        }

        private void OnPluginError(PluginErrorEventArgs e)
        {
            PluginError?.Invoke(this, e);
        }

        public void Dispose()
        {
            foreach (var plugin in _loadedPlugins.Values)
            {
                try
                {
                    plugin.Dispose();
                }
                catch (Exception)
                {
                    // Ignorer les erreurs lors du nettoyage
                }
            }

            _loadedPlugins.Clear();
            _loadedAssemblies.Clear();
        }
    }
}
