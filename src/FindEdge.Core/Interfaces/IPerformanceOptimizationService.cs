using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour le service d'optimisation et de performance
    /// </summary>
    public interface IPerformanceOptimizationService
    {
        /// <summary>
        /// Configure le cache intelligent
        /// </summary>
        Task<IntelligentCache> ConfigureIntelligentCacheAsync(CacheConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les statistiques du cache
        /// </summary>
        Task<CacheStatistics> GetCacheStatisticsAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Vide le cache
        /// </summary>
        Task ClearCacheAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure la recherche incrémentale
        /// </summary>
        Task<IncrementalSearch> ConfigureIncrementalSearchAsync(IncrementalSearchConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les résultats de recherche incrémentale
        /// </summary>
        Task<IEnumerable<SearchResult>> GetIncrementalSearchResultsAsync(string query, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure la compression des index
        /// </summary>
        Task<IndexCompression> ConfigureIndexCompressionAsync(CompressionConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les statistiques de compression
        /// </summary>
        Task<CompressionStatistics> GetCompressionStatisticsAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure le mode économie d'énergie
        /// </summary>
        Task<PowerSavingMode> ConfigurePowerSavingModeAsync(PowerSavingConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient le statut du mode économie d'énergie
        /// </summary>
        Task<PowerSavingStatus> GetPowerSavingStatusAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure la recherche en arrière-plan
        /// </summary>
        Task<BackgroundSearch> ConfigureBackgroundSearchAsync(BackgroundSearchConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient le statut de la recherche en arrière-plan
        /// </summary>
        Task<BackgroundSearchStatus> GetBackgroundSearchStatusAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure le tableau de bord d'administration
        /// </summary>
        Task<AdminDashboard> ConfigureAdminDashboardAsync(AdminDashboardConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les données du tableau de bord d'administration
        /// </summary>
        Task<AdminDashboardData> GetAdminDashboardDataAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure les logs détaillés
        /// </summary>
        Task<DetailedLogging> ConfigureDetailedLoggingAsync(LoggingConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les logs pour une période donnée
        /// </summary>
        Task<IEnumerable<LogEntry>> GetLogsAsync(DateTime startDate, DateTime endDate, LogLevel level, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure les alertes système
        /// </summary>
        Task<SystemAlert> ConfigureSystemAlertAsync(AlertConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les alertes actives
        /// </summary>
        Task<IEnumerable<SystemAlert>> GetActiveAlertsAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure la sauvegarde automatique
        /// </summary>
        Task<AutomaticBackup> ConfigureAutomaticBackupAsync(BackupConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient le statut de la sauvegarde
        /// </summary>
        Task<BackupStatus> GetBackupStatusAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure le mode diagnostic
        /// </summary>
        Task<DiagnosticMode> ConfigureDiagnosticModeAsync(DiagnosticConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Exécute un diagnostic complet
        /// </summary>
        Task<DiagnosticReport> RunFullDiagnosticAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les métriques de performance en temps réel
        /// </summary>
        Task<RealtimeMetrics> GetRealtimeMetricsAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Optimise les performances du système
        /// </summary>
        Task<PerformanceOptimization> OptimizeSystemPerformanceAsync(OptimizationOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Événement déclenché lors de la réception d'une alerte système
        /// </summary>
        event EventHandler<SystemAlertEventArgs>? SystemAlert;
        
        /// <summary>
        /// Événement déclenché lors de la mise à jour des métriques
        /// </summary>
        event EventHandler<MetricsUpdatedEventArgs>? MetricsUpdated;
    }

    /// <summary>
    /// Cache intelligent
    /// </summary>
    public class IntelligentCache
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public CacheConfiguration Configuration { get; set; } = new();
        public CacheStatistics Statistics { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Configuration de cache
    /// </summary>
    public class CacheConfiguration
    {
        public long MaxSize { get; set; } = 100 * 1024 * 1024; // 100 MB
        public TimeSpan DefaultTTL { get; set; } = TimeSpan.FromHours(1);
        public CacheEvictionPolicy EvictionPolicy { get; set; }
        public bool EnablePredictiveCaching { get; set; } = true;
        public bool EnableCompression { get; set; } = true;
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Politique d'éviction de cache
    /// </summary>
    public enum CacheEvictionPolicy
    {
        LeastRecentlyUsed,
        LeastFrequentlyUsed,
        FirstInFirstOut,
        Random
    }

    /// <summary>
    /// Statistiques de cache
    /// </summary>
    public class CacheStatistics
    {
        public long TotalSize { get; set; }
        public int ItemCount { get; set; }
        public int HitCount { get; set; }
        public int MissCount { get; set; }
        public double HitRate { get; set; }
        public TimeSpan AverageAccessTime { get; set; }
        public Dictionary<string, object> CustomMetrics { get; set; } = new();
    }

    /// <summary>
    /// Recherche incrémentale
    /// </summary>
    public class IncrementalSearch
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public IncrementalSearchConfiguration Configuration { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Configuration de recherche incrémentale
    /// </summary>
    public class IncrementalSearchConfiguration
    {
        public int MinQueryLength { get; set; } = 2;
        public TimeSpan DebounceDelay { get; set; } = TimeSpan.FromMilliseconds(300);
        public int MaxResults { get; set; } = 50;
        public bool EnableFuzzyMatching { get; set; } = true;
        public bool EnableAutocomplete { get; set; } = true;
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Compression d'index
    /// </summary>
    public class IndexCompression
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public CompressionConfiguration Configuration { get; set; } = new();
        public CompressionStatistics Statistics { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Configuration de compression
    /// </summary>
    public class CompressionConfiguration
    {
        public CompressionAlgorithm Algorithm { get; set; }
        public int CompressionLevel { get; set; } = 6;
        public bool EnableDictionaryCompression { get; set; } = true;
        public bool EnableDeltaCompression { get; set; } = true;
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Algorithme de compression
    /// </summary>
    public enum CompressionAlgorithm
    {
        GZip,
        Deflate,
        Brotli,
        LZ4,
        Zstd
    }

    /// <summary>
    /// Statistiques de compression
    /// </summary>
    public class CompressionStatistics
    {
        public long OriginalSize { get; set; }
        public long CompressedSize { get; set; }
        public double CompressionRatio { get; set; }
        public TimeSpan CompressionTime { get; set; }
        public TimeSpan DecompressionTime { get; set; }
        public Dictionary<string, object> CustomMetrics { get; set; } = new();
    }

    /// <summary>
    /// Mode économie d'énergie
    /// </summary>
    public class PowerSavingMode
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public PowerSavingConfiguration Configuration { get; set; } = new();
        public PowerSavingStatus Status { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Configuration d'économie d'énergie
    /// </summary>
    public class PowerSavingConfiguration
    {
        public bool EnableCpuThrottling { get; set; } = true;
        public bool EnableDiskThrottling { get; set; } = true;
        public bool EnableNetworkThrottling { get; set; } = true;
        public int CpuThrottlePercentage { get; set; } = 50;
        public int DiskThrottlePercentage { get; set; } = 50;
        public int NetworkThrottlePercentage { get; set; } = 50;
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Statut d'économie d'énergie
    /// </summary>
    public class PowerSavingStatus
    {
        public bool IsActive { get; set; }
        public double CpuUsage { get; set; }
        public double DiskUsage { get; set; }
        public double NetworkUsage { get; set; }
        public double PowerConsumption { get; set; }
        public Dictionary<string, object> CustomMetrics { get; set; } = new();
    }

    /// <summary>
    /// Recherche en arrière-plan
    /// </summary>
    public class BackgroundSearch
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public BackgroundSearchConfiguration Configuration { get; set; } = new();
        public BackgroundSearchStatus Status { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Configuration de recherche en arrière-plan
    /// </summary>
    public class BackgroundSearchConfiguration
    {
        public bool EnableContinuousIndexing { get; set; } = true;
        public TimeSpan IndexingInterval { get; set; } = TimeSpan.FromMinutes(5);
        public bool EnableLowPriorityMode { get; set; } = true;
        public int MaxConcurrentOperations { get; set; } = 2;
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Statut de recherche en arrière-plan
    /// </summary>
    public class BackgroundSearchStatus
    {
        public bool IsRunning { get; set; }
        public int ActiveOperations { get; set; }
        public int QueuedOperations { get; set; }
        public DateTime LastOperation { get; set; }
        public Dictionary<string, object> CustomMetrics { get; set; } = new();
    }

    /// <summary>
    /// Tableau de bord d'administration
    /// </summary>
    public class AdminDashboard
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public AdminDashboardConfiguration Configuration { get; set; } = new();
        public AdminDashboardData Data { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Configuration du tableau de bord d'administration
    /// </summary>
    public class AdminDashboardConfiguration
    {
        public bool EnableRealTimeUpdates { get; set; } = true;
        public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromSeconds(30);
        public List<string> EnabledWidgets { get; set; } = new();
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Données du tableau de bord d'administration
    /// </summary>
    public class AdminDashboardData
    {
        public SystemMetrics SystemMetrics { get; set; } = new();
        public SearchMetrics SearchMetrics { get; set; } = new();
        public UserMetrics UserMetrics { get; set; } = new();
        public List<SystemAlert> RecentAlerts { get; set; } = new();
        public Dictionary<string, object> CustomData { get; set; } = new();
    }

    /// <summary>
    /// Métriques système
    /// </summary>
    public class SystemMetrics
    {
        public double CpuUsage { get; set; }
        public long MemoryUsage { get; set; }
        public long DiskUsage { get; set; }
        public double NetworkUsage { get; set; }
        public int ProcessCount { get; set; }
        public Dictionary<string, object> CustomMetrics { get; set; } = new();
    }

    /// <summary>
    /// Métriques de recherche
    /// </summary>
    public class SearchMetrics
    {
        public int TotalSearches { get; set; }
        public int SuccessfulSearches { get; set; }
        public int FailedSearches { get; set; }
        public TimeSpan AverageSearchTime { get; set; }
        public int TotalResults { get; set; }
        public Dictionary<string, object> CustomMetrics { get; set; } = new();
    }

    /// <summary>
    /// Métriques utilisateur
    /// </summary>
    public class UserMetrics
    {
        public int ActiveUsers { get; set; }
        public int TotalUsers { get; set; }
        public int NewUsers { get; set; }
        public TimeSpan AverageSessionDuration { get; set; }
        public Dictionary<string, object> CustomMetrics { get; set; } = new();
    }

    /// <summary>
    /// Logging détaillé
    /// </summary>
    public class DetailedLogging
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public LoggingConfiguration Configuration { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Configuration de logging
    /// </summary>
    public class LoggingConfiguration
    {
        public LogLevel MinLevel { get; set; } = LogLevel.Information;
        public bool EnableFileLogging { get; set; } = true;
        public bool EnableConsoleLogging { get; set; } = true;
        public bool EnableDatabaseLogging { get; set; } = false;
        public string LogFilePath { get; set; } = string.Empty;
        public int MaxLogFileSize { get; set; } = 10 * 1024 * 1024; // 10 MB
        public int MaxLogFiles { get; set; } = 10;
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Niveau de log
    /// </summary>
    public enum LogLevel
    {
        Trace,
        Debug,
        Information,
        Warning,
        Error,
        Critical
    }

    /// <summary>
    /// Entrée de log
    /// </summary>
    public class LogEntry
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public LogLevel Level { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    /// <summary>
    /// Alerte système
    /// </summary>
    public class SystemAlert
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public AlertSeverity Severity { get; set; }
        public AlertType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Gravité d'alerte
    /// </summary>
    public enum AlertSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    /// <summary>
    /// Type d'alerte
    /// </summary>
    public enum AlertType
    {
        System,
        Performance,
        Security,
        Error,
        Warning,
        Information
    }

    /// <summary>
    /// Configuration d'alerte
    /// </summary>
    public class AlertConfiguration
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public AlertSeverity Severity { get; set; }
        public AlertType Type { get; set; }
        public List<string> Recipients { get; set; } = new();
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Sauvegarde automatique
    /// </summary>
    public class AutomaticBackup
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public BackupConfiguration Configuration { get; set; } = new();
        public BackupStatus Status { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Configuration de sauvegarde
    /// </summary>
    public class BackupConfiguration
    {
        public bool EnableAutomaticBackup { get; set; } = true;
        public TimeSpan BackupInterval { get; set; } = TimeSpan.FromHours(24);
        public int MaxBackupFiles { get; set; } = 30;
        public string BackupPath { get; set; } = string.Empty;
        public bool EnableCompression { get; set; } = true;
        public bool EnableEncryption { get; set; } = false;
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Statut de sauvegarde
    /// </summary>
    public class BackupStatus
    {
        public bool IsRunning { get; set; }
        public DateTime LastBackup { get; set; }
        public DateTime NextBackup { get; set; }
        public int BackupCount { get; set; }
        public long TotalBackupSize { get; set; }
        public Dictionary<string, object> CustomMetrics { get; set; } = new();
    }

    /// <summary>
    /// Mode diagnostic
    /// </summary>
    public class DiagnosticMode
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DiagnosticConfiguration Configuration { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Configuration de diagnostic
    /// </summary>
    public class DiagnosticConfiguration
    {
        public bool EnableSystemDiagnostics { get; set; } = true;
        public bool EnablePerformanceDiagnostics { get; set; } = true;
        public bool EnableSecurityDiagnostics { get; set; } = true;
        public bool EnableNetworkDiagnostics { get; set; } = true;
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Rapport de diagnostic
    /// </summary>
    public class DiagnosticReport
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public List<DiagnosticIssue> Issues { get; set; } = new();
        public List<DiagnosticRecommendation> Recommendations { get; set; } = new();
        public DiagnosticScore OverallScore { get; set; } = new();
        public Dictionary<string, object> CustomData { get; set; } = new();
    }

    /// <summary>
    /// Problème de diagnostic
    /// </summary>
    public class DiagnosticIssue
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DiagnosticSeverity Severity { get; set; }
        public string Category { get; set; } = string.Empty;
        public Dictionary<string, object> Details { get; set; } = new();
    }

    /// <summary>
    /// Gravité de diagnostic
    /// </summary>
    public enum DiagnosticSeverity
    {
        Info,
        Warning,
        Error,
        Critical
    }

    /// <summary>
    /// Recommandation de diagnostic
    /// </summary>
    public class DiagnosticRecommendation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public DiagnosticPriority Priority { get; set; }
        public Dictionary<string, object> Details { get; set; } = new();
    }

    /// <summary>
    /// Priorité de diagnostic
    /// </summary>
    public enum DiagnosticPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    /// <summary>
    /// Score de diagnostic
    /// </summary>
    public class DiagnosticScore
    {
        public double OverallScore { get; set; }
        public double SystemScore { get; set; }
        public double PerformanceScore { get; set; }
        public double SecurityScore { get; set; }
        public double NetworkScore { get; set; }
        public Dictionary<string, double> CustomScores { get; set; } = new();
    }

    /// <summary>
    /// Métriques en temps réel
    /// </summary>
    public class RealtimeMetrics
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public SystemMetrics SystemMetrics { get; set; } = new();
        public SearchMetrics SearchMetrics { get; set; } = new();
        public UserMetrics UserMetrics { get; set; } = new();
        public Dictionary<string, object> CustomMetrics { get; set; } = new();
    }

    /// <summary>
    /// Optimisation de performance
    /// </summary>
    public class PerformanceOptimization
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public OptimizationOptions Options { get; set; } = new();
        public OptimizationResult Result { get; set; } = new();
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }

    /// <summary>
    /// Options d'optimisation
    /// </summary>
    public class OptimizationOptions
    {
        public bool OptimizeCache { get; set; } = true;
        public bool OptimizeIndex { get; set; } = true;
        public bool OptimizeMemory { get; set; } = true;
        public bool OptimizeDisk { get; set; } = true;
        public bool OptimizeNetwork { get; set; } = true;
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    /// <summary>
    /// Résultat d'optimisation
    /// </summary>
    public class OptimizationResult
    {
        public bool IsSuccessful { get; set; }
        public List<OptimizationImprovement> Improvements { get; set; } = new();
        public Dictionary<string, object> Metrics { get; set; } = new();
        public string ErrorMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// Amélioration d'optimisation
    /// </summary>
    public class OptimizationImprovement
    {
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double ImprovementPercentage { get; set; }
        public Dictionary<string, object> Details { get; set; } = new();
    }

    // Événements
    public class SystemAlertEventArgs : EventArgs
    {
        public SystemAlert Alert { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class MetricsUpdatedEventArgs : EventArgs
    {
        public RealtimeMetrics Metrics { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
