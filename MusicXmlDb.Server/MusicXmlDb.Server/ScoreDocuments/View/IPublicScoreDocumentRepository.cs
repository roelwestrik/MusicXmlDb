namespace MusicXmlDb.Server.ScoreDocuments.View;

public interface IPublicScoreDocumentRepository
{
    Task<ScoreDocument?> GetScoreDocumentWithHistoriesAsync(string? userId, Guid id);
    Task<MusicXmlDocument?> GetMusicXmlDocumentAsync(string? userId, Guid scoreDocumentId, Guid scoreDocumentHistoryId);
}