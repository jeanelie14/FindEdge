using FindEdge.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FindEdge.Infrastructure.Services
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddAdvancedFindEdgeServices(this IServiceCollection services)
        {
            // Core working services
            services.AddSingleton<ISemanticSearchEngine, SemanticSearchEngine>();
            services.AddSingleton<IIntelligentSuggestions, IntelligentSuggestionsService>();
            services.AddSingleton<IVisualizationService, VisualizationService>();
            
            // Temporarily comment out problematic services until they're fully implemented
            // services.AddSingleton<IDistributedSearchEngine, DistributedSearchEngine>();
            // services.AddSingleton<IPerformanceOptimizationService, PerformanceOptimizationService>();
            // services.AddSingleton<IAnalyticsService, AnalyticsService>();
            // services.AddSingleton<ISecurityService, SecurityService>();
            // services.AddSingleton<ICollaborationService, CollaborationService>();
            // services.AddSingleton<IHelpLearningService, HelpLearningService>();
            // services.AddSingleton<IPersonalizationService, PersonalizationService>();
            // services.AddSingleton<IApiIntegrationService, ApiIntegrationService>();
            // services.AddSingleton<IWorkflowAutomationService, WorkflowAutomationService>();
            // services.AddSingleton<IEmergingTechnologiesService, EmergingTechnologiesService>();

            return services;
        }
    }
}