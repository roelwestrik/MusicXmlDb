using MusicXmlDb.Server.MusicXmlDocuments;
using Npgsql;

namespace MusicXmlDb.Server.ScoreDocuments;

public class ScoreDocumentRepository
{
    private readonly IConfiguration _configuration;

    public ScoreDocumentRepository(IConfiguration _configuration)
    {
        this._configuration = _configuration;
    }


    public async Task InsertScoreDocumentAsync()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        // SQL Queries
        var insertScoreDocumentQuery = @"
            INSERT INTO ScoreDocuments (Id, UserId, Name, Views, Created, Modified, IsPublic)
            VALUES (@Id, @UserId, @Name, @Views, @Created, @Modified, @IsPublic);
        ";

        var insertScoreDocumentHistoryQuery = @"
            INSERT INTO ScoreDocumentHistories (Id, ScoreDocumentId, UserId, Created)
            VALUES (@Id, @ScoreDocumentId, @UserId, @Created);
        ";

        var insertMusicXmlDocumentQuery = @"
            INSERT INTO MusicXmlDocuments (Id, ScoreDocumentHistoryId, Content)
            VALUES (@Id, @ScoreDocumentHistoryId, @Content);
        ";

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            // Insert ScoreDocument
            var scoreDocumentId = Guid.NewGuid();
            await using (var command = new NpgsqlCommand(insertScoreDocumentQuery, connection))
            {
                command.Parameters.AddWithValue("Id", scoreDocumentId);
                command.Parameters.AddWithValue("UserId", "user123");
                command.Parameters.AddWithValue("Name", "Test Document");
                command.Parameters.AddWithValue("Views", 0);
                command.Parameters.AddWithValue("Created", DateTime.UtcNow);
                command.Parameters.AddWithValue("Modified", DateTime.UtcNow);
                command.Parameters.AddWithValue("IsPublic", true);

                await command.ExecuteNonQueryAsync();
            }

            // Insert ScoreDocumentHistory
            var scoreDocumentHistoryId = Guid.NewGuid();
            await using (var command = new NpgsqlCommand(insertScoreDocumentHistoryQuery, connection))
            {
                command.Parameters.AddWithValue("Id", scoreDocumentHistoryId);
                command.Parameters.AddWithValue("ScoreDocumentId", scoreDocumentId);
                command.Parameters.AddWithValue("UserId", "user123");
                command.Parameters.AddWithValue("Created", DateTime.UtcNow);

                await command.ExecuteNonQueryAsync();
            }

            // Insert MusicXmlDocument
            var musicXmlDocumentId = Guid.NewGuid();
            await using (var command = new NpgsqlCommand(insertMusicXmlDocumentQuery, connection))
            {
                command.Parameters.AddWithValue("Id", musicXmlDocumentId);
                command.Parameters.AddWithValue("ScoreDocumentHistoryId", scoreDocumentHistoryId);
                command.Parameters.AddWithValue("Content", "<music><note>C</note><note>D</note><note>E</note></music>");

                await command.ExecuteNonQueryAsync();
            }

