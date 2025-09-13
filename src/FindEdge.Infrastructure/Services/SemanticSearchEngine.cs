using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;
using FindEdge.Core.Models;

namespace FindEdge.Infrastructure.Services
{
    /// <summary>
    /// Moteur de recherche sémantique basé sur l'IA
    /// </summary>
    public class SemanticSearchEngine : ISemanticSearchEngine
    {
        private readonly ISearchEngine _searchEngine;
        private readonly IContentParser _contentParser;
        private readonly Dictionary<string, float[]> _embeddingsCache;
        private readonly Dictionary<string, FileClassification> _classificationCache;

        public SemanticSearchEngine(ISearchEngine searchEngine, IContentParser contentParser)
        {
            _searchEngine = searchEngine ?? throw new ArgumentNullException(nameof(searchEngine));
            _contentParser = contentParser ?? throw new ArgumentNullException(nameof(contentParser));
            _embeddingsCache = new Dictionary<string, float[]>();
            _classificationCache = new Dictionary<string, FileClassification>();
        }

        public event EventHandler<SemanticSearchProgressEventArgs>? SemanticSearchProgress;

        public async Task<IEnumerable<SearchResult>> SearchBySimilarityAsync(string query, SearchOptions options, CancellationToken cancellationToken = default)
        {
            var startTime = DateTime.UtcNow;
            var results = new List<SearchResult>();

            try
            {
                // Générer l'embedding de la requête
                var queryEmbedding = await GenerateEmbeddingAsync(query, cancellationToken);

                // Rechercher dans les fichiers indexés
                var searchResults = await _searchEngine.SearchAsync(options, cancellationToken);
                var similarityResults = new List<(SearchResult Result, double Similarity)>();

                foreach (var result in searchResults)
                {
                    // Générer l'embedding du contenu du fichier
                    var contentEmbedding = await GenerateEmbeddingAsync(result.Content, cancellationToken);
                    
                    // Calculer la similarité cosinus
                    var similarity = CalculateCosineSimilarity(queryEmbedding, contentEmbedding);
                    
                    if (similarity > 0.3) // Seuil de similarité
                    {
                        similarityResults.Add((result, similarity));
                    }
                }

                // Trier par similarité
                results = similarityResults
                    .OrderByDescending(x => x.Similarity)
                    .Select(x => 
                    {
                        x.Result.RelevanceScore = x.Similarity;
                        return x.Result;
                    })
                    .ToList();

                // Déclencher l'événement de progression
                SemanticSearchProgress?.Invoke(this, new SemanticSearchProgressEventArgs
                {
                    FilesProcessed = searchResults.Count(),
                    TotalFiles = searchResults.Count(),
                    CurrentFile = "Recherche sémantique terminée",
                    ElapsedTime = DateTime.UtcNow - startTime,
                    CurrentOperation = "Recherche par similarité"
                });
            }
            catch (Exception ex)
            {
                // En cas d'erreur, fallback vers la recherche classique
                results = (await _searchEngine.SearchAsync(options, cancellationToken)).ToList();
            }

            return results;
        }

        public async Task<IEnumerable<SearchResult>> SearchByNaturalLanguageAsync(string naturalLanguageQuery, SearchOptions options, CancellationToken cancellationToken = default)
        {
            var startTime = DateTime.UtcNow;
            var results = new List<SearchResult>();

            try
            {
                // Traduire la requête en langage naturel en termes de recherche
                var searchTerms = await TranslateNaturalLanguageQueryAsync(naturalLanguageQuery, cancellationToken);
                
                // Construire les options de recherche basées sur les termes extraits
                var semanticOptions = BuildSearchOptionsFromTerms(searchTerms, options);
                
                // Effectuer la recherche sémantique
                results = (await SearchBySimilarityAsync(naturalLanguageQuery, semanticOptions, cancellationToken)).ToList();

                // Déclencher l'événement de progression
                SemanticSearchProgress?.Invoke(this, new SemanticSearchProgressEventArgs
                {
                    FilesProcessed = results.Count,
                    TotalFiles = results.Count,
                    CurrentFile = "Recherche en langage naturel terminée",
                    ElapsedTime = DateTime.UtcNow - startTime,
                    CurrentOperation = "Recherche en langage naturel"
                });
            }
            catch (Exception ex)
            {
                // En cas d'erreur, fallback vers la recherche classique
                results = (await _searchEngine.SearchAsync(options, cancellationToken)).ToList();
            }

            return results;
        }

