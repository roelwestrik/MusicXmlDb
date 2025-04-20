namespace MusicXmlDb.Server.ScoreDocuments;

public class PostScoreDocumentHistoryReponse
{
    public Guid Id { get; set; }
    public PostScoreDocumentXmlDocumentReponse XmlDocument { get; set; }
}
