using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroserviceDB.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUselessDbEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MyEntities");

            migrationBuilder.DropTable(
                name: "PriceEntries");

            migrationBuilder.DropTable(
                name: "LatestPriceData");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LatestPriceData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LatestPriceData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MyEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EndDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatestPriceDataId = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDate = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceEntries_LatestPriceData_LatestPriceDataId",
                        column: x => x.LatestPriceDataId,
                        principalTable: "LatestPriceData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriceEntries_LatestPriceDataId",
                table: "PriceEntries",
                column: "LatestPriceDataId");
        }
    }
}
