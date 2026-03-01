using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class checkCantidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_UbicacionProducto_Cantidad",
                table: "ubicacion_producto",
                sql: "cantidad >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_MovimientosStock_Cantidad",
                table: "movimiento_stock",
                sql: "cantidad >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_UbicacionProducto_Cantidad",
                table: "ubicacion_producto");

            migrationBuilder.DropCheckConstraint(
                name: "CK_MovimientosStock_Cantidad",
                table: "movimiento_stock");
        }
    }
}
