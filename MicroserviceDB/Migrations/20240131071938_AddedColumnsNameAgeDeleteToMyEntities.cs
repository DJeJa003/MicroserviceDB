using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroserviceDB.Migrations
{
    /// <inheritdoc />
    public partial class AddedColumnsNameAgeDeleteToMyEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "MyEntities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "MyEntities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "MyEntities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "MyEntities");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "MyEntities");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "MyEntities");
        }
    }
}
