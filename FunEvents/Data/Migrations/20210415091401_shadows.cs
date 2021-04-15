using Microsoft.EntityFrameworkCore.Migrations;

namespace FunEvents.Data.Migrations
{
    public partial class shadows : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Events_EventId",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "Events",
                newName: "PendingEditEventId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_EventId",
                table: "Events",
                newName: "IX_Events_PendingEditEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Events_PendingEditEventId",
                table: "Events",
                column: "PendingEditEventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Events_PendingEditEventId",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "PendingEditEventId",
                table: "Events",
                newName: "EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_PendingEditEventId",
                table: "Events",
                newName: "IX_Events_EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Events_EventId",
                table: "Events",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
