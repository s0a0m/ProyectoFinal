using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class ImplementacionGestionDeNovedades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "novedades_proveedor",
                columns: table => new
                {
                    id_novedad = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_proveedor = table.Column<short>(type: "smallint", nullable: false),
                    codigo_barra_externo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nombre_sugerido = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    precio_sugerido = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    estado = table.Column<string>(type: "text", nullable: false),
                    fecha_importacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductoIdProducto = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_novedades_proveedor", x => x.id_novedad);
                    table.ForeignKey(
                        name: "FK_novedades_proveedor_producto_ProductoIdProducto",
                        column: x => x.ProductoIdProducto,
                        principalTable: "producto",
                        principalColumn: "id_producto");
                    table.ForeignKey(
                        name: "FK_novedades_proveedor_proveedor_id_proveedor",
                        column: x => x.id_proveedor,
                        principalTable: "proveedor",
                        principalColumn: "id_proveedor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "producto_codigo_externo",
                columns: table => new
                {
                    id_proveedor = table.Column<short>(type: "smallint", nullable: false),
                    codigo_barra_proveedor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    id_producto = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producto_codigo_externo", x => new { x.id_proveedor, x.codigo_barra_proveedor });
                    table.ForeignKey(
                        name: "FK_producto_codigo_externo_producto_id_producto",
                        column: x => x.id_producto,
                        principalTable: "producto",
                        principalColumn: "id_producto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_producto_codigo_externo_proveedor_id_proveedor",
                        column: x => x.id_proveedor,
                        principalTable: "proveedor",
                        principalColumn: "id_proveedor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_novedades_proveedor_id_proveedor",
                table: "novedades_proveedor",
                column: "id_proveedor");

            migrationBuilder.CreateIndex(
                name: "IX_novedades_proveedor_ProductoIdProducto",
                table: "novedades_proveedor",
                column: "ProductoIdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_producto_codigo_externo_id_producto",
                table: "producto_codigo_externo",
                column: "id_producto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "novedades_proveedor");

            migrationBuilder.DropTable(
                name: "producto_codigo_externo");
        }
    }
}
