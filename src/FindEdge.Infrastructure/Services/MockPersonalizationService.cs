using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FindEdge.Core.Interfaces;

namespace FindEdge.Infrastructure.Services
{
    /// <summary>
    /// Mock implementation of IPersonalizationService for testing and development
    /// </summary>
    public class MockPersonalizationService : IPersonalizationService
    {
        public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
        public event EventHandler<SearchProfileChangedEventArgs>? SearchProfileChanged;

        public Task<CustomTheme> CreateCustomThemeAsync(string name, ThemeConfiguration config, CancellationToken cancellationToken = default)
        {
            // Mock implementation - return dummy custom theme
            var mockTheme = new CustomTheme
            {
                Name = name,
                Description = "Mock custom theme",
                Configuration = config,
                CreatedBy = "MockUser",
                CreatedAt = DateTime.UtcNow,
                IsPublic = false,
                Tags = new List<string>()
            };
            
            return Task.FromResult(mockTheme);
        }

        public Task<IEnumerable<Theme>> GetAvailableThemesAsync(CancellationToken cancellationToken = default)
        {
            // Mock implementation - return default themes
            var themes = new List<Theme>
            {
                new Theme
                {
                    Name = "Default",
                    Description = "Default theme",
                    Type = ThemeType.Light,
                    IsBuiltIn = true,
                    Configuration = new ThemeConfiguration()
                },
                new Theme
                {
                    Name = "Dark",
                    Description = "Dark theme",
                    Type = ThemeType.Dark,
                    IsBuiltIn = true,
                    Configuration = new ThemeConfiguration()
                }
            };
            
            return Task.FromResult<IEnumerable<Theme>>(themes);
        }

        public Task ApplyThemeAsync(string themeId, CancellationToken cancellationToken = default)
        {
            // Mock implementation - trigger theme changed event
            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs
            {
                OldThemeId = "previous",
                NewThemeId = themeId,
                Timestamp = DateTime.UtcNow
            });
            
            return Task.CompletedTask;
        }

        public Task<SearchProfile> CreateSearchProfileAsync(string name, SearchProfileConfiguration config, CancellationToken cancellationToken = default)
        {
            // Mock implementation - return dummy search profile
            var mockProfile = new SearchProfile
            {
                Name = name,
                Description = "Mock search profile",
                Configuration = config,
                CreatedBy = "MockUser",
                CreatedAt = DateTime.UtcNow,
                IsActive = false,
                Tags = new List<string>()
            };
            
            return Task.FromResult(mockProfile);
        }

        public Task<IEnumerable<SearchProfile>> GetUserSearchProfilesAsync(string userId, CancellationToken cancellationToken = default)
        {
            // Mock implementation - return empty profiles
            return Task.FromResult<IEnumerable<SearchProfile>>(new List<SearchProfile>());
        }

        public Task ActivateSearchProfileAsync(string profileId, CancellationToken cancellationToken = default)
        {
            // Mock implementation - trigger search profile changed event
            SearchProfileChanged?.Invoke(this, new SearchProfileChangedEventArgs
            {
                OldProfileId = "previous",
                NewProfileId = profileId,
                Timestamp = DateTime.UtcNow
            });
            
            return Task.CompletedTask;
        }

        public Task<KeyboardShortcut> ConfigureKeyboardShortcutAsync(string action, string keyCombination, CancellationToken cancellationToken = default)
        {
            // Mock implementation - return dummy keyboard shortcut
            var mockShortcut = new KeyboardShortcut
            {
                Action = action,
                KeyCombination = keyCombination,
                Description = $"Mock shortcut for {action}",
                IsEnabled = true,
                Category = "General",
                Metadata = new Dictionary<string, object>()
            };
            
            return Task.FromResult(mockShortcut);
        }

        public Task<IEnumerable<KeyboardShortcut>> GetKeyboardShortcutsAsync(CancellationToken cancellationToken = default)
        {
            // Mock implementation - return empty shortcuts
            return Task.FromResult<IEnumerable<KeyboardShortcut>>(new List<KeyboardShortcut>());
        }

        public Task<AdaptiveInterface> ConfigureAdaptiveInterfaceAsync(AdaptiveInterfaceConfiguration config, CancellationToken cancellationToken = default)
        {
            // Mock implementation - return dummy adaptive interface
            var mockInterface = new AdaptiveInterface
            {
                Configuration = config,
                UserId = "MockUser",
                LastUpdated = DateTime.UtcNow,
                Metrics = new Dictionary<string, object>()
            };
            
            return Task.FromResult(mockInterface);
        }

        public Task<AdaptiveInterface> GetAdaptiveInterfaceConfigurationAsync(CancellationToken cancellationToken = default)
        {
            // Mock implementation - return default adaptive interface
            var mockInterface = new AdaptiveInterface
            {
                Configuration = new AdaptiveInterfaceConfiguration(),
                UserId = "MockUser",
                LastUpdated = DateTime.UtcNow,
                Metrics = new Dictionary<string, object>()
            };
            
            return Task.FromResult(mockInterface);
        }

