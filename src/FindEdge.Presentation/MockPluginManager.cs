using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;
using FindEdge.Core.Models;

namespace FindEdge.Presentation
{
    /// <summary>
    /// Mock du gestionnaire de plugins pour les tests
    /// </summary>
    public class MockPluginManager : IPluginManager
    {
        private readonly List<IPlugin> _plugins = new();

        public MockPluginManager()
        {
            // Ajouter quelques plugins mock pour les tests
            _plugins.Add(new MockContentParserPlugin("PDF Parser", "1.0.0", "Parseur PDF avancé", "FindEdge Team"));
            _plugins.Add(new MockContentParserPlugin("Office Parser", "1.0.0", "Parseur Microsoft Office", "FindEdge Team"));
            _plugins.Add(new MockExportPlugin("CSV Export", "1.0.0", "Export vers CSV", "FindEdge Team"));
            _plugins.Add(new MockExportPlugin("JSON Export", "1.0.0", "Export vers JSON", "FindEdge Team"));
        }

        public event EventHandler<PluginLoadedEventArgs>? PluginLoaded;
        public event EventHandler<PluginUnloadedEventArgs>? PluginUnloaded;
        public event EventHandler<PluginErrorEventArgs>? PluginError;

        public async Task LoadPluginsAsync()
        {
            // Simuler le chargement des plugins
            await Task.Delay(100);
        }

        public async Task<IPlugin> LoadPluginAsync(string pluginPath)
        {
            await Task.Delay(50);
            throw new NotImplementedException("Mock ne supporte pas le chargement de plugins externes");
        }

        public async Task UnloadPluginAsync(string pluginName)
        {
            await Task.Delay(50);
            var plugin = _plugins.FirstOrDefault(p => p.Name == pluginName);
            if (plugin != null)
            {
                _plugins.Remove(plugin);
                OnPluginUnloaded(new PluginUnloadedEventArgs { PluginName = pluginName });
            }
        }

        public IEnumerable<IPlugin> GetAllPlugins()
        {
            return _plugins.ToList();
        }

        public IEnumerable<IContentParserPlugin> GetContentParserPlugins()
        {
            return _plugins.OfType<IContentParserPlugin>().ToList();
        }

        public IEnumerable<ISearchEnginePlugin> GetSearchEnginePlugins()
        {
            return _plugins.OfType<ISearchEnginePlugin>().ToList();
        }

        public IEnumerable<IExportPlugin> GetExportPlugins()
        {
            return _plugins.OfType<IExportPlugin>().ToList();
        }

        public IPlugin? GetPlugin(string name)
        {
            return _plugins.FirstOrDefault(p => p.Name == name);
        }

        public bool IsPluginLoaded(string name)
        {
            return _plugins.Any(p => p.Name == name);
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
    }

    /// <summary>
    /// Mock d'un plugin de parseur de contenu
    /// </summary>
    public class MockContentParserPlugin : IContentParserPlugin
    {
        public string Name { get; }
        public string Version { get; }
        public string Description { get; }
        public string Author { get; }
        public string[] SupportedExtensions { get; } = new[] { ".pdf", ".docx", ".txt" };
        public int Priority { get; } = 100;
        public Dictionary<string, object> Configuration { get; set; } = new();

        public MockContentParserPlugin(string name, string version, string description, string author)
        {
            Name = name;
            Version = version;
            Description = description;
            Author = author;
        }

        public void Initialize()
        {
            // Mock initialization
        }

        public void Dispose()
        {
            // Mock disposal
        }

        public bool IsCompatible(string applicationVersion)
        {
            return true;
        }

        public bool CanParse(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            var extension = System.IO.Path.GetExtension(filePath).ToLowerInvariant();
            return SupportedExtensions.Contains(extension);
        }

        public async Task<string> ExtractContentAsync(string filePath, System.Threading.CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);
            return $"Contenu extrait par {Name} depuis {filePath}";
        }
    }

    /// <summary>
    /// Mock d'un plugin d'export
    /// </summary>
    public class MockExportPlugin : IExportPlugin
    {
        public string Name { get; }
        public string Version { get; }
        public string Description { get; }
        public string Author { get; }
        public string[] SupportedExportFormats { get; } = new[] { "csv", "json", "xml" };

        public MockExportPlugin(string name, string version, string description, string author)
        {
            Name = name;
            Version = version;
            Description = description;
            Author = author;
        }

        public void Initialize()
        {
            // Mock initialization
        }

        public void Dispose()
        {
            // Mock disposal
        }

        public bool IsCompatible(string applicationVersion)
        {
            return true;
        }

        public async Task<byte[]> ExportAsync(IEnumerable<SearchResult> results, string format, Dictionary<string, object>? options = null)
        {
            await Task.Delay(100);
            return System.Text.Encoding.UTF8.GetBytes($"Export {format} généré par {Name}");
        }
    }
}