        public async Task<IEnumerable<FileClassification>> ClassifyFilesAsync(IEnumerable<string> filePaths, CancellationToken cancellationToken = default)
        {
            var classifications = new List<FileClassification>();

            foreach (var filePath in filePaths)
            {
                try
                {
                    // Vérifier le cache
                    if (_classificationCache.ContainsKey(filePath))
                    {
                        classifications.Add(_classificationCache[filePath]);
                        continue;
                    }

                    // Lire le contenu du fichier
                    var content = await _contentParser.ExtractContentAsync(filePath, cancellationToken);
                    
                    // Classifier le contenu
                    var classification = await ClassifyContentAsync(content, filePath, cancellationToken);
                    
                    // Mettre en cache
                    _classificationCache[filePath] = classification;
                    classifications.Add(classification);
                }
                catch (Exception ex)
                {
                    // Classification par défaut en cas d'erreur
                    classifications.Add(new FileClassification
                    {
                        FilePath = filePath,
                        Category = "Unknown",
                        Confidence = 0.0,
                        Tags = new List<string> { "Error" }
                    });
                }
            }

            return classifications;
        }

        public async Task<LanguageDetectionResult> DetectLanguageAsync(string filePath, CancellationToken cancellationToken = default)
        {
            try
            {
                // Lire le contenu du fichier
                var content = await _contentParser.ExtractContentAsync(filePath, cancellationToken);
                
                // Détecter la langue
                var language = DetectLanguageFromContent(content);
                
                return new LanguageDetectionResult
                {
                    DetectedLanguage = language,
                    Confidence = CalculateLanguageConfidence(content, language),
                    LanguageScores = CalculateLanguageScores(content)
                };
            }
            catch (Exception ex)
            {
                return new LanguageDetectionResult
                {
                    DetectedLanguage = "Unknown",
                    Confidence = 0.0,
                    LanguageScores = new Dictionary<string, double>()
                };
            }
        }

        public async Task<float[]> GenerateEmbeddingAsync(string content, CancellationToken cancellationToken = default)
        {
            // Vérifier le cache
            var contentHash = content.GetHashCode().ToString();
            if (_embeddingsCache.ContainsKey(contentHash))
            {
                return _embeddingsCache[contentHash];
            }

            // Générer l'embedding (implémentation simplifiée)
            var embedding = GenerateSimpleEmbedding(content);
            
            // Mettre en cache
            _embeddingsCache[contentHash] = embedding;
            
            return embedding;
        }

        public async Task<double> CalculateSimilarityAsync(string content1, string content2, CancellationToken cancellationToken = default)
        {
            var embedding1 = await GenerateEmbeddingAsync(content1, cancellationToken);
            var embedding2 = await GenerateEmbeddingAsync(content2, cancellationToken);
            
            return CalculateCosineSimilarity(embedding1, embedding2);
        }

        private async Task<Dictionary<string, object>> TranslateNaturalLanguageQueryAsync(string query, CancellationToken cancellationToken)
        {
            // Implémentation simplifiée de traduction de requête en langage naturel
            var terms = new Dictionary<string, object>();
            
            // Extraire les mots-clés
            var keywords = ExtractKeywords(query);
            terms["keywords"] = keywords;
            
            // Extraire les types de fichiers
            var fileTypes = ExtractFileTypes(query);
            terms["fileTypes"] = fileTypes;
            
            // Extraire les dates
            var dates = ExtractDates(query);
            terms["dates"] = dates;
            
            // Extraire les tailles
            var sizes = ExtractSizes(query);
            terms["sizes"] = sizes;
            
            return terms;
        }

