using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour le service d'aide et d'apprentissage
    /// </summary>
    public interface IHelpLearningService
    {
        /// <summary>
        /// Lance le mode apprentissage avec des tutoriels interactifs
        /// </summary>
        Task<LearningSession> StartLearningModeAsync(string userId, LearningPath? path = null, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les tutoriels disponibles
        /// </summary>
        Task<IEnumerable<Tutorial>> GetAvailableTutorialsAsync(string category = "", CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient un tutoriel interactif
        /// </summary>
        Task<InteractiveTutorial> GetInteractiveTutorialAsync(string tutorialId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient l'assistant virtuel pour une fonctionnalité
        /// </summary>
        Task<VirtualAssistant> GetVirtualAssistantAsync(string feature, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Pose une question à l'assistant virtuel
        /// </summary>
        Task<AssistantResponse> AskAssistantAsync(string question, string context, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Recherche dans la base de connaissances
        /// </summary>
        Task<IEnumerable<KnowledgeBaseArticle>> SearchKnowledgeBaseAsync(string query, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient un article de la base de connaissances
        /// </summary>
        Task<KnowledgeBaseArticle> GetKnowledgeBaseArticleAsync(string articleId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Accède aux forums de la communauté
        /// </summary>
        Task<IEnumerable<CommunityPost>> GetCommunityPostsAsync(string category = "", int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Poste une question sur le forum communautaire
        /// </summary>
        Task<CommunityPost> PostToCommunityAsync(string title, string content, string category, string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les exemples de recherche avec explications
        /// </summary>
        Task<IEnumerable<SearchExample>> GetSearchExamplesAsync(string category = "", CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Exécute un exemple de recherche
        /// </summary>
        Task<SearchExampleResult> ExecuteSearchExampleAsync(string exampleId, Dictionary<string, object> parameters, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les conseils contextuels pour une fonctionnalité
        /// </summary>
        Task<IEnumerable<ContextualTip>> GetContextualTipsAsync(string feature, string context, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Enregistre le progrès d'apprentissage de l'utilisateur
        /// </summary>
        Task RecordLearningProgressAsync(string userId, string tutorialId, LearningProgress progress, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient le progrès d'apprentissage de l'utilisateur
        /// </summary>
        Task<LearningProgress> GetLearningProgressAsync(string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les recommandations d'apprentissage personnalisées
        /// </summary>
        Task<IEnumerable<LearningRecommendation>> GetPersonalizedRecommendationsAsync(string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les statistiques d'utilisation des fonctionnalités d'aide
        /// </summary>
        Task<HelpUsageStatistics> GetHelpUsageStatisticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Événement déclenché lors de la progression d'apprentissage
        /// </summary>
        event EventHandler<LearningProgressEventArgs>? LearningProgress;
        
        /// <summary>
        /// Événement déclenché lors de la réception d'une réponse d'assistant
        /// </summary>
        event EventHandler<AssistantResponseEventArgs>? AssistantResponse;
    }

    /// <summary>
    /// Session d'apprentissage
    /// </summary>
    public class LearningSession
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public LearningPath? Path { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public LearningSessionStatus Status { get; set; }
        public List<TutorialProgress> TutorialProgress { get; set; } = new();
        public Dictionary<string, object> SessionData { get; set; } = new();
    }

    /// <summary>
    /// Statut de session d'apprentissage
    /// </summary>
    public enum LearningSessionStatus
    {
        Active,
        Paused,
        Completed,
        Abandoned
    }

    /// <summary>
    /// Chemin d'apprentissage
    /// </summary>
    public class LearningPath
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public LearningLevel Level { get; set; }
        public List<string> TutorialIds { get; set; } = new();
        public TimeSpan EstimatedDuration { get; set; }
        public List<string> Prerequisites { get; set; } = new();
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Niveau d'apprentissage
    /// </summary>
    public enum LearningLevel
    {
        Beginner,
        Intermediate,
        Advanced,
        Expert
    }

    /// <summary>
    /// Tutoriel
    /// </summary>
    public class Tutorial
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public LearningLevel Level { get; set; }
        public List<TutorialStep> Steps { get; set; } = new();
        public TimeSpan EstimatedDuration { get; set; }
        public List<string> Prerequisites { get; set; } = new();
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Tutoriel interactif
    /// </summary>
    public class InteractiveTutorial
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public LearningLevel Level { get; set; }
        public List<InteractiveStep> Steps { get; set; } = new();
        public TimeSpan EstimatedDuration { get; set; }
        public List<string> Prerequisites { get; set; } = new();
        public bool IsInteractive { get; set; } = true;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Étape interactive
    /// </summary>
    public class InteractiveStep
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public InteractiveStepType Type { get; set; }
        public List<InteractiveAction> Actions { get; set; } = new();
        public List<InteractiveValidation> Validations { get; set; } = new();
        public Dictionary<string, object> Parameters { get; set; } = new();
        public int Order { get; set; }
    }

    /// <summary>
    /// Type d'étape interactive
    /// </summary>
    public enum InteractiveStepType
    {
        Information,
        Demonstration,
        Practice,
        Quiz,
        Challenge,
        Simulation
    }

    /// <summary>
    /// Action interactive
    /// </summary>
    public class InteractiveAction
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public InteractiveActionType Type { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
        public bool IsRequired { get; set; } = false;
    }

    /// <summary>
    /// Type d'action interactive
    /// </summary>
    public enum InteractiveActionType
    {
        Click,
        Type,
        Select,
        Drag,
        Drop,
        Navigate,
        Execute
    }

    /// <summary>
    /// Validation interactive
    /// </summary>
    public class InteractiveValidation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Expression { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public bool IsRequired { get; set; } = true;
    }

    /// <summary>
    /// Étape de tutoriel
    /// </summary>
    public class TutorialStep
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public TutorialStepType Type { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
        public List<TutorialStepAction> Actions { get; set; } = new();
        public TutorialStepValidation? Validation { get; set; }
    }

    /// <summary>
    /// Type d'étape de tutoriel
    /// </summary>
    public enum TutorialStepType
    {
        Information,
        Interactive,
        Demonstration,
        Exercise,
        Quiz,
        Video
    }

    /// <summary>
    /// Action d'étape de tutoriel
    /// </summary>
    public class TutorialStepAction
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TutorialActionType Type { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
    }

    /// <summary>
    /// Type d'action de tutoriel
    /// </summary>
    public enum TutorialActionType
    {
        Click,
        Type,
        Select,
        Navigate,
        Execute,
        Wait
    }

    /// <summary>
    /// Validation d'étape de tutoriel
    /// </summary>
    public class TutorialStepValidation
    {
        public string Expression { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;
        public string FailureMessage { get; set; } = string.Empty;
        public int MaxAttempts { get; set; } = 3;
    }

    /// <summary>
    /// Progrès de tutoriel
    /// </summary>
    public class TutorialProgress
    {
        public string TutorialId { get; set; } = string.Empty;
        public string StepId { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int Attempts { get; set; }
        public Dictionary<string, object> StepData { get; set; } = new();
    }

    /// <summary>
    /// Assistant virtuel
    /// </summary>
    public class VirtualAssistant
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Feature { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Capabilities { get; set; } = new();
        public Dictionary<string, object> Configuration { get; set; } = new();
    }

    /// <summary>
    /// Réponse d'assistant
    /// </summary>
    public class AssistantResponse
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public List<string> RelatedTopics { get; set; } = new();
        public List<AssistantAction> SuggestedActions { get; set; } = new();
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Action suggérée par l'assistant
    /// </summary>
    public class AssistantAction
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new();
    }

    /// <summary>
    /// Article de base de connaissances
    /// </summary>
    public class KnowledgeBaseArticle
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public string Author { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModified { get; set; }
        public int ViewCount { get; set; }
        public double RelevanceScore { get; set; }
        public List<string> RelatedArticles { get; set; } = new();
    }

    /// <summary>
    /// Poste communautaire
    /// </summary>
    public class CommunityPost
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastModified { get; set; }
        public int ViewCount { get; set; }
        public int ReplyCount { get; set; }
        public int LikeCount { get; set; }
        public List<string> Tags { get; set; } = new();
        public PostStatus Status { get; set; }
    }

    /// <summary>
    /// Statut de poste
    /// </summary>
    public enum PostStatus
    {
        Draft,
        Published,
        Archived,
        Deleted
    }

    /// <summary>
    /// Exemple de recherche
    /// </summary>
    public class SearchExample
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public SearchOptions SearchOptions { get; set; } = new();
        public string Explanation { get; set; } = string.Empty;
        public List<string> UseCases { get; set; } = new();
        public Dictionary<string, object> Parameters { get; set; } = new();
        public int UsageCount { get; set; }
        public double Rating { get; set; }
    }

    /// <summary>
    /// Résultat d'exemple de recherche
    /// </summary>
    public class SearchExampleResult
    {
        public string ExampleId { get; set; } = string.Empty;
        public IEnumerable<SearchResult> Results { get; set; } = new List<SearchResult>();
        public TimeSpan ExecutionTime { get; set; }
        public Dictionary<string, object> ExecutionData { get; set; } = new();
    }

    /// <summary>
    /// Conseil contextuel
    /// </summary>
    public class ContextualTip
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Feature { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty;
        public TipType Type { get; set; }
        public TipPriority Priority { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Type de conseil
    /// </summary>
    public enum TipType
    {
        Hint,
        Tip,
        Warning,
        Information,
        BestPractice
    }

    /// <summary>
    /// Priorité de conseil
    /// </summary>
    public enum TipPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    /// <summary>
    /// Progrès d'apprentissage
    /// </summary>
    public class LearningProgress
    {
        public string UserId { get; set; } = string.Empty;
        public Dictionary<string, TutorialProgress> TutorialProgress { get; set; } = new();
        public List<string> CompletedTutorials { get; set; } = new();
        public List<string> CompletedPaths { get; set; } = new();
        public int TotalPoints { get; set; }
        public LearningLevel CurrentLevel { get; set; }
        public DateTime LastActivity { get; set; }
        public Dictionary<string, object> Achievements { get; set; } = new();
    }

    /// <summary>
    /// Recommandation d'apprentissage
    /// </summary>
    public class LearningRecommendation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RecommendationType Type { get; set; }
        public string TargetId { get; set; } = string.Empty;
        public double RelevanceScore { get; set; }
        public string Reason { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Type de recommandation
    /// </summary>
    public enum RecommendationType
    {
        Tutorial,
        Path,
        Article,
        Example,
        CommunityPost
    }

    /// <summary>
    /// Statistiques d'utilisation de l'aide
    /// </summary>
    public class HelpUsageStatistics
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalSessions { get; set; }
        public int TotalTutorials { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalArticlesViewed { get; set; }
        public Dictionary<string, int> FeatureUsage { get; set; } = new();
        public Dictionary<string, int> CategoryUsage { get; set; } = new();
        public TimeSpan AverageSessionDuration { get; set; }
        public List<HelpUsageInsight> Insights { get; set; } = new();
    }

    /// <summary>
    /// Insight d'utilisation de l'aide
    /// </summary>
    public class HelpUsageInsight
    {
        public string Category { get; set; } = string.Empty;
        public string Insight { get; set; } = string.Empty;
        public double Value { get; set; }
    }

    // Événements
    public class LearningProgressEventArgs : EventArgs
    {
        public string UserId { get; set; } = string.Empty;
        public string TutorialId { get; set; } = string.Empty;
        public LearningProgress Progress { get; set; } = new();
    }

    public class AssistantResponseEventArgs : EventArgs
    {
        public string UserId { get; set; } = string.Empty;
        public AssistantResponse Response { get; set; } = new();
    }
}
