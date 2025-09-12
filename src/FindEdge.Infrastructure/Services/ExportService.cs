using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using FindEdge.Core.Interfaces;
using FindEdge.Core.Models;

namespace FindEdge.Infrastructure.Services
{
    /// <summary>
    /// Service d'export de données dans différents formats
    /// </summary>
    public class ExportService : IExportService
    {
        public async Task<byte[]> ExportSearchResultsAsync(IEnumerable<SearchResult> results, ExportFormat format, ExportOptions options = null, CancellationToken cancellationToken = default)
        {
            options ??= new ExportOptions();
            var resultsList = results.ToList();

            return format switch
            {
                ExportFormat.Csv => await ExportToCsvAsync(resultsList, options, cancellationToken),
                ExportFormat.Json => await ExportToJsonAsync(resultsList, options, cancellationToken),
                ExportFormat.Xml => await ExportToXmlAsync(resultsList, options, cancellationToken),
                ExportFormat.Html => await ExportToHtmlAsync(resultsList, options, cancellationToken),
                _ => throw new NotSupportedException($"Format d'export non supporté: {format}")
            };
        }

        public async Task<byte[]> ExportDuplicateGroupsAsync(IEnumerable<DuplicateGroup> groups, ExportFormat format, ExportOptions options = null, CancellationToken cancellationToken = default)
        {
            options ??= new ExportOptions();
            var groupsList = groups.ToList();

            return format switch
            {
                ExportFormat.Csv => await ExportDuplicateGroupsToCsvAsync(groupsList, options, cancellationToken),
                ExportFormat.Json => await ExportDuplicateGroupsToJsonAsync(groupsList, options, cancellationToken),
                ExportFormat.Xml => await ExportDuplicateGroupsToXmlAsync(groupsList, options, cancellationToken),
                ExportFormat.Html => await ExportDuplicateGroupsToHtmlAsync(groupsList, options, cancellationToken),
                _ => throw new NotSupportedException($"Format d'export non supporté: {format}")
            };
        }

        public async Task<byte[]> ExportStatisticsAsync(SearchStatistics statistics, ExportFormat format, ExportOptions options = null, CancellationToken cancellationToken = default)
        {
            options ??= new ExportOptions();

            return format switch
            {
                ExportFormat.Json => await ExportStatisticsToJsonAsync(statistics, options, cancellationToken),
                ExportFormat.Xml => await ExportStatisticsToXmlAsync(statistics, options, cancellationToken),
                ExportFormat.Html => await ExportStatisticsToHtmlAsync(statistics, options, cancellationToken),
                _ => throw new NotSupportedException($"Format d'export non supporté: {format}")
            };
        }

        public IEnumerable<ExportFormat> GetSupportedFormats()
        {
            return Enum.GetValues<ExportFormat>();
        }

        public bool ValidateExportOptions(ExportFormat format, ExportOptions options)
        {
            if (options == null)
                return false;

            return format switch
            {
                ExportFormat.Csv => !string.IsNullOrEmpty(options.CsvSeparator),
                ExportFormat.Json => true,
                ExportFormat.Xml => true,
                ExportFormat.Html => true,
                _ => false
            };
        }

        #region Export CSV

        private async Task<byte[]> ExportToCsvAsync(List<SearchResult> results, ExportOptions options, CancellationToken cancellationToken)
        {
            var csv = new StringBuilder();
            var separator = options.CsvSeparator;

            // En-tête
            if (options.IncludeCsvHeader)
            {
                var headers = new List<string>
                {
                    "Nom du fichier",
                    "Chemin complet",
                    "Répertoire",
                    "Taille",
                    "Date de modification",
                    "Extension",
                    "Type de correspondance",
                    "Score de pertinence"
                };

                if (options.IncludeContent)
                    headers.Add("Contenu");

                csv.AppendLine(string.Join(separator, headers));
            }

            // Données
            foreach (var result in results)
            {
                var row = new List<string>
                {
                    EscapeCsvValue(result.FileName),
                    EscapeCsvValue(result.FilePath),
                    EscapeCsvValue(result.Directory),
                    FormatFileSize(result.FileSize, options.FileSizeFormat),
                    result.LastModified.ToString(options.DateFormat),
                    result.FileExtension,
                    result.MatchType.ToString(),
                    result.RelevanceScore.ToString("F2")
                };

                if (options.IncludeContent)
                {
                    var content = result.Content;
                    if (content.Length > options.MaxContentLength)
                        content = content.Substring(0, options.MaxContentLength) + "...";
                    row.Add(EscapeCsvValue(content));
                }

                csv.AppendLine(string.Join(separator, row));
            }

            return Encoding.GetEncoding(options.Encoding).GetBytes(csv.ToString());
        }

