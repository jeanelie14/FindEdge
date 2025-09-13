using System;
using System.Collections.Generic;

namespace FindEdge.Core.Models
{
    /// <summary>
    /// Article d'aide (modèle simplifié pour compatibilité)
    /// </summary>
    public class HelpArticle
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Module d'apprentissage
    /// </summary>
    public class LearningModule
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// Feedback utilisateur
    /// </summary>
    public class Feedback
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Content { get; set; } = string.Empty;
        public FeedbackType Type { get; set; }
        public string SubmittedBy { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Type de feedback
    /// </summary>
    public enum FeedbackType
    {
        Bug,
        Feature,
        Improvement,
        General
    }

    /// <summary>
    /// Difficulté du tutoriel
    /// </summary>
    public enum TutorialDifficulty
    {
        Beginner,
        Intermediate,
        Advanced
    }
}
