using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class Bedrijfwagenparkbeheerdersv4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BedrijfWagenparkbeheerders_Account_AccountId",
                table: "BedrijfWagenparkbeheerders");

            migrationBuilder.DropIndex(
                name: "IX_BedrijfWagenparkbeheerders_AccountId",
                table: "BedrijfWagenparkbeheerders");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "BedrijfWagenparkbeheerders");

            migrationBuilder.AddForeignKey(
                name: "FK_BedrijfWagenparkbeheerders_Account_account_id",
                table: "BedrijfWagenparkbeheerders",
                column: "account_id",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BedrijfWagenparkbeheerders_Account_account_id",
                table: "BedrijfWagenparkbeheerders");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "BedrijfWagenparkbeheerders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BedrijfWagenparkbeheerders_AccountId",
                table: "BedrijfWagenparkbeheerders",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_BedrijfWagenparkbeheerders_Account_AccountId",
                table: "BedrijfWagenparkbeheerders",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
