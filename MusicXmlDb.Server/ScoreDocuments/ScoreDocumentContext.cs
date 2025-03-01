using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MusicXmlDb.Server.MusicXmlDocuments;
using System.Reflection;

namespace MusicXmlDb.Server.ScoreDocuments;

public class ScoreDocumentContext : DbContext
{
    public DbSet<ScoreDocument> ScoreDocuments { get; set; }
    public DbSet<ScoreDocumentHistory> ScoreDocumentHistories { get; set; }
    public DbSet<MusicXmlDocument> MusicXmlDocuments { get; set; }

    public ScoreDocumentContext(DbContextOptions<ScoreDocumentContext> dbContextOptions) : base(dbContextOptions)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("ScoreDocuments");

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
