using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour le moteur de recherche sémantique basé sur l'IA
    /// </summary>
    public interface ISemanticSearchEngine
    {
        /// <summary>
        /// Recherche sémantique par similarité de contenu
        /// </summary>
        Task<IEnumerable<SearchResult>> SearchBySimilarityAsync(string query, SearchOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Recherche en langage naturel
        /// </summary>
        Task<IEnumerable<SearchResult>> SearchByNaturalLanguageAsync(string naturalLanguageQuery, SearchOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Classification automatique des fichiers par contenu
        /// </summary>
        Task<IEnumerable<FileClassification>> ClassifyFilesAsync(IEnumerable<string> filePaths, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Détection automatique de la langue des documents
        /// </summary>
        Task<LanguageDetectionResult> DetectLanguageAsync(string filePath, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Génération d'embeddings vectoriels pour un contenu
        /// </summary>
        Task<float[]> GenerateEmbeddingAsync(string content, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Calcul de similarité entre deux contenus
        /// </summary>
        Task<double> CalculateSimilarityAsync(string content1, string content2, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Événement déclenché lors de la progression de la recherche sémantique
        /// </summary>
        event EventHandler<SemanticSearchProgressEventArgs>? SemanticSearchProgress;
    }

    /// <summary>
    /// Résultat de classification de fichier
    /// </summary>
    public class FileClassification
    {
        public string FilePath { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public Dictionary<string, double> CategoryScores { get; set; } = new();
        public List<string> Tags { get; set; } = new();
    }

    /// <summary>
    /// Résultat de détection de langue
    /// </summary>
    public class LanguageDetectionResult
    {
        public string DetectedLanguage { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public Dictionary<string, double> LanguageScores { get; set; } = new();
    }

    /// <summary>
    /// Arguments pour l'événement de progression de recherche sémantique
    /// </summary>
    public class SemanticSearchProgressEventArgs : EventArgs
    {
        public int FilesProcessed { get; set; }
        public int TotalFiles { get; set; }
        public string CurrentFile { get; set; } = string.Empty;
        public TimeSpan ElapsedTime { get; set; }
        public string CurrentOperation { get; set; } = string.Empty;
    }
}
