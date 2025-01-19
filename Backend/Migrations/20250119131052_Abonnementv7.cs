using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class Abonnementv7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<int>(
                name: "AbonnementId",
                table: "Bedrijf",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Bedrijf_Abonnement_AbonnementId",
                table: "Bedrijf",
                column: "AbonnementId",
                principalTable: "Abonnement",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<int>(
                name: "abonnementId",
                table: "Bedrijf",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bedrijf_Abonnement_abonnementId",
                table: "Bedrijf",
                column: "abonnementId",
                principalTable: "Abonnement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
