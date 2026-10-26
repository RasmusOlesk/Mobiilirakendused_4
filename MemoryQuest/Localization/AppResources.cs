using System.Globalization;
using System.Resources;

namespace MemoryQuest.Localization;

public static class AppResources
{
    private static readonly ResourceManager ResourceManager = new("MemoryQuest.Localization.AppResources", typeof(AppResources).Assembly);

    public static string Get(string key)
    {
        return ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    }
}
