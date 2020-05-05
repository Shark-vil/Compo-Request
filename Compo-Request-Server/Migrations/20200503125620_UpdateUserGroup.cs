using Microsoft.EntityFrameworkCore.Migrations;

namespace Compo_Request_Server.Migrations
{
    public partial class UpdateUserGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "TeamGroups",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TeamGroups_OwnerId",
                table: "TeamGroups",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamGroups_Users_OwnerId",
                table: "TeamGroups",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamGroups_Users_OwnerId",
                table: "TeamGroups");

            migrationBuilder.DropIndex(
                name: "IX_TeamGroups_OwnerId",
                table: "TeamGroups");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "TeamGroups");
        }
    }
}
