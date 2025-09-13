using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;
using FindEdge.Core.Models;

namespace FindEdge.Presentation
{
    /// <summary>
    /// Mock du service d'export pour les tests
    /// </summary>
    public class MockExportService : IExportService
    {
        public async Task<byte[]> ExportSearchResultsAsync(IEnumerable<SearchResult> results, ExportFormat format, ExportOptions options, CancellationToken cancellationToken = default)
        {
            await Task.Delay(500, cancellationToken);

            var resultsList = results.ToList();
            var content = format switch
            {
                ExportFormat.Csv => GenerateCsvContent(resultsList),
                ExportFormat.Json => GenerateJsonContent(resultsList),
                ExportFormat.Xml => GenerateXmlContent(resultsList),
                ExportFormat.Html => GenerateHtmlContent(resultsList),
                _ => "Format non supporté"
            };

            return Encoding.UTF8.GetBytes(content);
        }

        public async Task<byte[]> ExportDuplicateGroupsAsync(IEnumerable<DuplicateGroup> groups, ExportFormat format, ExportOptions options, CancellationToken cancellationToken = default)
        {
            await Task.Delay(500, cancellationToken);

            var groupsList = groups.ToList();
            var content = format switch
            {
                ExportFormat.Csv => GenerateDuplicateGroupsCsvContent(groupsList),
                ExportFormat.Json => GenerateDuplicateGroupsJsonContent(groupsList),
                ExportFormat.Xml => GenerateDuplicateGroupsXmlContent(groupsList),
                ExportFormat.Html => GenerateDuplicateGroupsHtmlContent(groupsList),
                _ => "Format non supporté"
            };

            return Encoding.UTF8.GetBytes(content);
        }

        public async Task<byte[]> ExportStatisticsAsync(SearchStatistics statistics, ExportFormat format, ExportOptions options, CancellationToken cancellationToken = default)
        {
            await Task.Delay(500, cancellationToken);

            var content = format switch
            {
                ExportFormat.Json => GenerateStatisticsJsonContent(statistics),
                ExportFormat.Xml => GenerateStatisticsXmlContent(statistics),
                ExportFormat.Html => GenerateStatisticsHtmlContent(statistics),
                _ => "Format non supporté"
            };

            return Encoding.UTF8.GetBytes(content);
        }

        public IEnumerable<ExportFormat> GetSupportedFormats()
        {
            return Enum.GetValues<ExportFormat>();
        }

        public bool ValidateExportOptions(ExportFormat format, ExportOptions options)
        {
            return options != null;
        }

        #region Méthodes de génération de contenu

        private string GenerateCsvContent(List<SearchResult> results)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Nom du fichier,Chemin complet,Répertoire,Taille,Date de modification,Extension,Type de correspondance,Score de pertinence");

            foreach (var result in results)
            {
                csv.AppendLine($"{result.FileName},{result.FilePath},{result.Directory},{result.FileSize},{result.LastModified:yyyy-MM-dd HH:mm:ss},{result.FileExtension},{result.MatchType},{result.RelevanceScore:F2}");
            }

            return csv.ToString();
        }

        private string GenerateJsonContent(List<SearchResult> results)
        {
            var json = new StringBuilder();
            json.AppendLine("{");
            json.AppendLine("  \"ExportDate\": \"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\",");
            json.AppendLine("  \"Format\": \"SearchResults\",");
            json.AppendLine("  \"Count\": " + results.Count + ",");
            json.AppendLine("  \"Results\": [");

            for (int i = 0; i < results.Count; i++)
            {
                var result = results[i];
                json.AppendLine("    {");
                json.AppendLine($"      \"FileName\": \"{result.FileName}\",");
                json.AppendLine($"      \"FilePath\": \"{result.FilePath}\",");
                json.AppendLine($"      \"Directory\": \"{result.Directory}\",");
                json.AppendLine($"      \"FileSize\": {result.FileSize},");
                json.AppendLine($"      \"LastModified\": \"{result.LastModified:yyyy-MM-dd HH:mm:ss}\",");
                json.AppendLine($"      \"FileExtension\": \"{result.FileExtension}\",");
                json.AppendLine($"      \"MatchType\": \"{result.MatchType}\",");
                json.AppendLine($"      \"RelevanceScore\": {result.RelevanceScore:F2}");
                json.AppendLine("    }" + (i < results.Count - 1 ? "," : ""));
            }

            json.AppendLine("  ]");
            json.AppendLine("}");

            return json.ToString();
        }

