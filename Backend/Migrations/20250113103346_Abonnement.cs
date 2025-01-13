using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class Abonnement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VerhuurAanvraag_Account_accountId",
                table: "VerhuurAanvraag");

            migrationBuilder.RenameColumn(
                name: "accountId",
                table: "VerhuurAanvraag",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_VerhuurAanvraag_accountId",
                table: "VerhuurAanvraag",
                newName: "IX_VerhuurAanvraag_AccountId");

            migrationBuilder.CreateTable(
                name: "Abonnement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prijs_multiplier = table.Column<double>(type: "float", nullable: false),
                    Beschrijving = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Max_medewerkers = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abonnement", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_VerhuurAanvraag_Account_AccountId",
                table: "VerhuurAanvraag",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VerhuurAanvraag_Account_AccountId",
                table: "VerhuurAanvraag");

            migrationBuilder.DropTable(
                name: "Abonnement");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "VerhuurAanvraag",
                newName: "accountId");

            migrationBuilder.RenameIndex(
                name: "IX_VerhuurAanvraag_AccountId",
                table: "VerhuurAanvraag",
                newName: "IX_VerhuurAanvraag_accountId");

            migrationBuilder.AddForeignKey(
                name: "FK_VerhuurAanvraag_Account_accountId",
                table: "VerhuurAanvraag",
                column: "accountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
