using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FindEdge.Core.Interfaces
{
    /// <summary>
    /// Interface pour le service de personnalisation avancée
    /// </summary>
    public interface IPersonalizationService
    {
        /// <summary>
        /// Crée un thème personnalisé
        /// </summary>
        Task<CustomTheme> CreateCustomThemeAsync(string name, ThemeConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les thèmes disponibles
        /// </summary>
        Task<IEnumerable<Theme>> GetAvailableThemesAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Applique un thème à l'interface
        /// </summary>
        Task ApplyThemeAsync(string themeId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Crée un profil de recherche personnalisé
        /// </summary>
        Task<SearchProfile> CreateSearchProfileAsync(string name, SearchProfileConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les profils de recherche de l'utilisateur
        /// </summary>
        Task<IEnumerable<SearchProfile>> GetUserSearchProfilesAsync(string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Active un profil de recherche
        /// </summary>
        Task ActivateSearchProfileAsync(string profileId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure les raccourcis clavier personnalisés
        /// </summary>
        Task<KeyboardShortcut> ConfigureKeyboardShortcutAsync(string action, string keyCombination, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les raccourcis clavier configurés
        /// </summary>
        Task<IEnumerable<KeyboardShortcut>> GetKeyboardShortcutsAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure l'interface adaptative
        /// </summary>
        Task<AdaptiveInterface> ConfigureAdaptiveInterfaceAsync(AdaptiveInterfaceConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient la configuration d'interface adaptative
        /// </summary>
        Task<AdaptiveInterface> GetAdaptiveInterfaceConfigurationAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure les préférences utilisateur
        /// </summary>
        Task<UserPreferences> ConfigureUserPreferencesAsync(UserPreferences preferences, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les préférences utilisateur
        /// </summary>
        Task<UserPreferences> GetUserPreferencesAsync(string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure les widgets de l'interface
        /// </summary>
        Task<InterfaceWidget> ConfigureWidgetAsync(string widgetId, WidgetConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les widgets configurés
        /// </summary>
        Task<IEnumerable<InterfaceWidget>> GetConfiguredWidgetsAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure les notifications personnalisées
        /// </summary>
        Task<NotificationPreference> ConfigureNotificationPreferenceAsync(string eventType, NotificationPreferenceConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les préférences de notification
        /// </summary>
        Task<IEnumerable<NotificationPreference>> GetNotificationPreferencesAsync(string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure les filtres personnalisés
        /// </summary>
        Task<CustomFilter> CreateCustomFilterAsync(string name, FilterConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les filtres personnalisés
        /// </summary>
        Task<IEnumerable<CustomFilter>> GetCustomFiltersAsync(string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Configure les vues personnalisées
        /// </summary>
        Task<CustomView> CreateCustomViewAsync(string name, ViewConfiguration config, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtient les vues personnalisées
        /// </summary>
        Task<IEnumerable<CustomView>> GetCustomViewsAsync(string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Exporte la configuration personnalisée
        /// </summary>
        Task<byte[]> ExportPersonalizationAsync(string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Importe une configuration personnalisée
        /// </summary>
        Task ImportPersonalizationAsync(string userId, byte[] configuration, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Réinitialise la personnalisation aux valeurs par défaut
        /// </summary>
        Task ResetPersonalizationAsync(string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Événement déclenché lors du changement de thème
        /// </summary>
        event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
        
        /// <summary>
        /// Événement déclenché lors du changement de profil de recherche
        /// </summary>
        event EventHandler<SearchProfileChangedEventArgs>? SearchProfileChanged;
    }

    /// <summary>
    /// Thème personnalisé
    /// </summary>
    public class CustomTheme
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ThemeConfiguration Configuration { get; set; } = new();
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPublic { get; set; } = false;
        public List<string> Tags { get; set; } = new();
    }

    /// <summary>
    /// Thème
    /// </summary>
    public class Theme
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ThemeType Type { get; set; }
        public bool IsBuiltIn { get; set; }
        public ThemeConfiguration Configuration { get; set; } = new();
    }

    /// <summary>
    /// Type de thème
    /// </summary>
    public enum ThemeType
    {
        Light,
        Dark,
        HighContrast,
        Custom
    }

    /// <summary>
    /// Configuration de thème
    /// </summary>
    public class ThemeConfiguration
    {
        public string PrimaryColor { get; set; } = "#0078D4";
        public string SecondaryColor { get; set; } = "#106EBE";
        public string BackgroundColor { get; set; } = "#FFFFFF";
        public string SurfaceColor { get; set; } = "#F8F9FA";
        public string TextColor { get; set; } = "#323130";
        public string AccentColor { get; set; } = "#0078D4";
        public string ErrorColor { get; set; } = "#D13438";
        public string WarningColor { get; set; } = "#FF8C00";
        public string SuccessColor { get; set; } = "#107C10";
        public string FontFamily { get; set; } = "Segoe UI";
        public int FontSize { get; set; } = 14;
        public Dictionary<string, object> CustomProperties { get; set; } = new();
    }

    /// <summary>
    /// Profil de recherche
    /// </summary>
    public class SearchProfile
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public SearchProfileConfiguration Configuration { get; set; } = new();
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = false;
        public List<string> Tags { get; set; } = new();
    }

    /// <summary>
    /// Configuration de profil de recherche
    /// </summary>
    public class SearchProfileConfiguration
    {
        public List<string> DefaultSearchPaths { get; set; } = new();
        public List<string> DefaultFileTypes { get; set; } = new();
        public List<string> DefaultExclusions { get; set; } = new();
        public bool CaseSensitive { get; set; } = false;
        public bool UseRegex { get; set; } = false;
        public bool WholeWord { get; set; } = false;
        public int MaxResults { get; set; } = 1000;
        public bool IncludeHiddenFiles { get; set; } = false;
        public bool IncludeSystemFiles { get; set; } = false;
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Raccourci clavier
    /// </summary>
    public class KeyboardShortcut
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Action { get; set; } = string.Empty;
        public string KeyCombination { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
        public string Category { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Interface adaptative
    /// </summary>
    public class AdaptiveInterface
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public AdaptiveInterfaceConfiguration Configuration { get; set; } = new();
        public string UserId { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> Metrics { get; set; } = new();
    }

    /// <summary>
    /// Configuration d'interface adaptative
    /// </summary>
    public class AdaptiveInterfaceConfiguration
    {
        public bool AutoAdjustLayout { get; set; } = true;
        public bool AutoAdjustFontSize { get; set; } = true;
        public bool AutoAdjustColors { get; set; } = false;
        public int MinFontSize { get; set; } = 10;
        public int MaxFontSize { get; set; } = 24;
        public List<ScreenResolution> SupportedResolutions { get; set; } = new();
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Résolution d'écran
    /// </summary>
    public class ScreenResolution
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public double DPI { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Préférences utilisateur
    /// </summary>
    public class UserPreferences
    {
        public string UserId { get; set; } = string.Empty;
        public string Language { get; set; } = "fr-FR";
        public string Region { get; set; } = "FR";
        public string TimeZone { get; set; } = "Europe/Paris";
        public string DateFormat { get; set; } = "dd/MM/yyyy";
        public string TimeFormat { get; set; } = "HH:mm";
        public string NumberFormat { get; set; } = "0,00";
        public bool ShowTooltips { get; set; } = true;
        public bool ShowStatusBar { get; set; } = true;
        public bool ShowSidebar { get; set; } = true;
        public bool EnableAnimations { get; set; } = true;
        public bool EnableSounds { get; set; } = true;
        public Dictionary<string, object> CustomPreferences { get; set; } = new();
    }

    /// <summary>
    /// Widget d'interface
    /// </summary>
    public class InterfaceWidget
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public WidgetType Type { get; set; }
        public WidgetConfiguration Configuration { get; set; } = new();
        public WidgetPosition Position { get; set; } = new();
        public bool IsVisible { get; set; } = true;
        public int Order { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    /// <summary>
    /// Type de widget
    /// </summary>
    public enum WidgetType
    {
        SearchBox,
        RecentSearches,
        Favorites,
        Statistics,
        QuickActions,
        FilePreview,
        SearchHistory,
        Custom
    }

    /// <summary>
    /// Configuration de widget
    /// </summary>
    public class WidgetConfiguration
    {
        public string Title { get; set; } = string.Empty;
        public int Width { get; set; } = 200;
        public int Height { get; set; } = 150;
        public bool IsResizable { get; set; } = true;
        public bool IsMovable { get; set; } = true;
        public bool IsCollapsible { get; set; } = true;
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Position de widget
    /// </summary>
    public class WidgetPosition
    {
        public int X { get; set; }
        public int Y { get; set; }
        public WidgetDock Dock { get; set; }
        public int ZIndex { get; set; }
    }

    /// <summary>
    /// Dock de widget
    /// </summary>
    public enum WidgetDock
    {
        Left,
        Right,
        Top,
        Bottom,
        Center,
        Floating
    }

    /// <summary>
    /// Préférence de notification
    /// </summary>
    public class NotificationPreference
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string EventType { get; set; } = string.Empty;
        public NotificationPreferenceConfiguration Configuration { get; set; } = new();
        public string UserId { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
    }

    /// <summary>
    /// Configuration de préférence de notification
    /// </summary>
    public class NotificationPreferenceConfiguration
    {
        public bool EnableEmail { get; set; } = true;
        public bool EnablePush { get; set; } = true;
        public bool EnableInApp { get; set; } = true;
        public string EmailTemplate { get; set; } = string.Empty;
        public string PushTemplate { get; set; } = string.Empty;
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Filtre personnalisé
    /// </summary>
    public class CustomFilter
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FilterConfiguration Configuration { get; set; } = new();
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPublic { get; set; } = false;
        public List<string> Tags { get; set; } = new();
    }

    /// <summary>
    /// Configuration de filtre
    /// </summary>
    public class FilterConfiguration
    {
        public List<string> FileTypes { get; set; } = new();
        public List<string> Directories { get; set; } = new();
        public List<string> Exclusions { get; set; } = new();
        public long? MinFileSize { get; set; }
        public long? MaxFileSize { get; set; }
        public DateTime? ModifiedAfter { get; set; }
        public DateTime? ModifiedBefore { get; set; }
        public List<string> Keywords { get; set; } = new();
        public Dictionary<string, object> CustomFilters { get; set; } = new();
    }

    /// <summary>
    /// Vue personnalisée
    /// </summary>
    public class CustomView
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ViewConfiguration Configuration { get; set; } = new();
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPublic { get; set; } = false;
        public List<string> Tags { get; set; } = new();
    }

    /// <summary>
    /// Configuration de vue
    /// </summary>
    public class ViewConfiguration
    {
        public ViewType Type { get; set; }
        public List<ViewColumn> Columns { get; set; } = new();
        public ViewSorting Sorting { get; set; } = new();
        public ViewGrouping Grouping { get; set; } = new();
        public ViewFiltering Filtering { get; set; } = new();
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Type de vue
    /// </summary>
    public enum ViewType
    {
        List,
        Grid,
        Details,
        Thumbnail,
        Timeline,
        Tree
    }

    /// <summary>
    /// Colonne de vue
    /// </summary>
    public class ViewColumn
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int Width { get; set; } = 100;
        public bool IsVisible { get; set; } = true;
        public bool IsSortable { get; set; } = true;
        public bool IsFilterable { get; set; } = true;
        public int Order { get; set; }
    }

    /// <summary>
    /// Tri de vue
    /// </summary>
    public class ViewSorting
    {
        public string Column { get; set; } = string.Empty;
        public SortDirection Direction { get; set; }
        public List<ViewSortingRule> SecondarySorts { get; set; } = new();
    }

    /// <summary>
    /// Direction de tri
    /// </summary>
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    /// <summary>
    /// Règle de tri de vue
    /// </summary>
    public class ViewSortingRule
    {
        public string Column { get; set; } = string.Empty;
        public SortDirection Direction { get; set; }
    }

    /// <summary>
    /// Groupement de vue
    /// </summary>
    public class ViewGrouping
    {
        public string Column { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = false;
        public GroupingType Type { get; set; }
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Type de groupement
    /// </summary>
    public enum GroupingType
    {
        ByFileType,
        ByDate,
        BySize,
        ByDirectory,
        Custom
    }

    /// <summary>
    /// Filtrage de vue
    /// </summary>
    public class ViewFiltering
    {
        public List<ViewFilter> Filters { get; set; } = new();
        public FilterLogic Logic { get; set; }
        public bool IsEnabled { get; set; } = false;
    }

    /// <summary>
    /// Filtre de vue
    /// </summary>
    public class ViewFilter
    {
        public string Column { get; set; } = string.Empty;
        public FilterOperator Operator { get; set; }
        public string Value { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
    }

    /// <summary>
    /// Opérateur de filtre
    /// </summary>
    public enum FilterOperator
    {
        Equals,
        NotEquals,
        Contains,
        NotContains,
        StartsWith,
        EndsWith,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Between,
        In,
        NotIn
    }

    /// <summary>
    /// Logique de filtre
    /// </summary>
    public enum FilterLogic
    {
        And,
        Or
    }

    // Événements
    public class ThemeChangedEventArgs : EventArgs
    {
        public string OldThemeId { get; set; } = string.Empty;
        public string NewThemeId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class SearchProfileChangedEventArgs : EventArgs
    {
        public string OldProfileId { get; set; } = string.Empty;
        public string NewProfileId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
