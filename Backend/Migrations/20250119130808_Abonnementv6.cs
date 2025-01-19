using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class Abonnementv6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bedrijf_Abonnement_AbonnementId",
                table: "Bedrijf");

            migrationBuilder.RenameColumn(
                name: "AbonnementId",
                table: "Bedrijf",
                newName: "abonnementId");

            migrationBuilder.RenameIndex(
                name: "IX_Bedrijf_AbonnementId",
                table: "Bedrijf",
                newName: "IX_Bedrijf_abonnementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bedrijf_Abonnement_abonnementId",
                table: "Bedrijf",
                column: "abonnementId",
                principalTable: "Abonnement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bedrijf_Abonnement_abonnementId",
                table: "Bedrijf");

            migrationBuilder.RenameColumn(
                name: "abonnementId",
                table: "Bedrijf",
                newName: "AbonnementId");

            migrationBuilder.RenameIndex(
                name: "IX_Bedrijf_abonnementId",
                table: "Bedrijf",
                newName: "IX_Bedrijf_AbonnementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bedrijf_Abonnement_AbonnementId",
                table: "Bedrijf",
                column: "AbonnementId",
                principalTable: "Abonnement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
