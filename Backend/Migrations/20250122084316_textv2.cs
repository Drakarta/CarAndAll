using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class textv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Text",
                newName: "Texts");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Texts",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "Texts",
                newName: "Content");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Texts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Texts",
                table: "Texts",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Texts",
                table: "Texts");

            migrationBuilder.RenameTable(
                name: "Texts",
                newName: "Text");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Text",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Text",
                newName: "type");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "Text",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
