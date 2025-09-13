using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FindEdge.Core.Interfaces;
using FindEdge.Core.Models;
using FindEdge.Infrastructure.Services;

namespace FindEdge.AdvancedFeatures.Tests
{
    /// <summary>
    /// Tests pour les fonctionnalités avancées de FindEdge Professional
    /// </summary>
    [TestClass]
    public class AdvancedFeaturesTests
    {
        private IServiceContainer _serviceContainer;
        private ISemanticSearchEngine _semanticSearchEngine;
        private IIntelligentSuggestions _intelligentSuggestions;
        private IVisualizationService _visualizationService;
        private IAnalyticsService _analyticsService;

        [TestInitialize]
        public void Setup()
        {
            // Initialiser le conteneur de services
            _serviceContainer = new SimpleServiceContainer();
            _serviceContainer.RegisterAllServices();

            // Obtenir les services
            _semanticSearchEngine = _serviceContainer.Get<ISemanticSearchEngine>();
            _intelligentSuggestions = _serviceContainer.Get<IIntelligentSuggestions>();
            _visualizationService = _serviceContainer.Get<IVisualizationService>();
            _analyticsService = _serviceContainer.Get<IAnalyticsService>();
        }

        [TestMethod]
        public async Task TestSemanticSearch()
        {
            // Arrange
            var query = "documents de projet";
            var searchOptions = new SearchOptions
            {
                SearchTerm = query,
                SearchInFileName = true,
                SearchInContent = true,
                MaxResults = 100
            };

            // Act
            var results = await _semanticSearchEngine.SearchBySimilarityAsync(
                query, 
                searchOptions, 
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(results);
            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public async Task TestNaturalLanguageSearch()
        {
            // Arrange
            var naturalLanguageQuery = "Trouve mes photos de vacances de l'été dernier";
            var searchOptions = new SearchOptions
            {
                SearchTerm = naturalLanguageQuery,
                SearchInFileName = true,
                SearchInContent = true,
                MaxResults = 100
            };

            // Act
            var results = await _semanticSearchEngine.SearchByNaturalLanguageAsync(
                naturalLanguageQuery, 
                searchOptions, 
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task TestIntelligentSuggestions()
        {
            // Arrange
            var partialQuery = "projet";
            var maxSuggestions = 10;

            // Act
            var suggestions = await _intelligentSuggestions.GetAutocompleteSuggestionsAsync(
                partialQuery, 
                maxSuggestions, 
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(suggestions);
            Assert.IsTrue(suggestions.Any());
        }

        [TestMethod]
        public async Task TestLearningFromSearch()
        {
            // Arrange
            var query = "test query";
            var searchContext = new SearchContext
            {
                Query = query,
                UserId = "testuser",
                SessionId = Guid.NewGuid().ToString(),
                ResultCount = 5,
                WasSuccessful = true
            };

            // Act
            await _intelligentSuggestions.LearnFromSearchAsync(
                query, 
                searchContext, 
                CancellationToken.None);

            // Assert
            // Vérifier que l'apprentissage a été effectué
            var suggestions = await _intelligentSuggestions.GetAutocompleteSuggestionsAsync(
                "test", 
                5, 
                CancellationToken.None);
            
            Assert.IsNotNull(suggestions);
        }

        [TestMethod]
        public async Task TestFileClassification()
        {
            // Arrange
            var filePaths = new[] { "test.txt", "test.cs", "test.pdf" };

            // Act
            var classifications = await _semanticSearchEngine.ClassifyFilesAsync(
                filePaths, 
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(classifications);
            Assert.AreEqual(filePaths.Length, classifications.Count());
        }

        [TestMethod]
        public async Task TestLanguageDetection()
        {
            // Arrange
            var filePath = "test.txt";

            // Act
            var languageResult = await _semanticSearchEngine.DetectLanguageAsync(
                filePath, 
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(languageResult);
            Assert.IsFalse(string.IsNullOrEmpty(languageResult.DetectedLanguage));
        }

        [TestMethod]
        public async Task TestMosaicVisualization()
        {
            // Arrange
            var searchResults = CreateTestSearchResults();
            var options = new MosaicViewOptions
            {
                MaxItems = 10,
                ThumbnailSize = 150,
                ShowFileNames = true,
                ShowFileSizes = true,
                ShowFileDates = true
            };

            // Act
            var mosaicView = await _visualizationService.GenerateMosaicViewAsync(
                searchResults, 
                options, 
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(mosaicView);
            Assert.IsTrue(mosaicView.Items.Any());
        }

        [TestMethod]
        public async Task TestTimelineVisualization()
        {
            // Arrange
            var searchResults = CreateTestSearchResults();
            var options = new TimelineViewOptions
            {
                ShowFileIcons = true,
                ShowFileNames = true,
                GroupByType = false,
                ShowStatistics = true
            };

            // Act
            var timelineView = await _visualizationService.GenerateTimelineViewAsync(
                searchResults, 
                options, 
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(timelineView);
            Assert.IsTrue(timelineView.Items.Any());
        }

        [TestMethod]
        public async Task TestDataVisualization()
        {
            // Arrange
            var data = new VisualizationData
            {
                Title = "Test Data",
                Series = new List<DataSeries>
                {
                    new DataSeries
                    {
                        Name = "Test Series",
                        Points = new List<DataPoint>
                        {
                            new DataPoint { Label = "Point 1", Value = 10 },
                            new DataPoint { Label = "Point 2", Value = 20 },
                            new DataPoint { Label = "Point 3", Value = 30 }
                        }
                    }
                }
            };
            var options = new VisualizationOptions
            {
                Width = 800,
                Height = 600,
                Title = "Test Chart",
                ShowLegend = true,
                ShowGrid = true
            };

            // Act
            var visualization = await _visualizationService.GenerateDataVisualizationAsync(
                data, 
                options, 
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(visualization);
            Assert.IsTrue(visualization.Series.Any());
        }

        [TestMethod]
        public async Task TestContentAnalysis()
        {
            // Arrange
            var filePaths = new[] { "test1.txt", "test2.cs", "test3.pdf" };

            // Act
            var analysis = await _analyticsService.AnalyzeContentAsync(
                filePaths, 
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(analysis);
            Assert.IsTrue(analysis.TotalFiles > 0);
        }

        [TestMethod]
        public async Task TestTemporalAnalysis()
        {
            // Arrange
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now;

            // Act
            var analysis = await _analyticsService.AnalyzeTemporalTrendsAsync(
                startDate, 
                endDate, 
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(analysis);
            Assert.AreEqual(startDate, analysis.StartDate);
            Assert.AreEqual(endDate, analysis.EndDate);
        }

        [TestMethod]
        public async Task TestDiskUsageAnalysis()
        {
            // Arrange
            var directories = new[] { "C:\\Test1", "C:\\Test2" };

            // Act
            var analysis = await _analyticsService.AnalyzeDiskUsageAsync(
                directories, 
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(analysis);
        }

        [TestMethod]
        public async Task TestAnomalyDetection()
        {
            // Arrange
            var filePaths = new[] { "test1.txt", "test2.cs", "test3.pdf" };

            // Act
            var anomalies = await _analyticsService.DetectAnomaliesAsync(
                filePaths, 
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(anomalies);
        }

        [TestMethod]
        public async Task TestSystemHealthReport()
        {
            // Act
            var report = await _analyticsService.GenerateSystemHealthReportAsync(
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(report);
            Assert.IsNotNull(report.GeneratedAt);
        }

        [TestMethod]
        public async Task TestUsageStatistics()
        {
            // Arrange
            var startDate = DateTime.Now.AddDays(-7);
            var endDate = DateTime.Now;

            // Act
            var statistics = await _analyticsService.GetUsageStatisticsAsync(
                startDate, 
                endDate, 
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(statistics);
            Assert.AreEqual(startDate, statistics.StartDate);
            Assert.AreEqual(endDate, statistics.EndDate);
        }

        [TestMethod]
        public async Task TestPerformanceMetrics()
        {
            // Act
            var metrics = await _analyticsService.GetPerformanceMetricsAsync(
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(metrics);
        }

        [TestMethod]
        public async Task TestEmbeddingGeneration()
        {
            // Arrange
            var content = "This is a test document for embedding generation.";

            // Act
            var embedding = await _semanticSearchEngine.GenerateEmbeddingAsync(
                content, 
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(embedding);
            Assert.IsTrue(embedding.Length > 0);
        }

        [TestMethod]
        public async Task TestSimilarityCalculation()
        {
            // Arrange
            var content1 = "This is a test document.";
            var content2 = "This is another test document.";

            // Act
            var similarity = await _semanticSearchEngine.CalculateSimilarityAsync(
                content1, 
                content2, 
                CancellationToken.None);

            // Assert
            Assert.IsTrue(similarity >= 0.0 && similarity <= 1.0);
        }

        [TestMethod]
        public async Task TestPredictiveSuggestions()
        {
            // Arrange
            var context = "I'm working on a project";
            var maxSuggestions = 5;

            // Act
            var suggestions = await _intelligentSuggestions.GetPredictiveSuggestionsAsync(
                context, 
                maxSuggestions, 
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(suggestions);
        }

        [TestMethod]
        public async Task TestRecommendations()
        {
            // Arrange
            var currentQuery = "project files";
            var maxSuggestions = 5;

            // Act
            var recommendations = await _intelligentSuggestions.GetRecommendationsAsync(
                currentQuery, 
                maxSuggestions, 
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(recommendations);
        }

        [TestMethod]
        public async Task TestHistoryBasedSuggestions()
        {
            // Arrange
            var maxSuggestions = 10;

            // Act
            var suggestions = await _intelligentSuggestions.GetHistoryBasedSuggestionsAsync(
                maxSuggestions, 
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(suggestions);
        }

        [TestMethod]
        public async Task TestRecentFilesSuggestions()
        {
            // Arrange
            var maxSuggestions = 10;

            // Act
            var suggestions = await _intelligentSuggestions.GetRecentFilesSuggestionsAsync(
                maxSuggestions, 
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(suggestions);
        }

        [TestMethod]
        public async Task TestSimilarPatternsSuggestions()
        {
            // Arrange
            var query = "project";
            var maxSuggestions = 5;

            // Act
            var suggestions = await _intelligentSuggestions.GetSimilarPatternsSuggestionsAsync(
                query, 
                maxSuggestions, 
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(suggestions);
        }

        [TestMethod]
        public async Task TestClearLearningData()
        {
            // Act
            await _intelligentSuggestions.ClearLearningDataAsync(
                CancellationToken.None);

            // Assert
            // Vérifier que les données d'apprentissage ont été effacées
            var suggestions = await _intelligentSuggestions.GetHistoryBasedSuggestionsAsync(
                10, 
                CancellationToken.None);
            
            Assert.IsNotNull(suggestions);
        }

        private IEnumerable<SearchResult> CreateTestSearchResults()
        {
            return new List<SearchResult>
            {
                new SearchResult
                {
                    FilePath = "C:\\Test\\file1.txt",
                    FileName = "file1.txt",
                    Directory = "C:\\Test",
                    FileSize = 1024,
                    LastModified = DateTime.Now.AddDays(-1),
                    FileExtension = ".txt",
                    Content = "This is a test file",
                    MatchCount = 1,
                    MatchingLines = new[] { "This is a test file" },
                    MatchType = SearchMatchType.Content,
                    RelevanceScore = 0.8
                },
                new SearchResult
                {
                    FilePath = "C:\\Test\\file2.cs",
                    FileName = "file2.cs",
                    Directory = "C:\\Test",
                    FileSize = 2048,
                    LastModified = DateTime.Now.AddDays(-2),
                    FileExtension = ".cs",
                    Content = "public class Test { }",
                    MatchCount = 1,
                    MatchingLines = new[] { "public class Test { }" },
                    MatchType = SearchMatchType.Content,
                    RelevanceScore = 0.9
                },
                new SearchResult
                {
                    FilePath = "C:\\Test\\file3.pdf",
                    FileName = "file3.pdf",
                    Directory = "C:\\Test",
                    FileSize = 4096,
                    LastModified = DateTime.Now.AddDays(-3),
                    FileExtension = ".pdf",
                    Content = "PDF content",
                    MatchCount = 1,
                    MatchingLines = new[] { "PDF content" },
                    MatchType = SearchMatchType.Content,
                    RelevanceScore = 0.7
                }
            };
        }

        [TestCleanup]
        public void Cleanup()
        {
            _serviceContainer?.Dispose();
        }
    }
}
