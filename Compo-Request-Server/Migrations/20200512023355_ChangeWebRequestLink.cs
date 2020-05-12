using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Compo_Request_Server.Migrations
{
    public partial class ChangeWebRequestLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamUser_TeamGroups_TeamGroupId",
                table: "TeamUser");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamUser_Users_UserId",
                table: "TeamUser");

            migrationBuilder.DropForeignKey(
                name: "FK_WebRequestDirs_WebRequestItem_WebRequestId",
                table: "WebRequestDirs");

            migrationBuilder.DropForeignKey(
                name: "FK_WebRequestItem_Projects_ProjectId",
                table: "WebRequestItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WebRequestItem",
                table: "WebRequestItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamUser",
                table: "TeamUser");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "WebRequestItem");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "WebRequestItem");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "WebRequestItem");

            migrationBuilder.RenameTable(
                name: "WebRequestItem",
                newName: "WebRequestItems");

            migrationBuilder.RenameTable(
                name: "TeamUser",
                newName: "TeamUsers");

            migrationBuilder.RenameIndex(
                name: "IX_WebRequestItem_ProjectId",
                table: "WebRequestItems",
                newName: "IX_WebRequestItems_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_TeamUser_UserId",
                table: "TeamUsers",
                newName: "IX_TeamUsers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TeamUser_TeamGroupId",
                table: "TeamUsers",
                newName: "IX_TeamUsers_TeamGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WebRequestItems",
                table: "WebRequestItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamUsers",
                table: "TeamUsers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "WebRequestParamsItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    WebRequestItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebRequestParamsItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebRequestParamsItems_WebRequestItems_WebRequestItemId",
                        column: x => x.WebRequestItemId,
                        principalTable: "WebRequestItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WebRequestParamsItems_WebRequestItemId",
                table: "WebRequestParamsItems",
                column: "WebRequestItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamUsers_TeamGroups_TeamGroupId",
                table: "TeamUsers",
                column: "TeamGroupId",
                principalTable: "TeamGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamUsers_Users_UserId",
                table: "TeamUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WebRequestDirs_WebRequestItems_WebRequestId",
                table: "WebRequestDirs",
                column: "WebRequestId",
                principalTable: "WebRequestItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WebRequestItems_Projects_ProjectId",
                table: "WebRequestItems",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamUsers_TeamGroups_TeamGroupId",
                table: "TeamUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamUsers_Users_UserId",
                table: "TeamUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_WebRequestDirs_WebRequestItems_WebRequestId",
                table: "WebRequestDirs");

            migrationBuilder.DropForeignKey(
                name: "FK_WebRequestItems_Projects_ProjectId",
                table: "WebRequestItems");

            migrationBuilder.DropTable(
                name: "WebRequestParamsItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WebRequestItems",
                table: "WebRequestItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamUsers",
                table: "TeamUsers");

            migrationBuilder.RenameTable(
                name: "WebRequestItems",
                newName: "WebRequestItem");

            migrationBuilder.RenameTable(
                name: "TeamUsers",
                newName: "TeamUser");

            migrationBuilder.RenameIndex(
                name: "IX_WebRequestItems_ProjectId",
                table: "WebRequestItem",
                newName: "IX_WebRequestItem_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_TeamUsers_UserId",
                table: "TeamUser",
                newName: "IX_TeamUser_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TeamUsers_TeamGroupId",
                table: "TeamUser",
                newName: "IX_TeamUser_TeamGroupId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "WebRequestItem",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "WebRequestItem",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "WebRequestItem",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WebRequestItem",
                table: "WebRequestItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamUser",
                table: "TeamUser",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamUser_TeamGroups_TeamGroupId",
                table: "TeamUser",
                column: "TeamGroupId",
                principalTable: "TeamGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamUser_Users_UserId",
                table: "TeamUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WebRequestDirs_WebRequestItem_WebRequestId",
                table: "WebRequestDirs",
                column: "WebRequestId",
                principalTable: "WebRequestItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WebRequestItem_Projects_ProjectId",
                table: "WebRequestItem",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
