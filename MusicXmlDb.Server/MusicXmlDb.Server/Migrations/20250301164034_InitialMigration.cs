using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicXmlDb.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ScoreDocuments");

            migrationBuilder.CreateTable(
                name: "ScoreDocuments",
                schema: "ScoreDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Views = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScoreDocumentHistories",
                schema: "ScoreDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScoreDocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    MusicXmlId = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreDocumentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoreDocumentHistories_ScoreDocuments_ScoreDocumentId",
                        column: x => x.ScoreDocumentId,
                        principalSchema: "ScoreDocuments",
                        principalTable: "ScoreDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MusicXmlDocuments",
                schema: "ScoreDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScoreDocumentHistoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicXmlDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MusicXmlDocuments_ScoreDocumentHistories_ScoreDocumentHisto~",
                        column: x => x.ScoreDocumentHistoryId,
                        principalSchema: "ScoreDocuments",
                        principalTable: "ScoreDocumentHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MusicXmlDocuments_ScoreDocumentHistoryId",
                schema: "ScoreDocuments",
                table: "MusicXmlDocuments",
                column: "ScoreDocumentHistoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScoreDocumentHistories_ScoreDocumentId",
                schema: "ScoreDocuments",
                table: "ScoreDocumentHistories",
                column: "ScoreDocumentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MusicXmlDocuments",
                schema: "ScoreDocuments");

            migrationBuilder.DropTable(
                name: "ScoreDocumentHistories",
                schema: "ScoreDocuments");

            migrationBuilder.DropTable(
                name: "ScoreDocuments",
                schema: "ScoreDocuments");
        }
    }
}
