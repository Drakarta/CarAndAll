using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class Bedrijfwagenparkbeheerdersv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BedrijfWagenparkbeheerders",
                columns: table => new
                {
                    account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    bedrijf_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BedrijfWagenparkbeheerders", x => new { x.account_id, x.bedrijf_id });
                    table.ForeignKey(
                        name: "FK_BedrijfWagenparkbeheerders_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BedrijfWagenparkbeheerders_Bedrijf_bedrijf_id",
                        column: x => x.bedrijf_id,
                        principalTable: "Bedrijf",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BedrijfWagenparkbeheerders_AccountId",
                table: "BedrijfWagenparkbeheerders",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BedrijfWagenparkbeheerders_bedrijf_id",
                table: "BedrijfWagenparkbeheerders",
                column: "bedrijf_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BedrijfWagenparkbeheerders");
        }
    }
}
