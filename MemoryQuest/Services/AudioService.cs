using Plugin.Maui.Audio;

namespace MemoryQuest.Services;

public sealed class AudioService
{
    private static readonly Lazy<AudioService> InstanceLazy = new(() => new AudioService());

    private readonly IAudioManager _audioManager;
    private readonly Dictionary<string, IAudioPlayer> _players = new(StringComparer.OrdinalIgnoreCase);
    private IAudioPlayer? _backgroundSongPlayer;

    public static AudioService Instance => InstanceLazy.Value;

    private AudioService()
    {
        _audioManager = AudioManager.Current;
    }

    public async Task PlayMenuClickAsync()
    {
        await PlayAssetAsync("Click_1.mp3");
    }

    public async Task PlayGameClickAsync()
    {
        await PlayAssetAsync("Click_2.mp3");
    }

    public async Task PlayVictoryAsync()
    {
        await PlayAssetAsync("Victory.mp3");
    }

    public async Task StartGameSongAsync()
    {
        if (_backgroundSongPlayer is not null && _backgroundSongPlayer.IsPlaying)
        {
            return;
        }

        _backgroundSongPlayer = await GetOrCreatePlayerAsync("Song.mp3");
        _backgroundSongPlayer.Volume = 0.55;
        _backgroundSongPlayer.Play();
    }

    public void StopGameSong()
    {
        _backgroundSongPlayer?.Stop();
    }

    private async Task PlayAssetAsync(string assetPath)
    {
        var player = await GetOrCreatePlayerAsync(assetPath);
        player.Stop();
        player.Play();
    }

    private async Task<IAudioPlayer> GetOrCreatePlayerAsync(string assetPath)
    {
        if (_players.TryGetValue(assetPath, out var existingPlayer) && existingPlayer is not null)
        {
            return existingPlayer;
        }

        var stream = await FileSystem.OpenAppPackageFileAsync(assetPath);
        var player = _audioManager.CreatePlayer(stream);
        _players[assetPath] = player;
        return player;
    }
}
