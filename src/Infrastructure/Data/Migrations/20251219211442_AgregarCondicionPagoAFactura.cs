using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCondicionPagoAFactura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "IdCondicionPagoUsadaNavigationIdCondicionPago",
                table: "factura",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "id_condicion_pago_usada",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "id_condicion_pago_usada",
                table: "factura");
        }
    }
}
