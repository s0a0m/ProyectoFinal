using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class CorregirFKCondicionPago : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_factura_condicion_pago_IdCondicionPagoUsadaNavigationIdCond~",
                table: "factura");

            migrationBuilder.DropIndex(
                name: "IX_factura_IdCondicionPagoUsadaNavigationIdCondicionPago",
                table: "factura");

            migrationBuilder.DropColumn(
                name: "IdCondicionPagoUsadaNavigationIdCondicionPago",
                table: "factura");

            migrationBuilder.CreateIndex(
                name: "IX_factura_id_condicion_pago_usada",
                table: "factura",
                column: "id_condicion_pago_usada");

            migrationBuilder.AddForeignKey(
                name: "FK_factura_condicion_pago_id_condicion_pago_usada",
                table: "factura",
                column: "id_condicion_pago_usada",
                principalTable: "condicion_pago",
                principalColumn: "id_condicion_pago",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_factura_condicion_pago_id_condicion_pago_usada",
                table: "factura");

            migrationBuilder.DropIndex(
                name: "IX_factura_id_condicion_pago_usada",
                table: "factura");

            migrationBuilder.AddColumn<short>(
                name: "IdCondicionPagoUsadaNavigationIdCondicionPago",
                table: "factura",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateIndex(
                name: "IX_factura_IdCondicionPagoUsadaNavigationIdCondicionPago",
                table: "factura",
                column: "IdCondicionPagoUsadaNavigationIdCondicionPago");

            migrationBuilder.AddForeignKey(
                name: "FK_factura_condicion_pago_IdCondicionPagoUsadaNavigationIdCond~",
                table: "factura",
                column: "IdCondicionPagoUsadaNavigationIdCondicionPago",
                principalTable: "condicion_pago",
                principalColumn: "id_condicion_pago",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
