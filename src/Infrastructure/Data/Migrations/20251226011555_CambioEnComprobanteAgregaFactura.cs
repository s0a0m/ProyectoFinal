using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class CambioEnComprobanteAgregaFactura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_nota_credito_factura_id_factura_referencia",
                table: "nota_credito");

            migrationBuilder.DropForeignKey(
                name: "FK_nota_debito_factura_id_factura_referencia",
                table: "nota_debito");

            migrationBuilder.DropIndex(
                name: "IX_nota_debito_id_factura_referencia",
                table: "nota_debito");

            migrationBuilder.DropIndex(
                name: "IX_nota_credito_id_factura_referencia",
                table: "nota_credito");

            migrationBuilder.DropColumn(
                name: "id_factura_referencia",
                table: "nota_debito");

            migrationBuilder.DropColumn(
                name: "id_factura_referencia",
                table: "nota_credito");

            migrationBuilder.AddColumn<int>(
                name: "id_factura_referencia",
                table: "comprobante",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_comprobante_id_factura_referencia",
                table: "comprobante",
                column: "id_factura_referencia");

            migrationBuilder.AddForeignKey(
                name: "FK_comprobante_factura_id_factura_referencia",
                table: "comprobante",
                column: "id_factura_referencia",
                principalTable: "factura",
                principalColumn: "id_factura",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comprobante_factura_id_factura_referencia",
                table: "comprobante");

            migrationBuilder.DropIndex(
                name: "IX_comprobante_id_factura_referencia",
                table: "comprobante");

            migrationBuilder.DropColumn(
                name: "id_factura_referencia",
                table: "comprobante");

            migrationBuilder.AddColumn<int>(
                name: "id_factura_referencia",
                table: "nota_debito",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "id_factura_referencia",
                table: "nota_credito",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_nota_debito_id_factura_referencia",
                table: "nota_debito",
                column: "id_factura_referencia");

            migrationBuilder.CreateIndex(
                name: "IX_nota_credito_id_factura_referencia",
                table: "nota_credito",
                column: "id_factura_referencia");

            migrationBuilder.AddForeignKey(
                name: "FK_nota_credito_factura_id_factura_referencia",
                table: "nota_credito",
                column: "id_factura_referencia",
                principalTable: "factura",
                principalColumn: "id_factura",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_nota_debito_factura_id_factura_referencia",
                table: "nota_debito",
                column: "id_factura_referencia",
                principalTable: "factura",
                principalColumn: "id_factura",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
