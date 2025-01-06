using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class NewMigrationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Account_Id",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "GebuikerID",
                table: "VerhuurAanvraag",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "VerhuurAanvraag",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "accountId",
                table: "VerhuurAanvraag",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_VerhuurAanvraag_accountId",
                table: "VerhuurAanvraag",
                column: "accountId");

            migrationBuilder.AddForeignKey(
                name: "FK_VerhuurAanvraag_Account_accountId",
                table: "VerhuurAanvraag",
                column: "accountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VerhuurAanvraag_Account_accountId",
                table: "VerhuurAanvraag");

            migrationBuilder.DropIndex(
                name: "IX_VerhuurAanvraag_accountId",
                table: "VerhuurAanvraag");

            migrationBuilder.DropColumn(
                name: "GebuikerID",
                table: "VerhuurAanvraag");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "VerhuurAanvraag");

            migrationBuilder.DropColumn(
                name: "accountId",
                table: "VerhuurAanvraag");

            migrationBuilder.AddColumn<Guid>(
                name: "Account_Id",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
