using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProductoIdOpcionalANovedades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_novedades_proveedor_producto_ProductoIdProducto",
                table: "novedades_proveedor");

            migrationBuilder.RenameColumn(
                name: "ProductoIdProducto",
                table: "novedades_proveedor",
                newName: "id_producto");

            migrationBuilder.RenameIndex(
                name: "IX_novedades_proveedor_ProductoIdProducto",
                table: "novedades_proveedor",
                newName: "IX_novedades_proveedor_id_producto");

            migrationBuilder.AddForeignKey(
                name: "FK_novedades_proveedor_producto_id_producto",
                table: "novedades_proveedor",
                column: "id_producto",
                principalTable: "producto",
                principalColumn: "id_producto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_novedades_proveedor_producto_id_producto",
                table: "novedades_proveedor");

            migrationBuilder.RenameColumn(
                name: "id_producto",
                table: "novedades_proveedor",
                newName: "ProductoIdProducto");

            migrationBuilder.RenameIndex(
                name: "IX_novedades_proveedor_id_producto",
                table: "novedades_proveedor",
                newName: "IX_novedades_proveedor_ProductoIdProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_novedades_proveedor_producto_ProductoIdProducto",
                table: "novedades_proveedor",
                column: "ProductoIdProducto",
                principalTable: "producto",
                principalColumn: "id_producto");
        }
    }
}
