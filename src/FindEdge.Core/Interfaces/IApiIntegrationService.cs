using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour le service d'API et d'intégration
    /// </summary>
    public interface IApiIntegrationService
    {
        /// <summary>
        /// Configure l'API REST
        /// </summary>
        Task<RestApiConfiguration> ConfigureRestApiAsync(RestApiSettings settings, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient la configuration de l'API REST
        /// </summary>
        Task<RestApiConfiguration> GetRestApiConfigurationAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Exécute une recherche via l'API REST
        /// </summary>
        Task<ApiSearchResult> ExecuteApiSearchAsync(ApiSearchRequest request, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure l'interface CLI
        /// </summary>
        Task<CliInterface> ConfigureCliInterfaceAsync(CliConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Exécute une commande CLI
        /// </summary>
        Task<CliCommandResult> ExecuteCliCommandAsync(string command, Dictionary<string, object> parameters, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure l'interface web
        /// </summary>
        Task<WebInterface> ConfigureWebInterfaceAsync(WebInterfaceSettings settings, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient la configuration de l'interface web
        /// </summary>
        Task<WebInterface> GetWebInterfaceConfigurationAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure l'intégration avec des systèmes externes
        /// </summary>
        Task<ExternalIntegration> ConfigureExternalIntegrationAsync(ExternalIntegrationSettings settings, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les intégrations externes configurées
        /// </summary>
        Task<IEnumerable<ExternalIntegration>> GetExternalIntegrationsAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure l'intégration LDAP
        /// </summary>
        Task<LdapIntegration> ConfigureLdapIntegrationAsync(LdapSettings settings, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient la configuration LDAP
        /// </summary>
        Task<LdapIntegration> GetLdapConfigurationAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure l'intégration Active Directory
        /// </summary>
        Task<ActiveDirectoryIntegration> ConfigureActiveDirectoryIntegrationAsync(ActiveDirectorySettings settings, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient la configuration Active Directory
        /// </summary>
        Task<ActiveDirectoryIntegration> GetActiveDirectoryConfigurationAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure l'intégration avec des systèmes d'entreprise
        /// </summary>
        Task<EnterpriseIntegration> ConfigureEnterpriseIntegrationAsync(EnterpriseIntegrationSettings settings, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les intégrations d'entreprise configurées
        /// </summary>
        Task<IEnumerable<EnterpriseIntegration>> GetEnterpriseIntegrationsAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure l'import/export de configurations
        /// </summary>
        Task<ConfigurationImportExport> ConfigureConfigurationImportExportAsync(ImportExportSettings settings, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Exporte une configuration
        /// </summary>
        Task<byte[]> ExportConfigurationAsync(string configurationId, ExportFormat format, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Importe une configuration
        /// </summary>
        Task ImportConfigurationAsync(byte[] configuration, ImportFormat format, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure l'intégration avec des connecteurs
        /// </summary>
        Task<ConnectorIntegration> ConfigureConnectorIntegrationAsync(ConnectorSettings settings, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les connecteurs configurés
        /// </summary>
        Task<IEnumerable<ConnectorIntegration>> GetConnectorIntegrationsAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Événement déclenché lors de l'exécution d'une commande API
        /// </summary>
        event EventHandler<ApiCommandExecutedEventArgs>? ApiCommandExecuted;
        
        /// <summary>
        /// Événement déclenché lors de l'exécution d'une commande CLI
        /// </summary>
        event EventHandler<CliCommandExecutedEventArgs>? CliCommandExecuted;
    }

    /// <summary>
    /// Configuration de l'API REST
    /// </summary>
    public class RestApiConfiguration
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public RestApiSettings Settings { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public List<ApiEndpoint> Endpoints { get; set; } = new();
    }

    /// <summary>
    /// Paramètres de l'API REST
    /// </summary>
    public class RestApiSettings
    {
        public string BaseUrl { get; set; } = "https://api.findedge.com";
        public int Port { get; set; } = 8080;
        public bool EnableHttps { get; set; } = true;
        public bool EnableAuthentication { get; set; } = true;
        public string AuthenticationMethod { get; set; } = "Bearer";
        public bool EnableCors { get; set; } = true;
        public List<string> AllowedOrigins { get; set; } = new();
        public int RateLimit { get; set; } = 1000;
        public TimeSpan RateLimitWindow { get; set; } = TimeSpan.FromHours(1);
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Point de terminaison API
    /// </summary>
    public class ApiEndpoint
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Path { get; set; } = string.Empty;
        public string Method { get; set; } = "GET";
        public string Description { get; set; } = string.Empty;
        public List<ApiParameter> Parameters { get; set; } = new();
        public ApiResponse Response { get; set; } = new();
        public bool RequiresAuthentication { get; set; } = true;
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Paramètre API
    /// </summary>
    public class ApiParameter
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsRequired { get; set; } = false;
        public string Description { get; set; } = string.Empty;
        public object DefaultValue { get; set; } = new();
        public Dictionary<string, object> Validation { get; set; } = new();
    }

    /// <summary>
    /// Réponse API
    /// </summary>
    public class ApiResponse
    {
        public int StatusCode { get; set; } = 200;
        public string ContentType { get; set; } = "application/json";
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, object> Headers { get; set; } = new();
        public Dictionary<string, object> Schema { get; set; } = new();
    }

    /// <summary>
    /// Requête de recherche API
    /// </summary>
    public class ApiSearchRequest
    {
        public string Query { get; set; } = string.Empty;
        public SearchOptions Options { get; set; } = new();
        public string UserId { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Résultat de recherche API
    /// </summary>
    public class ApiSearchResult
    {
        public string RequestId { get; set; } = Guid.NewGuid().ToString();
        public IEnumerable<SearchResult> Results { get; set; } = new List<SearchResult>();
        public int TotalCount { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Interface CLI
    /// </summary>
    public class CliInterface
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public CliConfiguration Configuration { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public List<CliCommand> Commands { get; set; } = new();
    }

    /// <summary>
    /// Configuration CLI
    /// </summary>
    public class CliConfiguration
    {
        public string ExecutableName { get; set; } = "findedge";
        public string Version { get; set; } = "1.0.0";
        public bool EnableScripting { get; set; } = true;
        public bool EnableInteractiveMode { get; set; } = true;
        public string ScriptingLanguage { get; set; } = "PowerShell";
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Commande CLI
    /// </summary>
    public class CliCommand
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<CliParameter> Parameters { get; set; } = new();
        public CliCommandHandler Handler { get; set; } = new();
        public bool RequiresAuthentication { get; set; } = false;
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Paramètre CLI
    /// </summary>
    public class CliParameter
    {
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsRequired { get; set; } = false;
        public string Description { get; set; } = string.Empty;
        public object DefaultValue { get; set; } = new();
        public Dictionary<string, object> Validation { get; set; } = new();
    }

    /// <summary>
    /// Gestionnaire de commande CLI
    /// </summary>
    public class CliCommandHandler
    {
        public string Type { get; set; } = string.Empty;
        public string Assembly { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public Dictionary<string, object> Configuration { get; set; } = new();
    }

    /// <summary>
    /// Résultat de commande CLI
    /// </summary>
    public class CliCommandResult
    {
        public string CommandId { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
        public string Output { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public int ExitCode { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Interface web
    /// </summary>
    public class WebInterface
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public WebInterfaceSettings Settings { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public List<WebPage> Pages { get; set; } = new();
    }

    /// <summary>
    /// Paramètres de l'interface web
    /// </summary>
    public class WebInterfaceSettings
    {
        public string BaseUrl { get; set; } = "https://web.findedge.com";
        public int Port { get; set; } = 80;
        public bool EnableHttps { get; set; } = true;
        public bool EnableAuthentication { get; set; } = true;
        public string AuthenticationMethod { get; set; } = "JWT";
        public bool EnableCors { get; set; } = true;
        public List<string> AllowedOrigins { get; set; } = new();
        public string Theme { get; set; } = "default";
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Page web
    /// </summary>
    public class WebPage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Path { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<WebComponent> Components { get; set; } = new();
        public bool RequiresAuthentication { get; set; } = true;
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Composant web
    /// </summary>
    public class WebComponent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, object> Properties { get; set; } = new();
        public List<WebComponent> Children { get; set; } = new();
    }

    /// <summary>
    /// Intégration externe
    /// </summary>
    public class ExternalIntegration
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public ExternalIntegrationSettings Settings { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Paramètres d'intégration externe
    /// </summary>
    public class ExternalIntegrationSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public Dictionary<string, object> Configuration { get; set; } = new();
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Intégration LDAP
    /// </summary>
    public class LdapIntegration
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public LdapSettings Settings { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Paramètres LDAP
    /// </summary>
    public class LdapSettings
    {
        public string Server { get; set; } = string.Empty;
        public int Port { get; set; } = 389;
        public bool UseSsl { get; set; } = false;
        public string BaseDn { get; set; } = string.Empty;
        public string UserDn { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string UserFilter { get; set; } = string.Empty;
        public string GroupFilter { get; set; } = string.Empty;
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Intégration Active Directory
    /// </summary>
    public class ActiveDirectoryIntegration
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public ActiveDirectorySettings Settings { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Paramètres Active Directory
    /// </summary>
    public class ActiveDirectorySettings
    {
        public string Domain { get; set; } = string.Empty;
        public string Server { get; set; } = string.Empty;
        public int Port { get; set; } = 389;
        public bool UseSsl { get; set; } = false;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string BaseDn { get; set; } = string.Empty;
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Intégration d'entreprise
    /// </summary>
    public class EnterpriseIntegration
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public EnterpriseIntegrationSettings Settings { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Paramètres d'intégration d'entreprise
    /// </summary>
    public class EnterpriseIntegrationSettings
    {
        public string SystemName { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string ApiEndpoint { get; set; } = string.Empty;
        public string AuthenticationMethod { get; set; } = string.Empty;
        public Dictionary<string, object> Configuration { get; set; } = new();
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Import/Export de configuration
    /// </summary>
    public class ConfigurationImportExport
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public ImportExportSettings Settings { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Paramètres d'import/export
    /// </summary>
    public class ImportExportSettings
    {
        public List<ExportFormat> SupportedExportFormats { get; set; } = new();
        public List<ImportFormat> SupportedImportFormats { get; set; } = new();
        public bool EnableCompression { get; set; } = true;
        public bool EnableEncryption { get; set; } = false;
        public string EncryptionKey { get; set; } = string.Empty;
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }


    /// <summary>
    /// Format d'import
    /// </summary>
    public enum ImportFormat
    {
        Json,
        Xml,
        Yaml,
        Binary
    }

    /// <summary>
    /// Intégration de connecteur
    /// </summary>
    public class ConnectorIntegration
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public ConnectorSettings Settings { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Paramètres de connecteur
    /// </summary>
    public class ConnectorSettings
    {
        public string ConnectorName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string AssemblyPath { get; set; } = string.Empty;
        public string Configuration { get; set; } = string.Empty;
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    // Événements
    public class ApiCommandExecutedEventArgs : EventArgs
    {
        public string CommandId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> Parameters { get; set; } = new();
    }

    public class CliCommandExecutedEventArgs : EventArgs
    {
        public string CommandId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
}
