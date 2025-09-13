using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using FindEdge.Core.Interfaces;
using FindEdge.Core.Models;

namespace FindEdge.Presentation.ViewModels
{
    /// <summary>
    /// ViewModel pour la gestion des doublons
    /// </summary>
    public class DuplicatesViewModel : INotifyPropertyChanged
    {
        private readonly IDuplicateDetector _duplicateDetector;
        private readonly IExportService _exportService;

        public DuplicatesViewModel(IDuplicateDetector duplicateDetector, IExportService exportService)
        {
            _duplicateDetector = duplicateDetector ?? throw new ArgumentNullException(nameof(duplicateDetector));
            _exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));

            // Initialiser les commandes
            DetectDuplicatesCommand = new RelayCommandSimple(async () => await ExecuteDetectDuplicatesAsync(), () => !IsDetecting && SearchResults.Any());
            ExportDuplicatesCommand = new RelayCommandSimple(async () => await ExecuteExportDuplicatesAsync(), () => DuplicateGroups.Any());
            ClearDuplicatesCommand = new RelayCommandSimple(() => ExecuteClearDuplicates(), () => DuplicateGroups.Any());
            DeleteSelectedCommand = new RelayCommandSimple(() => ExecuteDeleteSelected(), () => SelectedFiles.Any());

            // Initialiser les collections
            DuplicateGroups = new ObservableCollection<DuplicateGroup>();
            SelectedFiles = new ObservableCollection<DuplicateFile>();
            SearchResults = new ObservableCollection<SearchResult>();

            // Initialiser les options
            DetectionOptions = new DuplicateDetectionOptions
            {
                Method = DuplicateDetectionMethod.Hybrid,
                MinFileSize = 1024,
                MaxFileSize = 100 * 1024 * 1024,
                SimilarityThreshold = 0.95,
                IncludeHiddenFiles = false,
                IncludeSystemFiles = false
            };

            // S'abonner aux événements
            _duplicateDetector.DetectionProgress += OnDetectionProgress;
            _duplicateDetector.DuplicateGroupFound += OnDuplicateGroupFound;
        }

        #region Propriétés

        private string _statusMessage = "Prêt à détecter les doublons";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private bool _isDetecting;
        public bool IsDetecting
        {
            get => _isDetecting;
            set
            {
                if (SetProperty(ref _isDetecting, value))
                {
                    ((RelayCommand)DetectDuplicatesCommand).RaiseCanExecuteChanged();
                }
            }
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

        private long _totalSpaceWasted;
        public long TotalSpaceWasted
        {
            get => _totalSpaceWasted;
            set => SetProperty(ref _totalSpaceWasted, value);
        }

        public ObservableCollection<DuplicateGroup> DuplicateGroups { get; }
        public ObservableCollection<DuplicateFile> SelectedFiles { get; }
        public ObservableCollection<SearchResult> SearchResults { get; }
        public DuplicateDetectionOptions DetectionOptions { get; }

        #endregion

        #region Commandes

        public ICommand DetectDuplicatesCommand { get; }
        public ICommand ExportDuplicatesCommand { get; }
        public ICommand ClearDuplicatesCommand { get; }
        public ICommand DeleteSelectedCommand { get; }

        #endregion

        #region Méthodes publiques

        public void SetSearchResults(IEnumerable<SearchResult> results)
        {
            SearchResults.Clear();
            foreach (var result in results)
            {
                SearchResults.Add(result);
            }
        }

        #endregion

        #region Méthodes privées

        private async Task ExecuteDetectDuplicatesAsync()
        {
            try
            {
                IsDetecting = true;
                StatusMessage = "Détection des doublons en cours...";
                FilesProcessed = 0;
                TotalFiles = SearchResults.Count;
                ElapsedTime = TimeSpan.Zero;
                TotalSpaceWasted = 0;

                var startTime = DateTime.Now;

                var groups = await _duplicateDetector.DetectDuplicatesAsync(SearchResults, DetectionOptions);

                ElapsedTime = DateTime.Now - startTime;
                TotalSpaceWasted = groups.Sum(g => g.SpaceWasted);

                DuplicateGroups.Clear();
                foreach (var group in groups)
                {
                    DuplicateGroups.Add(group);
                }

                StatusMessage = $"Détection terminée - {DuplicateGroups.Count} groupe(s) de doublons trouvé(s)";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors de la détection: {ex.Message}";
            }
            finally
            {
                IsDetecting = false;
            }
        }

        private async Task ExecuteExportDuplicatesAsync()
        {
            try
            {
                StatusMessage = "Export des doublons en cours...";

                var options = new ExportOptions
                {
                    FileName = $"doublons_{DateTime.Now:yyyyMMdd_HHmmss}",
                    IncludeMetadata = true,
                    IncludeStatistics = true,
                    FileSizeFormat = FileSizeFormat.Formatted
                };

                var exportData = await _exportService.ExportDuplicateGroupsAsync(DuplicateGroups, ExportFormat.Csv, options);

                // TODO: Sauvegarder le fichier
                StatusMessage = $"Export terminé - {exportData.Length} octets générés";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors de l'export: {ex.Message}";
            }
        }

        private void ExecuteClearDuplicates()
        {
            DuplicateGroups.Clear();
            SelectedFiles.Clear();
            StatusMessage = "Liste des doublons effacée";
        }

        private void ExecuteDeleteSelected()
        {
            // TODO: Implémenter la suppression des fichiers sélectionnés
            StatusMessage = "Suppression des fichiers sélectionnés - Fonctionnalité à implémenter";
        }

        private void OnDetectionProgress(object? sender, DuplicateDetectionProgressEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                FilesProcessed = e.FilesProcessed;
                TotalFiles = e.TotalFiles;
                ElapsedTime = e.ElapsedTime;
                StatusMessage = $"Traitement: {e.FilesProcessed}/{e.TotalFiles} fichiers - {e.DuplicateGroupsFound} groupes trouvés";
            });
        }

        private void OnDuplicateGroupFound(object? sender, DuplicateGroupFoundEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                DuplicateGroups.Add(e.Group);
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
