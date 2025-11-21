using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class AgregaReferenciasProveedorProductoCodExtYCambiaNovedadDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_importacion",
                table: "novedades_proveedor",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_producto_codigo_externo_id_proveedor",
                table: "producto_codigo_externo",
                column: "id_proveedor",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_producto_codigo_externo_id_proveedor",
                table: "producto_codigo_externo");

            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_importacion",
                table: "novedades_proveedor",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");
        }
    }
}
