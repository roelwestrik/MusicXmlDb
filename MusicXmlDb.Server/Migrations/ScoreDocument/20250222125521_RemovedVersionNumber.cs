using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicXmlDb.Server.Migrations.ScoreDocument
{
    /// <inheritdoc />
    public partial class RemovedVersionNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                schema: "ScoreDocuments",
                table: "ScoreDocumentHistories");

            migrationBuilder.CreateTable(
                name: "MusicXmlDocument",
                schema: "ScoreDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicXmlDocument", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MusicXmlDocument",
                schema: "ScoreDocuments");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                schema: "ScoreDocuments",
                table: "ScoreDocumentHistories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
