    using Microsoft.EntityFrameworkCore.Migrations;

    #nullable disable

    namespace MicroserviceDB.Migrations
    {
        /// <inheritdoc />
        public partial class RemovedAgeAndNameFromMyEntity : Migration
        {
            /// <inheritdoc />
            protected override void Up(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.DropColumn(
                    name: "Age",
                    table: "MyEntities");

                migrationBuilder.DropColumn(
                    name: "Name",
                    table: "MyEntities");
            }

            /// <inheritdoc />
            protected override void Down(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.AddColumn<int>(
                    name: "Age",
                    table: "MyEntities",
                    type: "int",
                    nullable: false,
                    defaultValue: 0);

                migrationBuilder.AddColumn<string>(
                    name: "Name",
                    table: "MyEntities",
                    type: "nvarchar(max)",
                    nullable: false,
                    defaultValue: "");
            }
        }
    }
