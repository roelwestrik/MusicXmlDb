using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MusicXmlDb.Server.MusicXmlDocuments;
using System.Reflection;

namespace MusicXmlDb.Server.ScoreDocuments;

public class ScoreDocumentContext : DbContext
{
    public DbSet<ScoreDocument> ScoreDocument { get; set; }
    public DbSet<ScoreDocumentHistory> ScoreDocumentHistory { get; set; }
    public DbSet<MusicXmlDocument> MusicXmlDocument { get; set; }

    public ScoreDocumentContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("score_documents");

        modelBuilder.Entity<ScoreDocumentHistory>()
            .HasOne(e => e.ScoreDocument)
            .WithMany(e => e.History)
            .HasForeignKey(e => e.ScoreDocumentId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(); ;

        modelBuilder.Entity<ScoreDocumentHistory>()
            .HasOne(e => e.MusicXmlDocument)
            .WithOne(e => e.ScoreDocumentHistory)
            .HasForeignKey<MusicXmlDocument>(e => e.ScoreDocumentHistoryId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
