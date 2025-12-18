using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class AddCompraFacturaModulo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "compra",
                columns: table => new
                {
                    id_compra = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_proveedor = table.Column<short>(type: "smallint", nullable: false),
                    id_usuario = table.Column<short>(type: "smallint", nullable: false),
                    fecha_compra = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_compra", x => x.id_compra);
                    table.ForeignKey(
                        name: "FK_compra_proveedor_id_proveedor",
                        column: x => x.id_proveedor,
                        principalTable: "proveedor",
                        principalColumn: "id_proveedor",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_compra_usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "detalle_compra",
                columns: table => new
                {
                    id_detalle_compra = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_compra = table.Column<short>(type: "smallint", nullable: false),
                    id_producto = table.Column<short>(type: "smallint", nullable: false),
                    cantidad = table.Column<int>(type: "integer", nullable: false),
                    precio_pactado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalle_compra", x => x.id_detalle_compra);
                    table.ForeignKey(
                        name: "FK_detalle_compra_compra_id_compra",
                        column: x => x.id_compra,
                        principalTable: "compra",
                        principalColumn: "id_compra",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_detalle_compra_producto_id_producto",
                        column: x => x.id_producto,
                        principalTable: "producto",
                        principalColumn: "id_producto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "factura",
                columns: table => new
                {
                    id_factura = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_compra = table.Column<short>(type: "smallint", nullable: false),
                    id_proveedor = table.Column<short>(type: "smallint", nullable: false),
                    numero_factura = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    fecha_emision = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    total_facturado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    pagada = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_factura", x => x.id_factura);
                    table.ForeignKey(
                        name: "FK_factura_compra_id_compra",
                        column: x => x.id_compra,
                        principalTable: "compra",
                        principalColumn: "id_compra",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_factura_proveedor_id_proveedor",
                        column: x => x.id_proveedor,
                        principalTable: "proveedor",
                        principalColumn: "id_proveedor",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "detalle_factura",
                columns: table => new
                {
                    id_detalle_factura = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_factura = table.Column<int>(type: "integer", nullable: false),
                    id_producto = table.Column<short>(type: "smallint", nullable: false),
                    cantidad = table.Column<int>(type: "integer", nullable: false),
                    precio_bruto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    porcentaje_descuento = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    precio_neto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalle_factura", x => x.id_detalle_factura);
                    table.ForeignKey(
                        name: "FK_detalle_factura_factura_id_factura",
                        column: x => x.id_factura,
                        principalTable: "factura",
                        principalColumn: "id_factura",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_detalle_factura_producto_id_producto",
                        column: x => x.id_producto,
                        principalTable: "producto",
                        principalColumn: "id_producto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_compra_id_proveedor",
                table: "compra",
                column: "id_proveedor");

            migrationBuilder.CreateIndex(
                name: "IX_compra_id_usuario",
                table: "compra",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_compra_id_compra",
                table: "detalle_compra",
                column: "id_compra");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_compra_id_producto",
                table: "detalle_compra",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_factura_id_factura",
                table: "detalle_factura",
                column: "id_factura");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_factura_id_producto",
                table: "detalle_factura",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_factura_id_compra",
                table: "factura",
                column: "id_compra");

            migrationBuilder.CreateIndex(
                name: "IX_factura_id_proveedor",
                table: "factura",
                column: "id_proveedor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "detalle_compra");

            migrationBuilder.DropTable(
                name: "detalle_factura");

            migrationBuilder.DropTable(
                name: "factura");

            migrationBuilder.DropTable(
                name: "compra");
        }
    }
}
