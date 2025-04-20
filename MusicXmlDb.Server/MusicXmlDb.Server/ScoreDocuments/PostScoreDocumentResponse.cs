namespace MusicXmlDb.Server.ScoreDocuments;

public class PostScoreDocumentResponse
{
    public Guid Id { get; set; }
    public string Documentname { get; set; } = "";
    public string XmlString { get; set; } = "";
    public bool IsPublic { get; set; } = false;
    public PostScoreDocumentHistoryReponse History { get; set; }
}
