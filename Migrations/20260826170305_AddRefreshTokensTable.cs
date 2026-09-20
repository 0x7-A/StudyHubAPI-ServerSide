using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokensTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
        name: "RefreshTokens",
        columns: table => new
        {
            Id = table.Column<int>(type: "int", nullable: false)
                .Annotation("SqlServer:Identity", "1, 1"),
            TokenHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            PersonID = table.Column<int>(type: "int", nullable: false)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_RefreshTokens", x => x.Id);
            table.ForeignKey(
                name: "FK_RefreshTokens_Person_PersonID",
                column: x => x.PersonID,
                principalTable: "Person",
                principalColumn: "PersonID",
                onDelete: ReferentialAction.Cascade);
        });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_PersonID",
                table: "RefreshTokens",
                column: "PersonID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");
        }
    }
}
