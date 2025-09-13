using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour le service de collaboration et partage
    /// </summary>
    public interface ICollaborationService
    {
        /// <summary>
        /// Partage une recherche avec d'autres utilisateurs
        /// </summary>
        Task<SharedSearch> ShareSearchAsync(string searchId, IEnumerable<string> userIds, SharePermissions permissions, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Partage une requête de recherche avec d'autres utilisateurs
        /// </summary>
        Task<SharedSearch> ShareSearchQueryAsync(string searchQuery, IEnumerable<string> userIds, SharePermissions permissions, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les recherches partagées avec l'utilisateur
        /// </summary>
        Task<IEnumerable<SharedSearch>> GetSharedSearchesAsync(string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Ajoute une annotation à un fichier
        /// </summary>
        Task<FileAnnotation> AddAnnotationAsync(string filePath, string annotation, string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les annotations d'un fichier
        /// </summary>
        Task<IEnumerable<FileAnnotation>> GetFileAnnotationsAsync(string filePath, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Crée un espace de travail collaboratif
        /// </summary>
        Task<Workspace> CreateWorkspaceAsync(string name, string description, IEnumerable<string> memberIds, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les espaces de travail de l'utilisateur
        /// </summary>
        Task<IEnumerable<Workspace>> GetUserWorkspacesAsync(string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Ajoute un membre à un espace de travail
        /// </summary>
        Task AddWorkspaceMemberAsync(string workspaceId, string userId, WorkspaceRole role, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure les notifications pour un utilisateur
        /// </summary>
        Task ConfigureNotificationsAsync(string userId, NotificationSettings settings, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Envoie une notification
        /// </summary>
        Task SendNotificationAsync(string userId, Notification notification, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Suit les modifications d'un fichier
        /// </summary>
        Task<FileWatcher> WatchFileAsync(string filePath, string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient l'historique des versions d'un fichier
        /// </summary>
        Task<IEnumerable<FileVersion>> GetFileVersionsAsync(string filePath, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Restaure une version précédente d'un fichier
        /// </summary>
        Task RestoreFileVersionAsync(string filePath, string versionId, CancellationToken cancellationToken = default);
        
        
        /// <summary>
        /// Événement déclenché lors de la réception d'une notification
        /// </summary>
        event EventHandler<NotificationReceivedEventArgs>? NotificationReceived;
        
        /// <summary>
        /// Événement déclenché lors de la modification d'un fichier suivi
        /// </summary>
        event EventHandler<FileChangedEventArgs>? FileChanged;
        
        /// <summary>
        /// Événement déclenché lors de l'exécution d'un workflow
        /// </summary>
        event EventHandler<WorkflowExecutedEventArgs>? WorkflowExecuted;
    }

    /// <summary>
    /// Recherche partagée
    /// </summary>
    public class SharedSearch
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string SearchId { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public List<string> SharedWithUserIds { get; set; } = new();
        public SharePermissions Permissions { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Permissions de partage
    /// </summary>
    public class SharePermissions
    {
        public bool CanView { get; set; } = true;
        public bool CanEdit { get; set; } = false;
        public bool CanShare { get; set; } = false;
        public bool CanDelete { get; set; } = false;
        public Dictionary<string, object> CustomPermissions { get; set; } = new();
    }

    /// <summary>
    /// Annotation de fichier
    /// </summary>
    public class FileAnnotation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FilePath { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; set; }
        public List<string> Tags { get; set; } = new();
        public AnnotationType Type { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Type d'annotation
    /// </summary>
    public enum AnnotationType
    {
        Comment,
        Tag,
        Rating,
        Bookmark,
        Note
    }

    /// <summary>
    /// Espace de travail
    /// </summary>
    public class Workspace
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public List<WorkspaceMember> Members { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastActivity { get; set; }
        public WorkspaceSettings Settings { get; set; } = new();
    }

    /// <summary>
    /// Membre d'espace de travail
    /// </summary>
    public class WorkspaceMember
    {
        public string UserId { get; set; } = string.Empty;
        public WorkspaceRole Role { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastActive { get; set; }
    }

    /// <summary>
    /// Rôle dans l'espace de travail
    /// </summary>
    public enum WorkspaceRole
    {
        Owner,
        Admin,
        Editor,
        Viewer
    }

    /// <summary>
    /// Paramètres d'espace de travail
    /// </summary>
    public class WorkspaceSettings
    {
        public bool AllowPublicSharing { get; set; } = false;
        public bool RequireApprovalForSharing { get; set; } = false;
        public int MaxMembers { get; set; } = 100;
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Paramètres de notification
    /// </summary>
    public class NotificationSettings
    {
        public bool EnableEmailNotifications { get; set; } = true;
        public bool EnablePushNotifications { get; set; } = true;
        public bool EnableInAppNotifications { get; set; } = true;
        public List<NotificationType> EnabledTypes { get; set; } = new();
        public TimeSpan QuietHoursStart { get; set; } = TimeSpan.FromHours(22);
        public TimeSpan QuietHoursEnd { get; set; } = TimeSpan.FromHours(8);
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Type de notification
    /// </summary>
    public enum NotificationType
    {
        FileChanged,
        SearchShared,
        AnnotationAdded,
        WorkspaceInvitation,
        WorkflowCompleted,
        SystemAlert
    }

    /// <summary>
    /// Notification
    /// </summary>
    public class Notification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public Dictionary<string, object> Data { get; set; } = new();
    }

    /// <summary>
    /// Observateur de fichier
    /// </summary>
    public class FileWatcher
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FilePath { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public List<WatchEvent> Events { get; set; } = new();
    }

    /// <summary>
    /// Événement de surveillance
    /// </summary>
    public class WatchEvent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public WatchEventType Type { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, object> Details { get; set; } = new();
    }

    /// <summary>
    /// Type d'événement de surveillance
    /// </summary>
    public enum WatchEventType
    {
        Created,
        Modified,
        Deleted,
        Moved,
        Renamed
    }

    /// <summary>
    /// Version de fichier
    /// </summary>
    public class FileVersion
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FilePath { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public long Size { get; set; }
        public string Checksum { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Définition de workflow
    /// </summary>
    public class WorkflowDefinition
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<WorkflowStep> Steps { get; set; } = new();
        public Dictionary<string, object> Configuration { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
    }

    /// <summary>
    /// Étape de workflow
    /// </summary>
    public class WorkflowStep
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public WorkflowStepType Type { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
        public List<string> NextSteps { get; set; } = new();
        public WorkflowCondition? Condition { get; set; }
    }

    /// <summary>
    /// Type d'étape de workflow
    /// </summary>
    public enum WorkflowStepType
    {
        Search,
        Filter,
        Transform,
        Export,
        Notify,
        ExecuteScript,
        Wait,
        Conditional
    }

    /// <summary>
    /// Condition de workflow
    /// </summary>
    public class WorkflowCondition
    {
        public string Expression { get; set; } = string.Empty;
        public Dictionary<string, object> Variables { get; set; } = new();
    }

    /// <summary>
    /// Exécution de workflow
    /// </summary>
    public class WorkflowExecution
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string WorkflowId { get; set; } = string.Empty;
        public WorkflowStatus Status { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
        public Dictionary<string, object> Results { get; set; } = new();
        public List<WorkflowExecutionStep> ExecutedSteps { get; set; } = new();
    }

    /// <summary>
    /// Statut de workflow
    /// </summary>
    public enum WorkflowStatus
    {
        Pending,
        Running,
        Completed,
        Failed,
        Cancelled
    }

    /// <summary>
    /// Étape exécutée de workflow
    /// </summary>
    public class WorkflowExecutionStep
    {
        public string StepId { get; set; } = string.Empty;
        public WorkflowStatus Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public Dictionary<string, object> Input { get; set; } = new();
        public Dictionary<string, object> Output { get; set; } = new();
        public string ErrorMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// Configuration CI/CD
    /// </summary>
    public class CICDConfiguration
    {
        public string Name { get; set; } = string.Empty;
        public CICDProvider Provider { get; set; }
        public string ConnectionString { get; set; } = string.Empty;
        public Dictionary<string, object> Settings { get; set; } = new();
    }

    /// <summary>
    /// Fournisseur CI/CD
    /// </summary>
    public enum CICDProvider
    {
        AzureDevOps,
        GitHubActions,
        Jenkins,
        GitLabCI,
        TeamCity,
        Bamboo
    }

    /// <summary>
    /// Intégration CI/CD
    /// </summary>
    public class CICDIntegration
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public CICDProvider Provider { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> Configuration { get; set; } = new();
    }

    /// <summary>
    /// Configuration de webhook
    /// </summary>
    public class WebhookConfiguration
    {
        public string Name { get; set; } = string.Empty;
        public List<WebhookEvent> Events { get; set; } = new();
        public Dictionary<string, string> Headers { get; set; } = new();
        public string Secret { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 30;
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Événement de webhook
    /// </summary>
    public enum WebhookEvent
    {
        SearchCompleted,
        FileFound,
        WorkflowCompleted,
        AnnotationAdded,
        FileChanged
    }

    /// <summary>
    /// Webhook
    /// </summary>
    public class Webhook
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Url { get; set; } = string.Empty;
        public WebhookConfiguration Configuration { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Définition de planification
    /// </summary>
    public class ScheduleDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CronExpression { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
    }

    /// <summary>
    /// Tâche planifiée
    /// </summary>
    public class ScheduledTask
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public ScheduleDefinition Definition { get; set; } = new();
        public DateTime NextRun { get; set; }
        public DateTime? LastRun { get; set; }
        public bool IsEnabled { get; set; } = true;
    }


    // Événements
    public class NotificationReceivedEventArgs : EventArgs
    {
        public Notification Notification { get; set; } = new();
    }

    public class FileChangedEventArgs : EventArgs
    {
        public string FilePath { get; set; } = string.Empty;
        public WatchEventType ChangeType { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class WorkflowExecutedEventArgs : EventArgs
    {
        public WorkflowExecution Execution { get; set; } = new();
    }
}
