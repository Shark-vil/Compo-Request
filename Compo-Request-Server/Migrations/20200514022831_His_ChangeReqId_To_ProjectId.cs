using Microsoft.EntityFrameworkCore.Migrations;

namespace Compo_Request_Server.Migrations
{
    public partial class His_ChangeReqId_To_ProjectId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WebRequestsHistory_WebRequestItems_WebRequestItemId",
                table: "WebRequestsHistory");

            migrationBuilder.DropIndex(
                name: "IX_WebRequestsHistory_WebRequestItemId",
                table: "WebRequestsHistory");

            migrationBuilder.DropColumn(
                name: "WebRequestItemId",
                table: "WebRequestsHistory");

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "WebRequestsHistory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WebRequestsHistory_ProjectId",
                table: "WebRequestsHistory",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_WebRequestsHistory_Projects_ProjectId",
                table: "WebRequestsHistory",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WebRequestsHistory_Projects_ProjectId",
                table: "WebRequestsHistory");

            migrationBuilder.DropIndex(
                name: "IX_WebRequestsHistory_ProjectId",
                table: "WebRequestsHistory");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "WebRequestsHistory");

            migrationBuilder.AddColumn<int>(
                name: "WebRequestItemId",
                table: "WebRequestsHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WebRequestsHistory_WebRequestItemId",
                table: "WebRequestsHistory",
                column: "WebRequestItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_WebRequestsHistory_WebRequestItems_WebRequestItemId",
                table: "WebRequestsHistory",
                column: "WebRequestItemId",
                principalTable: "WebRequestItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
