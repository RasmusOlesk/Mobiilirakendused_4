using System.Globalization;
using Microsoft.Maui.Storage;

namespace MemoryQuest.Services;

// Stores application-wide settings such as language and the SQLite database path.
public static class AppSettingsService
{
    // Key used to persist the selected language in device preferences.
    private const string LanguageKey = "MemoryQuest.Language";

    // Event raised when the language changes so the UI can refresh its text.
    public static event EventHandler? LanguageChanged;

    // Returns the current language and also persists a new one when changed.
    public static string CurrentLanguage
    {
        get => Preferences.Default.Get(LanguageKey, "et");
        set
        {
            var normalized = string.IsNullOrWhiteSpace(value) ? "et" : value.Trim();
            var current = Preferences.Default.Get(LanguageKey, "et");

            if (!string.Equals(current, normalized, StringComparison.OrdinalIgnoreCase))
            {
                Preferences.Default.Set(LanguageKey, normalized);
            }

            ApplyLanguage();
            LanguageChanged?.Invoke(null, EventArgs.Empty);
        }
    }

    // Helper method used when a language change should be updated via the same logic.
    public static void SetLanguage(string value)
    {
        CurrentLanguage = value;
    }

    // Forces the current culture to be reapplied and notifies listeners.
    public static void NotifyLanguageChanged()
    {
        ApplyLanguage();
        LanguageChanged?.Invoke(null, EventArgs.Empty);
    }

    // Local SQLite database path used by the app for saved profiles and game statistics.
    public static string ConnectionString { get; set; } = Path.Combine(FileSystem.AppDataDirectory, "MemoryQuest.db");

    // Applies the selected language to the current and default thread cultures.
    public static void ApplyLanguage()
    {
        var culture = CurrentLanguage switch
        {
            "en" or "en-US" => new CultureInfo("en-US"),
            _ => new CultureInfo("et-EE")
        };

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}



















































































