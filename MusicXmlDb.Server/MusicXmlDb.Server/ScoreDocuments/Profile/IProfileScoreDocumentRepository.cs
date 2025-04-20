namespace MusicXmlDb.Server.ScoreDocuments.Profile;

public interface IProfileScoreDocumentRepository
{
    Task<List<ScoreDocument>> GetPublicScoreDocumentsForProfileAsync(string userId);
}