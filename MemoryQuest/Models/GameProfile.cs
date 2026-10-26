namespace MemoryQuest.Models;

// Stores information about a saved player profile and the statistics linked to it.
public class GameProfile
{
    // Database identifier for the profile.
    public int Id { get; set; }

    // The user-visible profile name.
    public string Name { get; set; } = string.Empty;

    // The date and time the profile was created.
    public DateTime CreatedAt { get; set; }

    // How many games this profile has played.
    public int TotalGames { get; set; }

    // Best score recorded for this profile.
    public int BestScore { get; set; }

    // Total accumulated score for this profile.
    public int TotalScore { get; set; }
}












































