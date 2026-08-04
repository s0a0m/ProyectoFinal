using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarSaldosProveedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "saldo",
                table: "proveedor",
                newName: "saldo_inicial");

            migrationBuilder.AddColumn<decimal>(
                name: "saldo_actual",
                table: "proveedor",
                type: "numeric(11,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "saldo_actual",
                table: "proveedor");

            migrationBuilder.RenameColumn(
                name: "saldo_inicial",
                table: "proveedor",
                newName: "saldo");
        }
    }
}