        private string GenerateXmlContent(List<SearchResult> results)
        {
            var xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.AppendLine("<SearchResults ExportDate=\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\" Count=\"" + results.Count + "\">");
            xml.AppendLine("  <Results>");

            foreach (var result in results)
            {
                xml.AppendLine("    <Result>");
                xml.AppendLine($"      <FileName>{result.FileName}</FileName>");
                xml.AppendLine($"      <FilePath>{result.FilePath}</FilePath>");
                xml.AppendLine($"      <Directory>{result.Directory}</Directory>");
                xml.AppendLine($"      <FileSize>{result.FileSize}</FileSize>");
                xml.AppendLine($"      <LastModified>{result.LastModified:yyyy-MM-dd HH:mm:ss}</LastModified>");
                xml.AppendLine($"      <FileExtension>{result.FileExtension}</FileExtension>");
                xml.AppendLine($"      <MatchType>{result.MatchType}</MatchType>");
                xml.AppendLine($"      <RelevanceScore>{result.RelevanceScore:F2}</RelevanceScore>");
                xml.AppendLine("    </Result>");
            }

            xml.AppendLine("  </Results>");
            xml.AppendLine("</SearchResults>");

            return xml.ToString();
        }

        private string GenerateHtmlContent(List<SearchResult> results)
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
                html.AppendLine($"<td>{result.FileName}</td>");
                html.AppendLine($"<td>{result.FilePath}</td>");
                html.AppendLine($"<td>{result.FileSize:N0}</td>");
                html.AppendLine($"<td>{result.LastModified:yyyy-MM-dd HH:mm:ss}</td>");
                html.AppendLine($"<td>{result.MatchType}</td>");
                html.AppendLine($"<td>{result.RelevanceScore:F2}</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("</table>");
            html.AppendLine("</body></html>");

            return html.ToString();
        }

        private string GenerateDuplicateGroupsCsvContent(List<DuplicateGroup> groups)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Groupe ID,Type,Confiance,Nombre de fichiers,Taille totale,Espace gaspillé,Fichier,Chemin,Taille,Date de modification,Est original");

            foreach (var group in groups)
            {
                foreach (var file in group.Files)
                {
                    csv.AppendLine($"{group.Id},{group.Type},{group.Confidence:F2},{group.Files.Count},{group.TotalSize},{group.SpaceWasted},{file.FileName},{file.FilePath},{file.FileSize},{file.LastModified:yyyy-MM-dd HH:mm:ss},{file.IsOriginal}");
                }
            }

