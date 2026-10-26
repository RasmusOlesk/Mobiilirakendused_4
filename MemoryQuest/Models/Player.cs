namespace MemoryQuest.Models;

// Represents the active player and the score information tracked during play.
public class Player
{
    // Display name of the player, shown in the game UI.
    public string Name { get; private set; }

    // Current score during the current game round.
    public int Score { get; private set; }

    // Total number of rounds played in this session.
    public int GamesPlayed { get; private set; }

    // Highest score achieved by the player.
    public int BestScore { get; private set; }

    public Player(string name)
    {
        // If the name is empty, a fallback name is used to keep the UI valid.
        Name = string.IsNullOrWhiteSpace(name)
            ? "Mängija"
            : name.Trim();
    }

    // Adds score points when the value is positive.
    public void AddPoints(int points)
    {
        if (points > 0)
            Score += points;
    }

    // Starts a new round and resets only the round score.
    public void StartNewGame()
    {
        Score = 0;
    }

    // Saves totals after a round is finished and updates the best score if needed.
    public void FinishGame()
    {
        GamesPlayed++;

        if (Score > BestScore)
            BestScore = Score;
    }
}
























































