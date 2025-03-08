using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicXmlDb.Server.Migrations
{
    /// <inheritdoc />
    public partial class DeleteUnnecessaryId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MusicXmlId",
                schema: "ScoreDocuments",
                table: "ScoreDocumentHistories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MusicXmlId",
                schema: "ScoreDocuments",
                table: "ScoreDocumentHistories",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
