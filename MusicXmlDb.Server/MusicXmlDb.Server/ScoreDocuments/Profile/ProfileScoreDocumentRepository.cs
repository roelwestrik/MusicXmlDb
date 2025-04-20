using MusicXmlDb.Server.MusicXmlDocuments;
using Npgsql;

namespace MusicXmlDb.Server.ScoreDocuments.Profile;

public class ProfileScoreDocumentRepository : IProfileScoreDocumentRepository
{
    private readonly IConfiguration _configuration;

    public ProfileScoreDocumentRepository(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public async Task<List<ScoreDocument>> GetPublicScoreDocumentsForProfileAsync(string userId)
    {
        var connectionString = _configuration.GetConnectionString("Database");

        const string query = """
                                 SELECT 
                                     d.id AS document_id,
                                     d.uid as user_id,
                                     h.id AS history_id, 
                                     d.*, h.*
                                 FROM score_documents.score_document d
                                 LEFT JOIN score_documents.score_document_history h 
                                     ON d.Id = h.score_document_id
                                 WHERE d.isPublic = True AND d.uid = @user_id;
                             """;

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

        var scoreDocuments = documents.Values.ToList();

        return scoreDocuments;
    }
}