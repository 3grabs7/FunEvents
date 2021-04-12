using Microsoft.EntityFrameworkCore.Migrations;

namespace FunEvents.Data.Migrations
{
    public partial class organizerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Analytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: true),
                    Ip = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Analytics_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Organizers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUserOrganizer",
                columns: table => new
                {
                    AssistantInOrganizationsId = table.Column<int>(type: "int", nullable: false),
                    OrganizerAssistantsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserOrganizer", x => new { x.AssistantInOrganizationsId, x.OrganizerAssistantsId });
                    table.ForeignKey(
                        name: "FK_AppUserOrganizer_AspNetUsers_OrganizerAssistantsId",
                        column: x => x.OrganizerAssistantsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserOrganizer_Organizers_AssistantInOrganizationsId",
                        column: x => x.AssistantInOrganizationsId,
                        principalTable: "Organizers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUserOrganizer1",
                columns: table => new
                {
                    ManagerInOrganizationsId = table.Column<int>(type: "int", nullable: false),
                    OrganizerManagersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserOrganizer1", x => new { x.ManagerInOrganizationsId, x.OrganizerManagersId });
                    table.ForeignKey(
                        name: "FK_AppUserOrganizer1_AspNetUsers_OrganizerManagersId",
                        column: x => x.OrganizerManagersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserOrganizer1_Organizers_ManagerInOrganizationsId",
                        column: x => x.ManagerInOrganizationsId,
                        principalTable: "Organizers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_EventId",
                table: "Analytics",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserOrganizer_OrganizerAssistantsId",
                table: "AppUserOrganizer",
                column: "OrganizerAssistantsId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserOrganizer1_OrganizerManagersId",
                table: "AppUserOrganizer1",
                column: "OrganizerManagersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Analytics");

            migrationBuilder.DropTable(
                name: "AppUserOrganizer");

            migrationBuilder.DropTable(
                name: "AppUserOrganizer1");

            migrationBuilder.DropTable(
                name: "Organizers");
        }
    }
}
