using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class AgregaConfiguracionDeProductoCodigoExternoEF_FIX : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_producto_codigo_externo_id_proveedor",
                table: "producto_codigo_externo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_producto_codigo_externo_id_proveedor",
                table: "producto_codigo_externo",
                column: "id_proveedor",
                unique: true);
        }
    }
}
