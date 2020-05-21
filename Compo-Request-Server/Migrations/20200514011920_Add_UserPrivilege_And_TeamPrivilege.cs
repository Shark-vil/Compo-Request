using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Compo_Request_Server.Migrations
{
    public partial class Add_UserPrivilege_And_TeamPrivilege : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TeamProjects",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TeamPrivileges",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Privilege = table.Column<string>(nullable: false),
                    TeamGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamPrivileges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamPrivileges_TeamGroups_TeamGroupId",
                        column: x => x.TeamGroupId,
                        principalTable: "TeamGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPrivileges",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Privilege = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPrivileges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPrivileges_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamProjects_UserId",
                table: "TeamProjects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPrivileges_TeamGroupId",
                table: "TeamPrivileges",
                column: "TeamGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPrivileges_UserId",
                table: "UserPrivileges",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamProjects_Users_UserId",
                table: "TeamProjects",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamProjects_Users_UserId",
                table: "TeamProjects");

            migrationBuilder.DropTable(
                name: "TeamPrivileges");

            migrationBuilder.DropTable(
                name: "UserPrivileges");

            migrationBuilder.DropIndex(
                name: "IX_TeamProjects_UserId",
                table: "TeamProjects");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TeamProjects");
        }
    }
}
