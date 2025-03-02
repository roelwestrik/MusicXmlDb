
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MusicXmlDb.Server.MusicXmlDocuments;
using MusicXmlDb.Server.ScoreDocuments;

namespace MusicXmlDb.Server;

public class Program
{
    public async static Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("Database");
        builder.Services.AddDbContext<ScoreDocumentContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(connectionString);
        });

        builder.Services.AddSingleton<IMusicXmlValidator, MusicXmlValidator>();

        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(o =>
        {
            o.CustomSchemaIds(id => id.FullName!.Replace("+", "-"));

            o.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri(builder.Configuration["Keycloak:AuthorizationUrl"]!),
                        Scopes = new Dictionary<string, string>()
                        {
                            { "openid", "openid" },
                            { "profile", "profile" }
                        }
                    }
                }
            });

            var securityRequirement = new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Id = "Keycloak",
                            Type = ReferenceType.SecurityScheme
                        },
                        In = ParameterLocation.Header,
                        Name = "Bearer",
                        Scheme = "Bearer"
                    },
                    []
                }
            };

            o.AddSecurityRequirement(securityRequirement);
        });

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.Audience = builder.Configuration["Authentication:Audience"];
                options.MetadataAddress = builder.Configuration["Authentication:MetadataAddress"]!;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = builder.Configuration["Authentication:ValidIssuer"]
                };
            });

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

                settings.EnableTryItOutByDefault();
            });

            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ScoreDocumentContext>();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseRouting();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
