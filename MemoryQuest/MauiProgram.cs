using Microsoft.Extensions.Logging;

namespace MemoryQuest
{
    // MAUI entry point that configures the app and platform services.
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // Creates the MAUI app builder and initializes the app shell.
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    // Registers custom fonts so all pages can use the same visual style.
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            // Enables debug logging during development.
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}




































