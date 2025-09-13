using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour les technologies émergentes et cas d'usage spécialisés
    /// </summary>
    public interface IEmergingTechnologiesService
    {
        /// <summary>
        /// Effectue une recherche vocale
        /// </summary>
        Task<VoiceSearchResult> PerformVoiceSearchAsync(VoiceSearchOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Traite la reconnaissance vocale en temps réel
        /// </summary>
        Task<RealtimeVoiceRecognition> StartRealtimeVoiceRecognitionAsync(VoiceRecognitionOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Effectue une reconnaissance de texte avancée (OCR) sur des images
        /// </summary>
        Task<OCRResult> PerformAdvancedOCRAsync(string imagePath, OCROptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Effectue une reconnaissance de texte sur des PDF scannés
        /// </summary>
        Task<OCRResult> PerformPDFOCRAsync(string pdfPath, OCROptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Génère une visualisation 3D en réalité augmentée
        /// </summary>
        Task<ARVisualization> GenerateARVisualizationAsync(IEnumerable<string> filePaths, AROptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Intègre avec la blockchain pour la preuve d'intégrité
        /// </summary>
        Task<BlockchainProof> CreateBlockchainProofAsync(string filePath, BlockchainOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Vérifie l'intégrité d'un fichier via la blockchain
        /// </summary>
        Task<BlockchainVerification> VerifyBlockchainProofAsync(string filePath, string proofHash, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Recherche dans les données IoT
        /// </summary>
        Task<IEnumerable<SearchResult>> SearchIoTDataAsync(IoTSearchOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure la connexion à un dispositif IoT
        /// </summary>
        Task<IoTDevice> ConnectIoTDeviceAsync(IoTDeviceConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Effectue une recherche forensique
        /// </summary>
        Task<ForensicSearchResult> PerformForensicSearchAsync(ForensicSearchOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Analyse les métadonnées forensiques d'un fichier
        /// </summary>
        Task<ForensicMetadata> AnalyzeForensicMetadataAsync(string filePath, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Recherche dans des formats de données scientifiques
        /// </summary>
        Task<IEnumerable<SearchResult>> SearchScientificDataAsync(ScientificSearchOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Analyse des données scientifiques
        /// </summary>
        Task<ScientificAnalysisResult> AnalyzeScientificDataAsync(string filePath, ScientificAnalysisOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Recherche spécialisée pour créateurs de contenu
        /// </summary>
        Task<IEnumerable<SearchResult>> SearchMediaContentAsync(MediaSearchOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Analyse des métadonnées de média
        /// </summary>
        Task<MediaMetadata> AnalyzeMediaMetadataAsync(string filePath, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Recherche avancée dans le code source
        /// </summary>
        Task<IEnumerable<CodeSearchResult>> SearchSourceCodeAsync(CodeSearchOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Analyse de la qualité du code
        /// </summary>
        Task<CodeQualityAnalysis> AnalyzeCodeQualityAsync(string filePath, CodeQualityOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Recherche académique avec support des formats de recherche
        /// </summary>
        Task<IEnumerable<AcademicSearchResult>> SearchAcademicContentAsync(AcademicSearchOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Analyse de citations et références
        /// </summary>
        Task<CitationAnalysis> AnalyzeCitationsAsync(string filePath, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Événement déclenché lors de la reconnaissance vocale
        /// </summary>
        event EventHandler<VoiceRecognitionEventArgs>? VoiceRecognized;
        
        /// <summary>
        /// Événement déclenché lors de la détection IoT
        /// </summary>
        event EventHandler<IoTDataEventArgs>? IoTDataReceived;
        
        /// <summary>
        /// Événement déclenché lors de l'analyse forensique
        /// </summary>
        event EventHandler<ForensicAnalysisEventArgs>? ForensicAnalysisCompleted;
    }

    /// <summary>
    /// Résultat de recherche vocale
    /// </summary>
    public class VoiceSearchResult
    {
        public string RecognizedText { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public IEnumerable<SearchResult> SearchResults { get; set; } = new List<SearchResult>();
        public TimeSpan ProcessingTime { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Options de recherche vocale
    /// </summary>
    public class VoiceSearchOptions
    {
        public string Language { get; set; } = "fr-FR";
        public bool EnablePunctuation { get; set; } = true;
        public bool EnableProfanityFilter { get; set; } = true;
        public int MaxAlternatives { get; set; } = 3;
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    /// <summary>
    /// Reconnaissance vocale en temps réel
    /// </summary>
    public class RealtimeVoiceRecognition
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool IsActive { get; set; }
        public string CurrentText { get; set; } = string.Empty;
        public List<VoiceRecognitionEvent> Events { get; set; } = new();
        public VoiceRecognitionOptions Options { get; set; } = new();
    }

    /// <summary>
    /// Options de reconnaissance vocale
    /// </summary>
    public class VoiceRecognitionOptions
    {
        public string Language { get; set; } = "fr-FR";
        public bool ContinuousListening { get; set; } = true;
        public bool EnableInterimResults { get; set; } = true;
        public int SilenceTimeout { get; set; } = 5000;
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    /// <summary>
    /// Événement de reconnaissance vocale
    /// </summary>
    public class VoiceRecognitionEvent
    {
        public string Text { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool IsFinal { get; set; }
    }

    /// <summary>
    /// Résultat OCR
    /// </summary>
    public class OCRResult
    {
        public string FilePath { get; set; } = string.Empty;
        public string ExtractedText { get; set; } = string.Empty;
        public List<OCRRegion> Regions { get; set; } = new();
        public double Confidence { get; set; }
        public string Language { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Région OCR
    /// </summary>
    public class OCRRegion
    {
        public string Text { get; set; } = string.Empty;
        public OCRBoundingBox BoundingBox { get; set; } = new();
        public double Confidence { get; set; }
        public string Language { get; set; } = string.Empty;
    }

    /// <summary>
    /// Boîte englobante OCR
    /// </summary>
    public class OCRBoundingBox
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    /// <summary>
    /// Options OCR
    /// </summary>
    public class OCROptions
    {
        public List<string> Languages { get; set; } = new();
        public bool EnableHandwritingRecognition { get; set; } = false;
        public bool EnableTableRecognition { get; set; } = false;
        public bool EnableFormRecognition { get; set; } = false;
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    /// <summary>
    /// Visualisation AR
    /// </summary>
    public class ARVisualization
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public List<ARObject> Objects { get; set; } = new();
        public AROptions Options { get; set; } = new();
        public ARScene Scene { get; set; } = new();
    }

    /// <summary>
    /// Objet AR
    /// </summary>
    public class ARObject
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FilePath { get; set; } = string.Empty;
        public ARPosition Position { get; set; } = new();
        public ARRotation Rotation { get; set; } = new();
        public ARScale Scale { get; set; } = new();
        public ARObjectType Type { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    /// <summary>
    /// Position AR
    /// </summary>
    public class ARPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    /// <summary>
    /// Rotation AR
    /// </summary>
    public class ARRotation
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
    }

    /// <summary>
    /// Échelle AR
    /// </summary>
    public class ARScale
    {
        public float X { get; set; } = 1.0f;
        public float Y { get; set; } = 1.0f;
        public float Z { get; set; } = 1.0f;
    }

    /// <summary>
    /// Type d'objet AR
    /// </summary>
    public enum ARObjectType
    {
        File,
        Directory,
        SearchResult,
        Custom
    }

    /// <summary>
    /// Options AR
    /// </summary>
    public class AROptions
    {
        public bool EnableTracking { get; set; } = true;
        public bool EnableLighting { get; set; } = true;
        public bool EnableShadows { get; set; } = true;
        public string Environment { get; set; } = "default";
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    /// <summary>
    /// Scène AR
    /// </summary>
    public class ARScene
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public ARLighting Lighting { get; set; } = new();
        public ARCamera Camera { get; set; } = new();
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    /// <summary>
    /// Éclairage AR
    /// </summary>
    public class ARLighting
    {
        public float Intensity { get; set; } = 1.0f;
        public string Color { get; set; } = "#FFFFFF";
        public ARPosition Position { get; set; } = new();
    }

    /// <summary>
    /// Caméra AR
    /// </summary>
    public class ARCamera
    {
        public ARPosition Position { get; set; } = new();
        public ARRotation Rotation { get; set; } = new();
        public float FieldOfView { get; set; } = 60.0f;
        public float NearPlane { get; set; } = 0.1f;
        public float FarPlane { get; set; } = 1000.0f;
    }

    /// <summary>
    /// Preuve blockchain
    /// </summary>
    public class BlockchainProof
    {
        public string FilePath { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string BlockHash { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Network { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Options blockchain
    /// </summary>
    public class BlockchainOptions
    {
        public string Network { get; set; } = "ethereum";
        public string ContractAddress { get; set; } = string.Empty;
        public string PrivateKey { get; set; } = string.Empty;
        public decimal GasPrice { get; set; }
        public decimal GasLimit { get; set; }
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    /// <summary>
    /// Vérification blockchain
    /// </summary>
    public class BlockchainVerification
    {
        public bool IsValid { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string ExpectedHash { get; set; } = string.Empty;
        public string ActualHash { get; set; } = string.Empty;
        public DateTime VerificationTime { get; set; } = DateTime.UtcNow;
        public string ErrorMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// Options de recherche IoT
    /// </summary>
    public class IoTSearchOptions
    {
        public List<string> DeviceIds { get; set; } = new();
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string> DataTypes { get; set; } = new();
        public Dictionary<string, object> Filters { get; set; } = new();
    }

    /// <summary>
    /// Configuration de dispositif IoT
    /// </summary>
    public class IoTDeviceConfiguration
    {
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    /// <summary>
    /// Dispositif IoT
    /// </summary>
    public class IoTDevice
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string DeviceId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsConnected { get; set; }
        public DateTime LastSeen { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    /// <summary>
    /// Résultat de recherche forensique
    /// </summary>
    public class ForensicSearchResult
    {
        public string SearchId { get; set; } = Guid.NewGuid().ToString();
        public List<ForensicEvidence> Evidence { get; set; } = new();
        public ForensicSearchOptions Options { get; set; } = new();
        public DateTime SearchTime { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Options de recherche forensique
    /// </summary>
    public class ForensicSearchOptions
    {
        public List<string> SearchPaths { get; set; } = new();
        public List<string> FileTypes { get; set; } = new();
        public bool IncludeDeletedFiles { get; set; } = true;
        public bool IncludeUnallocatedSpace { get; set; } = false;
        public bool IncludeRegistry { get; set; } = false;
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    /// <summary>
    /// Preuve forensique
    /// </summary>
    public class ForensicEvidence
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FilePath { get; set; } = string.Empty;
        public EvidenceType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Type de preuve
    /// </summary>
    public enum EvidenceType
    {
        File,
        DeletedFile,
        RegistryEntry,
        NetworkConnection,
        Process,
        UserActivity,
        SystemEvent
    }

    /// <summary>
    /// Métadonnées forensiques
    /// </summary>
    public class ForensicMetadata
    {
        public string FilePath { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime Accessed { get; set; }
        public string Owner { get; set; } = string.Empty;
        public string Permissions { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;
        public Dictionary<string, object> ExtendedAttributes { get; set; } = new();
    }

    /// <summary>
    /// Options de recherche scientifique
    /// </summary>
    public class ScientificSearchOptions
    {
        public List<string> FileFormats { get; set; } = new();
        public List<string> DataTypes { get; set; } = new();
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Dictionary<string, object> Filters { get; set; } = new();
    }

    /// <summary>
    /// Résultat d'analyse scientifique
    /// </summary>
    public class ScientificAnalysisResult
    {
        public string FilePath { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new();
        public List<ScientificMeasurement> Measurements { get; set; } = new();
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Mesure scientifique
    /// </summary>
    public class ScientificMeasurement
    {
        public string Name { get; set; } = string.Empty;
        public double Value { get; set; }
        public string Unit { get; set; } = string.Empty;
        public double Uncertainty { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    /// <summary>
    /// Options d'analyse scientifique
    /// </summary>
    public class ScientificAnalysisOptions
    {
        public List<string> AnalysisTypes { get; set; } = new();
        public bool IncludeStatistics { get; set; } = true;
        public bool IncludeVisualization { get; set; } = true;
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    /// <summary>
    /// Options de recherche média
    /// </summary>
    public class MediaSearchOptions
    {
        public List<string> MediaTypes { get; set; } = new();
        public List<string> Formats { get; set; } = new();
        public TimeSpan? Duration { get; set; }
        public int? Resolution { get; set; }
        public Dictionary<string, object> Filters { get; set; } = new();
    }

    /// <summary>
    /// Métadonnées de média
    /// </summary>
    public class MediaMetadata
    {
        public string FilePath { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int BitRate { get; set; }
        public string Codec { get; set; } = string.Empty;
        public Dictionary<string, object> ExtendedMetadata { get; set; } = new();
    }

    /// <summary>
    /// Résultat de recherche de code
    /// </summary>
    public class CodeSearchResult
    {
        public string FilePath { get; set; } = string.Empty;
        public string FunctionName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public int LineNumber { get; set; }
        public string CodeSnippet { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public double RelevanceScore { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Options de recherche de code
    /// </summary>
    public class CodeSearchOptions
    {
        public List<string> Languages { get; set; } = new();
        public List<string> SearchTypes { get; set; } = new();
        public bool IncludeComments { get; set; } = true;
        public bool IncludeStrings { get; set; } = true;
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    /// <summary>
    /// Analyse de qualité de code
    /// </summary>
    public class CodeQualityAnalysis
    {
        public string FilePath { get; set; } = string.Empty;
        public double QualityScore { get; set; }
        public List<CodeIssue> Issues { get; set; } = new();
        public CodeMetrics Metrics { get; set; } = new();
        public Dictionary<string, object> Recommendations { get; set; } = new();
    }

    /// <summary>
    /// Problème de code
    /// </summary>
    public class CodeIssue
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int LineNumber { get; set; }
        public CodeIssueSeverity Severity { get; set; }
        public string Recommendation { get; set; } = string.Empty;
    }

    /// <summary>
    /// Gravité du problème de code
    /// </summary>
    public enum CodeIssueSeverity
    {
        Info,
        Warning,
        Error,
        Critical
    }

    /// <summary>
    /// Métriques de code
    /// </summary>
    public class CodeMetrics
    {
        public int LinesOfCode { get; set; }
        public int CyclomaticComplexity { get; set; }
        public int MaintainabilityIndex { get; set; }
        public double TestCoverage { get; set; }
        public Dictionary<string, object> CustomMetrics { get; set; } = new();
    }

    /// <summary>
    /// Options de qualité de code
    /// </summary>
    public class CodeQualityOptions
    {
        public List<string> Rules { get; set; } = new();
        public bool IncludeComplexityAnalysis { get; set; } = true;
        public bool IncludeSecurityAnalysis { get; set; } = true;
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    /// <summary>
    /// Options de recherche académique
    /// </summary>
    public class AcademicSearchOptions
    {
        public List<string> DocumentTypes { get; set; } = new();
        public List<string> Subjects { get; set; } = new();
        public DateTime? PublicationDate { get; set; }
        public List<string> Authors { get; set; } = new();
        public Dictionary<string, object> Filters { get; set; } = new();
    }

    /// <summary>
    /// Résultat de recherche académique
    /// </summary>
    public class AcademicSearchResult
    {
        public string FilePath { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public List<string> Authors { get; set; } = new();
        public string Journal { get; set; } = string.Empty;
        public DateTime PublicationDate { get; set; }
        public List<string> Keywords { get; set; } = new();
        public string Abstract { get; set; } = string.Empty;
        public double RelevanceScore { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Analyse de citations
    /// </summary>
    public class CitationAnalysis
    {
        public string FilePath { get; set; } = string.Empty;
        public List<Citation> Citations { get; set; } = new();
        public List<Reference> References { get; set; } = new();
        public CitationMetrics Metrics { get; set; } = new();
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Citation
    /// </summary>
    public class Citation
    {
        public string Text { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public int LineNumber { get; set; }
        public CitationType Type { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Type de citation
    /// </summary>
    public enum CitationType
    {
        Inline,
        Footnote,
        Endnote,
        Bibliography
    }

    /// <summary>
    /// Référence
    /// </summary>
    public class Reference
    {
        public string Title { get; set; } = string.Empty;
        public List<string> Authors { get; set; } = new();
        public string Journal { get; set; } = string.Empty;
        public DateTime PublicationDate { get; set; }
        public string DOI { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Métriques de citations
    /// </summary>
    public class CitationMetrics
    {
        public int TotalCitations { get; set; }
        public int UniqueSources { get; set; }
        public double AverageCitationsPerPage { get; set; }
        public Dictionary<string, int> CitationTypes { get; set; } = new();
    }

    // Événements
    public class VoiceRecognitionEventArgs : EventArgs
    {
        public string RecognizedText { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class IoTDataEventArgs : EventArgs
    {
        public string DeviceId { get; set; } = string.Empty;
        public Dictionary<string, object> Data { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class ForensicAnalysisEventArgs : EventArgs
    {
        public string FilePath { get; set; } = string.Empty;
        public ForensicAnalysisResult Result { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Résultat d'analyse forensique
    /// </summary>
    public class ForensicAnalysisResult
    {
        public string FilePath { get; set; } = string.Empty;
        public List<ForensicEvidence> Evidence { get; set; } = new();
        public ForensicMetadata Metadata { get; set; } = new();
        public double Confidence { get; set; }
        public Dictionary<string, object> AnalysisData { get; set; } = new();
    }
}
