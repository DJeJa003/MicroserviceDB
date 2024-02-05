using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroserviceDB.Migrations
{
    public partial class RemovedDeleteFromMyEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "MyEntities");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "MyEntities",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
