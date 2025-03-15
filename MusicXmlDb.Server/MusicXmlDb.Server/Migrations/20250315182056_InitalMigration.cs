using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicXmlDb.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitalMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "score_documents");

            migrationBuilder.CreateTable(
                name: "score_document",
                schema: "score_documents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    views = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_public = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_score_document", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "score_document_history",
                schema: "score_documents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    score_document_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_score_document_history", x => x.id);
                    table.ForeignKey(
                        name: "fk_score_document_history_score_document_score_document_id",
                        column: x => x.score_document_id,
                        principalSchema: "score_documents",
                        principalTable: "score_document",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "music_xml_document",
                schema: "score_documents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    score_document_history_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_music_xml_document", x => x.id);
                    table.ForeignKey(
                        name: "fk_music_xml_document_score_document_history_score_document_hi",
                        column: x => x.score_document_history_id,
                        principalSchema: "score_documents",
                        principalTable: "score_document_history",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_music_xml_document_score_document_history_id",
                schema: "score_documents",
                table: "music_xml_document",
                column: "score_document_history_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_score_document_history_score_document_id",
                schema: "score_documents",
                table: "score_document_history",
                column: "score_document_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "music_xml_document",
                schema: "score_documents");

            migrationBuilder.DropTable(
                name: "score_document_history",
                schema: "score_documents");

            migrationBuilder.DropTable(
                name: "score_document",
                schema: "score_documents");
        }
    }
}
