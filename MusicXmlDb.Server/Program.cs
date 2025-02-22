
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicXmlDb.Server.MusicXmlDocuments;
using MusicXmlDb.Server.ScoreDocuments;
using MusicXmlDb.Server.Users;
using NuGet.Configuration;

namespace MusicXmlDb.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<UserContext>();
        builder.Services.AddDbContext<ScoreDocumentContext>();
        builder.Services.AddDbContext<MusicXmlDocumentContext>();

        // Add authentication
        builder.Services.AddAuthorization();

        // Add authorization
        builder.Services
            .AddDefaultIdentity<ApplicationUser>()
            .AddEntityFrameworkStores<UserContext>()
            .AddApiEndpoints();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;
        });

        builder.Services.ConfigureApplicationCookie(options =>
        {
            // Cookie settings
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

            options.LoginPath = "/Identity/Account/Login";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.SlidingExpiration = true;
        });

        builder.Services.AddSingleton<IMusicXmlValidator, MusicXmlValidator>();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(settings =>
            {
                settings.ConfigObject.AdditionalItems["syntaxHighlight"] = new Dictionary<string, object>
                {
                    ["activated"] = false
                };
            });
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseRouting();

        app.MapGroup("/api/Users").MapIdentityApi<ApplicationUser>();
        app.MapPost("/api/Users/logout", async Task<IResult> ([FromServices] IServiceProvider sp, [FromBody] object? empty) =>
        {
            var signInManager = sp.GetRequiredService<SignInManager<ApplicationUser>>();

            if (empty == null)
            {
                return Results.Unauthorized();
            }

            await signInManager.SignOutAsync();
            return Results.Ok();
        })
        .RequireAuthorization();

        app.UseAuthorization();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}
