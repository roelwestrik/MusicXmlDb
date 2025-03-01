using MusicXmlDb.Server.MusicXmlDocuments;
using MusicXmlDb.Server.Users;

namespace MusicXmlDb.Server.ScoreDocuments;

public class ScoreDocumentHistory
{
    public Guid Id { get; set; }

    public Guid ScoreDocumentId { get; set; }
    public ScoreDocument ScoreDocument { get; set; }

    public string UserId { get; set; }

    public Guid MusicXmlId { get; set; }
    public MusicXmlDocument MusicXmlDocument { get; set; }

    public DateTime Created { get; set; } = DateTime.MinValue;
}
