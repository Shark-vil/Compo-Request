using Microsoft.EntityFrameworkCore.Migrations;

namespace Compo_Request_Server.Migrations
{
    public partial class UpdateTeamGroupUid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeamGroups_TeamUid",
                table: "TeamGroups");

            migrationBuilder.DropColumn(
                name: "TeamUid",
                table: "TeamGroups");

            migrationBuilder.AddColumn<string>(
                name: "Uid",
                table: "TeamGroups",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Uid",
                table: "Projects",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TeamGroups_Uid",
                table: "TeamGroups",
                column: "Uid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Uid",
                table: "Projects",
                column: "Uid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeamGroups_Uid",
                table: "TeamGroups");

            migrationBuilder.DropIndex(
                name: "IX_Projects_Uid",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Uid",
                table: "TeamGroups");

            migrationBuilder.AddColumn<string>(
                name: "TeamUid",
                table: "TeamGroups",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Uid",
                table: "Projects",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_TeamGroups_TeamUid",
                table: "TeamGroups",
                column: "TeamUid",
                unique: true);
        }
    }
}
