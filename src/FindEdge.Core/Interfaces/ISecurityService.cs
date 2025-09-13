using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour le service de sécurité et conformité
    /// </summary>
    public interface ISecurityService
    {
        /// <summary>
        /// Chiffre les index pour protéger les données sensibles
        /// </summary>
        Task EncryptIndexAsync(string indexPath, string encryptionKey, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Déchiffre les index
        /// </summary>
        Task DecryptIndexAsync(string indexPath, string encryptionKey, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Enregistre une action dans l'audit trail
        /// </summary>
        Task LogAuditEventAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient l'historique d'audit pour un utilisateur
        /// </summary>
        Task<IEnumerable<AuditEvent>> GetAuditTrailAsync(string userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Vérifie les permissions d'accès pour un utilisateur
        /// </summary>
        Task<bool> CheckAccessPermissionAsync(string userId, string resource, AccessAction action, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure les permissions pour un utilisateur ou groupe
        /// </summary>
        Task ConfigurePermissionsAsync(string userId, IEnumerable<Permission> permissions, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Anonymise les données sensibles dans les résultats
        /// </summary>
        Task<IEnumerable<SearchResult>> AnonymizeResultsAsync(IEnumerable<SearchResult> results, AnonymizationOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Active le mode incognito (pas d'enregistrement d'historique)
        /// </summary>
        Task EnableIncognitoModeAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Désactive le mode incognito
        /// </summary>
        Task DisableIncognitoModeAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Génère un rapport de conformité RGPD
        /// </summary>
        Task<GdprComplianceReport> GenerateGdprComplianceReportAsync(string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Supprime les données personnelles d'un utilisateur (droit à l'oubli)
        /// </summary>
        Task DeletePersonalDataAsync(string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Exporte les données personnelles d'un utilisateur
        /// </summary>
        Task<PersonalDataExport> ExportPersonalDataAsync(string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure la rétention des données
        /// </summary>
        Task ConfigureDataRetentionAsync(DataRetentionPolicy policy, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Classifie automatiquement les contenus sensibles
        /// </summary>
        Task<DataClassificationResult> ClassifyDataSensitivityAsync(IEnumerable<string> filePaths, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Génère un rapport de conformité réglementaire
        /// </summary>
        Task<ComplianceReport> GenerateComplianceReportAsync(ComplianceStandard standard, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Événement d'audit
    /// </summary>
    public class AuditEvent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Details { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Action d'accès
    /// </summary>
    public enum AccessAction
    {
        Read,
        Write,
        Delete,
        Search,
        Export,
        Admin
    }

    /// <summary>
    /// Permission d'accès
    /// </summary>
    public class Permission
    {
        public string Resource { get; set; } = string.Empty;
        public AccessAction Action { get; set; }
        public bool IsGranted { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public Dictionary<string, object> Conditions { get; set; } = new();
    }

    /// <summary>
    /// Options d'anonymisation
    /// </summary>
    public class AnonymizationOptions
    {
        public bool AnonymizeFileNames { get; set; } = false;
        public bool AnonymizeContent { get; set; } = true;
        public bool AnonymizePaths { get; set; } = false;
        public string AnonymizationMethod { get; set; } = "Hash";
        public Dictionary<string, object> CustomRules { get; set; } = new();
    }

    /// <summary>
    /// Rapport de conformité RGPD
    /// </summary>
    public class GdprComplianceReport
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public List<PersonalDataItem> PersonalDataItems { get; set; } = new();
        public List<DataProcessingActivity> ProcessingActivities { get; set; } = new();
        public List<DataSharing> DataSharings { get; set; } = new();
        public ComplianceStatus Status { get; set; }
        public List<ComplianceIssue> Issues { get; set; } = new();
    }

    /// <summary>
    /// Élément de données personnelles
    /// </summary>
    public class PersonalDataItem
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime LastAccessed { get; set; }
        public string LegalBasis { get; set; } = string.Empty;
    }

    /// <summary>
    /// Activité de traitement de données
    /// </summary>
    public class DataProcessingActivity
    {
        public string Activity { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string LegalBasis { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// Partage de données
    /// </summary>
    public class DataSharing
    {
        public string Recipient { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string LegalBasis { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    /// <summary>
    /// Statut de conformité
    /// </summary>
    public enum ComplianceStatus
    {
        Compliant,
        NonCompliant,
        RequiresReview,
        Unknown
    }

    /// <summary>
    /// Problème de conformité
    /// </summary>
    public class ComplianceIssue
    {
        public string Description { get; set; } = string.Empty;
        public ComplianceSeverity Severity { get; set; }
        public string Recommendation { get; set; } = string.Empty;
    }

    /// <summary>
    /// Gravité de conformité
    /// </summary>
    public enum ComplianceSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    /// <summary>
    /// Export de données personnelles
    /// </summary>
    public class PersonalDataExport
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public string Format { get; set; } = "JSON";
        public byte[] Data { get; set; } = Array.Empty<byte>();
        public string Checksum { get; set; } = string.Empty;
    }

    /// <summary>
    /// Politique de rétention des données
    /// </summary>
    public class DataRetentionPolicy
    {
        public string Name { get; set; } = string.Empty;
        public TimeSpan RetentionPeriod { get; set; }
        public DataRetentionAction Action { get; set; }
        public List<string> DataTypes { get; set; } = new();
        public Dictionary<string, object> Conditions { get; set; } = new();
    }

    /// <summary>
    /// Action de rétention des données
    /// </summary>
    public enum DataRetentionAction
    {
        Delete,
        Archive,
        Anonymize,
        Encrypt
    }

    /// <summary>
    /// Résultat de classification de données
    /// </summary>
    public class DataClassificationResult
    {
        public Dictionary<string, DataSensitivityLevel> FileClassifications { get; set; } = new();
        public List<SensitiveDataItem> SensitiveItems { get; set; } = new();
        public ComplianceStatus OverallStatus { get; set; }
    }

    /// <summary>
    /// Niveau de sensibilité des données
    /// </summary>
    public enum DataSensitivityLevel
    {
        Public,
        Internal,
        Confidential,
        Restricted
    }

    /// <summary>
    /// Élément de données sensibles
    /// </summary>
    public class SensitiveDataItem
    {
        public string FilePath { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public DataSensitivityLevel Level { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Confidence { get; set; }
    }

    /// <summary>
    /// Standard de conformité
    /// </summary>
    public enum ComplianceStandard
    {
        GDPR,
        CCPA,
        HIPAA,
        SOX,
        PCI_DSS,
        ISO27001
    }

    /// <summary>
    /// Rapport de conformité
    /// </summary>
    public class ComplianceReport
    {
        public ComplianceStandard Standard { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public ComplianceStatus Status { get; set; }
        public List<ComplianceRequirement> Requirements { get; set; } = new();
        public List<ComplianceIssue> Issues { get; set; } = new();
        public List<Recommendation> Recommendations { get; set; } = new();
    }

    /// <summary>
    /// Exigence de conformité
    /// </summary>
    public class ComplianceRequirement
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsImplemented { get; set; }
        public string ImplementationDetails { get; set; } = string.Empty;
        public ComplianceSeverity Severity { get; set; }
    }
}
