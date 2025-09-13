using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour le service de workflow et d'automatisation
    /// </summary>
    public interface IWorkflowAutomationService
    {
        /// <summary>
        /// Exécute une action automatisée
        /// </summary>
        Task<bool> ExecuteAutomatedActionAsync(string actionId, IEnumerable<SearchResult> results, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure l'intégration CI/CD
        /// </summary>
        Task<bool> IntegrateWithCICDAsync(string pipelineId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure une tâche planifiée
        /// </summary>
        Task<bool> ScheduleTaskAsync(string taskId, DateTime startTime, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure un template de recherche
        /// </summary>
        Task<bool> CreateSearchTemplateAsync(SearchTemplate template, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les templates de recherche
        /// </summary>
        Task<IEnumerable<SearchTemplate>> GetSearchTemplatesAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Template de recherche
    /// </summary>
    public class SearchTemplate
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public SearchOptions SearchOptions { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}