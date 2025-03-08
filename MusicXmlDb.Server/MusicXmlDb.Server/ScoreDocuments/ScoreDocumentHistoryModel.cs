namespace MusicXmlDb.Server.ScoreDocuments;

public class ScoreDocumentHistoryModel
{
    public Guid Id { get; }

    public DateTime Created { get; }


    private ScoreDocumentHistoryModel(Guid id, DateTime created)
    {
        Id = id;
        Created = created;
    }

    public static ScoreDocumentHistoryModel Create(ScoreDocumentHistory scoreDocumentHistory)
    {
        return new ScoreDocumentHistoryModel(
            scoreDocumentHistory.Id,
            scoreDocumentHistory.Created
        );
    }
}