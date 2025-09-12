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
    /// ViewModel pour la gestion de l'indexation
    /// </summary>
    public class IndexViewModel : INotifyPropertyChanged
    {
        private readonly IIndexManager _indexManager;
        private CancellationTokenSource? _cancellationTokenSource;

        public IndexViewModel(IIndexManager indexManager)
        {
            _indexManager = indexManager ?? throw new ArgumentNullException(nameof(indexManager));
            
            // Initialiser les commandes
            BuildIndexCommand = new RelayCommand(async () => await ExecuteBuildIndexAsync(), () => !IsIndexing);
            UpdateIndexCommand = new RelayCommand(async () => await ExecuteUpdateIndexAsync(), () => !IsIndexing && IsIndexAvailable);
            DeleteIndexCommand = new RelayCommand(async () => await ExecuteDeleteIndexAsync(), () => !IsIndexing);
            RefreshStatusCommand = new RelayCommand(async () => await ExecuteRefreshStatusAsync());

            // Initialiser la configuration
            IndexConfiguration = new IndexConfiguration
            {
                IndexedDirectories = new() { Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) },
                IndexContent = true,
                IndexMetadata = true,
                MaxFileSize = 50 * 1024 * 1024, // 50 MB
                MaxContentLength = 100000, // 100K caractères
                EnableIncrementalIndexing = true
            };

            // S'abonner aux événements
            _indexManager.IndexProgress += OnIndexProgress;
            _indexManager.IndexCompleted += OnIndexCompleted;
            _indexManager.IndexError += OnIndexError;

            // Charger le statut initial
            _ = Task.Run(async () => await ExecuteRefreshStatusAsync());
        }

        #region Propriétés

        private bool _isIndexing;
        public bool IsIndexing
        {
            get => _isIndexing;
            set
            {
                if (SetProperty(ref _isIndexing, value))
                {
                    ((RelayCommand)BuildIndexCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)UpdateIndexCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)DeleteIndexCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private bool _isIndexAvailable;
        public bool IsIndexAvailable
        {
            get => _isIndexAvailable;
            set
            {
                if (SetProperty(ref _isIndexAvailable, value))
                {
                    ((RelayCommand)UpdateIndexCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private string _statusMessage = "Chargement du statut de l'index...";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private int _documentCount;
        public int DocumentCount
        {
            get => _documentCount;
            set => SetProperty(ref _documentCount, value);
        }

        private long _indexSize;
        public long IndexSize
        {
            get => _indexSize;
            set => SetProperty(ref _indexSize, value);
        }

        private DateTime _lastUpdated;
        public DateTime LastUpdated
        {
            get => _lastUpdated;
            set => SetProperty(ref _lastUpdated, value);
        }

        private int _progressPercentage;
        public int ProgressPercentage
        {
            get => _progressPercentage;
            set => SetProperty(ref _progressPercentage, value);
        }

        private string _currentFile = string.Empty;
        public string CurrentFile
        {
            get => _currentFile;
            set => SetProperty(ref _currentFile, value);
        }

        private double _indexingSpeed;
        public double IndexingSpeed
        {
            get => _indexingSpeed;
            set => SetProperty(ref _indexingSpeed, value);
        }

        private TimeSpan _estimatedTimeRemaining;
        public TimeSpan EstimatedTimeRemaining
        {
            get => _estimatedTimeRemaining;
            set => SetProperty(ref _estimatedTimeRemaining, value);
        }

        public IndexConfiguration IndexConfiguration { get; }

        public ObservableCollection<string> IndexedDirectories { get; } = new();
        public ObservableCollection<string> ExcludedDirectories { get; } = new();

        #endregion

        #region Commandes

        public ICommand BuildIndexCommand { get; }
        public ICommand UpdateIndexCommand { get; }
        public ICommand DeleteIndexCommand { get; }
        public ICommand RefreshStatusCommand { get; }

        #endregion

        #region Méthodes privées

        private async Task ExecuteBuildIndexAsync()
        {
            try
            {
                IsIndexing = true;
                StatusMessage = "Construction de l'index en cours...";
                ProgressPercentage = 0;

                _cancellationTokenSource = new CancellationTokenSource();
                await _indexManager.BuildIndexAsync(IndexConfiguration, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                StatusMessage = "Construction de l'index annulée";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors de la construction de l'index : {ex.Message}";
            }
            finally
            {
                IsIndexing = false;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
                await ExecuteRefreshStatusAsync();
            }
        }

        private async Task ExecuteUpdateIndexAsync()
        {
            try
            {
                IsIndexing = true;
                StatusMessage = "Mise à jour de l'index en cours...";

                _cancellationTokenSource = new CancellationTokenSource();
                await _indexManager.UpdateIndexAsync(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                StatusMessage = "Mise à jour de l'index annulée";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors de la mise à jour de l'index : {ex.Message}";
            }
            finally
            {
                IsIndexing = false;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
                await ExecuteRefreshStatusAsync();
            }
        }

        private async Task ExecuteDeleteIndexAsync()
        {
            try
            {
                await _indexManager.DeleteIndexAsync();
                StatusMessage = "Index supprimé avec succès";
                await ExecuteRefreshStatusAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors de la suppression de l'index : {ex.Message}";
            }
        }

        private async Task ExecuteRefreshStatusAsync()
        {
            try
            {
                var status = await _indexManager.GetIndexStatusAsync();
                
                IsIndexAvailable = status.IsAvailable;
                DocumentCount = status.DocumentCount;
                IndexSize = status.IndexSize;
                LastUpdated = status.LastUpdated;
                ProgressPercentage = status.ProgressPercentage;

                if (status.IsAvailable)
                {
                    StatusMessage = $"Index disponible - {DocumentCount:N0} document(s), {FormatFileSize(IndexSize)}";
                }
                else
                {
                    StatusMessage = "Aucun index disponible";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors du chargement du statut : {ex.Message}";
            }
        }

        private void OnIndexProgress(object? sender, IndexProgressEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                ProgressPercentage = (int)((double)e.DocumentsProcessed / e.TotalDocuments * 100);
                CurrentFile = e.CurrentFile;
                IndexingSpeed = e.Speed;
                EstimatedTimeRemaining = e.EstimatedTimeRemaining;
                StatusMessage = $"Indexation en cours... {e.DocumentsProcessed:N0}/{e.TotalDocuments:N0} documents";
            });
        }

        private void OnIndexCompleted(object? sender, IndexCompletedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (e.Success)
                {
                    StatusMessage = $"Indexation terminée - {e.TotalDocuments:N0} document(s) indexés en {e.TotalTime:hh\\:mm\\:ss}";
                }
                else
                {
                    StatusMessage = "Erreur lors de l'indexation";
                }
                ProgressPercentage = 100;
            });
        }

        private void OnIndexError(object? sender, IndexErrorEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                StatusMessage = $"Erreur d'indexation : {e.ErrorMessage}";
            });
        }

        private string FormatFileSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return $"{number:n1} {suffixes[counter]}";
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
