using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FunEvents.Data.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUserOrganization",
                columns: table => new
                {
                    AssistantInOrganizationsId = table.Column<int>(type: "int", nullable: false),
                    OrganizationAssistantsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserOrganization", x => new { x.AssistantInOrganizationsId, x.OrganizationAssistantsId });
                    table.ForeignKey(
                        name: "FK_AppUserOrganization_AspNetUsers_OrganizationAssistantsId",
                        column: x => x.OrganizationAssistantsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserOrganization_Organizations_AssistantInOrganizationsId",
                        column: x => x.AssistantInOrganizationsId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUserOrganization1",
                columns: table => new
                {
                    ManagerInOrganizationsId = table.Column<int>(type: "int", nullable: false),
                    OrganizationManagersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserOrganization1", x => new { x.ManagerInOrganizationsId, x.OrganizationManagersId });
                    table.ForeignKey(
                        name: "FK_AppUserOrganization1_AspNetUsers_OrganizationManagersId",
                        column: x => x.OrganizationManagersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserOrganization1_Organizations_ManagerInOrganizationsId",
                        column: x => x.ManagerInOrganizationsId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Place = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SpotsAvailable = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    PageVisits = table.Column<int>(type: "int", nullable: false),
                    UniquePageVisits = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "AppUserEvent",
                columns: table => new
                {
                    AttendeesId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JoinedEventsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserEvent", x => new { x.AttendeesId, x.JoinedEventsId });
                    table.ForeignKey(
                        name: "FK_AppUserEvent_AspNetUsers_AttendeesId",
                        column: x => x.AttendeesId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserEvent_Events_JoinedEventsId",
                        column: x => x.JoinedEventsId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_EventId",
                table: "Analytics",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserEvent_JoinedEventsId",
                table: "AppUserEvent",
                column: "JoinedEventsId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserOrganization_OrganizationAssistantsId",
                table: "AppUserOrganization",
                column: "OrganizationAssistantsId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserOrganization1_OrganizationManagersId",
                table: "AppUserOrganization1",
                column: "OrganizationManagersId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventId",
                table: "Events",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_OrganizationId",
                table: "Events",
                column: "OrganizationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Analytics");

            migrationBuilder.DropTable(
                name: "AppUserEvent");

            migrationBuilder.DropTable(
                name: "AppUserOrganization");

            migrationBuilder.DropTable(
                name: "AppUserOrganization1");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");
        }
    }
}
