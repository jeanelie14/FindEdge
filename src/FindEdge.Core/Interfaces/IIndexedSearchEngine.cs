using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour le moteur de recherche avec support de l'indexation
    /// </summary>
    public interface IIndexedSearchEngine : ISearchEngine
    {
        /// <summary>
        /// Mode de recherche actuel
        /// </summary>
        SearchMode CurrentSearchMode { get; set; }

        /// <summary>
        /// Gestionnaire d'index
        /// </summary>
        IIndexManager IndexManager { get; }

        /// <summary>
        /// Recherche hybride (index + live scan)
        /// </summary>
        Task<IEnumerable<SearchResult>> SearchHybridAsync(SearchOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recherche uniquement dans l'index
        /// </summary>
        Task<IEnumerable<SearchResult>> SearchIndexOnlyAsync(SearchOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recherche uniquement en live scan
        /// </summary>
        Task<IEnumerable<SearchResult>> SearchLiveOnlyAsync(SearchOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Bascule entre les modes de recherche
        /// </summary>
        void SwitchSearchMode(SearchMode mode);

        /// <summary>
        /// Obtient les statistiques de performance
        /// </summary>
        SearchPerformanceStats GetPerformanceStats();
    }

    /// <summary>
    /// Modes de recherche disponibles
    /// </summary>
    public enum SearchMode
    {
        /// <summary>
        /// Recherche hybride (index + live scan)
        /// </summary>
        Hybrid,

        /// <summary>
        /// Recherche uniquement dans l'index
        /// </summary>
        IndexOnly,

        /// <summary>
        /// Recherche uniquement en live scan
        /// </summary>
        LiveOnly
    }

    /// <summary>
    /// Statistiques de performance de recherche
    /// </summary>
    public class SearchPerformanceStats
    {
        public TimeSpan AverageIndexSearchTime { get; set; }
        public TimeSpan AverageLiveSearchTime { get; set; }
        public TimeSpan AverageHybridSearchTime { get; set; }
        public int IndexSearchCount { get; set; }
        public int LiveSearchCount { get; set; }
        public int HybridSearchCount { get; set; }
        public long TotalIndexSize { get; set; }
        public int TotalIndexedDocuments { get; set; }
        public DateTime LastIndexUpdate { get; set; }
    }
}