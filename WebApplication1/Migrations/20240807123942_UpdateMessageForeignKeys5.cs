using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMessageForeignKeys5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Participants",
                table: "Conversations",
                newName: "SenderId");

            migrationBuilder.AddColumn<string>(
                name: "RecieverId",
                table: "Conversations",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecieverId",
                table: "Conversations");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Conversations",
                newName: "Participants");
        }
    }
}
