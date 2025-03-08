using MusicXmlDb.Server.Users;

namespace MusicXmlDb.Server.ScoreDocuments;

public class ScoreDocument
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = "";

    public ICollection<ScoreDocumentHistory> History { get; set; } = [];

    public string Name { get; set; } = "";
    public int Views { get; set; }
    public DateTime Created { get; set; } = DateTime.MinValue;
    public DateTime Modified { get; set; } = DateTime.MinValue;
    public bool IsPublic { get; set; }
}
