using MemoryQuest.Localization;
using MemoryQuest.Models;
using MemoryQuest.Services;

namespace MemoryQuest.Views;

public partial class HomePage : ContentPage
{
    private readonly Theme _currentTheme = Theme.Light;

    public HomePage()
    {
        InitializeComponent();
        _currentTheme.Apply(this);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AppSettingsService.ApplyLanguage();
        _currentTheme.Apply(this);
        UpdateTexts();
    }

    private void UpdateTexts()
    {
        Title = AppResources.Get("HomeTitle");
        TitleLabel.Text = AppResources.Get("HomeTitle");
        PlayButton.Text = AppResources.Get("PlayButton");
        SettingsButton.Text = AppResources.Get("SettingsButton");
    }

    private async void OnPlayClicked(object sender, EventArgs e)
    {
        await AudioService.Instance.PlayMenuClickAsync();
        await Shell.Current.Navigation.PushAsync(new ProfileSelectionPage());
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await AudioService.Instance.PlayMenuClickAsync();
        await Shell.Current.Navigation.PushAsync(new SettingsPage());
    }
}

