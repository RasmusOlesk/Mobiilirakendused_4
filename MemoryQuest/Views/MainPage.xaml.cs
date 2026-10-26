using MemoryQuest.Games;
using MemoryQuest.Localization;
using MemoryQuest.Models;
using MemoryQuest.Services;

namespace MemoryQuest.Views;

[QueryProperty(nameof(ProfileId), "profileId")]
public partial class MainPage : ContentPage
{
    private const int DefaultProfileId = -1;

    private Player _player;
    private Game _game;
    private readonly SqlProfileService _profileService = new();
    private GameProfile? _selectedProfile;

    private Theme _currentTheme = Theme.Light;
    private bool _isShowingResult;

    public int ProfileId { get; set; } = DefaultProfileId;

    public MainPage()
    {
        InitializeComponent();

        _player = new Player(AppResources.Get("DefaultPlayerName"));
        _game = new Game(_player);

        _currentTheme.Apply(this);
        UpdateTexts();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        AppSettingsService.ApplyLanguage();
        _currentTheme.Apply(this);
        await AudioService.Instance.StartGameSongAsync();

        if (_selectedProfile is null || _selectedProfile.Id != ProfileId)
        {
            await LoadSelectedProfileAsync();
        }
        else
        {
            UpdateTexts();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        AudioService.Instance.StopGameSong();
    }

    private async Task LoadSelectedProfileAsync()
    {
        if (ProfileId > 0)
        {
            _selectedProfile = await _profileService.GetProfileByIdAsync(ProfileId);
        }

        if (_selectedProfile is null)
        {
            _selectedProfile = new GameProfile { Id = DefaultProfileId, Name = AppResources.Get("DefaultPlayerName") };
        }

        _player = new Player(_selectedProfile.Name);
        _game = new Game(_player);
        UpdateTexts();
        StartNewGame();
    }

    private void UpdateTexts()
    {
        Title = AppResources.Get("GameTitle");
        GameTitleLabel.Text = AppResources.Get("GameTitle");
        WelcomeLabel.Text = AppResources.Get("WelcomeMessage");
        ScoreTitleLabel.Text = AppResources.Get("PointsLabel");
        MovesTitleLabel.Text = AppResources.Get("MovesLabel");
        MatchesTitleLabel.Text = AppResources.Get("MatchesLabel");
        NewGameButton.Text = AppResources.Get("NewGameButton");
        BackButton.Text = AppResources.Get("BackButton");
        ThemeLabel.Text = AppResources.Get("ChooseThemeLabel");
        LightThemeButton.Text = AppResources.Get("LightTheme");
        DarkThemeButton.Text = AppResources.Get("DarkTheme");
        ColorfulThemeButton.Text = AppResources.Get("ColorfulTheme");
        ProfileLabel.Text = _player.Name;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await AudioService.Instance.PlayMenuClickAsync();
        await Shell.Current.Navigation.PopAsync();
    }

    private void StartNewGame()
    {
        _isShowingResult = false;

        _game.Start();
        CardsView.ItemsSource = _game.Cards.ToList();
        UpdateStatistics();
    }

    [Obsolete]
    private async void OnCardClicked(
        object sender, EventArgs e)
    {
        if (sender is not Button button ||
            button.BindingContext is not Card card)
            return;

        await AudioService.Instance.PlayGameClickAsync();

        bool finished = await _game.FlipCardAsync(card);

        CardsView.ItemsSource = _game.Cards.ToList();
        UpdateStatistics();

        await button.ScaleToAsync(1.05, 70);
        await button.ScaleToAsync(1.0, 70);

        if (finished && !_isShowingResult)
        {
            _isShowingResult = true;
            await AudioService.Instance.PlayVictoryAsync();

            if (_selectedProfile is not null && _selectedProfile.Id > 0)
            {
                try
                {
                    await _profileService.SaveGameResultAsync(_selectedProfile.Id, _player.Score, _game.Moves, _game.Matches);
                }
                catch (Exception ex)
                {
                    await DisplayAlert(AppResources.Get("WinnerTitle"), $"Failed to save statistics: {ex.Message}", "OK");
                }
            }

            await DisplayAlert(
                AppResources.Get("WinnerTitle"),
                $"{AppResources.Get("WinnerMessage")}\n\n" +
                $"{AppResources.Get("PointsLabel")}: {_player.Score}\n" +
                $"{AppResources.Get("MovesLabel")}: {_game.Moves}\n" +
                $"{AppResources.Get("BestScoreLabel")}: {_player.BestScore}",
                AppResources.Get("PlayAgainButton"));

            StartNewGame();
        }
    }

    private void UpdateStatistics()
    {
        ScoreLabel.Text = _player.Score.ToString();
        MovesLabel.Text = _game.Moves.ToString();
        MatchesLabel.Text = $"{_game.Matches} / 6";
    }

    private async void OnNewGameClicked(
        object sender, EventArgs e)
    {
        await AudioService.Instance.PlayMenuClickAsync();
        StartNewGame();
    }

    private async void OnLightThemeClicked(
        object sender, EventArgs e)
    {
        await AudioService.Instance.PlayMenuClickAsync();
        ChangeTheme(Theme.Light);
    }

    private async void OnDarkThemeClicked(
        object sender, EventArgs e)
    {
        await AudioService.Instance.PlayMenuClickAsync();
        ChangeTheme(Theme.Dark);
    }

    private async void OnColorfulThemeClicked(
        object sender, EventArgs e)
    {
        await AudioService.Instance.PlayMenuClickAsync();
        ChangeTheme(Theme.Colorful);
    }

    private void ChangeTheme(Theme theme)
    {
        _currentTheme = theme;
        _currentTheme.Apply(this);
    }
}

