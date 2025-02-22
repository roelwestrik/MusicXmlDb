using MusicXmlDb.Server.Users;

namespace MusicXmlDb.Server.ScoreDocuments;

public class ScoreDocumentModel
{
    public Guid Id { get; }
    public IEnumerable<ScoreDocumentHistoryModel> History { get; }
    public string Name { get; }
    public string User { get; }
    public int Views { get; }
    public DateTime Created { get; }
    public DateTime Modified { get; }
    public bool IsPublic { get; }

    private ScoreDocumentModel(Guid id, IEnumerable<ScoreDocumentHistoryModel> history, string name, string user, int views, DateTime modified, DateTime created, bool isPublic)
    {
        Id = id;
        History = history;
        User = user;
        Name = name;
        Views = views;
        Modified = modified;
        IsPublic = isPublic;
        Created = created;
    }

    public static ScoreDocumentModel Create(ScoreDocument scoreDocument, ApplicationUser applicationUser)
    {
        return new ScoreDocumentModel(
            id: scoreDocument.Id,
            history: scoreDocument.History.Select(ScoreDocumentHistoryModel.Create),
            user: applicationUser.UserName ?? applicationUser.Email ?? "",
            name: scoreDocument.Name,
            views: scoreDocument.Views,
            created: scoreDocument.Created,
            modified: scoreDocument.Modified,
            isPublic: scoreDocument.IsPublic
        ); 
    }
}
