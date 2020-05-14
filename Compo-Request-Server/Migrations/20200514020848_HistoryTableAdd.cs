using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Compo_Request_Server.Migrations
{
    public partial class HistoryTableAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Surname = table.Column<string>(nullable: false),
                    Patronymic = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: false),
                    Login = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: false),
                    Uid = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: false),
                    Uid = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamGroups_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
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

            migrationBuilder.CreateTable(
                name: "WebRequestItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: false),
                    Link = table.Column<string>(nullable: false),
                    Method = table.Column<string>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebRequestItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebRequestItems_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "TeamProjects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TeamGroupId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamProjects_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamProjects_TeamGroups_TeamGroupId",
                        column: x => x.TeamGroupId,
                        principalTable: "TeamGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamProjects_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TeamGroupId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamUsers_TeamGroups_TeamGroupId",
                        column: x => x.TeamGroupId,
                        principalTable: "TeamGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebRequestDirs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: false),
                    WebRequestItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebRequestDirs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebRequestDirs_WebRequestItems_WebRequestItemId",
                        column: x => x.WebRequestItemId,
                        principalTable: "WebRequestItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "WebRequestsHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: false),
                    Link = table.Column<string>(nullable: false),
                    Method = table.Column<string>(nullable: false),
                    ParametrsInJson = table.Column<string>(type: "text", nullable: false),
                    ResponseDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ResponseResult = table.Column<string>(nullable: false),
                    WebRequestItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebRequestsHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebRequestsHistory_WebRequestItems_WebRequestItemId",
                        column: x => x.WebRequestItemId,
                        principalTable: "WebRequestItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Uid",
                table: "Projects",
                column: "Uid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserId",
                table: "Projects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamGroups_Uid",
                table: "TeamGroups",
                column: "Uid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamGroups_UserId",
                table: "TeamGroups",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPrivileges_TeamGroupId",
                table: "TeamPrivileges",
                column: "TeamGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamProjects_ProjectId",
                table: "TeamProjects",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamProjects_TeamGroupId",
                table: "TeamProjects",
                column: "TeamGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamProjects_UserId",
                table: "TeamProjects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamUsers_TeamGroupId",
                table: "TeamUsers",
                column: "TeamGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamUsers_UserId",
                table: "TeamUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPrivileges_UserId",
                table: "UserPrivileges",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email_Login",
                table: "Users",
                columns: new[] { "Email", "Login" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WebRequestDirs_WebRequestItemId",
                table: "WebRequestDirs",
                column: "WebRequestItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WebRequestItems_ProjectId",
                table: "WebRequestItems",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_WebRequestParamsItems_WebRequestItemId",
                table: "WebRequestParamsItems",
                column: "WebRequestItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WebRequestsHistory_WebRequestItemId",
                table: "WebRequestsHistory",
                column: "WebRequestItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamPrivileges");

            migrationBuilder.DropTable(
                name: "TeamProjects");

            migrationBuilder.DropTable(
                name: "TeamUsers");

            migrationBuilder.DropTable(
                name: "UserPrivileges");

            migrationBuilder.DropTable(
                name: "WebRequestDirs");

            migrationBuilder.DropTable(
                name: "WebRequestParamsItems");

            migrationBuilder.DropTable(
                name: "WebRequestsHistory");

            migrationBuilder.DropTable(
                name: "TeamGroups");

            migrationBuilder.DropTable(
                name: "WebRequestItems");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
