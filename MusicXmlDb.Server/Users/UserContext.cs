using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Emit;

namespace MusicXmlDb.Server.Users;

public class UserContext : IdentityDbContext<ApplicationUser>
{
    public string DbPath { get; }

    public UserContext()
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

        modelBuilder.HasDefaultSchema("Users");
    }
}
