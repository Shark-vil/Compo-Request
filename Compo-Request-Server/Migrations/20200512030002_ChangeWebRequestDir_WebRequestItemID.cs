using Microsoft.EntityFrameworkCore.Migrations;

namespace Compo_Request_Server.Migrations
{
    public partial class ChangeWebRequestDir_WebRequestItemID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WebRequestDirs_WebRequestItems_WebRequestId",
                table: "WebRequestDirs");

            migrationBuilder.DropIndex(
                name: "IX_WebRequestDirs_WebRequestId",
                table: "WebRequestDirs");

            migrationBuilder.DropColumn(
                name: "WebRequestId",
                table: "WebRequestDirs");

            migrationBuilder.AddColumn<int>(
                name: "WebRequestItemId",
                table: "WebRequestDirs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WebRequestDirs_WebRequestItemId",
                table: "WebRequestDirs",
                column: "WebRequestItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_WebRequestDirs_WebRequestItems_WebRequestItemId",
                table: "WebRequestDirs",
                column: "WebRequestItemId",
                principalTable: "WebRequestItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WebRequestDirs_WebRequestItems_WebRequestItemId",
                table: "WebRequestDirs");

            migrationBuilder.DropIndex(
                name: "IX_WebRequestDirs_WebRequestItemId",
                table: "WebRequestDirs");

            migrationBuilder.DropColumn(
                name: "WebRequestItemId",
                table: "WebRequestDirs");

            migrationBuilder.AddColumn<int>(
                name: "WebRequestId",
                table: "WebRequestDirs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WebRequestDirs_WebRequestId",
                table: "WebRequestDirs",
                column: "WebRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_WebRequestDirs_WebRequestItems_WebRequestId",
                table: "WebRequestDirs",
                column: "WebRequestId",
                principalTable: "WebRequestItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
