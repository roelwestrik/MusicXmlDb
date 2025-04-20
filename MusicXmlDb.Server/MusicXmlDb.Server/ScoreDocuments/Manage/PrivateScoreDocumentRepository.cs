using MusicXmlDb.Server.MusicXmlDocuments;
using Npgsql;

namespace MusicXmlDb.Server.ScoreDocuments.Manage;

public class PrivateScoreDocumentRepository : IPrivateScoreDocumentRepository
{
    private readonly IConfiguration _configuration;



    public PrivateScoreDocumentRepository(IConfiguration configuration)
    {
        this._configuration = configuration;
    }



    public async Task<PostScoreDocumentResponse> InsertScoreDocumentAsync(string userId, PostScoreDocumentBody body)
    {
        var connectionString = _configuration.GetConnectionString("Database");

        // SQL Queries
        var insertScoreDocumentQuery = """
           INSERT INTO score_documents.score_document (id, user_id, name, views, created, modified, is_public)
           VALUES (@id, @user_id, @name, @views, @created, @modified, @is_public);
       """;

        var insertScoreDocumentHistoryQuery = """
            INSERT INTO score_documents.score_document_history (id, score_document_id, user_id, created)
            VALUES (@id, @score_document_id, @user_id, @created);
        """;

        var insertMusicXmlDocumentQuery = """
            INSERT INTO score_documents.music_xml_document (id, score_document_history_id, content)
            VALUES (@id, @score_document_history_id, XMLPARSE(DOCUMENT @content));
        """;

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            // Insert ScoreDocument
            var scoreDocumentId = Guid.NewGuid();
            await using (var command = new NpgsqlCommand(insertScoreDocumentQuery, connection))
            {
                command.Parameters.AddWithValue("id", scoreDocumentId);
                command.Parameters.AddWithValue("user_id", userId);
                command.Parameters.AddWithValue("name", body.DocumentName);
                command.Parameters.AddWithValue("views", 0);
                command.Parameters.AddWithValue("created", DateTime.UtcNow);
                command.Parameters.AddWithValue("modified", DateTime.UtcNow);
                command.Parameters.AddWithValue("is_public", body.IsPublic);

                await command.ExecuteNonQueryAsync();
            }

            // Insert ScoreDocumentHistory
            var scoreDocumentHistoryId = Guid.NewGuid();
            await using (var command = new NpgsqlCommand(insertScoreDocumentHistoryQuery, connection))
            {
                command.Parameters.AddWithValue("id", scoreDocumentHistoryId);
                command.Parameters.AddWithValue("score_document_id", scoreDocumentId);
                command.Parameters.AddWithValue("user_id", userId);
                command.Parameters.AddWithValue("created", DateTime.UtcNow);

                await command.ExecuteNonQueryAsync();
            }

            // Insert MusicXmlDocument
            var musicXmlDocumentId = Guid.NewGuid();
            await using (var command = new NpgsqlCommand(insertMusicXmlDocumentQuery, connection))
            {
                command.Parameters.AddWithValue("id", musicXmlDocumentId);
                command.Parameters.AddWithValue("score_document_history_id", scoreDocumentHistoryId);
                command.Parameters.AddWithValue("content", body.XmlString);

                await command.ExecuteNonQueryAsync();
            }

            // Commit transaction
            await transaction.CommitAsync();

            return new PostScoreDocumentResponse()
            {
                Id = scoreDocumentId,
                History = new PostScoreDocumentHistoryReponse()
                {
                    Id = scoreDocumentHistoryId,
                    XmlDocument = new PostScoreDocumentXmlDocumentReponse()
                    {
                        Id = musicXmlDocumentId,
                    }
                }
            };
        }
        catch (Exception ex)
        {
            // Rollback transaction in case of an error
            await transaction.RollbackAsync();
            throw; // Re-throw exception for proper handling/logging
        }
    }

    public async Task<PostScoreDocumentHistoryReponse> AddScoreDocumentHistoryAsync(string userId, PostMusicXmlBody musicXml)
    {
        var connectionString = _configuration.GetConnectionString("Database");

        var insertScoreDocumentHistoryQuery = @"
            INSERT INTO score_documents.score_document_history (id, score_document_id, user_id, created)
            VALUES (@id, @score_document_id, @user_id, @created);
        ";

        var insertMusicXmlDocumentQuery = @"
            INSERT INTO score_documents.music_xml_document (id, score_document_history_id, content)
            VALUES (@id, @score_document_history_id, XMLPARSE(DOCUMENT @content));
        ";

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            // Insert the new ScoreDocumentHistory
            var historyId = Guid.NewGuid();
            await using (var command = new NpgsqlCommand(insertScoreDocumentHistoryQuery, connection))
            {
                command.Parameters.AddWithValue("id", historyId);
                command.Parameters.AddWithValue("score_document_id", musicXml.ScoreDocumentId); // Link to existing ScoreDocument
                command.Parameters.AddWithValue("user_id", userId);
                command.Parameters.AddWithValue("created", DateTime.UtcNow);

                await command.ExecuteNonQueryAsync();
            }

            // Insert the new MusicXmlDocument, if provided
            var musicXmlId = Guid.NewGuid();
            await using (var command = new NpgsqlCommand(insertMusicXmlDocumentQuery, connection))
            {
                command.Parameters.AddWithValue("id", musicXmlId);
                command.Parameters.AddWithValue("score_document_history_id", historyId); // Link to new history
                command.Parameters.AddWithValue("content", musicXml.XmlString);

                await command.ExecuteNonQueryAsync();
            }

            // Commit the transaction
            await transaction.CommitAsync();

            return new PostScoreDocumentHistoryReponse()
            {
                Id = historyId,
                XmlDocument = new PostScoreDocumentXmlDocumentReponse()
                {
                    Id = musicXmlId
                }
            };
        }
        catch (Exception ex)
        {
            // Roll back the transaction in case of an error
            await transaction.RollbackAsync();
            throw; // Re-throw exception for logging or further handling
        }
    }



    public async Task<List<ScoreDocument>> GetScoreDocumentsWithHistoriesAsync(string userId)
    {
        var scoreDocuments = new List<ScoreDocument>();
        var connectionString = _configuration.GetConnectionString("Database");

        var query = @"
            SELECT d.id AS document_id, h.id AS history_id, d.*, h.*
            FROM score_documents.score_document d
            LEFT JOIN score_documents.score_document_history h ON d.Id = h.score_document_id
            WHERE d.user_id = @user_id;
        ";

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("user_id", userId);

        await using var reader = await command.ExecuteReaderAsync();
        var documents = new Dictionary<Guid, ScoreDocument>();

        while (await reader.ReadAsync())
        {
            // Use the correct ID from the `score_document` table
            var scoreDocumentId = reader.GetGuid(reader.GetOrdinal("document_id"));

            // If the ScoreDocument is not already in the dictionary, add it
            if (!documents.TryGetValue(scoreDocumentId, out var scoreDocument))
            {
                scoreDocument = new ScoreDocument
                {
                    Id = scoreDocumentId,
                    UserId = reader.GetString(reader.GetOrdinal("user_id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Views = reader.GetInt32(reader.GetOrdinal("views")),
                    Created = reader.GetDateTime(reader.GetOrdinal("created")),
                    Modified = reader.GetDateTime(reader.GetOrdinal("modified")),
                    IsPublic = reader.GetBoolean(reader.GetOrdinal("is_public")),
                    History = new List<ScoreDocumentHistory>()
                };

                documents[scoreDocumentId] = scoreDocument;
            }

            // Map ScoreDocumentHistory if available
            if (!reader.IsDBNull(reader.GetOrdinal("history_id")))
            {
                var history = new ScoreDocumentHistory
                {
                    Id = reader.GetGuid(reader.GetOrdinal("history_id")),
                    ScoreDocumentId = reader.GetGuid(reader.GetOrdinal("document_id")),
                    UserId = reader.GetString(reader.GetOrdinal("user_id")),
                    Created = reader.GetDateTime(reader.GetOrdinal("created"))
                };

                scoreDocument.History.Add(history);
            }
        }

        scoreDocuments = documents.Values.ToList();

        return scoreDocuments;
    }

    public async Task<ScoreDocument?> GetScoreDocumentWithHistoriesAsync(string userId, Guid id)
    {
        var connectionString = _configuration.GetConnectionString("Database");

        var query = @"
            SELECT d.id AS document_id, h.id AS history_id, d.*, h.*
            FROM score_documents.score_document d
            LEFT JOIN score_documents.score_document_history h ON d.Id = h.score_document_id
            WHERE d.user_id = @user_id AND d.id = @score_document_id;
        ";

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("user_id", userId);
        command.Parameters.AddWithValue("score_document_id", id);

        await using var reader = await command.ExecuteReaderAsync();

        ScoreDocument? scoreDocument = null;

        while (await reader.ReadAsync())
        {
            var scoreDocumentId = reader.GetGuid(reader.GetOrdinal("id"));

            // If the ScoreDocument is not already initialized, create it
            scoreDocument ??= new ScoreDocument
            {
                Id = scoreDocumentId,
                UserId = reader.GetString(reader.GetOrdinal("user_id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Views = reader.GetInt32(reader.GetOrdinal("views")),
                Created = reader.GetDateTime(reader.GetOrdinal("created")),
                Modified = reader.GetDateTime(reader.GetOrdinal("modified")),
                IsPublic = reader.GetBoolean(reader.GetOrdinal("is_public")),
                History = new List<ScoreDocumentHistory>()
            };

            // Map ScoreDocumentHistory if available
            if (!reader.IsDBNull(reader.GetOrdinal("history_id")))
            {
                var history = new ScoreDocumentHistory
                {
                    Id = reader.GetGuid(reader.GetOrdinal("history_id")),
                    ScoreDocumentId = reader.GetGuid(reader.GetOrdinal("score_document_id")),
                    UserId = reader.GetString(reader.GetOrdinal("user_id")),
                    Created = reader.GetDateTime(reader.GetOrdinal("created"))
                };

                scoreDocument.History.Add(history);
            }
        }

        return scoreDocument;
    }

    public async Task<MusicXmlDocument?> GetMusicXmlDocumentAsync(string userId, Guid scoreDocumentId, Guid scoreDocumentHistoryId)
    {
        var connectionString = _configuration.GetConnectionString("Database");

        var query = @"
            SELECT m.Id, m.score_document_history_id, m.content
            FROM score_documents.music_xml_document m
            JOIN score_documents.score_document_history h ON m.score_document_history_id = h.Id
            WHERE h.score_document_id = @score_document_id
            AND h.id = @score_document_history_id
            AND h.user_id = @user_id;
        ";

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("score_document_id", scoreDocumentId);
        command.Parameters.AddWithValue("score_document_history_id", scoreDocumentHistoryId);
        command.Parameters.AddWithValue("user_id", userId);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new MusicXmlDocument
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Content = reader.GetString(reader.GetOrdinal("content"))
            };
        }

        return null; // Return null if no document is found
    }



    public async Task UpdateScoreDocumentAsync(string userId, PutScoreDocumentBody scoreDocument)
    {
        var connectionString = _configuration.GetConnectionString("Database");

        var query = @"
            UPDATE score_documents.score_document
            SET name = @name,
                modified = @modified,
                is_public = @is_public
            WHERE user_id = @user_id AND id = @id;
        ";

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var command = new NpgsqlCommand(query, connection);

            // Set parameters
            command.Parameters.AddWithValue("user_id", userId);
            command.Parameters.AddWithValue("id", scoreDocument.Id);
            command.Parameters.AddWithValue("name", scoreDocument.Name);
            command.Parameters.AddWithValue("is_public", scoreDocument.IsPublic);
            command.Parameters.AddWithValue("modified", DateTime.UtcNow);

            // Execute the update
            await command.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }



    public async Task<bool> DeleteScoreDocumentAsync(string userId, Guid scoreDocumentId)
    {
        var connectionString = _configuration.GetConnectionString("Database");

        var deleteScoreDocumentQuery = @"
            DELETE FROM score_documents.score_document 
            WHERE Id = @score_document_id AND user_id = @user_id;
        ";

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var command = new NpgsqlCommand(deleteScoreDocumentQuery, connection);
            command.Parameters.AddWithValue("score_document_id", scoreDocumentId);
            command.Parameters.AddWithValue("user_id", userId);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            await transaction.CommitAsync();

            // Return true if the row was successfully deleted
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteScoreDocumentHistoryAsync(string userId, Guid scoreDocumentId, Guid scoreDocumentHistoryId)
    {
        var connectionString = _configuration.GetConnectionString("Database");

        var checkRemainingHistoriesQuery = @"
            SELECT COUNT(*) 
            FROM score_documents.score_document_history 
            WHERE score_document_id = (
                SELECT score_document_id 
                FROM  score_documents.score_document_history  
                WHERE Id = @score_document_history_id
            );
        ";

        var deleteScoreDocumentHistoryQuery = @"
            DELETE FROM score_documents.score_document_history 
            WHERE id = @score_document_history_id AND score_document_id = @score_document_id AND user_id = @user_id;
        ";

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        // Check if at least two versions exist
        await using (var command = new NpgsqlCommand(checkRemainingHistoriesQuery, connection))
        {
            command.Parameters.AddWithValue("score_document_history_id", scoreDocumentHistoryId);

            var remainingCount = (long)(await command.ExecuteScalarAsync() ?? 0);
            if (remainingCount <= 1)
            {
                // Abort if this is the only remaining version
                return false;
            }
        }

        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            // Delete ScoreDocumentHistory
            await using (var command = new NpgsqlCommand(deleteScoreDocumentHistoryQuery, connection))
            {
                command.Parameters.AddWithValue("score_document_history_id", scoreDocumentHistoryId);
                command.Parameters.AddWithValue("score_document_id", scoreDocumentId);
                command.Parameters.AddWithValue("user_id", userId);
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