        private async Task<byte[]> ExportDuplicateGroupsToCsvAsync(List<DuplicateGroup> groups, ExportOptions options, CancellationToken cancellationToken)
        {
            var csv = new StringBuilder();
            var separator = options.CsvSeparator;

            // En-tête
            if (options.IncludeCsvHeader)
            {
                var headers = new List<string>
                {
                    "Groupe ID",
                    "Type",
                    "Confiance",
                    "Nombre de fichiers",
                    "Taille totale",
                    "Espace gaspillé",
                    "Fichier",
                    "Chemin",
                    "Taille",
                    "Date de modification",
                    "Est original"
                };

                csv.AppendLine(string.Join(separator, headers));
            }

            // Données
            foreach (var group in groups)
            {
                foreach (var file in group.Files)
                {
                    var row = new List<string>
                    {
                        group.Id,
                        group.Type.ToString(),
                        group.Confidence.ToString("F2"),
                        group.Files.Count.ToString(),
                        FormatFileSize(group.TotalSize, options.FileSizeFormat),
                        FormatFileSize(group.SpaceWasted, options.FileSizeFormat),
                        EscapeCsvValue(file.FileName),
                        EscapeCsvValue(file.FilePath),
                        FormatFileSize(file.FileSize, options.FileSizeFormat),
                        file.LastModified.ToString(options.DateFormat),
                        file.IsOriginal.ToString()
                    };

                    csv.AppendLine(string.Join(separator, row));
                }
            }

            return Encoding.GetEncoding(options.Encoding).GetBytes(csv.ToString());
        }

        #endregion

        #region Export JSON

        private async Task<byte[]> ExportToJsonAsync(List<SearchResult> results, ExportOptions options, CancellationToken cancellationToken)
        {
            var exportData = new
            {
                ExportDate = DateTime.Now,
                Format = "SearchResults",
                Options = options,
                Count = results.Count,
                Results = results.Select(r => new
                {
                    r.FileName,
                    r.FilePath,
                    r.Directory,
                    FileSize = FormatFileSize(r.FileSize, options.FileSizeFormat),
                    r.LastModified,
                    r.FileExtension,
                    r.MatchType,
                    r.RelevanceScore,
                    Content = options.IncludeContent ? TruncateContent(r.Content, options.MaxContentLength) : null
                })
            };

            var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            return Encoding.GetEncoding(options.Encoding).GetBytes(json);
        }

        private async Task<byte[]> ExportDuplicateGroupsToJsonAsync(List<DuplicateGroup> groups, ExportOptions options, CancellationToken cancellationToken)
        {
            var exportData = new
            {
                ExportDate = DateTime.Now,
                Format = "DuplicateGroups",
                Options = options,
                Count = groups.Count,
                Groups = groups.Select(g => new
                {
                    g.Id,
                    g.Type,
                    g.Confidence,
                    FileCount = g.Files.Count,
                    TotalSize = FormatFileSize(g.TotalSize, options.FileSizeFormat),
                    SpaceWasted = FormatFileSize(g.SpaceWasted, options.FileSizeFormat),
                    g.DetectedAt,
                    Files = g.Files.Select(f => new
                    {
                        f.FileName,
                        f.FilePath,
                        f.Directory,
                        FileSize = FormatFileSize(f.FileSize, options.FileSizeFormat),
                        f.LastModified,
                        f.IsOriginal,
                        f.SimilarityScore
                    })
                })
            };

            var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            return Encoding.GetEncoding(options.Encoding).GetBytes(json);
        }

