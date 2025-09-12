using System;
using System.Collections.Generic;

namespace FindEdge.Core.Models
{
    /// <summary>
    /// Options de configuration pour la recherche
    /// </summary>
    public class SearchOptions
    {
        public string SearchTerm { get; set; } = string.Empty;
        public bool SearchInFileName { get; set; } = true;
        public bool SearchInContent { get; set; } = true;
        public bool UseRegex { get; set; } = false;
        public bool CaseSensitive { get; set; } = false;
        public bool WholeWord { get; set; } = false;
        
        // Filtres de fichiers
        public List<string> IncludeExtensions { get; set; } = new();
        public List<string> ExcludeExtensions { get; set; } = new();
        public List<string> IncludeDirectories { get; set; } = new();
        public List<string> ExcludeDirectories { get; set; } = new();
        
        // Filtres de taille et date
        public long? MinFileSize { get; set; }
        public long? MaxFileSize { get; set; }
        public DateTime? ModifiedAfter { get; set; }
        public DateTime? ModifiedBefore { get; set; }
        
        // Options de performance
        public int MaxResults { get; set; } = 1000;
        public int MaxContentLength { get; set; } = 10000;
        public bool IncludeHiddenFiles { get; set; } = false;
        public bool IncludeSystemFiles { get; set; } = false;
    }
}
