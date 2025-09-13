using System;
using System.Windows;
using FindEdge.Core.Interfaces;
using FindEdge.Infrastructure.Services;
using FindEdge.Presentation.ViewModels;
using FindEdge.Presentation.Views;

namespace FindEdge.Presentation
{
    /// <summary>
    /// Application principale de FindEdge Professional
    /// </summary>
    public partial class App : Application
    {
        private IServiceContainer _serviceContainer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Initialiser le conteneur de services
                InitializeServiceContainer();

                // Créer la fenêtre principale
                var mainWindow = CreateMainWindow();
                mainWindow.Show();

                // Créer la fenêtre des fonctionnalités avancées
                var advancedFeaturesWindow = CreateAdvancedFeaturesWindow();
                advancedFeaturesWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du démarrage de l'application: {ex.Message}", 
                              "Erreur", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void InitializeServiceContainer()
        {
            // Créer le conteneur de services
            _serviceContainer = new SimpleServiceContainer();

            // Enregistrer tous les services
            _serviceContainer.RegisterAllServices();

            // Configuration des services
            ConfigureServices();
        }

        private void ConfigureServices()
        {
            // Configuration des services de base
            // (Les services de base sont déjà configurés dans le projet existant)

            // Configuration des services avancés
            ConfigureAdvancedServices();
        }

        private void ConfigureAdvancedServices()
        {
            // Configuration du service de suggestions intelligentes
            var suggestionsService = _serviceContainer.Get<IIntelligentSuggestions>();
            if (suggestionsService != null)
            {
                // Configuration par défaut
            }

            // Configuration du service de recherche sémantique
            var semanticSearchService = _serviceContainer.Get<ISemanticSearchEngine>();
            if (semanticSearchService != null)
            {
                // Configuration par défaut
            }

            // Configuration du service de visualisation
            var visualizationService = _serviceContainer.Get<IVisualizationService>();
            if (visualizationService != null)
            {
                // Configuration par défaut
            }

            // Configuration du service d'analytics
            var analyticsService = _serviceContainer.Get<IAnalyticsService>();
            if (analyticsService != null)
            {
                // Configuration par défaut
            }

            // Configuration du service de collaboration
            var collaborationService = _serviceContainer.Get<ICollaborationService>();
            if (collaborationService != null)
            {
                // Configuration par défaut
            }

            // Configuration du service d'aide et apprentissage
            var helpLearningService = _serviceContainer.Get<IHelpLearningService>();
            if (helpLearningService != null)
            {
                // Configuration par défaut
            }

            // Configuration du service de personnalisation
            var personalizationService = _serviceContainer.Get<IPersonalizationService>();
            if (personalizationService != null)
            {
                // Configuration par défaut
            }
        }

        private MainWindow CreateMainWindow()
        {
            // Récupérer le service de recherche depuis le conteneur
            var searchEngine = _serviceContainer.Resolve<IIndexedSearchEngine>();
            
            // Créer le ViewModel principal
            var mainViewModel = new MainViewModel(searchEngine);

            // Créer la fenêtre principale
            return new MainWindow(mainViewModel);
        }

        private AdvancedFeaturesWindow CreateAdvancedFeaturesWindow()
        {
            // Créer le ViewModel des fonctionnalités avancées
            var advancedFeaturesViewModel = new AdvancedFeaturesViewModel(
                _serviceContainer.Get<ISemanticSearchEngine>(),
                _serviceContainer.Get<IIntelligentSuggestions>(),
                _serviceContainer.Get<IVisualizationService>(),
                _serviceContainer.Get<IAnalyticsService>(),
                _serviceContainer.Get<ICollaborationService>(),
                _serviceContainer.Get<IHelpLearningService>(),
                _serviceContainer.Get<IPersonalizationService>());

            // Créer la fenêtre des fonctionnalités avancées
            return new AdvancedFeaturesWindow(advancedFeaturesViewModel);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Nettoyage avant fermeture
            _serviceContainer?.Dispose();
            base.OnExit(e);
        }
    }
}