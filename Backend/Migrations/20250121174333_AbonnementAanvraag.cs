using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AbonnementAanvraag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbonnementAanvraag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Beschrijving = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PrijsMultiplier = table.Column<double>(type: "float", nullable: false),
                    MaxMedewerkers = table.Column<int>(type: "int", nullable: false),
                    BedrijfId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "In behandeling")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbonnementAanvraag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbonnementAanvraag_Bedrijf_BedrijfId",
                        column: x => x.BedrijfId,
                        principalTable: "Bedrijf",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbonnementAanvraag_BedrijfId",
                table: "AbonnementAanvraag",
                column: "BedrijfId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbonnementAanvraag");
        }
    }
}
