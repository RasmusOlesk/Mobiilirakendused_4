using MemoryQuest.Models;
using Microsoft.Data.Sqlite;

namespace MemoryQuest.Services;

// Handles all SQLite database access for saved player profiles and their game statistics.
public class SqlProfileService
{
    // Local SQLite database path. It is resolved from the app settings.
    private readonly string _connectionString;

    public SqlProfileService(string? connectionString = null)
    {
        // If no custom connection string is supplied, use the shared app database path.
        _connectionString = string.IsNullOrWhiteSpace(connectionString)
            ? AppSettingsService.ConnectionString
            : connectionString;
    }

    // Ensures the database file exists and the required tables are created.
    public async Task EnsureDatabaseAsync()
    {
        var dbPath = _connectionString;
        var directory = Path.GetDirectoryName(dbPath);

        // Create the folder if it does not exist yet.
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var connection = new SqliteConnection($"Data Source={dbPath}");
        await connection.OpenAsync();

        const string sql = @"
            CREATE TABLE IF NOT EXISTS Profiles (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE,
                CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                TotalGames INTEGER NOT NULL DEFAULT 0,
                BestScore INTEGER NOT NULL DEFAULT 0,
                TotalScore INTEGER NOT NULL DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS ProfileStatistics (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ProfileId INTEGER NOT NULL,
                Score INTEGER NOT NULL,
                Moves INTEGER NOT NULL,
                Matches INTEGER NOT NULL,
                PlayedAt TEXT NOT NULL DEFAULT (datetime('now')),
                FOREIGN KEY(ProfileId) REFERENCES Profiles(Id) ON DELETE CASCADE
            );";

        await using var command = new SqliteCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    // Reads all saved profiles from the database and returns them ordered by creation date.
    public async Task<List<GameProfile>> GetProfilesAsync()
    {
        const string sql = @"SELECT Id, Name, CreatedAt, TotalGames, BestScore, TotalScore
                             FROM Profiles
                             ORDER BY CreatedAt ASC;";

        var profiles = new List<GameProfile>();

        await using var connection = new SqliteConnection($"Data Source={_connectionString}");
        await connection.OpenAsync();
        await using var command = new SqliteCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            profiles.Add(new GameProfile
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                CreatedAt = DateTime.Parse(reader.GetString(2), null, System.Globalization.DateTimeStyles.RoundtripKind),
                TotalGames = reader.GetInt32(3),
                BestScore = reader.GetInt32(4),
                TotalScore = reader.GetInt32(5)
            });
        }

        return profiles;
    }

    // Finds a single profile by its database id.
    public async Task<GameProfile?> GetProfileByIdAsync(int id)
    {
        const string sql = @"SELECT Id, Name, CreatedAt, TotalGames, BestScore, TotalScore
                             FROM Profiles
                             WHERE Id = @Id;";

        await using var connection = new SqliteConnection($"Data Source={_connectionString}");
        await connection.OpenAsync();
        await using var command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new GameProfile
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            CreatedAt = DateTime.Parse(reader.GetString(2), null, System.Globalization.DateTimeStyles.RoundtripKind),
            TotalGames = reader.GetInt32(3),
            BestScore = reader.GetInt32(4),
            TotalScore = reader.GetInt32(5)
        };
    }

    // Checks whether a profile with the same name already exists.
    public async Task<bool> ProfileExistsAsync(string name)
    {
        const string sql = "SELECT COUNT(1) FROM Profiles WHERE Name = @Name;";

        await using var connection = new SqliteConnection($"Data Source={_connectionString}");
        await connection.OpenAsync();
        await using var command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@Name", name.Trim());

        var result = Convert.ToInt32(await command.ExecuteScalarAsync());
        return result > 0;
    }

    // Creates a new profile and stores the initial default statistics values.
    public async Task<int> CreateProfileAsync(string name)
    {
        var trimmedName = name.Trim();
        if (string.IsNullOrWhiteSpace(trimmedName))
        {
            throw new InvalidOperationException("Profile name cannot be empty.");
        }

        await EnsureDatabaseAsync();

        var profiles = await GetProfilesAsync();
        if (profiles.Count >= 5)
        {
            throw new InvalidOperationException("Maximum of 5 profiles reached.");
        }

        if (await ProfileExistsAsync(trimmedName))
        {
            throw new InvalidOperationException("Profile already exists.");
        }

        const string sql = @"INSERT INTO Profiles (Name, CreatedAt, TotalGames, BestScore, TotalScore)
                             VALUES (@Name, datetime('now'), 0, 0, 0); SELECT last_insert_rowid();";

        await using var connection = new SqliteConnection($"Data Source={_connectionString}");
        await connection.OpenAsync();
        await using var command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@Name", trimmedName);

        var id = Convert.ToInt32(await command.ExecuteScalarAsync());
        return id;
    }

    // Deletes the selected profile and removes it from the Profiles table.
    public async Task DeleteProfileAsync(int profileId)
    {
        if (profileId <= 0)
        {
            return;
        }

        const string sql = "DELETE FROM Profiles WHERE Id = @ProfileId;";

        await using var connection = new SqliteConnection($"Data Source={_connectionString}");
        await connection.OpenAsync();
        await using var command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@ProfileId", profileId);
        await command.ExecuteNonQueryAsync();
    }

    // Saves one completed game result to the profile statistics and updates aggregate totals.
    public async Task SaveGameResultAsync(int profileId, int score, int moves, int matches)
    {
        if (profileId <= 0)
        {
            return;
        }

        await EnsureDatabaseAsync();

        const string sql = @"
            UPDATE Profiles
            SET TotalGames = TotalGames + 1,
                BestScore = CASE WHEN BestScore < @Score THEN @Score ELSE BestScore END,
                TotalScore = TotalScore + @Score
            WHERE Id = @ProfileId;

            INSERT INTO ProfileStatistics (ProfileId, Score, Moves, Matches, PlayedAt)
            VALUES (@ProfileId, @Score, @Moves, @Matches, datetime('now'));";

        await using var connection = new SqliteConnection($"Data Source={_connectionString}");
        await connection.OpenAsync();
        await using var command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@ProfileId", profileId);
        command.Parameters.AddWithValue("@Score", score);
        command.Parameters.AddWithValue("@Moves", moves);
        command.Parameters.AddWithValue("@Matches", matches);

        await command.ExecuteNonQueryAsync();
    }
}


































































































































































































































