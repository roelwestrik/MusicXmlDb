using Microsoft.EntityFrameworkCore;
using MusicXmlDb.Server.Users;
using System.Reflection;

namespace MusicXmlDb.Server.MusicXmlDocuments;

public class MusicXmlDocumentContext : DbContext
{
    public DbSet<MusicXmlDocument> MusicXmlDocuments { get; set; }

    public string DbPath { get; }

    public MusicXmlDocumentContext()
    {
        var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        DbPath = Path.Join(folder, "score-documents.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("MusicXml");
    }
}
