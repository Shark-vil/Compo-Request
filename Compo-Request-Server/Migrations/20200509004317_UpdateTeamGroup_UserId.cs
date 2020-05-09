using Microsoft.EntityFrameworkCore.Migrations;

namespace Compo_Request_Server.Migrations
{
    public partial class UpdateTeamGroup_UserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TeamGroups",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TeamGroups_UserId",
                table: "TeamGroups",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamGroups_Users_UserId",
                table: "TeamGroups",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamGroups_Users_UserId",
                table: "TeamGroups");

            migrationBuilder.DropIndex(
                name: "IX_TeamGroups_UserId",
                table: "TeamGroups");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TeamGroups");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "TeamGroups",
                type: "int",
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
    }
}
