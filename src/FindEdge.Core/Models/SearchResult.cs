using System;

namespace FindEdge.Core.Models
{
    /// <summary>
    /// Représente un résultat de recherche avec toutes les informations nécessaires
    /// </summary>
    public class SearchResult
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Directory { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime LastModified { get; set; }
        public string FileExtension { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int MatchCount { get; set; }
        public string[] MatchingLines { get; set; } = Array.Empty<string>();
        public SearchMatchType MatchType { get; set; }
        public double RelevanceScore { get; set; }
    }

    /// <summary>
    /// Type de correspondance trouvée
    /// </summary>
    public enum SearchMatchType
    {
        FileName,
        Content,
        Both
    }
}
