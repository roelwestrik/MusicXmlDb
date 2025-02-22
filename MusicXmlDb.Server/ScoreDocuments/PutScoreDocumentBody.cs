namespace MusicXmlDb.Server.ScoreDocuments;

public class PutScoreDocumentBody
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsPublic { get; set; }
}