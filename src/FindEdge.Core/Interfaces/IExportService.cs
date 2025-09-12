using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour les services d'export de données
    /// </summary>
    public interface IExportService
    {
        /// <summary>
        /// Exporte les résultats de recherche dans le format spécifié
        /// </summary>
        Task<byte[]> ExportSearchResultsAsync(IEnumerable<SearchResult> results, ExportFormat format, ExportOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Exporte les groupes de doublons dans le format spécifié
        /// </summary>
        Task<byte[]> ExportDuplicateGroupsAsync(IEnumerable<DuplicateGroup> groups, ExportFormat format, ExportOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Exporte les statistiques de recherche dans le format spécifié
        /// </summary>
        Task<byte[]> ExportStatisticsAsync(SearchStatistics statistics, ExportFormat format, ExportOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtient les formats d'export supportés
        /// </summary>
        IEnumerable<ExportFormat> GetSupportedFormats();

        /// <summary>
        /// Valide les options d'export
        /// </summary>
        bool ValidateExportOptions(ExportFormat format, ExportOptions options);
    }

    /// <summary>
    /// Formats d'export supportés
    /// </summary>
    public enum ExportFormat
    {
        /// <summary>
        /// Format CSV (Comma-Separated Values)
        /// </summary>
        Csv,

        /// <summary>
        /// Format JSON (JavaScript Object Notation)
        /// </summary>
        Json,

        /// <summary>
        /// Format XML (eXtensible Markup Language)
        /// </summary>
        Xml,

        /// <summary>
        /// Format Excel (.xlsx)
        /// </summary>
        Excel,

        /// <summary>
        /// Format HTML (HyperText Markup Language)
        /// </summary>
        Html,

        /// <summary>
        /// Format PDF (Portable Document Format)
        /// </summary>
        Pdf
    }

    /// <summary>
    /// Options de configuration pour l'export
    /// </summary>
    public class ExportOptions
    {
        /// <summary>
        /// Nom du fichier de sortie
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Inclure les métadonnées
        /// </summary>
        public bool IncludeMetadata { get; set; } = true;

        /// <summary>
        /// Inclure le contenu des fichiers
        /// </summary>
        public bool IncludeContent { get; set; } = false;

        /// <summary>
        /// Limiter la longueur du contenu (en caractères)
        /// </summary>
        public int MaxContentLength { get; set; } = 1000;

        /// <summary>
        /// Inclure les statistiques
        /// </summary>
        public bool IncludeStatistics { get; set; } = true;

        /// <summary>
        /// Inclure les informations de doublons
        /// </summary>
        public bool IncludeDuplicateInfo { get; set; } = false;

        /// <summary>
        /// Encodage du fichier de sortie
        /// </summary>
        public string Encoding { get; set; } = "UTF-8";

        /// <summary>
        /// Séparateur pour le format CSV
        /// </summary>
        public string CsvSeparator { get; set; } = ",";

        /// <summary>
        /// Inclure l'en-tête dans le CSV
        /// </summary>
        public bool IncludeCsvHeader { get; set; } = true;

        /// <summary>
        /// Format de date pour l'export
        /// </summary>
        public string DateFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Format de taille de fichier
        /// </summary>
        public FileSizeFormat FileSizeFormat { get; set; } = FileSizeFormat.Bytes;

        /// <summary>
        /// Inclure les colonnes spécifiées
        /// </summary>
        public List<string> IncludeColumns { get; set; } = new();

        /// <summary>
        /// Exclure les colonnes spécifiées
        /// </summary>
        public List<string> ExcludeColumns { get; set; } = new();
    }

    /// <summary>
    /// Format d'affichage de la taille des fichiers
    /// </summary>
    public enum FileSizeFormat
    {
        /// <summary>
        /// Taille en octets
        /// </summary>
        Bytes,

        /// <summary>
        /// Taille en KB
        /// </summary>
        Kilobytes,

        /// <summary>
        /// Taille en MB
        /// </summary>
        Megabytes,

        /// <summary>
        /// Taille formatée (1.5 MB)
        /// </summary>
        Formatted
    }

    /// <summary>
    /// Statistiques de recherche
    /// </summary>
    public class SearchStatistics
    {
        public DateTime SearchDate { get; set; } = DateTime.Now;
        public string SearchTerm { get; set; } = string.Empty;
        public int TotalFilesFound { get; set; }
        public int FilesWithContentMatch { get; set; }
        public int FilesWithNameMatch { get; set; }
        public int FilesWithBothMatch { get; set; }
        public long TotalSize { get; set; }
        public TimeSpan SearchDuration { get; set; }
        public int DirectoriesSearched { get; set; }
        public int DuplicateGroupsFound { get; set; }
        public long SpaceWasted { get; set; }
        public Dictionary<string, int> FileTypeDistribution { get; set; } = new();
        public Dictionary<string, int> DirectoryDistribution { get; set; } = new();
        public List<string> SearchOptions { get; set; } = new();
    }
}
