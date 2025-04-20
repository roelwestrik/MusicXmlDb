namespace MusicXmlDb.Server.ScoreDocuments;

public class PostMusicXmlBody
{
    public Guid ScoreDocumentId { get; set; }
    public string XmlString { get; set; } = "";
}
