using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour le moteur de recherche distribuée et cloud
    /// </summary>
    public interface IDistributedSearchEngine
    {
        /// <summary>
        /// Recherche sur plusieurs machines du réseau local
        /// </summary>
        Task<IEnumerable<SearchResult>> SearchLocalNetworkAsync(SearchOptions options, IEnumerable<string> machineAddresses, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Recherche dans les services cloud (OneDrive, Google Drive, etc.)
        /// </summary>
        Task<IEnumerable<SearchResult>> SearchCloudServicesAsync(SearchOptions options, IEnumerable<CloudService> services, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Synchronise les index entre plusieurs appareils
        /// </summary>
        Task SyncIndexesAsync(IEnumerable<string> deviceIds, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient la liste des machines disponibles sur le réseau
        /// </summary>
        Task<IEnumerable<NetworkMachine>> GetAvailableMachinesAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient la liste des services cloud configurés
        /// </summary>
        Task<IEnumerable<CloudService>> GetConfiguredCloudServicesAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure un service cloud
        /// </summary>
        Task ConfigureCloudServiceAsync(CloudService service, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Synchronise l'historique de recherche entre appareils
        /// </summary>
        Task SyncSearchHistoryAsync(string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Synchronise les favoris entre appareils
        /// </summary>
        Task SyncFavoritesAsync(string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Événement déclenché lors de la découverte d'une nouvelle machine
        /// </summary>
        event EventHandler<NetworkMachineDiscoveredEventArgs>? MachineDiscovered;
        
        /// <summary>
        /// Événement déclenché lors de la progression de la synchronisation
        /// </summary>
        event EventHandler<SyncProgressEventArgs>? SyncProgress;
    }

    /// <summary>
    /// Service cloud configuré
    /// </summary>
    public class CloudService
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public CloudServiceType Type { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenExpiry { get; set; }
        public Dictionary<string, string> Configuration { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
    }

    /// <summary>
    /// Type de service cloud
    /// </summary>
    public enum CloudServiceType
    {
        OneDrive,
        GoogleDrive,
        Dropbox,
        SharePoint,
        Box,
        iCloud
    }

    /// <summary>
    /// Machine du réseau local
    /// </summary>
    public class NetworkMachine
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastSeen { get; set; }
        public string OperatingSystem { get; set; } = string.Empty;
        public string FindEdgeVersion { get; set; } = string.Empty;
    }

    /// <summary>
    /// Arguments pour l'événement de découverte de machine
    /// </summary>
    public class NetworkMachineDiscoveredEventArgs : EventArgs
    {
        public NetworkMachine Machine { get; set; } = new();
    }

    /// <summary>
    /// Arguments pour l'événement de progression de synchronisation
    /// </summary>
    public class SyncProgressEventArgs : EventArgs
    {
        public string Operation { get; set; } = string.Empty;
        public int ItemsProcessed { get; set; }
        public int TotalItems { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public string CurrentItem { get; set; } = string.Empty;
    }
}
