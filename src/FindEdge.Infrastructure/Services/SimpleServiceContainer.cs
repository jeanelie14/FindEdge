using System;
using System.Collections.Generic;
using FindEdge.Core.Interfaces;
using FindEdge.Core.Services;
using FindEdge.Infrastructure.Parsers;

namespace FindEdge.Infrastructure.Services
{
    /// <summary>
    /// Implémentation simple du conteneur de services
    /// </summary>
    public class SimpleServiceContainer : IServiceContainer
    {
        private readonly Dictionary<Type, object> _services = new();
        private readonly Dictionary<Type, Func<object>> _factories = new();

        public void Register<T>(T service) where T : class
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _services[typeof(T)] = service;
        }

        public void Register<T>(Func<T> factory) where T : class
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _factories[typeof(T)] = () => factory();
        }

        public T Resolve<T>() where T : class
        {
            var type = typeof(T);

            // Chercher d'abord dans les services enregistrés
            if (_services.TryGetValue(type, out var service))
                return (T)service;

            // Chercher dans les factories
            if (_factories.TryGetValue(type, out var factory))
                return (T)factory();

            throw new InvalidOperationException($"Service de type {type.Name} non enregistré");
        }

        public bool IsRegistered<T>() where T : class
        {
            var type = typeof(T);
            return _services.ContainsKey(type) || _factories.ContainsKey(type);
        }

        public T Get<T>() where T : class
        {
            return Resolve<T>();
        }

        public void RegisterAllServices()
        {
            // Enregistrer les services de base
            Register<IFileScanner>(() => new FileScanner());
            Register<IPluginManager>(() => new PluginManager());
            Register<IContentParserRegistry>(() => new ContentParserRegistry(Resolve<IPluginManager>()));
            Register<IIndexManager>(() => new SimpleIndexManager(Resolve<IFileScanner>(), Resolve<IContentParserRegistry>()));
            Register<ISearchEngine>(() => new LiveSearchEngine(Resolve<IContentParserRegistry>()));
            Register<IHybridSearchEngine>(() => new HybridSearchEngine(Resolve<ISearchEngine>(), Resolve<IIndexManager>()));
            Register<IIndexedSearchEngine>(() => new MockIndexedSearchEngine());
            Register<IDuplicateDetector>(() => new DuplicateDetector(Resolve<IContentParserRegistry>()));
            
            // Enregistrer les services avancés
            Register<ISemanticSearchEngine>(() => new SemanticSearchEngine(
                Resolve<ISearchEngine>(),
                new TextFileParser()
            ));
            Register<IIntelligentSuggestions>(() => new IntelligentSuggestionsService(Resolve<ISearchEngine>(), Resolve<IIndexManager>()));
            Register<IVisualizationService>(() => new VisualizationService());
            Register<ICollaborationService>(() => new MockCollaborationService());
            Register<IHelpLearningService>(() => new MockHelpLearningService());
            
            // Register missing services with mock implementations
            Register<IAnalyticsService>(() => new MockAnalyticsService());
            Register<IPersonalizationService>(() => new MockPersonalizationService());
            // Register<IExportService>(() => new MockExportService());
        }

        public void Dispose()
        {
            // Nettoyer les services si nécessaire
            foreach (var service in _services.Values)
            {
                if (service is IDisposable disposable)
                    disposable.Dispose();
            }
            
            _services.Clear();
            _factories.Clear();
        }
    }
}
