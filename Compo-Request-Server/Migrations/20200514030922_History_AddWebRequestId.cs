using Microsoft.EntityFrameworkCore.Migrations;

namespace Compo_Request_Server.Migrations
{
    public partial class History_AddWebRequestId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WebRequestItemId",
                table: "WebRequestsHistory",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
