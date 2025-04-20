namespace MusicXmlDb.Server.ScoreDocuments;

public class PostScoreDocumentBody
{
    public string DocumentName { get; set; } = "";
    public string XmlString { get; set; } = "";
    public bool IsPublic { get; set; } = false;
}
