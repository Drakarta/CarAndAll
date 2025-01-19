using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class Abonnementv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Abbonement",
                table: "Bedrijf");

            migrationBuilder.AddColumn<int>(
                name: "AbonnementId",
                table: "Bedrijf",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bedrijf_AbonnementId",
                table: "Bedrijf",
                column: "AbonnementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bedrijf_Abonnement_AbonnementId",
                table: "Bedrijf",
                column: "AbonnementId",
                principalTable: "Abonnement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bedrijf_Abonnement_AbonnementId",
                table: "Bedrijf");

            migrationBuilder.DropIndex(
                name: "IX_Bedrijf_AbonnementId",
                table: "Bedrijf");

            migrationBuilder.DropColumn(
                name: "AbonnementId",
                table: "Bedrijf");

            migrationBuilder.AddColumn<string>(
                name: "Abbonement",
                table: "Bedrijf",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