            return csv.ToString();
        }

        private string GenerateDuplicateGroupsJsonContent(List<DuplicateGroup> groups)
        {
            var json = new StringBuilder();
            json.AppendLine("{");
            json.AppendLine("  \"ExportDate\": \"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\",");
            json.AppendLine("  \"Format\": \"DuplicateGroups\",");
            json.AppendLine("  \"Count\": " + groups.Count + ",");
            json.AppendLine("  \"Groups\": [");

            for (int i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                json.AppendLine("    {");
                json.AppendLine($"      \"Id\": \"{group.Id}\",");
                json.AppendLine($"      \"Type\": \"{group.Type}\",");
                json.AppendLine($"      \"Confidence\": {group.Confidence:F2},");
                json.AppendLine($"      \"FileCount\": {group.Files.Count},");
                json.AppendLine($"      \"TotalSize\": {group.TotalSize},");
                json.AppendLine($"      \"SpaceWasted\": {group.SpaceWasted}");
                json.AppendLine("    }" + (i < groups.Count - 1 ? "," : ""));
            }

            json.AppendLine("  ]");
            json.AppendLine("}");

            return json.ToString();
        }

        private string GenerateDuplicateGroupsXmlContent(List<DuplicateGroup> groups)
        {
            var xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.AppendLine("<DuplicateGroups ExportDate=\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\" Count=\"" + groups.Count + "\">");
            xml.AppendLine("  <Groups>");

            foreach (var group in groups)
            {
                xml.AppendLine("    <Group Id=\"" + group.Id + "\" Type=\"" + group.Type + "\" Confidence=\"" + group.Confidence.ToString("F2") + "\">");
                xml.AppendLine($"      <FileCount>{group.Files.Count}</FileCount>");
                xml.AppendLine($"      <TotalSize>{group.TotalSize}</TotalSize>");
                xml.AppendLine($"      <SpaceWasted>{group.SpaceWasted}</SpaceWasted>");
                xml.AppendLine("    </Group>");
            }

            xml.AppendLine("  </Groups>");
            xml.AppendLine("</DuplicateGroups>");

            return xml.ToString();
        }

        private string GenerateDuplicateGroupsHtmlContent(List<DuplicateGroup> groups)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html><head>");
            html.AppendLine("<title>Groupes de doublons - FindEdge</title>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
            html.AppendLine(".group { border: 1px solid #ccc; margin: 10px 0; padding: 10px; }");
            html.AppendLine("</style>");
            html.AppendLine("</head><body>");
            html.AppendLine("<h1>Groupes de doublons - FindEdge</h1>");
            html.AppendLine($"<p>Date d'export: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
            html.AppendLine($"<p>Nombre de groupes: {groups.Count}</p>");

            foreach (var group in groups)
            {
                html.AppendLine("<div class='group'>");
                html.AppendLine($"<h3>Groupe {group.Id} - {group.Type} (Confiance: {group.Confidence:P})</h3>");
                html.AppendLine($"<p>Fichiers: {group.Files.Count} | Taille totale: {group.TotalSize:N0} | Espace gaspillé: {group.SpaceWasted:N0}</p>");
                html.AppendLine("</div>");
            }

            html.AppendLine("</body></html>");

            return html.ToString();
        }

        private string GenerateStatisticsJsonContent(SearchStatistics statistics)
        {
            var json = new StringBuilder();
            json.AppendLine("{");
            json.AppendLine("  \"ExportDate\": \"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\",");
            json.AppendLine("  \"Format\": \"SearchStatistics\",");
            json.AppendLine("  \"Statistics\": {");
            json.AppendLine($"    \"SearchDate\": \"{statistics.SearchDate:yyyy-MM-dd HH:mm:ss}\",");
            json.AppendLine($"    \"SearchTerm\": \"{statistics.SearchTerm}\",");
            json.AppendLine($"    \"TotalFilesFound\": {statistics.TotalFilesFound},");
            json.AppendLine($"    \"FilesWithContentMatch\": {statistics.FilesWithContentMatch},");
            json.AppendLine($"    \"FilesWithNameMatch\": {statistics.FilesWithNameMatch},");
            json.AppendLine($"    \"FilesWithBothMatch\": {statistics.FilesWithBothMatch},");
            json.AppendLine($"    \"TotalSize\": {statistics.TotalSize},");
            json.AppendLine($"    \"SearchDuration\": \"{statistics.SearchDuration}\",");
            json.AppendLine($"    \"DirectoriesSearched\": {statistics.DirectoriesSearched},");
            json.AppendLine($"    \"DuplicateGroupsFound\": {statistics.DuplicateGroupsFound},");
            json.AppendLine($"    \"SpaceWasted\": {statistics.SpaceWasted}");
            json.AppendLine("  }");
            json.AppendLine("}");

            return json.ToString();
        }

        private string GenerateStatisticsXmlContent(SearchStatistics statistics)
        {
            var xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.AppendLine("<SearchStatistics ExportDate=\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\">");
            xml.AppendLine($"  <SearchDate>{statistics.SearchDate:yyyy-MM-dd HH:mm:ss}</SearchDate>");
            xml.AppendLine($"  <SearchTerm>{statistics.SearchTerm}</SearchTerm>");
            xml.AppendLine($"  <TotalFilesFound>{statistics.TotalFilesFound}</TotalFilesFound>");
            xml.AppendLine($"  <FilesWithContentMatch>{statistics.FilesWithContentMatch}</FilesWithContentMatch>");
            xml.AppendLine($"  <FilesWithNameMatch>{statistics.FilesWithNameMatch}</FilesWithNameMatch>");
            xml.AppendLine($"  <FilesWithBothMatch>{statistics.FilesWithBothMatch}</FilesWithBothMatch>");
            xml.AppendLine($"  <TotalSize>{statistics.TotalSize}</TotalSize>");
            xml.AppendLine($"  <SearchDuration>{statistics.SearchDuration}</SearchDuration>");
            xml.AppendLine($"  <DirectoriesSearched>{statistics.DirectoriesSearched}</DirectoriesSearched>");
            xml.AppendLine($"  <DuplicateGroupsFound>{statistics.DuplicateGroupsFound}</DuplicateGroupsFound>");
            xml.AppendLine($"  <SpaceWasted>{statistics.SpaceWasted}</SpaceWasted>");
            xml.AppendLine("</SearchStatistics>");

            return xml.ToString();
        }

        private string GenerateStatisticsHtmlContent(SearchStatistics statistics)
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
            html.AppendLine($"<div class='stat'><strong>Terme de recherche:</strong> {statistics.SearchTerm}</div>");
            html.AppendLine($"<div class='stat'><strong>Date de recherche:</strong> {statistics.SearchDate:yyyy-MM-dd HH:mm:ss}</div>");
            html.AppendLine($"<div class='stat'><strong>Durée de recherche:</strong> {statistics.SearchDuration}</div>");
            html.AppendLine($"<div class='stat'><strong>Fichiers trouvés:</strong> {statistics.TotalFilesFound}</div>");
            html.AppendLine($"<div class='stat'><strong>Correspondance nom:</strong> {statistics.FilesWithNameMatch}</div>");
            html.AppendLine($"<div class='stat'><strong>Correspondance contenu:</strong> {statistics.FilesWithContentMatch}</div>");
            html.AppendLine($"<div class='stat'><strong>Correspondance les deux:</strong> {statistics.FilesWithBothMatch}</div>");
            html.AppendLine($"<div class='stat'><strong>Taille totale:</strong> {statistics.TotalSize:N0} octets</div>");
            html.AppendLine($"<div class='stat'><strong>Répertoires explorés:</strong> {statistics.DirectoriesSearched}</div>");
            html.AppendLine($"<div class='stat'><strong>Groupes de doublons:</strong> {statistics.DuplicateGroupsFound}</div>");
            html.AppendLine($"<div class='stat'><strong>Espace gaspillé:</strong> {statistics.SpaceWasted:N0} octets</div>");
            html.AppendLine("</body></html>");

            return html.ToString();
        }

        #endregion
    }
}