        private SearchOptions BuildSearchOptionsFromTerms(Dictionary<string, object> terms, SearchOptions originalOptions)
        {
            var options = new SearchOptions
            {
                SearchTerm = string.Join(" ", (List<string>)terms.GetValueOrDefault("keywords", new List<string>())),
                SearchInFileName = originalOptions.SearchInFileName,
                SearchInContent = originalOptions.SearchInContent,
                UseRegex = originalOptions.UseRegex,
                CaseSensitive = originalOptions.CaseSensitive,
                WholeWord = originalOptions.WholeWord,
                IncludeExtensions = (List<string>)terms.GetValueOrDefault("fileTypes", new List<string>()),
                IncludeDirectories = originalOptions.IncludeDirectories,
                ExcludeDirectories = originalOptions.ExcludeDirectories,
                MinFileSize = originalOptions.MinFileSize,
                MaxFileSize = originalOptions.MaxFileSize,
                ModifiedAfter = originalOptions.ModifiedAfter,
                ModifiedBefore = originalOptions.ModifiedBefore,
                MaxResults = originalOptions.MaxResults,
                MaxContentLength = originalOptions.MaxContentLength,
                IncludeHiddenFiles = originalOptions.IncludeHiddenFiles,
                IncludeSystemFiles = originalOptions.IncludeSystemFiles
            };

            return options;
        }

        private async Task<FileClassification> ClassifyContentAsync(string content, string filePath, CancellationToken cancellationToken)
        {
            var classification = new FileClassification
            {
                FilePath = filePath,
                Category = "Unknown",
                Confidence = 0.0,
                CategoryScores = new Dictionary<string, double>(),
                Tags = new List<string>()
            };

            // Classification basée sur le contenu
            var category = ClassifyContentByKeywords(content);
            classification.Category = category;
            classification.Confidence = CalculateClassificationConfidence(content, category);
            
            // Ajouter des tags
            classification.Tags = ExtractTags(content);
            
            // Calculer les scores pour différentes catégories
            classification.CategoryScores = CalculateCategoryScores(content);

            return classification;
        }

        private string ClassifyContentByKeywords(string content)
        {
            var contentLower = content.ToLowerInvariant();
            
            // Catégories basées sur des mots-clés
            var categories = new Dictionary<string, List<string>>
            {
                ["Code"] = new List<string> { "function", "class", "import", "export", "var", "let", "const", "if", "else", "for", "while" },
                ["Document"] = new List<string> { "document", "report", "memo", "letter", "proposal", "contract" },
                ["Email"] = new List<string> { "dear", "sincerely", "regards", "subject", "from", "to", "cc", "bcc" },
                ["Configuration"] = new List<string> { "config", "setting", "option", "parameter", "property", "value" },
                ["Log"] = new List<string> { "error", "warning", "info", "debug", "trace", "exception", "stack" },
                ["Data"] = new List<string> { "data", "record", "table", "row", "column", "field", "value" }
            };

            var categoryScores = new Dictionary<string, int>();
            
            foreach (var category in categories)
            {
                var score = category.Value.Count(keyword => contentLower.Contains(keyword));
                categoryScores[category.Key] = score;
            }

            var bestCategory = categoryScores.OrderByDescending(x => x.Value).FirstOrDefault();
            return bestCategory.Value > 0 ? bestCategory.Key : "Unknown";
        }

        private double CalculateClassificationConfidence(string content, string category)
        {
            // Calculer la confiance basée sur la fréquence des mots-clés
            var contentLower = content.ToLowerInvariant();
            var keywords = GetCategoryKeywords(category);
            var matches = keywords.Count(keyword => contentLower.Contains(keyword));
            
            return Math.Min(1.0, matches / (double)Math.Max(1, keywords.Count));
        }

