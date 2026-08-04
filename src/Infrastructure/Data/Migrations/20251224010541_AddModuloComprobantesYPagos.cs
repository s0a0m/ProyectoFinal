using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class AddModuloComprobantesYPagos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "motivo_comprobante",
                columns: table => new
                {
                    id_motivo_comprobante = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descripcion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    clase_comprobante = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_motivo_comprobante", x => x.id_motivo_comprobante);
                });

            migrationBuilder.CreateTable(
                name: "orden_pago",
                columns: table => new
                {
                    id_orden_pago = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_proveedor = table.Column<short>(type: "smallint", nullable: false),
                    monto_total = table.Column<decimal>(type: "numeric", nullable: false),
                    fecha_pago = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("orden_pago_pkey", x => x.id_orden_pago);
                    table.ForeignKey(
                        name: "FK_orden_pago_proveedor_id_proveedor",
                        column: x => x.id_proveedor,
                        principalTable: "proveedor",
                        principalColumn: "id_proveedor",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "comprobante",
                columns: table => new
                {
                    id_comprobante = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_proveedor = table.Column<short>(type: "smallint", nullable: false),
                    id_condicion_pago_usada = table.Column<short>(type: "smallint", nullable: false),
                    numero_comprobante = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    total = table.Column<decimal>(type: "numeric", nullable: false),
                    fecha_emision = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    id_motivo = table.Column<short>(type: "smallint", nullable: false),
                    comentario = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comprobante", x => x.id_comprobante);
                    table.ForeignKey(
                        name: "FK_comprobante_condicion_pago_id_condicion_pago_usada",
                        column: x => x.id_condicion_pago_usada,
                        principalTable: "condicion_pago",
                        principalColumn: "id_condicion_pago",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comprobante_motivo_comprobante_id_motivo",
                        column: x => x.id_motivo,
                        principalTable: "motivo_comprobante",
                        principalColumn: "id_motivo_comprobante",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comprobante_proveedor_id_proveedor",
                        column: x => x.id_proveedor,
                        principalTable: "proveedor",
                        principalColumn: "id_proveedor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pago_detalle",
                columns: table => new
                {
                    id_pago_detalle = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_orden_pago = table.Column<int>(type: "integer", nullable: false),
                    id_factura = table.Column<int>(type: "integer", nullable: false),
                    monto_aplicado = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pago_detalle_pkey", x => x.id_pago_detalle);
                    table.ForeignKey(
                        name: "FK_pago_detalle_factura_id_factura",
                        column: x => x.id_factura,
                        principalTable: "factura",
                        principalColumn: "id_factura",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pago_detalle_orden_pago_id_orden_pago",
                        column: x => x.id_orden_pago,
                        principalTable: "orden_pago",
                        principalColumn: "id_orden_pago",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "nota_credito",
                columns: table => new
                {
                    id_comprobante = table.Column<int>(type: "integer", nullable: false),
                    id_factura_referencia = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nota_credito", x => x.id_comprobante);
                    table.ForeignKey(
                        name: "FK_nota_credito_comprobante_id_comprobante",
                        column: x => x.id_comprobante,
                        principalTable: "comprobante",
                        principalColumn: "id_comprobante",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_nota_credito_factura_id_factura_referencia",
                        column: x => x.id_factura_referencia,
                        principalTable: "factura",
                        principalColumn: "id_factura",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "nota_debito",
                columns: table => new
                {
                    id_comprobante = table.Column<int>(type: "integer", nullable: false),
                    id_factura_referencia = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nota_debito", x => x.id_comprobante);
                    table.ForeignKey(
                        name: "FK_nota_debito_comprobante_id_comprobante",
                        column: x => x.id_comprobante,
                        principalTable: "comprobante",
                        principalColumn: "id_comprobante",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_nota_debito_factura_id_factura_referencia",
                        column: x => x.id_factura_referencia,
                        principalTable: "factura",
                        principalColumn: "id_factura",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_comprobante_id_condicion_pago_usada",
                table: "comprobante",
                column: "id_condicion_pago_usada");

            migrationBuilder.CreateIndex(
                name: "IX_comprobante_id_motivo",
                table: "comprobante",
                column: "id_motivo");

            migrationBuilder.CreateIndex(
                name: "IX_comprobante_id_proveedor",
                table: "comprobante",
                column: "id_proveedor");

            migrationBuilder.CreateIndex(
                name: "IX_nota_credito_id_factura_referencia",
                table: "nota_credito",
                column: "id_factura_referencia");

            migrationBuilder.CreateIndex(
                name: "IX_nota_debito_id_factura_referencia",
                table: "nota_debito",
                column: "id_factura_referencia");

            migrationBuilder.CreateIndex(
                name: "IX_orden_pago_id_proveedor",
                table: "orden_pago",
                column: "id_proveedor");

            migrationBuilder.CreateIndex(
                name: "IX_pago_detalle_id_factura",
                table: "pago_detalle",
                column: "id_factura");

            migrationBuilder.CreateIndex(
                name: "IX_pago_detalle_id_orden_pago",
                table: "pago_detalle",
                column: "id_orden_pago");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nota_credito");

            migrationBuilder.DropTable(
                name: "nota_debito");

            migrationBuilder.DropTable(
                name: "pago_detalle");

            migrationBuilder.DropTable(
                name: "comprobante");

            migrationBuilder.DropTable(
                name: "orden_pago");

            migrationBuilder.DropTable(
                name: "motivo_comprobante");
        }
    }
}
