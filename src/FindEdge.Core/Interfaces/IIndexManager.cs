using System;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour la gestion de l'index de recherche
    /// </summary>
    public interface IIndexManager
    {
        /// <summary>
        /// Crée ou met à jour l'index de recherche
        /// </summary>
        Task BuildIndexAsync(IndexConfiguration configuration, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recherche dans l'index
        /// </summary>
        Task<IEnumerable<SearchResult>> SearchIndexAsync(SearchOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Met à jour l'index de manière incrémentale
        /// </summary>
        Task UpdateIndexAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Supprime l'index
        /// </summary>
        Task DeleteIndexAsync();

        /// <summary>
        /// Obtient le statut de l'index
        /// </summary>
        Task<IndexStatus> GetIndexStatusAsync();

        /// <summary>
        /// Vérifie si l'index est disponible
        /// </summary>
        bool IsIndexAvailable { get; }

        /// <summary>
        /// Obtient la configuration de l'index
        /// </summary>
        IndexConfiguration? Configuration { get; }

        /// <summary>
        /// Événement déclenché lors de la progression de l'indexation
        /// </summary>
        event EventHandler<IndexProgressEventArgs>? IndexProgress;

        /// <summary>
        /// Événement déclenché lors de la fin de l'indexation
        /// </summary>
        event EventHandler<IndexCompletedEventArgs>? IndexCompleted;

        /// <summary>
        /// Événement déclenché lors d'une erreur d'indexation
        /// </summary>
        event EventHandler<IndexErrorEventArgs>? IndexError;
    }

    /// <summary>
    /// Arguments pour l'événement de progression de l'indexation
    /// </summary>
    public class IndexProgressEventArgs : EventArgs
    {
        public int DocumentsProcessed { get; set; }
        public int TotalDocuments { get; set; }
        public string CurrentFile { get; set; } = string.Empty;
        public TimeSpan ElapsedTime { get; set; }
        public double Speed { get; set; }
        public TimeSpan EstimatedTimeRemaining { get; set; }
    }

    /// <summary>
    /// Arguments pour l'événement de fin d'indexation
    /// </summary>
    public class IndexCompletedEventArgs : EventArgs
    {
        public int TotalDocuments { get; set; }
        public TimeSpan TotalTime { get; set; }
        public long IndexSize { get; set; }
        public bool Success { get; set; }
    }

    /// <summary>
    /// Arguments pour l'événement d'erreur d'indexation
    /// </summary>
    public class IndexErrorEventArgs : EventArgs
    {
        public string ErrorMessage { get; set; } = string.Empty;
        public Exception? Exception { get; set; }
        public string? FilePath { get; set; }
    }
}