        private async Task<byte[]> ExportStatisticsToJsonAsync(SearchStatistics statistics, ExportOptions options, CancellationToken cancellationToken)
        {
            var exportData = new
            {
                ExportDate = DateTime.Now,
                Format = "SearchStatistics",
                Options = options,
                Statistics = new
                {
                    statistics.SearchDate,
                    statistics.SearchTerm,
                    statistics.TotalFilesFound,
                    statistics.FilesWithContentMatch,
                    statistics.FilesWithNameMatch,
                    statistics.FilesWithBothMatch,
                    TotalSize = FormatFileSize(statistics.TotalSize, options.FileSizeFormat),
                    SearchDuration = statistics.SearchDuration.ToString(),
                    statistics.DirectoriesSearched,
                    statistics.DuplicateGroupsFound,
                    SpaceWasted = FormatFileSize(statistics.SpaceWasted, options.FileSizeFormat),
                    statistics.FileTypeDistribution,
                    statistics.DirectoryDistribution,
                    statistics.SearchOptions
                }
            };

            var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            return Encoding.GetEncoding(options.Encoding).GetBytes(json);
        }

        #endregion

        #region Export XML

        private async Task<byte[]> ExportToXmlAsync(List<SearchResult> results, ExportOptions options, CancellationToken cancellationToken)
        {
            var root = new XElement("SearchResults",
                new XAttribute("ExportDate", DateTime.Now),
                new XAttribute("Count", results.Count),
                new XElement("Options",
                    new XElement("IncludeMetadata", options.IncludeMetadata),
                    new XElement("IncludeContent", options.IncludeContent),
                    new XElement("MaxContentLength", options.MaxContentLength)
                ),
                new XElement("Results",
                    results.Select(r => new XElement("Result",
                        new XElement("FileName", r.FileName),
                        new XElement("FilePath", r.FilePath),
                        new XElement("Directory", r.Directory),
                        new XElement("FileSize", FormatFileSize(r.FileSize, options.FileSizeFormat)),
                        new XElement("LastModified", r.LastModified.ToString(options.DateFormat)),
                        new XElement("FileExtension", r.FileExtension),
                        new XElement("MatchType", r.MatchType.ToString()),
                        new XElement("RelevanceScore", r.RelevanceScore),
                        options.IncludeContent ? new XElement("Content", TruncateContent(r.Content, options.MaxContentLength)) : null
                    ))
                )
            );

            var xml = root.ToString();
            return Encoding.GetEncoding(options.Encoding).GetBytes(xml);
        }

        private async Task<byte[]> ExportDuplicateGroupsToXmlAsync(List<DuplicateGroup> groups, ExportOptions options, CancellationToken cancellationToken)
        {
            var root = new XElement("DuplicateGroups",
                new XAttribute("ExportDate", DateTime.Now),
                new XAttribute("Count", groups.Count),
                new XElement("Groups",
                    groups.Select(g => new XElement("Group",
                        new XAttribute("Id", g.Id),
                        new XAttribute("Type", g.Type.ToString()),
                        new XAttribute("Confidence", g.Confidence),
                        new XElement("FileCount", g.Files.Count),
                        new XElement("TotalSize", FormatFileSize(g.TotalSize, options.FileSizeFormat)),
                        new XElement("SpaceWasted", FormatFileSize(g.SpaceWasted, options.FileSizeFormat)),
                        new XElement("DetectedAt", g.DetectedAt.ToString(options.DateFormat)),
                        new XElement("Files",
                            g.Files.Select(f => new XElement("File",
                                new XElement("FileName", f.FileName),
                                new XElement("FilePath", f.FilePath),
                                new XElement("Directory", f.Directory),
                                new XElement("FileSize", FormatFileSize(f.FileSize, options.FileSizeFormat)),
                                new XElement("LastModified", f.LastModified.ToString(options.DateFormat)),
                                new XElement("IsOriginal", f.IsOriginal),
                                new XElement("SimilarityScore", f.SimilarityScore)
                            ))
                        )
                    ))
                )
            );

            var xml = root.ToString();
            return Encoding.GetEncoding(options.Encoding).GetBytes(xml);
        }

