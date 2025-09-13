using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour le service d'analytics et reporting
    /// </summary>
    public interface IAnalyticsService
    {
        /// <summary>
        /// Analyse le contenu des fichiers et génère des statistiques
        /// </summary>
        Task<ContentAnalysisResult> AnalyzeContentAsync(IEnumerable<string> filePaths, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Analyse les tendances temporelles des fichiers
        /// </summary>
        Task<TemporalAnalysisResult> AnalyzeTemporalTrendsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Analyse l'utilisation de l'espace disque
        /// </summary>
        Task<DiskUsageAnalysisResult> AnalyzeDiskUsageAsync(IEnumerable<string> directories, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Détecte les anomalies dans les fichiers
        /// </summary>
        Task<IEnumerable<AnomalyDetectionResult>> DetectAnomaliesAsync(IEnumerable<string> filePaths, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Génère un rapport de santé du système de fichiers
        /// </summary>
        Task<SystemHealthReport> GenerateSystemHealthReportAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Exporte les données vers une base de données
        /// </summary>
        Task ExportToDatabaseAsync(ExportConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Exporte les données vers Excel avec graphiques
        /// </summary>
        Task ExportToExcelAsync(string filePath, ExcelExportOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Envoie des données via webhook
        /// </summary>
        Task SendWebhookAsync(string url, object data, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les statistiques d'utilisation de l'application
        /// </summary>
        Task<UsageStatistics> GetUsageStatisticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les métriques de performance
        /// </summary>
        Task<PerformanceMetrics> GetPerformanceMetricsAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Résultat d'analyse de contenu
    /// </summary>
    public class ContentAnalysisResult
    {
        public int TotalFiles { get; set; }
        public Dictionary<string, int> FileTypeDistribution { get; set; } = new();
        public Dictionary<string, int> LanguageDistribution { get; set; } = new();
        public Dictionary<string, int> CategoryDistribution { get; set; } = new();
        public long TotalSize { get; set; }
        public double AverageFileSize { get; set; }
        public List<FileInsight> Insights { get; set; } = new();
    }

    /// <summary>
    /// Résultat d'analyse temporelle
    /// </summary>
    public class TemporalAnalysisResult
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Dictionary<DateTime, int> FilesCreatedOverTime { get; set; } = new();
        public Dictionary<DateTime, int> FilesModifiedOverTime { get; set; } = new();
        public Dictionary<DateTime, long> SizeGrowthOverTime { get; set; } = new();
        public List<TemporalTrend> Trends { get; set; } = new();
    }

    /// <summary>
    /// Résultat d'analyse d'utilisation disque
    /// </summary>
    public class DiskUsageAnalysisResult
    {
        public long TotalSize { get; set; }
        public long UsedSpace { get; set; }
        public long FreeSpace { get; set; }
        public Dictionary<string, long> DirectorySizes { get; set; } = new();
        public Dictionary<string, long> FileTypeSizes { get; set; } = new();
        public List<DiskUsageInsight> Insights { get; set; } = new();
    }

    /// <summary>
    /// Résultat de détection d'anomalie
    /// </summary>
    public class AnomalyDetectionResult
    {
        public string FilePath { get; set; } = string.Empty;
        public AnomalyType Type { get; set; }
        public double Severity { get; set; }
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, object> Details { get; set; } = new();
    }

    /// <summary>
    /// Type d'anomalie détectée
    /// </summary>
    public enum AnomalyType
    {
        UnusualFileSize,
        UnusualFileType,
        DuplicateContent,
        CorruptedFile,
        SuspiciousActivity,
        UnusualAccessPattern
    }

    /// <summary>
    /// Rapport de santé du système
    /// </summary>
    public class SystemHealthReport
    {
        public DateTime GeneratedAt { get; set; }
        public SystemHealthStatus OverallStatus { get; set; }
        public List<HealthCheck> HealthChecks { get; set; } = new();
        public List<Recommendation> Recommendations { get; set; } = new();
        public Dictionary<string, object> Metrics { get; set; } = new();
    }

    /// <summary>
    /// Statut de santé du système
    /// </summary>
    public enum SystemHealthStatus
    {
        Healthy,
        Warning,
        Critical,
        Unknown
    }

    /// <summary>
    /// Vérification de santé
    /// </summary>
    public class HealthCheck
    {
        public string Name { get; set; } = string.Empty;
        public SystemHealthStatus Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, object> Details { get; set; } = new();
    }

    /// <summary>
    /// Recommandation d'amélioration
    /// </summary>
    public class Recommendation
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RecommendationPriority Priority { get; set; }
        public string Action { get; set; } = string.Empty;
    }

    /// <summary>
    /// Priorité de recommandation
    /// </summary>
    public enum RecommendationPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    /// <summary>
    /// Configuration d'export
    /// </summary>
    public class ExportConfiguration
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public DatabaseExportFormat Format { get; set; }
        public Dictionary<string, object> Options { get; set; } = new();
    }

    /// <summary>
    /// Format d'export de base de données
    /// </summary>
    public enum DatabaseExportFormat
    {
        SqlServer,
        MySql,
        PostgreSQL,
        Oracle,
        Csv,
        Json,
        Xml
    }

    /// <summary>
    /// Options d'export Excel
    /// </summary>
    public class ExcelExportOptions
    {
        public bool IncludeCharts { get; set; } = true;
        public bool IncludePivotTables { get; set; } = false;
        public string SheetName { get; set; } = "FindEdge Report";
        public Dictionary<string, object> ChartOptions { get; set; } = new();
    }

    /// <summary>
    /// Statistiques d'utilisation
    /// </summary>
    public class UsageStatistics
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalSearches { get; set; }
        public int UniqueUsers { get; set; }
        public Dictionary<string, int> SearchPatterns { get; set; } = new();
        public Dictionary<string, int> FileTypeSearches { get; set; } = new();
        public TimeSpan AverageSearchTime { get; set; }
        public List<UsageInsight> Insights { get; set; } = new();
    }

    /// <summary>
    /// Métriques de performance
    /// </summary>
    public class PerformanceMetrics
    {
        public TimeSpan AverageSearchTime { get; set; }
        public TimeSpan AverageIndexTime { get; set; }
        public int SearchesPerSecond { get; set; }
        public long MemoryUsage { get; set; }
        public double CpuUsage { get; set; }
        public Dictionary<string, object> CustomMetrics { get; set; } = new();
    }

    // Classes d'aide pour les insights
    public class FileInsight
    {
        public string FilePath { get; set; } = string.Empty;
        public string Insight { get; set; } = string.Empty;
        public double Confidence { get; set; }
    }

    public class TemporalTrend
    {
        public string Name { get; set; } = string.Empty;
        public TrendDirection Direction { get; set; }
        public double Strength { get; set; }
    }

    public enum TrendDirection
    {
        Increasing,
        Decreasing,
        Stable,
        Fluctuating
    }

    public class DiskUsageInsight
    {
        public string Path { get; set; } = string.Empty;
        public string Insight { get; set; } = string.Empty;
        public long Size { get; set; }
    }

    public class UsageInsight
    {
        public string Category { get; set; } = string.Empty;
        public string Insight { get; set; } = string.Empty;
        public double Value { get; set; }
    }
}
