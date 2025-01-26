using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class VoertuigEntitySplitsing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voertuig_VoertuigCategorie_Categorie",
                table: "Voertuig");

            migrationBuilder.DropTable(
                name: "VoertuigCategorie");

            migrationBuilder.DropIndex(
                name: "IX_Voertuig_Categorie",
                table: "Voertuig");

            migrationBuilder.DropColumn(
                name: "Categorie",
                table: "Voertuig");

            migrationBuilder.AddColumn<int>(
                name: "Aantal_deuren",
                table: "Voertuig",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Aantal_slaapplekken",
                table: "Voertuig",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Auto_Elektrisch",
                table: "Voertuig",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Camper_Aantal_slaapplekken",
                table: "Voertuig",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Deleted_on",
                table: "Voertuig",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Elektrisch",
                table: "Voertuig",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gewicht_kg",
                table: "Voertuig",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "voertuig_categorie",
                table: "Voertuig",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aantal_deuren",
                table: "Voertuig");

            migrationBuilder.DropColumn(
                name: "Aantal_slaapplekken",
                table: "Voertuig");

            migrationBuilder.DropColumn(
                name: "Auto_Elektrisch",
                table: "Voertuig");

            migrationBuilder.DropColumn(
                name: "Camper_Aantal_slaapplekken",
                table: "Voertuig");

            migrationBuilder.DropColumn(
                name: "Deleted_on",
                table: "Voertuig");

            migrationBuilder.DropColumn(
                name: "Elektrisch",
                table: "Voertuig");

            migrationBuilder.DropColumn(
                name: "Gewicht_kg",
                table: "Voertuig");

            migrationBuilder.DropColumn(
                name: "voertuig_categorie",
                table: "Voertuig");

            migrationBuilder.AddColumn<string>(
                name: "Categorie",
                table: "Voertuig",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "VoertuigCategorie",
                columns: table => new
                {
                    Categorie = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoertuigCategorie", x => x.Categorie);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Voertuig_Categorie",
                table: "Voertuig",
                column: "Categorie");

            migrationBuilder.AddForeignKey(
                name: "FK_Voertuig_VoertuigCategorie_Categorie",
                table: "Voertuig",
                column: "Categorie",
                principalTable: "VoertuigCategorie",
                principalColumn: "Categorie",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
