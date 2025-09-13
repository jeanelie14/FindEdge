using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Models;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour le service de visualisation et présentation
    /// </summary>
    public interface IVisualizationService
    {
        /// <summary>
        /// Génère une vue en mosaïque des résultats de recherche
        /// </summary>
        Task<MosaicView> GenerateMosaicViewAsync(IEnumerable<SearchResult> results, MosaicViewOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Génère une timeline visuelle des résultats
        /// </summary>
        Task<TimelineView> GenerateTimelineViewAsync(IEnumerable<SearchResult> results, TimelineViewOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Génère des graphiques de données pour les statistiques
        /// </summary>
        Task<DataVisualization> GenerateDataVisualizationAsync(VisualizationData data, VisualizationOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Génère une carte de chaleur des fichiers
        /// </summary>
        Task<HeatmapView> GenerateHeatmapViewAsync(IEnumerable<SearchResult> results, HeatmapOptions options, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Génère une visualisation de réseau de fichiers
        /// </summary>
        Task<NetworkView> GenerateNetworkViewAsync(IEnumerable<FileRelationship> relationships, NetworkViewOptions options, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Vue en mosaïque
    /// </summary>
    public class MosaicView
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public List<MosaicItem> Items { get; set; } = new();
        public MosaicLayout Layout { get; set; } = new();
        public MosaicViewOptions Options { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Élément de mosaïque
    /// </summary>
    public class MosaicItem
    {
        public string FilePath { get; set; } = string.Empty;
        public string ThumbnailPath { get; set; } = string.Empty;
        public MosaicItemType Type { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Type d'élément de mosaïque
    /// </summary>
    public enum MosaicItemType
    {
        Image,
        Document,
        Video,
        Audio,
        Archive,
        Code,
        Other
    }

    /// <summary>
    /// Layout de mosaïque
    /// </summary>
    public class MosaicLayout
    {
        public int TotalWidth { get; set; }
        public int TotalHeight { get; set; }
        public int ItemSpacing { get; set; }
        public MosaicLayoutType Type { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
    }

    /// <summary>
    /// Type de layout de mosaïque
    /// </summary>
    public enum MosaicLayoutType
    {
        Grid,
        Masonry,
        Circular,
        Spiral,
        Custom
    }

    /// <summary>
    /// Options de vue en mosaïque
    /// </summary>
    public class MosaicViewOptions
    {
        public int MaxItems { get; set; } = 100;
        public int ThumbnailSize { get; set; } = 150;
        public bool ShowFileNames { get; set; } = true;
        public bool ShowFileSizes { get; set; } = true;
        public bool ShowFileDates { get; set; } = true;
        public List<string> SupportedFormats { get; set; } = new();
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    /// <summary>
    /// Vue timeline
    /// </summary>
    public class TimelineView
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public List<TimelineItem> Items { get; set; } = new();
        public TimelineScale Scale { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimelineViewOptions Options { get; set; } = new();
    }

    /// <summary>
    /// Élément de timeline
    /// </summary>
    public class TimelineItem
    {
        public string FilePath { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimelineItemType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Type d'élément de timeline
    /// </summary>
    public enum TimelineItemType
    {
        Created,
        Modified,
        Accessed,
        Moved,
        Deleted
    }

    /// <summary>
    /// Échelle de timeline
    /// </summary>
    public enum TimelineScale
    {
        Minutes,
        Hours,
        Days,
        Weeks,
        Months,
        Years
    }

    /// <summary>
    /// Options de vue timeline
    /// </summary>
    public class TimelineViewOptions
    {
        public bool ShowFileIcons { get; set; } = true;
        public bool ShowFileNames { get; set; } = true;
        public bool GroupByType { get; set; } = false;
        public bool ShowStatistics { get; set; } = true;
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    /// <summary>
    /// Visualisation de données
    /// </summary>
    public class DataVisualization
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public VisualizationType Type { get; set; }
        public List<DataSeries> Series { get; set; } = new();
        public VisualizationOptions Options { get; set; } = new();
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Type de visualisation
    /// </summary>
    public enum VisualizationType
    {
        BarChart,
        LineChart,
        PieChart,
        ScatterPlot,
        Histogram,
        Heatmap,
        Treemap,
        Sankey,
        Gantt
    }

    /// <summary>
    /// Série de données
    /// </summary>
    public class DataSeries
    {
        public string Name { get; set; } = string.Empty;
        public List<DataPoint> Points { get; set; } = new();
        public string Color { get; set; } = string.Empty;
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    /// <summary>
    /// Point de données
    /// </summary>
    public class DataPoint
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
        public DateTime? Timestamp { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Données de visualisation
    /// </summary>
    public class VisualizationData
    {
        public string Title { get; set; } = string.Empty;
        public List<DataSeries> Series { get; set; } = new();
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Options de visualisation
    /// </summary>
    public class VisualizationOptions
    {
        public int Width { get; set; } = 800;
        public int Height { get; set; } = 600;
        public string Title { get; set; } = string.Empty;
        public bool ShowLegend { get; set; } = true;
        public bool ShowGrid { get; set; } = true;
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    /// <summary>
    /// Vue de carte de chaleur
    /// </summary>
    public class HeatmapView
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public List<HeatmapCell> Cells { get; set; } = new();
        public HeatmapOptions Options { get; set; } = new();
        public HeatmapStatistics Statistics { get; set; } = new();
    }

    /// <summary>
    /// Cellule de carte de chaleur
    /// </summary>
    public class HeatmapCell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double Value { get; set; }
        public string Color { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Options de carte de chaleur
    /// </summary>
    public class HeatmapOptions
    {
        public int Width { get; set; } = 100;
        public int Height { get; set; } = 100;
        public string ColorScheme { get; set; } = "viridis";
        public bool ShowValues { get; set; } = false;
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    /// <summary>
    /// Statistiques de carte de chaleur
    /// </summary>
    public class HeatmapStatistics
    {
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double AverageValue { get; set; }
        public int TotalCells { get; set; }
    }

    /// <summary>
    /// Vue de réseau
    /// </summary>
    public class NetworkView
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public List<NetworkNode> Nodes { get; set; } = new();
        public List<NetworkEdge> Edges { get; set; } = new();
        public NetworkViewOptions Options { get; set; } = new();
    }

    /// <summary>
    /// Nœud de réseau
    /// </summary>
    public class NetworkNode
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FilePath { get; set; } = string.Empty;
        public double X { get; set; }
        public double Y { get; set; }
        public string Color { get; set; } = string.Empty;
        public double Size { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    /// <summary>
    /// Arête de réseau
    /// </summary>
    public class NetworkEdge
    {
        public string FromNodeId { get; set; } = string.Empty;
        public string ToNodeId { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public double Thickness { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    /// <summary>
    /// Options de vue de réseau
    /// </summary>
    public class NetworkViewOptions
    {
        public bool EnablePhysics { get; set; } = true;
        public bool ShowLabels { get; set; } = true;
        public bool ShowEdges { get; set; } = true;
        public string Layout { get; set; } = "force";
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    /// <summary>
    /// Relation de fichier
    /// </summary>
    public class FileRelationship
    {
        public string SourcePath { get; set; } = string.Empty;
        public string TargetPath { get; set; } = string.Empty;
        public RelationshipType Type { get; set; }
        public double Strength { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Type de relation
    /// </summary>
    public enum RelationshipType
    {
        Contains,
        References,
        Similar,
        DependsOn,
        Imports,
        Exports
    }
}