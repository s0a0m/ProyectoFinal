using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class Agrega_tablas_relaciones_it3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categoria",
                columns: table => new
                {
                    id_categoria = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoria", x => x.id_categoria);
                });

            migrationBuilder.CreateTable(
                name: "codigo_barra",
                columns: table => new
                {
                    id_codigo_barra = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo_barra = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_codigo_barra", x => x.id_codigo_barra);
                });

            migrationBuilder.CreateTable(
                name: "grupo",
                columns: table => new
                {
                    id_grupo = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grupo", x => x.id_grupo);
                });

            migrationBuilder.CreateTable(
                name: "producto",
                columns: table => new
                {
                    id_producto = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    stock_minimo = table.Column<int>(type: "integer", nullable: false),
                    stock_total = table.Column<int>(type: "integer", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    id_grupo = table.Column<short>(type: "smallint", nullable: false),
                    id_categoria = table.Column<short>(type: "smallint", nullable: false),
                    id_codigo_barra = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producto", x => x.id_producto);
                    table.ForeignKey(
                        name: "FK_producto_categoria_id_categoria",
                        column: x => x.id_categoria,
                        principalTable: "categoria",
                        principalColumn: "id_categoria",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_producto_codigo_barra_id_codigo_barra",
                        column: x => x.id_codigo_barra,
                        principalTable: "codigo_barra",
                        principalColumn: "id_codigo_barra",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_producto_grupo_id_grupo",
                        column: x => x.id_grupo,
                        principalTable: "grupo",
                        principalColumn: "id_grupo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "producto_proveedor",
                columns: table => new
                {
                    id_producto = table.Column<short>(type: "smallint", nullable: false),
                    id_proveedor = table.Column<short>(type: "smallint", nullable: false),
                    precio = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    stock_asignado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producto_proveedor", x => new { x.id_producto, x.id_proveedor });
                    table.ForeignKey(
                        name: "FK_producto_proveedor_producto_id_producto",
                        column: x => x.id_producto,
                        principalTable: "producto",
                        principalColumn: "id_producto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_producto_proveedor_proveedor_id_proveedor",
                        column: x => x.id_proveedor,
                        principalTable: "proveedor",
                        principalColumn: "id_proveedor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_producto_id_categoria",
                table: "producto",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_producto_id_codigo_barra",
                table: "producto",
                column: "id_codigo_barra",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_producto_id_grupo",
                table: "producto",
                column: "id_grupo");

            migrationBuilder.CreateIndex(
                name: "IX_producto_proveedor_id_proveedor",
                table: "producto_proveedor",
                column: "id_proveedor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "producto_proveedor");

            migrationBuilder.DropTable(
                name: "producto");

            migrationBuilder.DropTable(
                name: "categoria");

            migrationBuilder.DropTable(
                name: "codigo_barra");

            migrationBuilder.DropTable(
                name: "grupo");
        }
    }
}
