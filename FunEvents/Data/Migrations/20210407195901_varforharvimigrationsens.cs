using Microsoft.EntityFrameworkCore.Migrations;

namespace FunEvents.Data.Migrations
{
    public partial class varforharvimigrationsens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PageVisits",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UniquePageVisits",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageVisits",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "UniquePageVisits",
                table: "Events");
        }
    }
}
