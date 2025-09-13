using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using FindEdge.Core.Interfaces;

namespace FindEdge.Presentation.ViewModels
{
    /// <summary>
    /// ViewModel pour la gestion des plugins
    /// </summary>
    public class PluginsViewModel : INotifyPropertyChanged
    {
        private readonly IPluginManager _pluginManager;

        public PluginsViewModel(IPluginManager pluginManager)
        {
            _pluginManager = pluginManager ?? throw new ArgumentNullException(nameof(pluginManager));
            
            // Initialiser les commandes
            LoadPluginsCommand = new RelayCommandSimple(async () => await ExecuteLoadPluginsAsync());
            UnloadPluginCommand = new RelayCommandSimple(() => ExecuteUnloadPlugin("test"));

            // Initialiser les collections
            Plugins = new ObservableCollection<PluginInfo>();
            ContentParserPlugins = new ObservableCollection<PluginInfo>();
            SearchEnginePlugins = new ObservableCollection<PluginInfo>();
            ExportPlugins = new ObservableCollection<PluginInfo>();

            // S'abonner aux événements
            _pluginManager.PluginLoaded += OnPluginLoaded;
            _pluginManager.PluginUnloaded += OnPluginUnloaded;
            _pluginManager.PluginError += OnPluginError;

            // Charger les plugins au démarrage
            _ = Task.Run(async () => await ExecuteLoadPluginsAsync());
        }

        #region Propriétés

        private string _statusMessage = "Chargement des plugins...";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty(ref _isLoading, value))
                {
                    ((RelayCommand)LoadPluginsCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<PluginInfo> Plugins { get; }
        public ObservableCollection<PluginInfo> ContentParserPlugins { get; }
        public ObservableCollection<PluginInfo> SearchEnginePlugins { get; }
        public ObservableCollection<PluginInfo> ExportPlugins { get; }

        #endregion

        #region Commandes

        public ICommand LoadPluginsCommand { get; }
        public ICommand UnloadPluginCommand { get; }

        #endregion

        #region Méthodes privées

        private async Task ExecuteLoadPluginsAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Chargement des plugins...";

                await _pluginManager.LoadPluginsAsync();
                
                // Mettre à jour les collections
                await UpdatePluginCollectionsAsync();
                
                StatusMessage = $"Plugins chargés - {Plugins.Count} plugin(s) disponible(s)";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors du chargement des plugins: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExecuteUnloadPlugin(string pluginName)
        {
            try
            {
                await _pluginManager.UnloadPluginAsync(pluginName);
                await UpdatePluginCollectionsAsync();
                StatusMessage = $"Plugin '{pluginName}' déchargé";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors du déchargement du plugin: {ex.Message}";
            }
        }

        private async Task UpdatePluginCollectionsAsync()
        {
            Plugins.Clear();
            ContentParserPlugins.Clear();
            SearchEnginePlugins.Clear();
            ExportPlugins.Clear();

            var allPlugins = _pluginManager.GetAllPlugins();
            
            foreach (var plugin in allPlugins)
            {
                var pluginInfo = new PluginInfo
                {
                    Name = plugin.Name,
                    Version = plugin.Version,
                    Description = plugin.Description,
                    Author = plugin.Author,
                    IsLoaded = true,
                    Type = GetPluginType(plugin)
                };

                Plugins.Add(pluginInfo);

                // Ajouter aux collections spécialisées
                switch (plugin)
                {
                    case IContentParserPlugin:
                        ContentParserPlugins.Add(pluginInfo);
                        break;
                    case ISearchEnginePlugin:
                        SearchEnginePlugins.Add(pluginInfo);
                        break;
                    case IExportPlugin:
                        ExportPlugins.Add(pluginInfo);
                        break;
                }
            }
        }

        private string GetPluginType(IPlugin plugin)
        {
            return plugin switch
            {
                IContentParserPlugin => "Parseur de contenu",
                ISearchEnginePlugin => "Moteur de recherche",
                IExportPlugin => "Export",
                _ => "Générique"
            };
        }

        private void OnPluginLoaded(object? sender, PluginLoadedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(async () =>
            {
                await UpdatePluginCollectionsAsync();
                StatusMessage = $"Plugin '{e.Plugin.Name}' chargé avec succès";
            });
        }

        private void OnPluginUnloaded(object? sender, PluginUnloadedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(async () =>
            {
                await UpdatePluginCollectionsAsync();
                StatusMessage = $"Plugin '{e.PluginName}' déchargé";
            });
        }

        private void OnPluginError(object? sender, PluginErrorEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                StatusMessage = $"Erreur plugin '{e.PluginName}': {e.ErrorMessage}";
            });
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }

    /// <summary>
    /// Informations sur un plugin pour l'affichage
    /// </summary>
    public class PluginInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public bool IsLoaded { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
