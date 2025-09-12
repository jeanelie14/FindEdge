using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;
using FindEdge.Core.Models;

namespace FindEdge.Presentation
{
    /// <summary>
    /// Mock du détecteur de doublons pour les tests
    /// </summary>
    public class MockDuplicateDetector : IDuplicateDetector
    {
        public event EventHandler<DuplicateDetectionProgressEventArgs>? DetectionProgress;
        public event EventHandler<DuplicateGroupFoundEventArgs>? DuplicateGroupFound;

        public async Task<IEnumerable<DuplicateGroup>> DetectDuplicatesAsync(IEnumerable<SearchResult> files, DuplicateDetectionOptions options, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000, cancellationToken); // Simuler un délai

            var groups = new List<DuplicateGroup>();
            var filesList = files.ToList();

            // Simuler quelques groupes de doublons
            if (filesList.Count >= 2)
            {
                var group1 = new DuplicateGroup
                {
                    Type = DuplicateGroupType.Identical,
                    Confidence = 1.0,
                    Files = new List<DuplicateFile>
                    {
                        new DuplicateFile
                        {
                            FilePath = filesList[0].FilePath,
                            FileName = filesList[0].FileName,
                            Directory = filesList[0].Directory,
                            FileSize = filesList[0].FileSize,
                            LastModified = filesList[0].LastModified,
                            FileHash = "ABC123",
                            IsOriginal = true
                        },
                        new DuplicateFile
                        {
                            FilePath = filesList[1].FilePath,
                            FileName = filesList[1].FileName,
                            Directory = filesList[1].Directory,
                            FileSize = filesList[1].FileSize,
                            LastModified = filesList[1].LastModified,
                            FileHash = "ABC123",
                            IsOriginal = false
                        }
                    }
                };

                group1.TotalSize = group1.Files.Sum(f => f.FileSize);
                group1.SpaceWasted = group1.TotalSize - group1.Files.First().FileSize;

                groups.Add(group1);
                OnDuplicateGroupFound(new DuplicateGroupFoundEventArgs { Group = group1 });
            }

            if (filesList.Count >= 4)
            {
                var group2 = new DuplicateGroup
                {
                    Type = DuplicateGroupType.Similar,
                    Confidence = 0.95,
                    Files = new List<DuplicateFile>
                    {
                        new DuplicateFile
                        {
                            FilePath = filesList[2].FilePath,
                            FileName = filesList[2].FileName,
                            Directory = filesList[2].Directory,
                            FileSize = filesList[2].FileSize,
                            LastModified = filesList[2].LastModified,
                            FileHash = "DEF456",
                            IsOriginal = true,
                            SimilarityScore = 1.0
                        },
                        new DuplicateFile
                        {
                            FilePath = filesList[3].FilePath,
                            FileName = filesList[3].FileName,
                            Directory = filesList[3].Directory,
                            FileSize = filesList[3].FileSize,
                            LastModified = filesList[3].LastModified,
                            FileHash = "DEF456",
                            IsOriginal = false,
                            SimilarityScore = 0.95
                        }
                    }
                };

                group2.TotalSize = group2.Files.Sum(f => f.FileSize);
                group2.SpaceWasted = group2.TotalSize - group2.Files.First().FileSize;

                groups.Add(group2);
                OnDuplicateGroupFound(new DuplicateGroupFoundEventArgs { Group = group2 });
            }

            // Simuler la progression
            for (int i = 0; i < filesList.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                OnDetectionProgress(new DuplicateDetectionProgressEventArgs
                {
                    FilesProcessed = i + 1,
                    TotalFiles = filesList.Count,
                    CurrentFile = filesList[i].FilePath,
                    ElapsedTime = TimeSpan.FromMilliseconds((i + 1) * 100),
                    DuplicateGroupsFound = groups.Count
                });

                await Task.Delay(50, cancellationToken);
            }

            return groups;
        }

        public async Task<string> CalculateFileHashAsync(string filePath, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);
            return $"HASH_{filePath.GetHashCode():X8}";
        }

        public async Task<bool> AreFilesIdenticalAsync(string filePath1, string filePath2, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);
            return filePath1.GetHashCode() == filePath2.GetHashCode();
        }

        private void OnDetectionProgress(DuplicateDetectionProgressEventArgs e)
        {
            DetectionProgress?.Invoke(this, e);
        }

        private void OnDuplicateGroupFound(DuplicateGroupFoundEventArgs e)
        {
            DuplicateGroupFound?.Invoke(this, e);
        }
    }
}
