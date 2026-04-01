using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class EliminarCondicionPagoDeComprobante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comprobante_condicion_pago_id_condicion_pago_usada",
                table: "comprobante");

            migrationBuilder.DropIndex(
                name: "IX_comprobante_id_condicion_pago_usada",
                table: "comprobante");

            migrationBuilder.DropColumn(
                name: "id_condicion_pago_usada",
                table: "comprobante");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "id_condicion_pago_usada",
                table: "comprobante",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateIndex(
                name: "IX_comprobante_id_condicion_pago_usada",
                table: "comprobante",
                column: "id_condicion_pago_usada");

            migrationBuilder.AddForeignKey(
                name: "FK_comprobante_condicion_pago_id_condicion_pago_usada",
                table: "comprobante",
                column: "id_condicion_pago_usada",
                principalTable: "condicion_pago",
                principalColumn: "id_condicion_pago",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
