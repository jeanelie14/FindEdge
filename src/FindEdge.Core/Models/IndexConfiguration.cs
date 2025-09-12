using System;
using System.Collections.Generic;

namespace FindEdge.Core.Models
{
    /// <summary>
    /// Configuration pour l'indexation des fichiers
    /// </summary>
    public class IndexConfiguration
    {
        /// <summary>
        /// Répertoires à indexer
        /// </summary>
        public List<string> IndexedDirectories { get; set; } = new();

        /// <summary>
        /// Répertoires à exclure de l'indexation
        /// </summary>
        public List<string> ExcludedDirectories { get; set; } = new()
        {
            "System Volume Information",
            "$Recycle.Bin",
            "Windows",
            "Program Files",
            "Program Files (x86)",
            "AppData",
            "Temp",
            "Tmp"
        };

        /// <summary>
        /// Extensions de fichiers à indexer
        /// </summary>
        public List<string> IndexedExtensions { get; set; } = new()
        {
            ".txt", ".log", ".csv", ".json", ".xml", ".html", ".css", ".js",
            ".ts", ".cs", ".vb", ".cpp", ".h", ".py", ".java", ".php",
            ".rb", ".go", ".rs", ".swift", ".kt", ".scala", ".sh", ".bat",
            ".ps1", ".sql", ".yaml", ".yml", ".ini", ".cfg", ".conf"
        };

        /// <summary>
        /// Extensions de fichiers à exclure de l'indexation
        /// </summary>
        public List<string> ExcludedExtensions { get; set; } = new()
        {
            ".exe", ".dll", ".sys", ".tmp", ".temp", ".log", ".cache",
            ".bin", ".obj", ".pdb", ".ilk", ".exp", ".lib", ".so", ".dylib"
        };

        /// <summary>
        /// Taille maximale des fichiers à indexer (en octets)
        /// </summary>
        public long MaxFileSize { get; set; } = 50 * 1024 * 1024; // 50 MB

        /// <summary>
        /// Taille maximale du contenu à indexer par fichier (en caractères)
        /// </summary>
        public int MaxContentLength { get; set; } = 100000; // 100K caractères

        /// <summary>
        /// Inclure les fichiers cachés
        /// </summary>
        public bool IncludeHiddenFiles { get; set; } = false;

        /// <summary>
        /// Inclure les fichiers système
        /// </summary>
        public bool IncludeSystemFiles { get; set; } = false;

        /// <summary>
        /// Indexer le contenu des fichiers (pas seulement les noms)
        /// </summary>
        public bool IndexContent { get; set; } = true;

        /// <summary>
        /// Indexer les métadonnées des fichiers
        /// </summary>
        public bool IndexMetadata { get; set; } = true;

        /// <summary>
        /// Chemin vers le répertoire d'index
        /// </summary>
        public string IndexPath { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "FindEdge",
            "Index"
        );

        /// <summary>
        /// Intervalle de mise à jour automatique de l'index (en minutes)
        /// </summary>
        public int AutoUpdateIntervalMinutes { get; set; } = 60;

        /// <summary>
        /// Nombre maximum de documents dans l'index
        /// </summary>
        public int MaxDocuments { get; set; } = 1000000; // 1 million

        /// <summary>
        /// Activer la compression de l'index
        /// </summary>
        public bool EnableCompression { get; set; } = true;

        /// <summary>
        /// Activer l'indexation incrémentale
        /// </summary>
        public bool EnableIncrementalIndexing { get; set; } = true;
    }
}
