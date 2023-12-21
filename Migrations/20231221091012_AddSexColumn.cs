using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kissarekisteri.Migrations
{
    /// <inheritdoc />
    public partial class AddSexColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Breed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BreederId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatShows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatShows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatParents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentCatId = table.Column<int>(type: "int", nullable: false),
                    ChildCatId = table.Column<int>(type: "int", nullable: false),
                    ParentType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatParents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatParents_Cats_ChildCatId",
                        column: x => x.ChildCatId,
                        principalTable: "Cats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatParents_Cats_ParentCatId",
                        column: x => x.ParentCatId,
                        principalTable: "Cats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CatId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatPhotos_Cats_CatId",
                        column: x => x.CatId,
                        principalTable: "Cats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attendees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendees_CatShows_EventId",
                        column: x => x.EventId,
                        principalTable: "CatShows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatShowPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CatShowId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatShowPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatShowPhotos_CatShows_CatShowId",
                        column: x => x.CatShowId,
                        principalTable: "CatShows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatShowResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatShowId = table.Column<int>(type: "int", nullable: false),
                    CatId = table.Column<int>(type: "int", nullable: false),
                    Breed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Place = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatShowResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatShowResults_CatShows_CatShowId",
                        column: x => x.CatShowId,
                        principalTable: "CatShows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatShowResults_Cats_CatId",
                        column: x => x.CatId,
                        principalTable: "Cats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatAttendees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    CatId = table.Column<int>(type: "int", nullable: false),
                    AttendeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatAttendees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatAttendees_Attendees_AttendeeId",
                        column: x => x.AttendeeId,
                        principalTable: "Attendees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatAttendees_Cats_CatId",
                        column: x => x.CatId,
                        principalTable: "Cats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendees_EventId",
                table: "Attendees",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_CatAttendees_AttendeeId",
                table: "CatAttendees",
                column: "AttendeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CatAttendees_CatId",
                table: "CatAttendees",
                column: "CatId");

            migrationBuilder.CreateIndex(
                name: "IX_CatParents_ChildCatId",
                table: "CatParents",
                column: "ChildCatId");

            migrationBuilder.CreateIndex(
                name: "IX_CatParents_ParentCatId",
                table: "CatParents",
                column: "ParentCatId");

            migrationBuilder.CreateIndex(
                name: "IX_CatPhotos_CatId",
                table: "CatPhotos",
                column: "CatId");

            migrationBuilder.CreateIndex(
                name: "IX_CatShowPhotos_CatShowId",
                table: "CatShowPhotos",
                column: "CatShowId");

            migrationBuilder.CreateIndex(
                name: "IX_CatShowResults_CatId",
                table: "CatShowResults",
                column: "CatId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatShowResults_CatShowId",
                table: "CatShowResults",
                column: "CatShowId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                table: "Permissions",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleName_PermissionName",
                table: "RolePermissions",
                columns: new[] { "RoleName", "PermissionName" },
                unique: true,
                filter: "[RoleName] IS NOT NULL AND [PermissionName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatAttendees");

            migrationBuilder.DropTable(
                name: "CatParents");

            migrationBuilder.DropTable(
                name: "CatPhotos");

            migrationBuilder.DropTable(
                name: "CatShowPhotos");

            migrationBuilder.DropTable(
                name: "CatShowResults");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Attendees");

            migrationBuilder.DropTable(
                name: "Cats");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "CatShows");
        }
    }
}