        private async Task<byte[]> ExportStatisticsToXmlAsync(SearchStatistics statistics, ExportOptions options, CancellationToken cancellationToken)
        {
            var root = new XElement("SearchStatistics",
                new XAttribute("ExportDate", DateTime.Now),
                new XElement("SearchDate", statistics.SearchDate.ToString(options.DateFormat)),
                new XElement("SearchTerm", statistics.SearchTerm),
                new XElement("TotalFilesFound", statistics.TotalFilesFound),
                new XElement("FilesWithContentMatch", statistics.FilesWithContentMatch),
                new XElement("FilesWithNameMatch", statistics.FilesWithNameMatch),
                new XElement("FilesWithBothMatch", statistics.FilesWithBothMatch),
                new XElement("TotalSize", FormatFileSize(statistics.TotalSize, options.FileSizeFormat)),
                new XElement("SearchDuration", statistics.SearchDuration.ToString()),
                new XElement("DirectoriesSearched", statistics.DirectoriesSearched),
                new XElement("DuplicateGroupsFound", statistics.DuplicateGroupsFound),
                new XElement("SpaceWasted", FormatFileSize(statistics.SpaceWasted, options.FileSizeFormat)),
                new XElement("FileTypeDistribution",
                    statistics.FileTypeDistribution.Select(kvp => new XElement("FileType",
                        new XAttribute("Extension", kvp.Key),
                        new XAttribute("Count", kvp.Value)
                    ))
                ),
                new XElement("DirectoryDistribution",
                    statistics.DirectoryDistribution.Select(kvp => new XElement("Directory",
                        new XAttribute("Path", kvp.Key),
                        new XAttribute("Count", kvp.Value)
                    ))
                ),
                new XElement("SearchOptions",
                    statistics.SearchOptions.Select(option => new XElement("Option", option))
                )
            );

            var xml = root.ToString();
            return Encoding.GetEncoding(options.Encoding).GetBytes(xml);
        }

        #endregion

        #region Export HTML

