using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using FindEdge.Core.Interfaces;
using FindEdge.Core.Models;

namespace FindEdge.Presentation.ViewModels
{
    /// <summary>
    /// ViewModel principal pour l'interface de recherche
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IIndexedSearchEngine _searchEngine;
        private CancellationTokenSource? _cancellationTokenSource;

        public MainViewModel(IIndexedSearchEngine searchEngine)
        {
            _searchEngine = searchEngine ?? throw new ArgumentNullException(nameof(searchEngine));
            
            // Initialiser les commandes
            SearchCommand = new RelayCommandSimple(async () => await ExecuteSearchAsync(), () => !IsSearching && !string.IsNullOrWhiteSpace(SearchTerm));
            StopSearchCommand = new RelayCommandSimple(() => ExecuteStopSearch(), () => IsSearching);
            ClearResultsCommand = new RelayCommandSimple(() => ExecuteClearResults(), () => SearchResults.Any());
            ExportCsvCommand = new RelayCommandSimple(() => ExecuteExportCsv(), () => SearchResults.Any());
            OpenIndexWindowCommand = new RelayCommandSimple(() => ExecuteOpenIndexWindow());
            OpenPluginsWindowCommand = new RelayCommandSimple(() => ExecuteOpenPluginsWindow());
            OpenDuplicatesWindowCommand = new RelayCommandSimple(() => ExecuteOpenDuplicatesWindow());
            OpenAdvancedFeaturesCommand = new RelayCommandSimple(() => ExecuteOpenAdvancedFeatures());
            SwitchSearchModeCommand = new RelayCommandSimple(() => ExecuteSwitchSearchMode(SearchMode.Hybrid));

            // Initialiser les options de recherche
            SearchOptions = new SearchOptions
            {
                SearchInFileName = true,
                SearchInContent = true,
                MaxResults = 1000,
                MaxContentLength = 10000
            };

            // S'abonner aux événements du moteur de recherche
            _searchEngine.SearchProgress += OnSearchProgress;
            _searchEngine.ResultFound += OnResultFound;
        }

        #region Propriétés

