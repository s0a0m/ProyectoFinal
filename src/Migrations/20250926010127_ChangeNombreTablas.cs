using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNombreTablas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Cuota",
                newName: "cuota");

            migrationBuilder.RenameTable(
                name: "Provincias",
                newName: "provincia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "cuota",
                newName: "Cuota");

            migrationBuilder.RenameTable(
                name: "provincia",
                newName: "Provincias");
        }
    }
}
