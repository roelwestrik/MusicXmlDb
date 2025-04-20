using MusicXmlDb.Server.MusicXmlDocuments;
using Npgsql;

namespace MusicXmlDb.Server.ScoreDocuments.View;

public class PublicScoreDocumentRepository : IPublicScoreDocumentRepository
{
    private readonly IConfiguration _configuration;

    public PublicScoreDocumentRepository(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public async Task<ScoreDocument?> GetScoreDocumentWithHistoriesAsync(string? userId, Guid id)
    {
        var connectionString = _configuration.GetConnectionString("Database");

        var query = userId is not null ? """
            SELECT 
                d.id AS document_id,
                h.id AS history_id, 
                d.*, h.*
            FROM score_documents.score_document d
            LEFT JOIN score_documents.score_document_history h 
                ON d.Id = h.score_document_id
            WHERE (d.is_public OR d.user_id = @user_id) AND d.id = @score_document_id;
        """ : """
            SELECT 
                d.id AS document_id,
                h.id AS history_id, 
                d.*, h.*
            FROM score_documents.score_document d
            LEFT JOIN score_documents.score_document_history h 
                ON d.Id = h.score_document_id
            WHERE d.is_public AND d.id = @score_document_id;
        """;

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("score_document_id", id);
        if (userId is not null)
        {
            command.Parameters.AddWithValue("user_id", userId);
        }

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

    public async Task<MusicXmlDocument?> GetMusicXmlDocumentAsync(string? userId, Guid scoreDocumentId, Guid scoreDocumentHistoryId)
    {
        var connectionString = _configuration.GetConnectionString("Database");

        var query = userId is not null ? """
            SELECT
                m.Id, 
                m.score_document_history_id, 
                m.content
            FROM score_documents.music_xml_document m
            JOIN score_documents.score_document_history h 
                ON m.score_document_history_id = h.Id
            JOIN score_documents.score_document d 
                ON h.score_document_id = d.Id
            WHERE 
                (d.is_public = TRUE OR d.user_id = @user_id)
                AND h.id = @score_document_history_id;
        """ : """
            SELECT
                m.Id, 
                m.score_document_history_id, 
                m.content
            FROM score_documents.music_xml_document m
            JOIN score_documents.score_document_history h 
                ON m.score_document_history_id = h.Id
            JOIN score_documents.score_document d 
                ON h.score_document_id = d.Id
            WHERE 
                d.is_public = TRUE
                AND h.id = @score_document_history_id;
        """;

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("score_document_id", scoreDocumentId);
        command.Parameters.AddWithValue("score_document_history_id", scoreDocumentHistoryId);
        if (userId is not null)
        {
            command.Parameters.AddWithValue("user_id", userId);
        }

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
}