        private string _searchTerm = string.Empty;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (SetProperty(ref _searchTerm, value))
                {
                    ((RelayCommandSimple)SearchCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private bool _isSearching;
        public bool IsSearching
        {
            get => _isSearching;
            set
            {
                if (SetProperty(ref _isSearching, value))
                {
                    ((RelayCommandSimple)SearchCommand).RaiseCanExecuteChanged();
                    ((RelayCommandSimple)StopSearchCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private string _statusMessage = "Prêt à rechercher";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private int _filesProcessed;
        public int FilesProcessed
        {
            get => _filesProcessed;
            set => SetProperty(ref _filesProcessed, value);
        }

        private int _totalFiles;
        public int TotalFiles
        {
            get => _totalFiles;
            set => SetProperty(ref _totalFiles, value);
        }

        private TimeSpan _elapsedTime;
        public TimeSpan ElapsedTime
        {
            get => _elapsedTime;
            set => SetProperty(ref _elapsedTime, value);
        }

        private string _currentFile = string.Empty;
        public string CurrentFile
        {
            get => _currentFile;
            set => SetProperty(ref _currentFile, value);
        }

        public ObservableCollection<SearchResult> SearchResults { get; } = new();
        public SearchOptions SearchOptions { get; }

        private SearchMode _currentSearchMode = SearchMode.Hybrid;
        public SearchMode CurrentSearchMode
        {
            get => _currentSearchMode;
            set
            {
                if (SetProperty(ref _currentSearchMode, value))
                {
                    _searchEngine.SwitchSearchMode(value);
                }
            }
        }

        private bool _isIndexAvailable;
        public bool IsIndexAvailable
        {
            get => _isIndexAvailable;
            set => SetProperty(ref _isIndexAvailable, value);
        }

        private int _indexedDocuments;
        public int IndexedDocuments
        {
            get => _indexedDocuments;
            set => SetProperty(ref _indexedDocuments, value);
        }

        #endregion

        #region Commandes

        public ICommand SearchCommand { get; }
        public ICommand StopSearchCommand { get; }
        public ICommand ClearResultsCommand { get; }
        public ICommand ExportCsvCommand { get; }
        public ICommand OpenIndexWindowCommand { get; }
        public ICommand OpenPluginsWindowCommand { get; }
        public ICommand OpenDuplicatesWindowCommand { get; }
        public ICommand OpenAdvancedFeaturesCommand { get; }
        public ICommand SwitchSearchModeCommand { get; }

        #endregion

        #region Méthodes privées

        private async Task ExecuteSearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
                return;

            try
            {
                IsSearching = true;
                SearchResults.Clear();
                FilesProcessed = 0;
                TotalFiles = 0;
                ElapsedTime = TimeSpan.Zero;

                _cancellationTokenSource = new CancellationTokenSource();
                SearchOptions.SearchTerm = SearchTerm;

                StatusMessage = "Recherche en cours...";
                var startTime = DateTime.UtcNow;

                var results = await _searchEngine.SearchAsync(SearchOptions, _cancellationTokenSource.Token);
                
                // Convertir en liste pour éviter les problèmes de thread
                var resultsList = results.ToList();
                
                // Mettre à jour l'interface sur le thread UI
                await Task.Run(() =>
                {
                    foreach (var result in resultsList)
                    {
                        App.Current.Dispatcher.Invoke(() => SearchResults.Add(result));
                    }
                });

                ElapsedTime = DateTime.UtcNow - startTime;
                StatusMessage = $"Recherche terminée - {SearchResults.Count} résultat(s) trouvé(s)";
            }
            catch (OperationCanceledException)
            {
                StatusMessage = "Recherche annulée";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors de la recherche : {ex.Message}";
            }
            finally
            {
                IsSearching = false;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private void ExecuteStopSearch()
        {
            _cancellationTokenSource?.Cancel();
            StatusMessage = "Arrêt de la recherche...";
        }

        private void ExecuteClearResults()
        {
            SearchResults.Clear();
            StatusMessage = "Résultats effacés";
        }

        private void ExecuteExportCsv()
        {
            // TODO: Implémenter l'export CSV
            StatusMessage = "Export CSV - Fonctionnalité à implémenter";
        }

        private void ExecuteOpenIndexWindow()
        {
            var indexWindow = new Views.IndexWindow();
            indexWindow.Owner = App.Current.MainWindow;
            indexWindow.ShowDialog();
        }

        private void ExecuteOpenPluginsWindow()
        {
            var pluginsWindow = new Views.PluginsWindow();
            pluginsWindow.Owner = App.Current.MainWindow;
            pluginsWindow.ShowDialog();
        }

        private void ExecuteOpenDuplicatesWindow()
        {
            var duplicatesWindow = new Views.DuplicatesWindow();
            duplicatesWindow.Owner = App.Current.MainWindow;
            duplicatesWindow.ShowDialog();
        }

        private void ExecuteOpenAdvancedFeatures()
        {
            // Récupérer le conteneur de services depuis l'App
            var app = (App)App.Current;
            var serviceContainer = app.ServiceContainer;
            
            var advancedFeaturesWindow = new Views.AdvancedFeaturesWindow(
                new AdvancedFeaturesViewModel(
                    serviceContainer.Get<ISemanticSearchEngine>(),
                    serviceContainer.Get<IIntelligentSuggestions>(),
                    serviceContainer.Get<IVisualizationService>(),
                    serviceContainer.Get<IAnalyticsService>(),
                    serviceContainer.Get<ICollaborationService>(),
                    serviceContainer.Get<IHelpLearningService>(),
                    serviceContainer.Get<IPersonalizationService>()
                )
            );
            advancedFeaturesWindow.Owner = App.Current.MainWindow;
            advancedFeaturesWindow.ShowDialog();
        }

        private void ExecuteSwitchSearchMode(SearchMode mode)
        {
            CurrentSearchMode = mode;
            StatusMessage = $"Mode de recherche : {GetSearchModeDescription(mode)}";
        }

        private string GetSearchModeDescription(SearchMode mode)
        {
            return mode switch
            {
                SearchMode.Hybrid => "Hybride (Index + Live)",
                SearchMode.IndexOnly => "Index uniquement",
                SearchMode.LiveOnly => "Live scan uniquement",
                _ => "Inconnu"
            };
        }

        private void OnSearchProgress(object? sender, SearchProgressEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                FilesProcessed = e.FilesProcessed;
                TotalFiles = e.TotalFiles;
                ElapsedTime = e.ElapsedTime;
                CurrentFile = e.CurrentFile;
                StatusMessage = $"Traitement de {e.FilesProcessed} fichier(s)...";
            });
        }

        private void OnResultFound(object? sender, SearchResultEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                SearchResults.Add(e.Result);
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

}
