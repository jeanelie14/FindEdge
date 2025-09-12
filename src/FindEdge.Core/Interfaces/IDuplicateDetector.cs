using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour la détection de doublons de fichiers
    /// </summary>
    public interface IDuplicateDetector
    {
        /// <summary>
        /// Détecte les doublons dans une liste de fichiers
        /// </summary>
        Task<IEnumerable<DuplicateGroup>> DetectDuplicatesAsync(IEnumerable<SearchResult> files, DuplicateDetectionOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Calcule l'empreinte numérique d'un fichier
        /// </summary>
        Task<string> CalculateFileHashAsync(string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Compare deux fichiers pour déterminer s'ils sont identiques
        /// </summary>
        Task<bool> AreFilesIdenticalAsync(string filePath1, string filePath2, CancellationToken cancellationToken = default);

        /// <summary>
        /// Événement déclenché lors de la progression de la détection
        /// </summary>
        event EventHandler<DuplicateDetectionProgressEventArgs>? DetectionProgress;

        /// <summary>
        /// Événement déclenché lors de la découverte d'un groupe de doublons
        /// </summary>
        event EventHandler<DuplicateGroupFoundEventArgs>? DuplicateGroupFound;
    }

    /// <summary>
    /// Options de configuration pour la détection de doublons
    /// </summary>
    public class DuplicateDetectionOptions
    {
        /// <summary>
        /// Méthode de détection à utiliser
        /// </summary>
        public DuplicateDetectionMethod Method { get; set; } = DuplicateDetectionMethod.Hash;

        /// <summary>
        /// Taille minimale des fichiers à analyser (en octets)
        /// </summary>
        public long MinFileSize { get; set; } = 1024; // 1 KB

        /// <summary>
        /// Taille maximale des fichiers à analyser (en octets)
        /// </summary>
        public long MaxFileSize { get; set; } = 100 * 1024 * 1024; // 100 MB

        /// <summary>
        /// Seuil de similarité pour la détection par contenu (0.0 à 1.0)
        /// </summary>
        public double SimilarityThreshold { get; set; } = 0.95;

        /// <summary>
        /// Inclure les fichiers cachés
        /// </summary>
        public bool IncludeHiddenFiles { get; set; } = false;

        /// <summary>
        /// Inclure les fichiers système
        /// </summary>
        public bool IncludeSystemFiles { get; set; } = false;

        /// <summary>
        /// Extensions de fichiers à exclure
        /// </summary>
        public List<string> ExcludeExtensions { get; set; } = new();

        /// <summary>
        /// Répertoires à exclure
        /// </summary>
        public List<string> ExcludeDirectories { get; set; } = new();
    }

    /// <summary>
    /// Méthodes de détection de doublons
    /// </summary>
    public enum DuplicateDetectionMethod
    {
        /// <summary>
        /// Détection par empreinte numérique (hash)
        /// </summary>
        Hash,

        /// <summary>
        /// Détection par contenu (comparaison de contenu)
        /// </summary>
        Content,

        /// <summary>
        /// Détection hybride (hash + contenu)
        /// </summary>
        Hybrid
    }

    /// <summary>
    /// Groupe de fichiers doublons
    /// </summary>
    public class DuplicateGroup
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public List<DuplicateFile> Files { get; set; } = new();
        public DuplicateGroupType Type { get; set; }
        public double Confidence { get; set; }
        public long TotalSize { get; set; }
        public long SpaceWasted { get; set; }
        public DateTime DetectedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Fichier dans un groupe de doublons
    /// </summary>
    public class DuplicateFile
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Directory { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime LastModified { get; set; }
        public string FileHash { get; set; } = string.Empty;
        public bool IsOriginal { get; set; } = false;
        public double SimilarityScore { get; set; } = 1.0;
    }

    /// <summary>
    /// Type de groupe de doublons
    /// </summary>
    public enum DuplicateGroupType
    {
        /// <summary>
        /// Fichiers identiques (même hash)
        /// </summary>
        Identical,

        /// <summary>
        /// Fichiers similaires (contenu similaire)
        /// </summary>
        Similar,

        /// <summary>
        /// Fichiers avec le même nom mais contenu différent
        /// </summary>
        SameName
    }

    /// <summary>
    /// Arguments pour l'événement de progression de détection
    /// </summary>
    public class DuplicateDetectionProgressEventArgs : EventArgs
    {
        public int FilesProcessed { get; set; }
        public int TotalFiles { get; set; }
        public string CurrentFile { get; set; } = string.Empty;
        public TimeSpan ElapsedTime { get; set; }
        public int DuplicateGroupsFound { get; set; }
    }

    /// <summary>
    /// Arguments pour l'événement de groupe de doublons trouvé
    /// </summary>
    public class DuplicateGroupFoundEventArgs : EventArgs
    {
        public DuplicateGroup Group { get; set; } = new();
    }
}
