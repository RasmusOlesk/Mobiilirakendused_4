using MemoryQuest.Localization;
using MemoryQuest.Models;
using MemoryQuest.Services;

namespace MemoryQuest.Views;

public partial class SettingsPage : ContentPage
{
    private readonly Theme _currentTheme = Theme.Light;
    private bool _isUpdatingLanguage;

    public SettingsPage()
    {
        InitializeComponent();
        _currentTheme.Apply(this);
        InitializeLanguagePicker();
        UpdateTexts();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AppSettingsService.ApplyLanguage();
        _currentTheme.Apply(this);
        UpdateTexts();
        InitializeLanguagePicker();
    }

    private void InitializeLanguagePicker()
    {
        _isUpdatingLanguage = true;

        var desiredIndex = AppSettingsService.CurrentLanguage.StartsWith("en", StringComparison.OrdinalIgnoreCase) ? 1 : 0;

        if (LanguagePicker.Items.Count != 2)
        {
            LanguagePicker.Items.Clear();
            LanguagePicker.Items.Add(AppResources.Get("EstonianLanguage"));
            LanguagePicker.Items.Add(AppResources.Get("EnglishLanguage"));
        }

        if (LanguagePicker.SelectedIndex != desiredIndex)
        {
            LanguagePicker.SelectedIndex = desiredIndex;
        }

        _isUpdatingLanguage = false;
    }

    private void UpdateTexts()
    {
        Title = AppResources.Get("SettingsTitle");
        TitleLabel.Text = AppResources.Get("SettingsTitle");
        LanguageLabel.Text = AppResources.Get("LanguageLabel");
        BackButton.Text = AppResources.Get("BackButton");
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await AudioService.Instance.PlayMenuClickAsync();
        await Shell.Current.Navigation.PopAsync();
    }

    private async void OnLanguagePickerChanged(object sender, EventArgs e)
    {
        if (_isUpdatingLanguage)
        {
            return;
        }

        var selectedLanguage = LanguagePicker.SelectedIndex == 1 ? "en" : "et";
        if (AppSettingsService.CurrentLanguage.Equals(selectedLanguage, StringComparison.OrdinalIgnoreCase))
        {
            UpdateTexts();
            return;
        }

        AppSettingsService.CurrentLanguage = selectedLanguage;
        UpdateTexts();
    }

    private async void OnLanguageChanged(object sender, EventArgs e)
    {
        OnLanguagePickerChanged(sender, e);
    }
}

