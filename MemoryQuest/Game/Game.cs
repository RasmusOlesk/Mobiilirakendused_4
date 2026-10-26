using MemoryQuest.Models;

namespace MemoryQuest.Games;

// Main game logic for the memory match challenge.
public class Game
{
    private readonly Random _random = new();
    private Card? _firstCard;
    private bool _isProcessing;

    public Player Player { get; }
    public List<Card> Cards { get; private set; } = [];

    public int Moves { get; private set; }
    public int Matches { get; private set; }
    public bool IsFinished => Matches == Cards.Count / 2;

    public Game(Player player)
    {
        Player = player;
    }

    // Starts a fresh memory game and shuffles all card pairs.
    public void Start()
    {
        Player.StartNewGame();

        Moves = 0;
        Matches = 0;
        _firstCard = null;
        _isProcessing = false;

        string[] symbols =
        [
            "🍎", "🚀", "🐱", "🫵", "⚽", "🎵"
        ];

        Cards = symbols
            .Concat(symbols)
            .OrderBy(_ => _random.Next())
            .Select((symbol, index) => new Card(index, symbol))
            .ToList();
    }

    // Handles a player's click on a card, compares it with the previous card and updates game state.
    public async Task<bool> FlipCardAsync(Card card)
    {
        if (_isProcessing || IsFinished ||
            card.IsMatched || card.IsRevealed)
            return false;

        card.Reveal();

        if (_firstCard is null)
        {
            _firstCard = card;
            return false;
        }

        Moves++;

        Card first = _firstCard;
        _firstCard = null;

        if (first.Symbol == card.Symbol)
        {
            first.MarkAsMatched();
            card.MarkAsMatched();

            Matches++;
            Player.AddPoints(10);

            if (IsFinished)
                Player.FinishGame();

            return IsFinished;
        }

        _isProcessing = true;

        await Task.Delay(800);

        first.Hide();
        card.Hide();

        _isProcessing = false;
        return false;
    }
}












































