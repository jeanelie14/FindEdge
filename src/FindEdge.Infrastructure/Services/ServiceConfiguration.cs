using FindEdge.Core.Interfaces;
using FindEdge.Infrastructure.Parsers;
using FindEdge.Infrastructure.Services;

namespace FindEdge.Infrastructure.Services
{
    /// <summary>
    /// Configuration des services FindEdge
    /// </summary>
    public static class ServiceConfiguration
    {
        /// <summary>
        /// Configure tous les services nécessaires
        /// </summary>
        public static void ConfigureServices(IServiceContainer container)
        {
            // Enregistrer les parseurs de contenu
            var contentParserRegistry = new ContentParserRegistry();
            contentParserRegistry.RegisterParser(new TextFileParser());
            contentParserRegistry.RegisterParser(new PdfParser());
            contentParserRegistry.RegisterParser(new OfficeParser());
            contentParserRegistry.RegisterParser(new ArchiveParser());
            
            container.Register<IContentParserRegistry>(contentParserRegistry);

            // Enregistrer le gestionnaire de plugins
            var pluginManager = new PluginManager();
            container.Register<IPluginManager>(pluginManager);

            // Enregistrer le scanner de fichiers
            var fileScanner = new FileScanner();
            container.Register<IFileScanner>(fileScanner);

            // Enregistrer le gestionnaire d'index
            var indexManager = new SimpleIndexManager(fileScanner, contentParserRegistry);
            container.Register<IIndexManager>(indexManager);

            // Enregistrer le moteur de recherche live
            var liveSearchEngine = new FindEdge.Core.Services.LiveSearchEngine(contentParserRegistry);
            container.Register<ISearchEngine>(liveSearchEngine);

            // Enregistrer le moteur de recherche hybride
            var searchEngine = new FindEdge.Core.Services.HybridSearchEngine(liveSearchEngine, indexManager);
            container.Register<ISearchEngine>(searchEngine);
        }
    }
}
