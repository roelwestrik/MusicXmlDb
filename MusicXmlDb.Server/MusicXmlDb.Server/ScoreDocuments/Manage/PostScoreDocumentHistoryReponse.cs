namespace MusicXmlDb.Server.ScoreDocuments.Manage;

public class PostScoreDocumentHistoryReponse
{
    public Guid Id { get; set; }
    public PostScoreDocumentXmlDocumentReponse XmlDocument { get; set; }
}
