namespace MusicXmlDb.Server.ScoreDocuments.View;

public interface IPublicScoreDocumentRepository
{
    Task<ScoreDocument?> GetScoreDocumentWithHistoriesAsync(Guid id);
    Task<MusicXmlDocument?> GetMusicXmlDocumentAsync(Guid scoreDocumentId, Guid scoreDocumentHistoryId);
}