using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class ImplementacionRelacionNaNCodigoBarraConProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_producto_codigo_barra_id_codigo_barra",
                table: "producto");

            migrationBuilder.DropIndex(
                name: "IX_producto_id_codigo_barra",
                table: "producto");

            migrationBuilder.DropColumn(
                name: "id_codigo_barra",
                table: "producto");

            migrationBuilder.CreateTable(
                name: "producto_codigo_barra",
                columns: table => new
                {
                    id_producto = table.Column<short>(type: "smallint", nullable: false),
                    id_codigo_barra = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producto_codigo_barra", x => new { x.id_producto, x.id_codigo_barra });
                    table.ForeignKey(
                        name: "FK_producto_codigo_barra_codigo_barra_id_codigo_barra",
                        column: x => x.id_codigo_barra,
                        principalTable: "codigo_barra",
                        principalColumn: "id_codigo_barra",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_producto_codigo_barra_producto_id_producto",
                        column: x => x.id_producto,
                        principalTable: "producto",
                        principalColumn: "id_producto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_producto_codigo_barra_id_codigo_barra",
                table: "producto_codigo_barra",
                column: "id_codigo_barra");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "producto_codigo_barra");

            migrationBuilder.AddColumn<short>(
                name: "id_codigo_barra",
                table: "producto",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateIndex(
                name: "IX_producto_id_codigo_barra",
                table: "producto",
                column: "id_codigo_barra",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_producto_codigo_barra_id_codigo_barra",
                table: "producto",
                column: "id_codigo_barra",
                principalTable: "codigo_barra",
                principalColumn: "id_codigo_barra",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
