using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class schade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Schades",
                columns: table => new
                {
                    SchadeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoertuigID = table.Column<int>(type: "int", nullable: false),
                    schade = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schades", x => x.SchadeID);
                    table.ForeignKey(
                        name: "FK_Schades_Voertuig_VoertuigID",
                        column: x => x.VoertuigID,
                        principalTable: "Voertuig",
                        principalColumn: "VoertuigID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schades_VoertuigID",
                table: "Schades",
                column: "VoertuigID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Schades");
        }
    }
}