        public Task<UserPreferences> ConfigureUserPreferencesAsync(UserPreferences preferences, CancellationToken cancellationToken = default)
        {
            // Mock implementation - return the preferences as-is
            return Task.FromResult(preferences);
        }

        public Task<UserPreferences> GetUserPreferencesAsync(string userId, CancellationToken cancellationToken = default)
        {
            // Mock implementation - return default preferences
            var mockPreferences = new UserPreferences
            {
                UserId = userId,
                Language = "fr-FR",
                Region = "FR",
                TimeZone = "Europe/Paris",
                DateFormat = "dd/MM/yyyy",
                TimeFormat = "HH:mm",
                NumberFormat = "0,00",
                ShowTooltips = true,
                ShowStatusBar = true,
                ShowSidebar = true,
                EnableAnimations = true,
                EnableSounds = true,
                CustomPreferences = new Dictionary<string, object>()
            };
            
            return Task.FromResult(mockPreferences);
        }

        public Task<InterfaceWidget> ConfigureWidgetAsync(string widgetId, WidgetConfiguration config, CancellationToken cancellationToken = default)
        {
            // Mock implementation - return dummy widget
            var mockWidget = new InterfaceWidget
            {
                Name = $"Widget {widgetId}",
                Type = WidgetType.Custom,
                Configuration = config,
                Position = new WidgetPosition(),
                IsVisible = true,
                Order = 0,
                Properties = new Dictionary<string, object>()
            };
            
            return Task.FromResult(mockWidget);
        }

        public Task<IEnumerable<InterfaceWidget>> GetConfiguredWidgetsAsync(CancellationToken cancellationToken = default)
        {
            // Mock implementation - return empty widgets
            return Task.FromResult<IEnumerable<InterfaceWidget>>(new List<InterfaceWidget>());
        }

        public Task<NotificationPreference> ConfigureNotificationPreferenceAsync(string eventType, NotificationPreferenceConfiguration config, CancellationToken cancellationToken = default)
        {
            // Mock implementation - return dummy notification preference
            var mockPreference = new NotificationPreference
            {
                EventType = eventType,
                Configuration = config,
                UserId = "MockUser",
                IsEnabled = true
            };
            
            return Task.FromResult(mockPreference);
        }

        public Task<IEnumerable<NotificationPreference>> GetNotificationPreferencesAsync(string userId, CancellationToken cancellationToken = default)
        {
            // Mock implementation - return empty preferences
            return Task.FromResult<IEnumerable<NotificationPreference>>(new List<NotificationPreference>());
        }

        public Task<CustomFilter> CreateCustomFilterAsync(string name, FilterConfiguration config, CancellationToken cancellationToken = default)
        {
            // Mock implementation - return dummy custom filter
            var mockFilter = new CustomFilter
            {
                Name = name,
                Description = "Mock custom filter",
                Configuration = config,
                CreatedBy = "MockUser",
                CreatedAt = DateTime.UtcNow,
                IsPublic = false,
                Tags = new List<string>()
            };
            
            return Task.FromResult(mockFilter);
        }

        public Task<IEnumerable<CustomFilter>> GetCustomFiltersAsync(string userId, CancellationToken cancellationToken = default)
        {
            // Mock implementation - return empty filters
            return Task.FromResult<IEnumerable<CustomFilter>>(new List<CustomFilter>());
        }

        public Task<CustomView> CreateCustomViewAsync(string name, ViewConfiguration config, CancellationToken cancellationToken = default)
        {
            // Mock implementation - return dummy custom view
            var mockView = new CustomView
            {
                Name = name,
                Description = "Mock custom view",
                Configuration = config,
                CreatedBy = "MockUser",
                CreatedAt = DateTime.UtcNow,
                IsPublic = false,
                Tags = new List<string>()
            };
            
            return Task.FromResult(mockView);
        }

        public Task<IEnumerable<CustomView>> GetCustomViewsAsync(string userId, CancellationToken cancellationToken = default)
        {
            // Mock implementation - return empty views
            return Task.FromResult<IEnumerable<CustomView>>(new List<CustomView>());
        }

        public Task<byte[]> ExportPersonalizationAsync(string userId, CancellationToken cancellationToken = default)
        {
            // Mock implementation - return empty byte array
            return Task.FromResult(new byte[0]);
        }

        public Task ImportPersonalizationAsync(string userId, byte[] configuration, CancellationToken cancellationToken = default)
        {
            // Mock implementation - do nothing
            return Task.CompletedTask;
        }

        public Task ResetPersonalizationAsync(string userId, CancellationToken cancellationToken = default)
        {
            // Mock implementation - do nothing
            return Task.CompletedTask;
        }
    }
}
