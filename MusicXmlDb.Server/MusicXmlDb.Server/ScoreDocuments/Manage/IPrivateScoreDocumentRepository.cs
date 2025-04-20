namespace MusicXmlDb.Server.ScoreDocuments.Manage;

public interface IPrivateScoreDocumentRepository
{
    Task<PostScoreDocumentResponse> InsertScoreDocumentAsync(string userId, PostScoreDocumentBody body);
    Task<PostScoreDocumentHistoryReponse> AddScoreDocumentHistoryAsync(string userId, PostMusicXmlBody musicXml);
    Task<List<ScoreDocument>> GetScoreDocumentsWithHistoriesAsync(string userId);
    Task<ScoreDocument?> GetScoreDocumentWithHistoriesAsync(string userId, Guid id);
    Task<MusicXmlDocument?> GetMusicXmlDocumentAsync(string userId, Guid scoreDocumentId, Guid scoreDocumentHistoryId);
    Task UpdateScoreDocumentAsync(string userId, PutScoreDocumentBody scoreDocument);
    Task<bool> DeleteScoreDocumentAsync(string userId, Guid scoreDocumentId);
    Task<bool> DeleteScoreDocumentHistoryAsync(string userId, Guid scoreDocumentId, Guid scoreDocumentHistoryId);
}