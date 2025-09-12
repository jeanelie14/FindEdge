using System;
using System.Collections.Generic;
using System.Linq;
using FindEdge.Core.Interfaces;

namespace FindEdge.Infrastructure.Services
{
    /// <summary>
    /// Registre des parseurs de contenu avec gestion des priorités et plugins
    /// </summary>
    public class ContentParserRegistry : IContentParserRegistry
    {
        private readonly List<IContentParser> _parsers = new();
        private readonly IPluginManager? _pluginManager;

        public ContentParserRegistry(IPluginManager? pluginManager = null)
        {
            _pluginManager = pluginManager;
        }

        public void RegisterParser(IContentParser parser)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            _parsers.Add(parser);
            _parsers.Sort((x, y) => y.Priority.CompareTo(x.Priority));
        }

        public IContentParser? GetParser(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            // Chercher d'abord dans les parseurs intégrés
            var integratedParser = _parsers.FirstOrDefault(parser => parser.CanParse(filePath));
            if (integratedParser != null)
                return integratedParser;

            // Chercher dans les plugins si disponible
            if (_pluginManager != null)
            {
                var pluginParser = _pluginManager.GetContentParserPlugins()
                    .FirstOrDefault(plugin => plugin.CanParse(filePath));
                if (pluginParser != null)
                    return pluginParser;
            }

            return null;
        }

        public IEnumerable<IContentParser> GetAllParsers()
        {
            var allParsers = new List<IContentParser>(_parsers);
            
            if (_pluginManager != null)
            {
                allParsers.AddRange(_pluginManager.GetContentParserPlugins());
            }

            return allParsers.OrderByDescending(p => p.Priority);
        }
    }
}