        private List<string> GetCategoryKeywords(string category)
        {
            var keywords = new Dictionary<string, List<string>>
            {
                ["Code"] = new List<string> { "function", "class", "import", "export", "var", "let", "const" },
                ["Document"] = new List<string> { "document", "report", "memo", "letter", "proposal" },
                ["Email"] = new List<string> { "dear", "sincerely", "regards", "subject", "from", "to" },
                ["Configuration"] = new List<string> { "config", "setting", "option", "parameter", "property" },
                ["Log"] = new List<string> { "error", "warning", "info", "debug", "trace", "exception" },
                ["Data"] = new List<string> { "data", "record", "table", "row", "column", "field" }
            };

            return keywords.GetValueOrDefault(category, new List<string>());
        }

        private List<string> ExtractTags(string content)
        {
            var tags = new List<string>();
            var contentLower = content.ToLowerInvariant();
            
            // Tags basés sur des patterns
            if (contentLower.Contains("todo") || contentLower.Contains("fixme"))
                tags.Add("TODO");
            
            if (contentLower.Contains("bug") || contentLower.Contains("issue"))
                tags.Add("Bug");
            
            if (contentLower.Contains("feature") || contentLower.Contains("enhancement"))
                tags.Add("Feature");
            
            if (contentLower.Contains("urgent") || contentLower.Contains("critical"))
                tags.Add("Urgent");
            
            return tags;
        }

        private Dictionary<string, double> CalculateCategoryScores(string content)
        {
            var scores = new Dictionary<string, double>();
            var contentLower = content.ToLowerInvariant();
            
            var categories = new[] { "Code", "Document", "Email", "Configuration", "Log", "Data" };
            
            foreach (var category in categories)
            {
                var keywords = GetCategoryKeywords(category);
                var matches = keywords.Count(keyword => contentLower.Contains(keyword));
                scores[category] = Math.Min(1.0, matches / (double)Math.Max(1, keywords.Count));
            }
            
            return scores;
        }

        private string DetectLanguageFromContent(string content)
        {
            // Détection de langue simplifiée basée sur des patterns
            var contentLower = content.ToLowerInvariant();
            
            // Patterns pour différentes langues
            var languagePatterns = new Dictionary<string, List<string>>
            {
                ["French"] = new List<string> { "le", "la", "les", "de", "du", "des", "et", "ou", "avec", "pour", "dans", "sur" },
                ["English"] = new List<string> { "the", "and", "or", "with", "for", "in", "on", "at", "to", "of", "a", "an" },
                ["Spanish"] = new List<string> { "el", "la", "los", "las", "de", "del", "y", "o", "con", "para", "en", "sobre" },
                ["German"] = new List<string> { "der", "die", "das", "und", "oder", "mit", "für", "in", "auf", "zu", "von", "ein" }
            };

            var languageScores = new Dictionary<string, int>();
            
            foreach (var language in languagePatterns)
            {
                var score = language.Value.Count(pattern => contentLower.Contains(pattern));
                languageScores[language.Key] = score;
            }

            var bestLanguage = languageScores.OrderByDescending(x => x.Value).FirstOrDefault();
            return bestLanguage.Value > 0 ? bestLanguage.Key : "Unknown";
        }

        private double CalculateLanguageConfidence(string content, string language)
        {
            var contentLower = content.ToLowerInvariant();
            var patterns = GetLanguagePatterns(language);
            var matches = patterns.Count(pattern => contentLower.Contains(pattern));
            
            return Math.Min(1.0, matches / (double)Math.Max(1, patterns.Count));
        }

        private List<string> GetLanguagePatterns(string language)
        {
            var patterns = new Dictionary<string, List<string>>
            {
                ["French"] = new List<string> { "le", "la", "les", "de", "du", "des", "et", "ou", "avec" },
                ["English"] = new List<string> { "the", "and", "or", "with", "for", "in", "on", "at", "to" },
                ["Spanish"] = new List<string> { "el", "la", "los", "las", "de", "del", "y", "o", "con" },
                ["German"] = new List<string> { "der", "die", "das", "und", "oder", "mit", "für", "in", "auf" }
            };

            return patterns.GetValueOrDefault(language, new List<string>());
        }

