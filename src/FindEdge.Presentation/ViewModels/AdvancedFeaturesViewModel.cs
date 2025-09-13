using System;
using System.Collections.Generic;
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
    /// ViewModel pour les fonctionnalités avancées de FindEdge Professional
    /// </summary>
    public class AdvancedFeaturesViewModel : ViewModelBase
    {
        private readonly ISemanticSearchEngine _semanticSearchEngine;
        private readonly IIntelligentSuggestions _intelligentSuggestions;
        private readonly IVisualizationService _visualizationService;
        private readonly IAnalyticsService _analyticsService;
        private readonly ICollaborationService _collaborationService;
        private readonly IHelpLearningService _helpLearningService;
        private readonly IPersonalizationService _personalizationService;

        public AdvancedFeaturesViewModel(
            ISemanticSearchEngine semanticSearchEngine,
            IIntelligentSuggestions intelligentSuggestions,
            IVisualizationService visualizationService,
            IAnalyticsService analyticsService,
            ICollaborationService collaborationService,
            IHelpLearningService helpLearningService,
            IPersonalizationService personalizationService)
        {
            _semanticSearchEngine = semanticSearchEngine ?? throw new ArgumentNullException(nameof(semanticSearchEngine));
            _intelligentSuggestions = intelligentSuggestions ?? throw new ArgumentNullException(nameof(intelligentSuggestions));
            _visualizationService = visualizationService ?? throw new ArgumentNullException(nameof(visualizationService));
            _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
            _collaborationService = collaborationService ?? throw new ArgumentNullException(nameof(collaborationService));
            _helpLearningService = helpLearningService ?? throw new ArgumentNullException(nameof(helpLearningService));
            _personalizationService = personalizationService ?? throw new ArgumentNullException(nameof(personalizationService));

            // Initialiser les collections
            SearchSuggestions = new ObservableCollection<SearchSuggestion>();
            SearchResults = new ObservableCollection<SearchResult>();
            Visualizations = new ObservableCollection<object>();
            AnalyticsData = new ObservableCollection<object>();
            CollaborationItems = new ObservableCollection<object>();
            HelpItems = new ObservableCollection<object>();

            // Initialiser les commandes
            InitializeCommands();

            // S'abonner aux événements
            SubscribeToEvents();
        }

        #region Propriétés

        private string _searchQuery = string.Empty;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (SetProperty(ref _searchQuery, value))
                {
                    OnSearchQueryChanged();
                }
            }
        }

        private string _naturalLanguageQuery = string.Empty;
        public string NaturalLanguageQuery
        {
            get => _naturalLanguageQuery;
            set => SetProperty(ref _naturalLanguageQuery, value);
        }

        private bool _isSemanticSearchEnabled = true;
        public bool IsSemanticSearchEnabled
        {
            get => _isSemanticSearchEnabled;
            set => SetProperty(ref _isSemanticSearchEnabled, value);
        }

        private bool _isVisualizationEnabled = true;
        public bool IsVisualizationEnabled
        {
            get => _isVisualizationEnabled;
            set => SetProperty(ref _isVisualizationEnabled, value);
        }

        private bool _isCollaborationEnabled = false;
        public bool IsCollaborationEnabled
        {
            get => _isCollaborationEnabled;
            set => SetProperty(ref _isCollaborationEnabled, value);
        }

        private string _selectedVisualizationType = "Mosaic";
        public string SelectedVisualizationType
        {
            get => _selectedVisualizationType;
            set => SetProperty(ref _selectedVisualizationType, value);
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ObservableCollection<SearchSuggestion> SearchSuggestions { get; }
        public ObservableCollection<SearchResult> SearchResults { get; }
        public ObservableCollection<object> Visualizations { get; }
        public ObservableCollection<object> AnalyticsData { get; }
        public ObservableCollection<object> CollaborationItems { get; }
        public ObservableCollection<object> HelpItems { get; }

        #endregion

        #region Commandes

        public ICommand SearchCommand { get; private set; }
        public ICommand SemanticSearchCommand { get; private set; }
        public ICommand NaturalLanguageSearchCommand { get; private set; }
        public ICommand GenerateVisualizationCommand { get; private set; }
        public ICommand GenerateAnalyticsCommand { get; private set; }
        public ICommand ShareSearchCommand { get; private set; }
        public ICommand GetHelpCommand { get; private set; }
        public ICommand PersonalizeInterfaceCommand { get; private set; }
        public ICommand ClearSuggestionsCommand { get; private set; }

        #endregion

        #region Méthodes privées

        private void InitializeCommands()
        {
            SearchCommand = new RelayCommandSimple(async () => await ExecuteSearchAsync());
            SemanticSearchCommand = new RelayCommandSimple(async () => await ExecuteSemanticSearchAsync());
            NaturalLanguageSearchCommand = new RelayCommandSimple(async () => await ExecuteNaturalLanguageSearchAsync());
            GenerateVisualizationCommand = new RelayCommandSimple(async () => await GenerateVisualizationAsync());
            GenerateAnalyticsCommand = new RelayCommandSimple(async () => await GenerateAnalyticsAsync());
            ShareSearchCommand = new RelayCommandSimple(async () => await ShareSearchAsync());
            GetHelpCommand = new RelayCommandSimple(async () => await GetHelpAsync());
            PersonalizeInterfaceCommand = new RelayCommandSimple(async () => await PersonalizeInterfaceAsync());
            ClearSuggestionsCommand = new RelayCommandSimple(() => ClearSuggestions());
        }

        private void SubscribeToEvents()
        {
            // S'abonner aux événements des services
            _semanticSearchEngine.SemanticSearchProgress += OnSemanticSearchProgress;
        }

        private async void OnSearchQueryChanged()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                SearchSuggestions.Clear();
                return;
            }

            try
            {
                // Obtenir des suggestions d'autocomplétion
                var suggestions = await _intelligentSuggestions.GetAutocompleteSuggestionsAsync(
                    SearchQuery, 
                    maxSuggestions: 10, 
                    CancellationToken.None);

                SearchSuggestions.Clear();
                foreach (var suggestion in suggestions)
                {
                    SearchSuggestions.Add(suggestion);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors de la récupération des suggestions: {ex.Message}";
            }
        }

        private async Task ExecuteSearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
                return;

            IsLoading = true;
            StatusMessage = "Recherche en cours...";

            try
            {
                var searchOptions = new SearchOptions
                {
                    SearchTerm = SearchQuery,
                    SearchInFileName = true,
                    SearchInContent = true,
                    MaxResults = 1000
                };

                var results = await _semanticSearchEngine.SearchBySimilarityAsync(
                    SearchQuery, 
                    searchOptions, 
                    CancellationToken.None);

                SearchResults.Clear();
                foreach (var result in results)
                {
                    SearchResults.Add(result);
                }

                StatusMessage = $"Recherche terminée: {results.Count()} résultats trouvés";

                // Apprendre de cette recherche
                var searchContext = new SearchContext
                {
                    Query = SearchQuery,
                    UserId = Environment.UserName,
                    SessionId = Guid.NewGuid().ToString(),
                    ResultCount = results.Count(),
                    WasSuccessful = true
                };

                await _intelligentSuggestions.LearnFromSearchAsync(SearchQuery, searchContext, CancellationToken.None);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors de la recherche: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExecuteSemanticSearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
                return;

            IsLoading = true;
            StatusMessage = "Recherche sémantique en cours...";

            try
            {
                var searchOptions = new SearchOptions
                {
                    SearchTerm = SearchQuery,
                    SearchInFileName = true,
                    SearchInContent = true,
                    MaxResults = 1000
                };

                var results = await _semanticSearchEngine.SearchBySimilarityAsync(
                    SearchQuery, 
                    searchOptions, 
                    CancellationToken.None);

                SearchResults.Clear();
                foreach (var result in results)
                {
                    SearchResults.Add(result);
                }

                StatusMessage = $"Recherche sémantique terminée: {results.Count()} résultats trouvés";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors de la recherche sémantique: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExecuteNaturalLanguageSearchAsync()
        {
            if (string.IsNullOrWhiteSpace(NaturalLanguageQuery))
                return;

            IsLoading = true;
            StatusMessage = "Recherche en langage naturel en cours...";

            try
            {
                var searchOptions = new SearchOptions
                {
                    SearchTerm = NaturalLanguageQuery,
                    SearchInFileName = true,
                    SearchInContent = true,
                    MaxResults = 1000
                };

                var results = await _semanticSearchEngine.SearchByNaturalLanguageAsync(
                    NaturalLanguageQuery, 
                    searchOptions, 
                    CancellationToken.None);

                SearchResults.Clear();
                foreach (var result in results)
                {
                    SearchResults.Add(result);
                }

                StatusMessage = $"Recherche en langage naturel terminée: {results.Count()} résultats trouvés";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors de la recherche en langage naturel: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task GenerateVisualizationAsync()
        {
            if (!SearchResults.Any())
                return;

            IsLoading = true;
            StatusMessage = "Génération de la visualisation...";

            try
            {
                // Mock visualization generation
                var visualization = new { Type = SelectedVisualizationType, Data = "Mock visualization data" };
                
                Visualizations.Clear();
                Visualizations.Add(visualization);
                StatusMessage = "Visualisation générée avec succès";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors de la génération de la visualisation: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task GenerateAnalyticsAsync()
        {
            if (!SearchResults.Any())
                return;

            IsLoading = true;
            StatusMessage = "Génération des analytics...";

            try
            {
                var filePaths = SearchResults.Select(r => r.FilePath);
                var contentAnalysis = await _analyticsService.AnalyzeContentAsync(
                    filePaths, 
                    CancellationToken.None);

                AnalyticsData.Clear();
                AnalyticsData.Add(contentAnalysis);

                StatusMessage = "Analytics générés avec succès";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors de la génération des analytics: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ShareSearchAsync()
        {
            if (!SearchResults.Any())
                return;

            IsLoading = true;
            StatusMessage = "Partage de la recherche...";

            try
            {
                var searchId = Guid.NewGuid().ToString();
                var userIds = new[] { Environment.UserName };

                await _collaborationService.ShareSearchQueryAsync(
                    searchId, 
                    userIds, 
                    new SharePermissions { CanView = true, CanEdit = false }, 
                    CancellationToken.None);

                StatusMessage = "Recherche partagée avec succès";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors du partage: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task GetHelpAsync()
        {
            IsLoading = true;
            StatusMessage = "Récupération de l'aide...";

            try
            {
                var tutorial = await _helpLearningService.GetInteractiveTutorialAsync(
                    "Recherche", 
                    CancellationToken.None);

                HelpItems.Clear();
                HelpItems.Add(tutorial);

                StatusMessage = "Aide récupérée avec succès";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors de la récupération de l'aide: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task PersonalizeInterfaceAsync()
        {
            IsLoading = true;
            StatusMessage = "Personnalisation de l'interface...";

            try
            {
                await _personalizationService.ApplyThemeAsync("Default", CancellationToken.None);
                StatusMessage = "Interface personnalisée avec succès";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur lors de la personnalisation: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ClearSuggestions()
        {
            SearchSuggestions.Clear();
            StatusMessage = "Suggestions effacées";
        }

        #endregion

        #region Gestionnaires d'événements

        private void OnSemanticSearchProgress(object sender, SemanticSearchProgressEventArgs e)
        {
            StatusMessage = $"Recherche sémantique: {e.FilesProcessed}/{e.TotalFiles} fichiers traités";
        }

        #endregion

        public override void Dispose()
        {
            // Nettoyer les ressources si nécessaire
            base.Dispose();
        }
    }

    /// <summary>
    /// Classe de base pour les ViewModels avec support IDisposable
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public virtual void Dispose()
        {
            // Override in derived classes if needed
        }
    }

}