using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class AgregaCamposNovedades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_modificacion",
                table: "novedades_proveedor",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "observaciones",
                table: "novedades_proveedor",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "stock_sugerido",
                table: "novedades_proveedor",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fecha_modificacion",
                table: "novedades_proveedor");

            migrationBuilder.DropColumn(
                name: "observaciones",
                table: "novedades_proveedor");

            migrationBuilder.DropColumn(
                name: "stock_sugerido",
                table: "novedades_proveedor");
        }
    }
}
