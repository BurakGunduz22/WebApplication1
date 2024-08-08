using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class Update4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "itemId",
                table: "Conversations",
                newName: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ItemId",
                table: "Conversations",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Items_ItemId",
                table: "Conversations",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Items_ItemId",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_ItemId",
                table: "Conversations");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "Conversations",
                newName: "itemId");
        }
    }
}