        private Dictionary<string, double> CalculateLanguageScores(string content)
        {
            var scores = new Dictionary<string, double>();
            var contentLower = content.ToLowerInvariant();
            
            var languages = new[] { "French", "English", "Spanish", "German" };
            
            foreach (var language in languages)
            {
                var patterns = GetLanguagePatterns(language);
                var matches = patterns.Count(pattern => contentLower.Contains(pattern));
                scores[language] = Math.Min(1.0, matches / (double)Math.Max(1, patterns.Count));
            }
            
            return scores;
        }

        private float[] GenerateSimpleEmbedding(string content)
        {
            // Implémentation simplifiée d'embedding basée sur des caractéristiques de texte
            var words = content.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var embedding = new float[100]; // Embedding de dimension 100
            
            // Caractéristiques basiques
            embedding[0] = words.Length; // Longueur du texte
            embedding[1] = content.Length; // Nombre de caractères
            embedding[2] = words.Count(w => w.Length > 5); // Mots longs
            embedding[3] = words.Count(w => char.IsUpper(w[0])); // Mots commençant par une majuscule
            embedding[4] = content.Count(c => char.IsDigit(c)); // Chiffres
            
            // Distribution des longueurs de mots
            for (int i = 0; i < Math.Min(20, words.Length); i++)
            {
                embedding[5 + i] = words[i].Length;
            }
            
            // Caractéristiques de fréquence
            var wordFreq = words.GroupBy(w => w.ToLowerInvariant())
                .ToDictionary(g => g.Key, g => g.Count());
            
            for (int i = 0; i < Math.Min(75, wordFreq.Count); i++)
            {
                var word = wordFreq.Keys.ElementAt(i);
                embedding[25 + i] = wordFreq[word];
            }
            
            return embedding;
        }

        private double CalculateCosineSimilarity(float[] embedding1, float[] embedding2)
        {
            if (embedding1.Length != embedding2.Length)
                return 0.0;

            double dotProduct = 0.0;
            double norm1 = 0.0;
            double norm2 = 0.0;

            for (int i = 0; i < embedding1.Length; i++)
            {
                dotProduct += embedding1[i] * embedding2[i];
                norm1 += embedding1[i] * embedding1[i];
                norm2 += embedding2[i] * embedding2[i];
            }

            if (norm1 == 0.0 || norm2 == 0.0)
                return 0.0;

            return dotProduct / (Math.Sqrt(norm1) * Math.Sqrt(norm2));
        }

        private List<string> ExtractKeywords(string query)
        {
            // Extraction de mots-clés simples
            return query.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(word => word.Length > 2)
                .ToList();
        }

        private List<string> ExtractFileTypes(string query)
        {
            var fileTypes = new List<string>();
            var queryLower = query.ToLowerInvariant();
            
            // Patterns pour les types de fichiers
            var patterns = new Dictionary<string, string>
            {
                ["pdf"] = "pdf",
                ["document"] = "doc",
                ["spreadsheet"] = "xls",
                ["presentation"] = "ppt",
                ["image"] = "jpg",
                ["video"] = "mp4",
                ["audio"] = "mp3",
                ["code"] = "cs",
                ["text"] = "txt"
            };

            foreach (var pattern in patterns)
            {
                if (queryLower.Contains(pattern.Key))
                {
                    fileTypes.Add(pattern.Value);
                }
            }

            return fileTypes;
        }

        private List<DateTime> ExtractDates(string query)
        {
            var dates = new List<DateTime>();
            // Implémentation simplifiée d'extraction de dates
            // Dans une implémentation réelle, on utiliserait une bibliothèque de parsing de dates
            return dates;
        }

        private List<long> ExtractSizes(string query)
        {
            var sizes = new List<long>();
            // Implémentation simplifiée d'extraction de tailles
            // Dans une implémentation réelle, on parserait les expressions comme "1MB", "500KB", etc.
            return sizes;
        }
    }
}
