using System;
using System.Collections.Generic;

namespace FindEdge.Core.Models
{
    /// <summary>
    /// Options de partage
    /// </summary>
    public class ShareOptions
    {
        public bool IsPublic { get; set; }
        public AccessLevel AccessLevel { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string[] AllowedUsers { get; set; } = Array.Empty<string>();
    }

    /// <summary>
    /// Niveau d'accès
    /// </summary>
    public enum AccessLevel
    {
        ReadOnly,
        ReadWrite,
        Admin
    }

    /// <summary>
    /// Espace de travail collaboratif (modèle simplifié pour compatibilité)
    /// </summary>
    public class CollaborativeWorkspace
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<WorkspaceMember> Members { get; set; } = new();
    }

    /// <summary>
    /// Membre d'un espace de travail (modèle simplifié pour compatibilité)
    /// </summary>
    public class WorkspaceMember
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public WorkspaceRole Role { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Rôle dans un espace de travail (modèle simplifié pour compatibilité)
    /// </summary>
    public enum WorkspaceRole
    {
        Viewer,
        Contributor,
        Admin,
        Owner
    }

    /// <summary>
    /// Annotation de recherche (modèle simplifié pour compatibilité)
    /// </summary>
    public class SearchAnnotation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string SearchId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public AnnotationType Type { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Type d'annotation (modèle simplifié pour compatibilité)
    /// </summary>
    public enum AnnotationType
    {
        Note,
        Comment,
        Tag,
        Rating
    }
}
