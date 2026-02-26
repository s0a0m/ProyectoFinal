using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class ImplementacionDeRelacionMAMConGrupoYCategoriaConProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_producto_categoria_id_categoria",
                table: "producto");

            migrationBuilder.DropForeignKey(
                name: "FK_producto_grupo_id_grupo",
                table: "producto");

            migrationBuilder.DropIndex(
                name: "IX_producto_id_categoria",
                table: "producto");

            migrationBuilder.DropIndex(
                name: "IX_producto_id_grupo",
                table: "producto");

            migrationBuilder.DropColumn(
                name: "id_categoria",
                table: "producto");

            migrationBuilder.DropColumn(
                name: "id_grupo",
                table: "producto");

            migrationBuilder.CreateTable(
                name: "producto_categoria",
                columns: table => new
                {
                    id_producto = table.Column<short>(type: "smallint", nullable: false),
                    id_categoria = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producto_categoria", x => new { x.id_producto, x.id_categoria });
                    table.ForeignKey(
                        name: "FK_producto_categoria_categoria_id_categoria",
                        column: x => x.id_categoria,
                        principalTable: "categoria",
                        principalColumn: "id_categoria",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_producto_categoria_producto_id_producto",
                        column: x => x.id_producto,
                        principalTable: "producto",
                        principalColumn: "id_producto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "producto_grupo",
                columns: table => new
                {
                    id_producto = table.Column<short>(type: "smallint", nullable: false),
                    id_grupo = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producto_grupo", x => new { x.id_producto, x.id_grupo });
                    table.ForeignKey(
                        name: "FK_producto_grupo_grupo_id_grupo",
                        column: x => x.id_grupo,
                        principalTable: "grupo",
                        principalColumn: "id_grupo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_producto_grupo_producto_id_producto",
                        column: x => x.id_producto,
                        principalTable: "producto",
                        principalColumn: "id_producto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_producto_categoria_id_categoria",
                table: "producto_categoria",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_producto_grupo_id_grupo",
                table: "producto_grupo",
                column: "id_grupo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "producto_categoria");

            migrationBuilder.DropTable(
                name: "producto_grupo");

            migrationBuilder.AddColumn<short>(
                name: "id_categoria",
                table: "producto",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "id_grupo",
                table: "producto",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateIndex(
                name: "IX_producto_id_categoria",
                table: "producto",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_producto_id_grupo",
                table: "producto",
                column: "id_grupo");

            migrationBuilder.AddForeignKey(
                name: "FK_producto_categoria_id_categoria",
                table: "producto",
                column: "id_categoria",
                principalTable: "categoria",
                principalColumn: "id_categoria",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_producto_grupo_id_grupo",
                table: "producto",
                column: "id_grupo",
                principalTable: "grupo",
                principalColumn: "id_grupo",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