        private async Task<byte[]> ExportToHtmlAsync(List<SearchResult> results, ExportOptions options, CancellationToken cancellationToken)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html><head>");
            html.AppendLine("<title>Résultats de recherche - FindEdge</title>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
            html.AppendLine("table { border-collapse: collapse; width: 100%; }");
            html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            html.AppendLine("th { background-color: #f2f2f2; }");
            html.AppendLine("tr:nth-child(even) { background-color: #f9f9f9; }");
            html.AppendLine("</style>");
            html.AppendLine("</head><body>");
            html.AppendLine("<h1>Résultats de recherche - FindEdge</h1>");
            html.AppendLine($"<p>Date d'export: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
            html.AppendLine($"<p>Nombre de résultats: {results.Count}</p>");
            html.AppendLine("<table>");
            html.AppendLine("<tr><th>Nom du fichier</th><th>Chemin</th><th>Taille</th><th>Date</th><th>Type</th><th>Score</th></tr>");

            foreach (var result in results)
            {
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{EscapeHtml(result.FileName)}</td>");
                html.AppendLine($"<td>{EscapeHtml(result.FilePath)}</td>");
                html.AppendLine($"<td>{FormatFileSize(result.FileSize, options.FileSizeFormat)}</td>");
                html.AppendLine($"<td>{result.LastModified:yyyy-MM-dd HH:mm:ss}</td>");
                html.AppendLine($"<td>{result.MatchType}</td>");
                html.AppendLine($"<td>{result.RelevanceScore:F2}</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("</table>");
            html.AppendLine("</body></html>");

            return Encoding.GetEncoding(options.Encoding).GetBytes(html.ToString());
        }

        private async Task<byte[]> ExportDuplicateGroupsToHtmlAsync(List<DuplicateGroup> groups, ExportOptions options, CancellationToken cancellationToken)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html><head>");
            html.AppendLine("<title>Groupes de doublons - FindEdge</title>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
            html.AppendLine(".group { border: 1px solid #ccc; margin: 10px 0; padding: 10px; }");
            html.AppendLine(".group-header { background-color: #f0f0f0; padding: 5px; font-weight: bold; }");
            html.AppendLine("table { border-collapse: collapse; width: 100%; }");
            html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            html.AppendLine("th { background-color: #f2f2f2; }");
            html.AppendLine("</style>");
            html.AppendLine("</head><body>");
            html.AppendLine("<h1>Groupes de doublons - FindEdge</h1>");
            html.AppendLine($"<p>Date d'export: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
            html.AppendLine($"<p>Nombre de groupes: {groups.Count}</p>");

            foreach (var group in groups)
            {
                html.AppendLine("<div class='group'>");
                html.AppendLine($"<div class='group-header'>Groupe {group.Id} - {group.Type} (Confiance: {group.Confidence:P})</div>");
                html.AppendLine($"<p>Fichiers: {group.Files.Count} | Taille totale: {FormatFileSize(group.TotalSize, options.FileSizeFormat)} | Espace gaspillé: {FormatFileSize(group.SpaceWasted, options.FileSizeFormat)}</p>");
                html.AppendLine("<table>");
                html.AppendLine("<tr><th>Fichier</th><th>Chemin</th><th>Taille</th><th>Date</th><th>Original</th></tr>");

                foreach (var file in group.Files)
                {
                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>{EscapeHtml(file.FileName)}</td>");
                    html.AppendLine($"<td>{EscapeHtml(file.FilePath)}</td>");
                    html.AppendLine($"<td>{FormatFileSize(file.FileSize, options.FileSizeFormat)}</td>");
                    html.AppendLine($"<td>{file.LastModified:yyyy-MM-dd HH:mm:ss}</td>");
                    html.AppendLine($"<td>{(file.IsOriginal ? "Oui" : "Non")}</td>");
                    html.AppendLine("</tr>");
                }

                html.AppendLine("</table>");
                html.AppendLine("</div>");
            }

            html.AppendLine("</body></html>");

            return Encoding.GetEncoding(options.Encoding).GetBytes(html.ToString());
        }

        private async Task<byte[]> ExportStatisticsToHtmlAsync(SearchStatistics statistics, ExportOptions options, CancellationToken cancellationToken)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html><head>");
            html.AppendLine("<title>Statistiques de recherche - FindEdge</title>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
            html.AppendLine(".stat { margin: 10px 0; padding: 10px; background-color: #f9f9f9; }");
            html.AppendLine("</style>");
            html.AppendLine("</head><body>");
            html.AppendLine("<h1>Statistiques de recherche - FindEdge</h1>");
            html.AppendLine($"<p>Date d'export: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
            html.AppendLine($"<div class='stat'><strong>Terme de recherche:</strong> {EscapeHtml(statistics.SearchTerm)}</div>");
            html.AppendLine($"<div class='stat'><strong>Date de recherche:</strong> {statistics.SearchDate:yyyy-MM-dd HH:mm:ss}</div>");
            html.AppendLine($"<div class='stat'><strong>Durée de recherche:</strong> {statistics.SearchDuration}</div>");
            html.AppendLine($"<div class='stat'><strong>Fichiers trouvés:</strong> {statistics.TotalFilesFound}</div>");
            html.AppendLine($"<div class='stat'><strong>Correspondance nom:</strong> {statistics.FilesWithNameMatch}</div>");
            html.AppendLine($"<div class='stat'><strong>Correspondance contenu:</strong> {statistics.FilesWithContentMatch}</div>");
            html.AppendLine($"<div class='stat'><strong>Correspondance les deux:</strong> {statistics.FilesWithBothMatch}</div>");
            html.AppendLine($"<div class='stat'><strong>Taille totale:</strong> {FormatFileSize(statistics.TotalSize, options.FileSizeFormat)}</div>");
            html.AppendLine($"<div class='stat'><strong>Répertoires explorés:</strong> {statistics.DirectoriesSearched}</div>");
            html.AppendLine($"<div class='stat'><strong>Groupes de doublons:</strong> {statistics.DuplicateGroupsFound}</div>");
            html.AppendLine($"<div class='stat'><strong>Espace gaspillé:</strong> {FormatFileSize(statistics.SpaceWasted, options.FileSizeFormat)}</div>");
            html.AppendLine("</body></html>");

            return Encoding.GetEncoding(options.Encoding).GetBytes(html.ToString());
        }

        #endregion

        #region Méthodes utilitaires

        private string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }

        private string EscapeHtml(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;");
        }

        private string FormatFileSize(long bytes, FileSizeFormat format)
        {
            return format switch
            {
                FileSizeFormat.Bytes => bytes.ToString("N0"),
                FileSizeFormat.Kilobytes => (bytes / 1024.0).ToString("N2") + " KB",
                FileSizeFormat.Megabytes => (bytes / (1024.0 * 1024.0)).ToString("N2") + " MB",
                FileSizeFormat.Formatted => FormatBytes(bytes),
                _ => bytes.ToString("N0")
            };
        }

        private string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return $"{number:N1} {suffixes[counter]}";
        }

        private string TruncateContent(string content, int maxLength)
        {
            if (string.IsNullOrEmpty(content))
                return string.Empty;

            if (content.Length <= maxLength)
                return content;

            return content.Substring(0, maxLength) + "...";
        }

        #endregion
    }
}
