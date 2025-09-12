using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour le moteur de recherche principal
    /// </summary>
    public interface ISearchEngine
    {
        /// <summary>
        /// Recherche asynchrone avec options configurables
        /// </summary>
        Task<IEnumerable<SearchResult>> SearchAsync(SearchOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Recherche synchrone (pour compatibilité)
        /// </summary>
        IEnumerable<SearchResult> Search(SearchOptions options);
        
        /// <summary>
        /// Événement déclenché lors de la progression de la recherche
        /// </summary>
        event EventHandler<SearchProgressEventArgs>? SearchProgress;
        
        /// <summary>
        /// Événement déclenché lors de la découverte d'un nouveau résultat
        /// </summary>
        event EventHandler<SearchResultEventArgs>? ResultFound;
    }

    /// <summary>
    /// Arguments pour l'événement de progression
    /// </summary>
    public class SearchProgressEventArgs : EventArgs
    {
        public int FilesProcessed { get; set; }
        public int TotalFiles { get; set; }
        public string CurrentFile { get; set; } = string.Empty;
        public TimeSpan ElapsedTime { get; set; }
    }

    /// <summary>
    /// Arguments pour l'événement de résultat trouvé
    /// </summary>
    public class SearchResultEventArgs : EventArgs
    {
        public SearchResult Result { get; set; } = new();
    }
}
