using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicXmlDb.Server.Migrations.ScoreDocument
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Views = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Modified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false)
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
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ScoreDocumentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    MusicXmlId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                name: "ScoreDocumentHistories",
                schema: "ScoreDocuments");

            migrationBuilder.DropTable(
                name: "ScoreDocuments",
                schema: "ScoreDocuments");
        }
    }
}
