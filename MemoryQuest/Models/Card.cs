namespace MemoryQuest.Models;

// Represents one memory card in the game board.
public class Card
{
    // Unique identifier for the card instance.
    public int Id { get; }

    // The symbol shown when the card is revealed.
    public string Symbol { get; }

    // Keeps track of whether the card is visible on the board right now.
    public bool IsRevealed { get; private set; }

    // Marks whether the card pair has already been matched.
    public bool IsMatched { get; private set; }

    // The text shown in the UI when the card is hidden or visible.
    public string DisplayText =>
        IsRevealed || IsMatched ? Symbol : "?";

    public Card(int id, string symbol)
    {
        Id = id;
        Symbol = symbol;
    }

    // Shows the card if it is not already visible or solved.
    public bool Reveal()
    {
        if (IsRevealed || IsMatched)
            return false;

        IsRevealed = true;
        return true;
    }

    // Hides the card again unless it was already matched and should remain visible.
    public void Hide()
    {
        if (!IsMatched)
            IsRevealed = false;
    }

    // Locks the card as solved and keeps it visible.
    public void MarkAsMatched()
    {
        IsRevealed = true;
        IsMatched = true;
    }
}




























































