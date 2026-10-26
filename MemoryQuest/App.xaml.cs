using MemoryQuest.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MemoryQuest
{
    // Main application entry point for the MAUI app.
    // This class initializes the app language and creates the root shell window.
    public partial class App : Application
    {
        public App()
        {
            // Apply the saved language before the UI is created.
            AppSettingsService.ApplyLanguage();
            InitializeComponent();
        }

        // Creates the main window and mounts the Shell-based application structure.
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}






























































































