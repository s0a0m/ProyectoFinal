using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class CambiaDetallePagoPK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pago_detalle_pkey",
                table: "pago_detalle");

            migrationBuilder.DropIndex(
                name: "IX_pago_detalle_id_orden_pago",
                table: "pago_detalle");

            migrationBuilder.DropColumn(
                name: "id_pago_detalle",
                table: "pago_detalle");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pago_detalle",
                table: "pago_detalle",
                columns: new[] { "id_orden_pago", "id_factura" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_pago_detalle",
                table: "pago_detalle");

            migrationBuilder.AddColumn<int>(
                name: "id_pago_detalle",
                table: "pago_detalle",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "pago_detalle_pkey",
                table: "pago_detalle",
                column: "id_pago_detalle");

            migrationBuilder.CreateIndex(
                name: "IX_pago_detalle_id_orden_pago",
                table: "pago_detalle",
                column: "id_orden_pago");
        }
    }
}