            // Commit transaction
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            // Rollback transaction in case of an error
            await transaction.RollbackAsync();
            throw; // Re-throw exception for proper handling/logging
        }
    }

    public async Task AddScoreDocumentHistoryAsync(Guid scoreDocumentId, ScoreDocumentHistory history)
    {
        var connectionString = "Host=your_host;Port=5432;Database=your_database;Username=your_user;Password=your_password";

        var insertScoreDocumentHistoryQuery = @"
            INSERT INTO ScoreDocumentHistories (Id, ScoreDocumentId, UserId, Created)
            VALUES (@Id, @ScoreDocumentId, @UserId, @Created);
        ";

        var insertMusicXmlDocumentQuery = @"
            INSERT INTO MusicXmlDocuments (Id, ScoreDocumentHistoryId, Content)
            VALUES (@Id, @ScoreDocumentHistoryId, @Content);
        ";

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            // Insert the new ScoreDocumentHistory
            await using (var command = new NpgsqlCommand(insertScoreDocumentHistoryQuery, connection))
            {
                command.Parameters.AddWithValue("Id", history.Id);
                command.Parameters.AddWithValue("ScoreDocumentId", scoreDocumentId); // Link to existing ScoreDocument
                command.Parameters.AddWithValue("UserId", history.UserId);
                command.Parameters.AddWithValue("Created", history.Created);

                await command.ExecuteNonQueryAsync();
            }

            // Insert the new MusicXmlDocument, if provided
            if (history.MusicXmlDocument != null)
            {
                await using (var command = new NpgsqlCommand(insertMusicXmlDocumentQuery, connection))
                {
                    command.Parameters.AddWithValue("Id", history.MusicXmlDocument.Id);
                    command.Parameters.AddWithValue("ScoreDocumentHistoryId", history.Id); // Link to new history
                    command.Parameters.AddWithValue("Content", history.MusicXmlDocument.Content);

                    await command.ExecuteNonQueryAsync();
                }
            }

            // Commit the transaction
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            // Roll back the transaction in case of an error
            await transaction.RollbackAsync();
            throw; // Re-throw exception for logging or further handling
        }
    }

    public async Task<List<ScoreDocument>> GetScoreDocumentsWithHistoriesAsync()
    {
        var scoreDocuments = new List<ScoreDocument>();
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        var query = @"
            SELECT d.*, h.*
            FROM ScoreDocuments d
            LEFT JOIN ScoreDocumentHistories h ON d.Id = h.ScoreDocumentId;
        ";

        await using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();
            var documents = new Dictionary<Guid, ScoreDocument>();

            while (await reader.ReadAsync())
            {
                var scoreDocumentId = reader.GetGuid(reader.GetOrdinal("Id"));

                // If the ScoreDocument is not already in the dictionary, add it
                if (!documents.TryGetValue(scoreDocumentId, out var scoreDocument))
                {
                    scoreDocument = new ScoreDocument
                    {
                        Id = scoreDocumentId,
                        UserId = reader.GetString(reader.GetOrdinal("UserId")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Views = reader.GetInt32(reader.GetOrdinal("Views")),
                        Created = reader.GetDateTime(reader.GetOrdinal("Created")),
                        Modified = reader.GetDateTime(reader.GetOrdinal("Modified")),
                        IsPublic = reader.GetBoolean(reader.GetOrdinal("IsPublic")),
                        History = new List<ScoreDocumentHistory>()
                    };

                    documents[scoreDocumentId] = scoreDocument;
                }

                // Map ScoreDocumentHistory if available
                if (!reader.IsDBNull(reader.GetOrdinal("ScoreDocumentId")))
                {
                    var history = new ScoreDocumentHistory
                    {
                        Id = reader.GetGuid(reader.GetOrdinal("Id")),
                        ScoreDocumentId = reader.GetGuid(reader.GetOrdinal("ScoreDocumentId")),
                        UserId = reader.GetString(reader.GetOrdinal("UserId")),
                        Created = reader.GetDateTime(reader.GetOrdinal("Created"))
                    };

                    scoreDocument.History.Add(history);
                }
            }

            scoreDocuments = [.. documents.Values];
        }

        return scoreDocuments;
    }

    public async Task<MusicXmlDocument?> GetMusicXmlDocumentAsync(Guid scoreDocumentId, Guid scoreDocumentHistoryId)
    {
        var connectionString = "Host=your_host;Port=5432;Database=your_database;Username=your_user;Password=your_password";

        var query = @"
            SELECT m.Id, m.ScoreDocumentHistoryId, m.Content
            FROM MusicXmlDocuments m
            JOIN ScoreDocumentHistories h ON m.ScoreDocumentHistoryId = h.Id
            WHERE h.ScoreDocumentId = @ScoreDocumentId
              AND h.Id = @ScoreDocumentHistoryId;
        ";

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("ScoreDocumentId", scoreDocumentId);
        command.Parameters.AddWithValue("ScoreDocumentHistoryId", scoreDocumentHistoryId);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new MusicXmlDocument
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                ScoreDocumentHistoryId = reader.GetGuid(reader.GetOrdinal("ScoreDocumentHistoryId")),
                Content = reader.GetString(reader.GetOrdinal("Content"))
            };
        }

        return null; // Return null if no document is found
    }

    public async Task<MusicXmlDocument?> GetLatestMusicXmlDocumentAsync(Guid scoreDocumentId)
    {
        var connectionString = "Host=your_host;Port=5432;Database=your_database;Username=your_user;Password=your_password";

        var query = @"
            SELECT m.Id, m.ScoreDocumentHistoryId, m.Content
            FROM MusicXmlDocuments m
            JOIN ScoreDocumentHistories h ON m.ScoreDocumentHistoryId = h.Id
            WHERE h.ScoreDocumentId = @ScoreDocumentId
            ORDER BY h.Created DESC
            LIMIT 1;
        ";

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("ScoreDocumentId", scoreDocumentId);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new MusicXmlDocument
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                ScoreDocumentHistoryId = reader.GetGuid(reader.GetOrdinal("ScoreDocumentHistoryId")),
                Content = reader.GetString(reader.GetOrdinal("Content"))
            };
        }

        return null; // Return null if no document is found
    }

    public async Task<bool> DeleteScoreDocumentHistoryAsync(Guid scoreDocumentHistoryId)
    {
        var connectionString = "Host=your_host;Port=5432;Database=your_database;Username=your_user;Password=your_password";

        var checkRemainingHistoriesQuery = @"
            SELECT COUNT(*) 
            FROM ScoreDocumentHistories 
            WHERE ScoreDocumentId = (
                SELECT ScoreDocumentId 
                FROM ScoreDocumentHistories 
                WHERE Id = @ScoreDocumentHistoryId
            );
        ";

        var deleteMusicXmlDocumentQuery = @"
            DELETE FROM MusicXmlDocuments 
            WHERE ScoreDocumentHistoryId = @ScoreDocumentHistoryId;
        ";

        var deleteScoreDocumentHistoryQuery = @"
            DELETE FROM ScoreDocumentHistories 
            WHERE Id = @ScoreDocumentHistoryId;
        ";

        await using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            await using (var transaction = await connection.BeginTransactionAsync())
            {
                try
                {
                    // Check if at least two versions exist
                    int remainingCount;
                    await using (var command = new NpgsqlCommand(checkRemainingHistoriesQuery, connection))
                    {
                        command.Parameters.AddWithValue("ScoreDocumentHistoryId", scoreDocumentHistoryId);

                        remainingCount = (int)(await command.ExecuteScalarAsync() ?? 0);
                    }

                    if (remainingCount <= 1)
                    {
                        // Abort if this is the only remaining version
                        return false;
                    }

                    // Delete associated MusicXmlDocument
                    await using (var command = new NpgsqlCommand(deleteMusicXmlDocumentQuery, connection))
                    {
                        command.Parameters.AddWithValue("ScoreDocumentHistoryId", scoreDocumentHistoryId);
                        await command.ExecuteNonQueryAsync();
                    }

                    // Delete ScoreDocumentHistory
                    await using (var command = new NpgsqlCommand(deleteScoreDocumentHistoryQuery, connection))
                    {
                        command.Parameters.AddWithValue("ScoreDocumentHistoryId", scoreDocumentHistoryId);
                        await command.ExecuteNonQueryAsync();
                    }

                    // Commit the transaction
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    // Roll back the transaction in case of an error
                    await transaction.RollbackAsync();
                    throw; // Re-throw exception for logging or further handling
                }
            }
        }
    }

    public async Task<bool> DeleteScoreDocumentAsync(Guid scoreDocumentId)
    {
        var connectionString = "Host=your_host;Port=5432;Database=your_database;Username=your_user;Password=your_password";

        var deleteScoreDocumentQuery = @"
            DELETE FROM ScoreDocuments 
            WHERE Id = @ScoreDocumentId;
        ";

        await using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            try
            {
                await using (var command = new NpgsqlCommand(deleteScoreDocumentQuery, connection))
                {
                    command.Parameters.AddWithValue("ScoreDocumentId", scoreDocumentId);
                    var rowsAffected = await command.ExecuteNonQueryAsync();

                    // Return true if the row was successfully deleted
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw;
            }
        }
    }

}
