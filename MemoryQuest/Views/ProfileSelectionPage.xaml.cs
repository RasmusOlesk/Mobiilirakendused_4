using MemoryQuest.Localization;
using MemoryQuest.Models;
using MemoryQuest.Services;

namespace MemoryQuest.Views;

public partial class ProfileSelectionPage : ContentPage
{
    private readonly Theme _currentTheme = Theme.Light;
    private readonly SqlProfileService _profileService = new();
    private GameProfile? _selectedProfile;

    public ProfileSelectionPage()
    {
        InitializeComponent();
        _currentTheme.Apply(this);
        LoadProfilesAsync();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AppSettingsService.ApplyLanguage();
        _currentTheme.Apply(this);
        UpdateTexts();
        LoadProfilesAsync();
        _ = FocusProfileNameEntryAsync();
    }

    private async Task FocusProfileNameEntryAsync()
    {
        await Task.Delay(200);
        ProfileNameEntry.Focus();
    }

    private async void LoadProfilesAsync()
    {
        try
        {
            await _profileService.EnsureDatabaseAsync();
            var profiles = await _profileService.GetProfilesAsync();
            ProfilesCollectionView.ItemsSource = profiles;
            _selectedProfile = null;
        }
        catch (Exception)
        {
            ProfilesCollectionView.ItemsSource = new List<GameProfile>();
        }
    }

    private void UpdateTexts()
    {
        Title = AppResources.Get("ProfileSelectionTitle");
        TitleLabel.Text = AppResources.Get("ProfileSelectionTitle");
        ProfileNameEntry.Placeholder = AppResources.Get("ProfileNamePlaceholder");
        CreateProfileButton.Text = AppResources.Get("CreateProfileButton");
        BackButton.Text = AppResources.Get("BackButton");
        ContinueButton.Text = AppResources.Get("SelectProfileButton");
    }

    private async void OnDeleteProfileClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int profileId)
        {
            await AudioService.Instance.PlayMenuClickAsync();
            var profile = await _profileService.GetProfileByIdAsync(profileId);
            if (profile is null)
            {
                return;
            }

            bool confirm = await DisplayAlert(
                AppResources.Get("DeleteProfileTitle"),
                string.Format(AppResources.Get("DeleteProfileMessage"), profile.Name),
                AppResources.Get("Yes"),
                AppResources.Get("No"));

            if (!confirm)
            {
                return;
            }

            await _profileService.DeleteProfileAsync(profileId);
            await LoadProfilesAsyncAndRefresh();
            _selectedProfile = null;
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await AudioService.Instance.PlayMenuClickAsync();
        await Shell.Current.Navigation.PopAsync();
    }

    private async void OnCreateProfileClicked(object sender, EventArgs e)
    {
        await AudioService.Instance.PlayMenuClickAsync();
        var name = ProfileNameEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            await DisplayAlert(AppResources.Get("CreateProfileTitle"), AppResources.Get("ProfileNamePlaceholder"), "OK");
            return;
        }

        try
        {
            var profiles = await _profileService.GetProfilesAsync();
            if (profiles.Count >= 5)
            {
                await DisplayAlert(AppResources.Get("CreateProfileTitle"), AppResources.Get("MaxProfilesReached"), "OK");
                return;
            }

            if (profiles.Any(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                await DisplayAlert(AppResources.Get("CreateProfileTitle"), AppResources.Get("ProfileAlreadyExists"), "OK");
                return;
            }

            var profileId = await _profileService.CreateProfileAsync(name);
            ProfileNameEntry.Text = string.Empty;
            await LoadProfilesAsyncAndRefresh();
            _selectedProfile = await _profileService.GetProfileByIdAsync(profileId);
            _ = FocusProfileNameEntryAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert(AppResources.Get("CreateProfileTitle"), ex.Message, "OK");
        }
    }

    private async Task LoadProfilesAsyncAndRefresh()
    {
        await _profileService.EnsureDatabaseAsync();
        var profiles = await _profileService.GetProfilesAsync();
        ProfilesCollectionView.ItemsSource = profiles;
    }

    private void OnProfileSelected(object sender, SelectionChangedEventArgs e)
    {
        _selectedProfile = e.CurrentSelection.FirstOrDefault() as GameProfile;
    }

    private async void OnContinueClicked(object sender, EventArgs e)
    {
        await AudioService.Instance.PlayMenuClickAsync();

        if (_selectedProfile is null)
        {
            await DisplayAlert(AppResources.Get("ProfileSelectionTitle"), AppResources.Get("ChooseProfileFirst"), "OK");
            return;
        }

        await Shell.Current.Navigation.PushAsync(new MainPage { ProfileId = _selectedProfile.Id });
    }
}
