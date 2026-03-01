using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class UbicacionMigrationN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "deposito",
                columns: table => new
                {
                    id_deposito = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    id_direccion = table.Column<short>(type: "smallint", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deposito", x => x.id_deposito);
                    table.ForeignKey(
                        name: "FK_deposito_domicilio_id_direccion",
                        column: x => x.id_direccion,
                        principalTable: "domicilio",
                        principalColumn: "id_domicilio",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "estante",
                columns: table => new
                {
                    id_estante = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    numero_estante = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    id_deposito = table.Column<int>(type: "integer", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    tiene_espacio = table.Column<bool>(type: "boolean", nullable: false),
                    observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estante", x => x.id_estante);
                    table.ForeignKey(
                        name: "FK_estante_deposito_id_deposito",
                        column: x => x.id_deposito,
                        principalTable: "deposito",
                        principalColumn: "id_deposito",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "fila",
                columns: table => new
                {
                    id_fila = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    n_fila = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    id_estante = table.Column<int>(type: "integer", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    tiene_espacio = table.Column<bool>(type: "boolean", nullable: false),
                    observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fila", x => x.id_fila);
                    table.ForeignKey(
                        name: "FK_fila_estante_id_estante",
                        column: x => x.id_estante,
                        principalTable: "estante",
                        principalColumn: "id_estante",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "movimiento_stock",
                columns: table => new
                {
                    id_movimiento_stock = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_producto = table.Column<short>(type: "smallint", nullable: false),
                    id_fila_origen = table.Column<int>(type: "integer", nullable: true),
                    id_fila_destino = table.Column<int>(type: "integer", nullable: true),
                    cantidad = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    fecha_movimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    id_usuario = table.Column<short>(type: "smallint", nullable: false),
                    ProductoIdProducto = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movimiento_stock", x => x.id_movimiento_stock);
                    table.ForeignKey(
                        name: "FK_movimiento_stock_fila_id_fila_destino",
                        column: x => x.id_fila_destino,
                        principalTable: "fila",
                        principalColumn: "id_fila",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movimiento_stock_fila_id_fila_origen",
                        column: x => x.id_fila_origen,
                        principalTable: "fila",
                        principalColumn: "id_fila",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movimiento_stock_producto_ProductoIdProducto",
                        column: x => x.ProductoIdProducto,
                        principalTable: "producto",
                        principalColumn: "id_producto");
                    table.ForeignKey(
                        name: "FK_movimiento_stock_producto_id_producto",
                        column: x => x.id_producto,
                        principalTable: "producto",
                        principalColumn: "id_producto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movimiento_stock_usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ubicacion_producto",
                columns: table => new
                {
                    id_producto = table.Column<short>(type: "smallint", nullable: false),
                    id_fila = table.Column<int>(type: "integer", nullable: false),
                    cantidad = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ubicacion_producto", x => new { x.id_producto, x.id_fila });
                    table.ForeignKey(
                        name: "FK_ubicacion_producto_fila_id_fila",
                        column: x => x.id_fila,
                        principalTable: "fila",
                        principalColumn: "id_fila",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ubicacion_producto_producto_id_producto",
                        column: x => x.id_producto,
                        principalTable: "producto",
                        principalColumn: "id_producto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_deposito_id_direccion",
                table: "deposito",
                column: "id_direccion");

            migrationBuilder.CreateIndex(
                name: "IX_estante_id_deposito",
                table: "estante",
                column: "id_deposito");

            migrationBuilder.CreateIndex(
                name: "IX_fila_id_estante",
                table: "fila",
                column: "id_estante");

            migrationBuilder.CreateIndex(
                name: "IX_movimiento_stock_id_fila_destino",
                table: "movimiento_stock",
                column: "id_fila_destino");

            migrationBuilder.CreateIndex(
                name: "IX_movimiento_stock_id_fila_origen",
                table: "movimiento_stock",
                column: "id_fila_origen");

            migrationBuilder.CreateIndex(
                name: "IX_movimiento_stock_id_producto",
                table: "movimiento_stock",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_movimiento_stock_id_usuario",
                table: "movimiento_stock",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_movimiento_stock_ProductoIdProducto",
                table: "movimiento_stock",
                column: "ProductoIdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_ubicacion_producto_id_fila",
                table: "ubicacion_producto",
                column: "id_fila");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "movimiento_stock");

            migrationBuilder.DropTable(
                name: "ubicacion_producto");

            migrationBuilder.DropTable(
                name: "fila");

            migrationBuilder.DropTable(
                name: "estante");

            migrationBuilder.DropTable(
                name: "deposito");
        }
    }
}
