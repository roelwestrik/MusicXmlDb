namespace MusicXmlDb.Server.ScoreDocuments.Manage;

public class PostMusicXmlBody
{
    public Guid ScoreDocumentId { get; set; }
    public string XmlString { get; set; } = "";
}
