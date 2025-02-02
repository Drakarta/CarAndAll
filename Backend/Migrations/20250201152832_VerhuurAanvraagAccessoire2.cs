using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class VerhuurAanvraagAccessoire2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accessoires",
                columns: table => new
                {
                    AccessoireNaam = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Extra_prijs_per_dag = table.Column<int>(type: "int", nullable: false),
                    Max_aantal = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accessoires", x => x.AccessoireNaam);
                });

            migrationBuilder.CreateTable(
                name: "VerhuurAanvraagAccessoires",
                columns: table => new
                {
                    AanvraagID = table.Column<int>(type: "int", nullable: false),
                    AccessoireNaam = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Aantal = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerhuurAanvraagAccessoires", x => new { x.AanvraagID, x.AccessoireNaam });
                    table.ForeignKey(
                        name: "FK_VerhuurAanvraagAccessoires_Accessoires_AccessoireNaam",
                        column: x => x.AccessoireNaam,
                        principalTable: "Accessoires",
                        principalColumn: "AccessoireNaam",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VerhuurAanvraagAccessoires_VerhuurAanvraag_AanvraagID",
                        column: x => x.AanvraagID,
                        principalTable: "VerhuurAanvraag",
                        principalColumn: "AanvraagID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VerhuurAanvraagAccessoires_AccessoireNaam",
                table: "VerhuurAanvraagAccessoires",
                column: "AccessoireNaam");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VerhuurAanvraagAccessoires");

            migrationBuilder.DropTable(
                name: "Accessoires");
        }
    }
}
