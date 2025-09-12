using System;

namespace FindEdge.Core.Models
{
    /// <summary>
    /// Statut de l'index de recherche
    /// </summary>
    public class IndexStatus
    {
        /// <summary>
        /// Indique si l'index est disponible
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Indique si l'index est en cours de construction
        /// </summary>
        public bool IsBuilding { get; set; }

        /// <summary>
        /// Nombre total de documents indexés
        /// </summary>
        public int DocumentCount { get; set; }

        /// <summary>
        /// Taille de l'index en octets
        /// </summary>
        public long IndexSize { get; set; }

        /// <summary>
        /// Date de dernière mise à jour de l'index
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Date de création de l'index
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Version de l'index
        /// </summary>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// Nombre de répertoires indexés
        /// </summary>
        public int DirectoryCount { get; set; }

        /// <summary>
        /// Nombre de fichiers traités lors de la dernière indexation
        /// </summary>
        public int FilesProcessed { get; set; }

        /// <summary>
        /// Temps de construction de l'index
        /// </summary>
        public TimeSpan BuildTime { get; set; }

        /// <summary>
        /// Message d'erreur s'il y en a un
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Progression de l'indexation (0-100)
        /// </summary>
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// Fichier actuellement en cours d'indexation
        /// </summary>
        public string? CurrentFile { get; set; }

        /// <summary>
        /// Vitesse d'indexation (documents par seconde)
        /// </summary>
        public double IndexingSpeed { get; set; }

        /// <summary>
        /// Temps estimé restant pour l'indexation
        /// </summary>
        public TimeSpan EstimatedTimeRemaining { get; set; }
    }
}
