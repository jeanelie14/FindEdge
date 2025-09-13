using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;
using FindEdge.Core.Models;
using WorkspaceRole = FindEdge.Core.Interfaces.WorkspaceRole;
using SharePermissions = FindEdge.Core.Interfaces.SharePermissions;
using NotificationSettings = FindEdge.Core.Interfaces.NotificationSettings;
using Notification = FindEdge.Core.Interfaces.Notification;
using FileAnnotation = FindEdge.Core.Interfaces.FileAnnotation;
using Workspace = FindEdge.Core.Interfaces.Workspace;
using WorkspaceMember = FindEdge.Core.Interfaces.WorkspaceMember;
using FileWatcher = FindEdge.Core.Interfaces.FileWatcher;
using FileVersion = FindEdge.Core.Interfaces.FileVersion;
using AnnotationType = FindEdge.Core.Interfaces.AnnotationType;

namespace FindEdge.Infrastructure.Services
{
    /// <summary>
    /// Implémentation mock du service de collaboration
    /// </summary>
    public class MockCollaborationService : ICollaborationService
    {
        public event EventHandler<NotificationReceivedEventArgs>? NotificationReceived;
        public event EventHandler<FileChangedEventArgs>? FileChanged;
        public event EventHandler<WorkflowExecutedEventArgs>? WorkflowExecuted;

        public async Task<SharedSearch> ShareSearchAsync(string searchId, IEnumerable<string> userIds, SharePermissions permissions, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);
            return new SharedSearch
            {
                Id = Guid.NewGuid().ToString(),
                SearchId = searchId,
                OwnerId = "current-user",
                SharedWithUserIds = new List<string>(userIds),
                Permissions = permissions,
                IsActive = true
            };
        }

        public async Task<SharedSearch> ShareSearchQueryAsync(string searchQuery, IEnumerable<string> userIds, SharePermissions permissions, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);
            return new SharedSearch
            {
                Id = Guid.NewGuid().ToString(),
                SearchId = Guid.NewGuid().ToString(), // Génère un ID pour la requête
                OwnerId = "current-user",
                SharedWithUserIds = new List<string>(userIds),
                Permissions = permissions,
                IsActive = true
            };
        }

        public async Task<IEnumerable<SharedSearch>> GetSharedSearchesAsync(string userId, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);
            return new List<SharedSearch>();
        }

        public async Task<FileAnnotation> AddAnnotationAsync(string filePath, string annotation, string userId, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);
            return new FileAnnotation
            {
                Id = Guid.NewGuid().ToString(),
                FilePath = filePath,
                Content = annotation,
                UserId = userId,
                Type = AnnotationType.Comment
            };
        }

        public async Task<IEnumerable<FileAnnotation>> GetFileAnnotationsAsync(string filePath, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);
            return new List<FileAnnotation>();
        }

        public async Task<Workspace> CreateWorkspaceAsync(string name, string description, IEnumerable<string> memberIds, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);
            return new Workspace
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Description = description,
                OwnerId = "current-user",
                Members = new List<WorkspaceMember>()
            };
        }

        public async Task<IEnumerable<Workspace>> GetUserWorkspacesAsync(string userId, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);
            return new List<Workspace>();
        }

        public async Task AddWorkspaceMemberAsync(string workspaceId, string userId, WorkspaceRole role, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);
            // Mock implementation - no actual work needed
        }

        public async Task ConfigureNotificationsAsync(string userId, NotificationSettings settings, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);
            // Mock implementation - no actual work needed
        }

        public async Task SendNotificationAsync(string userId, Notification notification, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);
            NotificationReceived?.Invoke(this, new NotificationReceivedEventArgs { Notification = notification });
        }

        public async Task<FileWatcher> WatchFileAsync(string filePath, string userId, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);
            return new FileWatcher
            {
                Id = Guid.NewGuid().ToString(),
                FilePath = filePath,
                UserId = userId,
                IsActive = true
            };
        }

        public async Task<IEnumerable<FileVersion>> GetFileVersionsAsync(string filePath, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);
            return new List<FileVersion>();
        }

        public async Task RestoreFileVersionAsync(string filePath, string versionId, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);
            // Mock implementation - no actual work needed
        }

    }
}