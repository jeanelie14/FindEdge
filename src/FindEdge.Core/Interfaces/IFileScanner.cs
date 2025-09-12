using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour le scanner de fichiers
    /// </summary>
    public interface IFileScanner
    {
        /// <summary>
        /// Scanne un répertoire et retourne tous les fichiers correspondant aux critères
        /// </summary>
        Task<IEnumerable<FileInfo>> ScanDirectoryAsync(string directoryPath, SearchOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Vérifie si un fichier correspond aux critères de filtrage
        /// </summary>
        bool IsFileMatch(FileInfo fileInfo, SearchOptions options);
        
        /// <summary>
        /// Événement déclenché lors de la découverte d'un nouveau fichier
        /// </summary>
        event EventHandler<FileFoundEventArgs>? FileFound;
    }

    /// <summary>
    /// Arguments pour l'événement de fichier trouvé
    /// </summary>
    public class FileFoundEventArgs : EventArgs
    {
        public FileInfo FileInfo { get; set; } = new();
    }

    /// <summary>
    /// Informations sur un fichier
    /// </summary>
    public class FileInfo
    {
        public string FullPath { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Directory { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public FileAttributes Attributes { get; set; }
    }
}
