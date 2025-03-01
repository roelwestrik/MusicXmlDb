namespace MusicXmlDb.Server.ScoreDocuments;

public class ScoreDocumentHistoryModel
{
    public Guid Id { get; }

    public Guid ScoreDocumentId { get; }

    public Guid MusicXmlId { get; }

    public DateTime Created { get; }


    private ScoreDocumentHistoryModel(Guid id, Guid scoreDocumentId, Guid musicXmlId, DateTime created)
    {
        Id = id;
        ScoreDocumentId = scoreDocumentId;
        MusicXmlId = musicXmlId;
        Created = created;
    }

    public static ScoreDocumentHistoryModel Create(ScoreDocumentHistory scoreDocumentHistory)
    {
        return new ScoreDocumentHistoryModel(
            scoreDocumentHistory.Id,
            scoreDocumentHistory.ScoreDocumentId,
            scoreDocumentHistory.MusicXmlId,
            scoreDocumentHistory.Created
        );
    }
}