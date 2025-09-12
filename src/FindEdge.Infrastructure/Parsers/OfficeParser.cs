using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Presentation;
using WordRun = DocumentFormat.OpenXml.Wordprocessing.Run;
using WordText = DocumentFormat.OpenXml.Wordprocessing.Text;
using SpreadsheetRun = DocumentFormat.OpenXml.Spreadsheet.Run;
using SpreadsheetText = DocumentFormat.OpenXml.Spreadsheet.Text;
using PresentationRun = DocumentFormat.OpenXml.Drawing.Run;
using PresentationText = DocumentFormat.OpenXml.Drawing.Text;

namespace FindEdge.Infrastructure.Parsers
{
    /// <summary>
    /// Parseur pour les fichiers Microsoft Office (Word, Excel, PowerPoint)
    /// </summary>
    public class OfficeParser : IContentParser
    {
        public string[] SupportedExtensions { get; } = new[]
        {
            ".docx", ".xlsx", ".pptx"
        };

        public int Priority => 150;

        public bool CanParse(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return Array.Exists(SupportedExtensions, ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<string> ExtractContentAsync(string filePath, CancellationToken cancellationToken = default)
        {
            try
            {
                var extension = Path.GetExtension(filePath).ToLowerInvariant();
                
                return extension switch
                {
                    ".docx" => await ExtractWordContentAsync(filePath, cancellationToken),
                    ".xlsx" => await ExtractExcelContentAsync(filePath, cancellationToken),
                    ".pptx" => await ExtractPowerPointContentAsync(filePath, cancellationToken),
                    _ => string.Empty
                };
            }
            catch (Exception ex)
            {
                return $"Erreur lors de l'extraction Office: {ex.Message}";
            }
        }

        private async Task<string> ExtractWordContentAsync(string filePath, CancellationToken cancellationToken)
        {
            var content = new StringBuilder();

            using var document = WordprocessingDocument.Open(filePath, false);
            var body = document.MainDocumentPart?.Document?.Body;

            if (body != null)
            {
                foreach (var paragraph in body.Elements<Paragraph>())
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var paragraphText = GetTextFromParagraph(paragraph);
                    if (!string.IsNullOrEmpty(paragraphText))
                    {
                        content.AppendLine(paragraphText);
                    }
                }
            }

            return content.ToString();
        }

        private async Task<string> ExtractExcelContentAsync(string filePath, CancellationToken cancellationToken)
        {
            var content = new StringBuilder();

            using var document = SpreadsheetDocument.Open(filePath, false);
            var workbookPart = document.WorkbookPart;

            if (workbookPart != null)
            {
                foreach (var worksheetPart in workbookPart.WorksheetParts)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var worksheet = worksheetPart.Worksheet;
                    var sheetData = worksheet.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.SheetData>();

                    if (sheetData != null)
                    {
                        foreach (var row in sheetData.Elements<DocumentFormat.OpenXml.Spreadsheet.Row>())
                        {
                            if (cancellationToken.IsCancellationRequested)
                                break;

                            var rowText = GetTextFromRow(row, workbookPart);
                            if (!string.IsNullOrEmpty(rowText))
                            {
                                content.AppendLine(rowText);
                            }
                        }
                    }
                }
            }

            return content.ToString();
        }

        private async Task<string> ExtractPowerPointContentAsync(string filePath, CancellationToken cancellationToken)
        {
            var content = new StringBuilder();

            using var document = PresentationDocument.Open(filePath, false);
            var presentationPart = document.PresentationPart;

            if (presentationPart != null)
            {
                foreach (var slidePart in presentationPart.SlideParts)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var slide = slidePart.Slide;
                    var slideText = GetTextFromSlide(slide);
                    if (!string.IsNullOrEmpty(slideText))
                    {
                        content.AppendLine(slideText);
                    }
                }
            }

            return content.ToString();
        }

        private string GetTextFromParagraph(Paragraph paragraph)
        {
            var text = new StringBuilder();
            
            foreach (var run in paragraph.Elements<WordRun>())
            {
                foreach (var textElement in run.Elements<WordText>())
                {
                    text.Append(textElement.Text);
                }
            }

            return text.ToString().Trim();
        }

        private string GetTextFromRow(DocumentFormat.OpenXml.Spreadsheet.Row row, WorkbookPart workbookPart)
        {
            var text = new StringBuilder();
            
            foreach (var cell in row.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>())
            {
                var cellValue = GetCellValue(cell, workbookPart);
                if (!string.IsNullOrEmpty(cellValue))
                {
                    text.Append(cellValue + " ");
                }
            }

            return text.ToString().Trim();
        }

        private string GetCellValue(DocumentFormat.OpenXml.Spreadsheet.Cell cell, WorkbookPart workbookPart)
        {
            if (cell.CellValue == null)
                return string.Empty;

            var value = cell.CellValue.Text;
            
            // Si c'est un nombre, retourner directement
            if (cell.DataType == null || cell.DataType.Value == DocumentFormat.OpenXml.Spreadsheet.CellValues.Number)
                return value;

            // Si c'est une référence partagée, récupérer la valeur
            if (cell.DataType.Value == DocumentFormat.OpenXml.Spreadsheet.CellValues.SharedString)
            {
                var stringTable = workbookPart.SharedStringTablePart?.SharedStringTable;
                if (stringTable != null && int.TryParse(value, out var index))
                {
                    var stringItem = stringTable.ElementAt(index);
                    return stringItem?.InnerText ?? value;
                }
            }

            return value;
        }

        private string GetTextFromSlide(DocumentFormat.OpenXml.Presentation.Slide slide)
        {
            var text = new StringBuilder();
            
            foreach (var textBody in slide.Descendants<DocumentFormat.OpenXml.Drawing.TextBody>())
            {
                foreach (var paragraph in textBody.Elements<DocumentFormat.OpenXml.Drawing.Paragraph>())
                {
                    foreach (var run in paragraph.Elements<PresentationRun>())
                    {
                        foreach (var textElement in run.Elements<PresentationText>())
                        {
                            text.Append(textElement.Text);
                        }
                    }
                    text.AppendLine();
                }
            }

            return text.ToString().Trim();
        }
    }
}
