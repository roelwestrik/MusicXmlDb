namespace MusicXmlDb.Server.ScoreDocuments.Manage;

public class PostScoreDocumentBody
{
    public string DocumentName { get; set; } = "";
    public string XmlString { get; set; } = "";
    public bool IsPublic { get; set; } = false;
}
