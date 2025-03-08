using MusicXmlDb.Server.ScoreDocuments;

namespace MusicXmlDb.Server.MusicXmlDocuments;

public class MusicXmlDocument
{
    public Guid Id { get; set; }

    public Guid ScoreDocumentHistoryId { get; set; }
    public ScoreDocumentHistory ScoreDocumentHistory { get; set; }

    public string Content { get; set; } = "";
}
