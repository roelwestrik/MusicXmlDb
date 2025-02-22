using Microsoft.EntityFrameworkCore;
using System.Reflection;
using MusicXmlDb.Server.MusicXmlDocuments;

namespace MusicXmlDb.Server.ScoreDocuments;

public class ScoreDocumentContext : DbContext
{
    public DbSet<ScoreDocument> ScoreDocuments { get; set; }
    public DbSet<ScoreDocumentHistory> ScoreDocumentHistories { get; set; }

    public string DbPath { get; }

    public ScoreDocumentContext()
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

        modelBuilder.HasDefaultSchema("ScoreDocuments");

        modelBuilder.Entity<ScoreDocumentHistory>()
            .HasOne(e => e.ScoreDocument)
            .WithMany(e => e.History)
            .HasForeignKey(e => e.ScoreDocumentId);
    }

public DbSet<MusicXmlDocument> MusicXmlDocument { get; set; } = default!;
}
