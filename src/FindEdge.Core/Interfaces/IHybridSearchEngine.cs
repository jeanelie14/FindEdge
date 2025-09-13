using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour le moteur de recherche hybride
    /// </summary>
    public interface IHybridSearchEngine : ISearchEngine
    {
        /// <summary>
        /// Effectue une recherche hybride combinant index et scan en direct
        /// </summary>
        Task<IEnumerable<SearchResult>> SearchHybridAsync(SearchOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Effectue une recherche uniquement sur l'index
        /// </summary>
        Task<IEnumerable<SearchResult>> SearchIndexOnlyAsync(SearchOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Effectue une recherche uniquement en scan direct
        /// </summary>
        Task<IEnumerable<SearchResult>> SearchLiveOnlyAsync(SearchOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les statistiques de performance du moteur hybride
        /// </summary>
        Task<HybridSearchStats> GetPerformanceStatsAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure le mode de recherche hybride
        /// </summary>
        Task ConfigureHybridModeAsync(HybridSearchMode mode, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Mode de recherche hybride
    /// </summary>
    public enum HybridSearchMode
    {
        IndexFirst,
        LiveFirst,
        Parallel,
        Adaptive
    }

    /// <summary>
    /// Statistiques de recherche hybride
    /// </summary>
    public class HybridSearchStats
    {
        public int TotalSearches { get; set; }
        public int IndexSearchCount { get; set; }
        public int LiveSearchCount { get; set; }
        public int HybridSearchCount { get; set; }
        public TimeSpan AverageIndexSearchTime { get; set; }
        public TimeSpan AverageLiveSearchTime { get; set; }
        public TimeSpan AverageHybridSearchTime { get; set; }
        public double IndexHitRate { get; set; }
        public double LiveHitRate { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
