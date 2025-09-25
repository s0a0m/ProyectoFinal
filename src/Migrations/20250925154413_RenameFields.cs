using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class RenameFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "proveedor_id_direccion_fkey",
                table: "proveedor");

            migrationBuilder.DropTable(
                name: "direccion");

            migrationBuilder.DropTable(
                name: "provincia");

            migrationBuilder.DropColumn(
                name: "domicilio",
                table: "proveedor");

            migrationBuilder.RenameTable(
                name: "cuotas",
                newName: "Cuota");

            migrationBuilder.RenameColumn(
                name: "id_direccion",
                table: "proveedor",
                newName: "id_domicilio");

            // migrationBuilder.RenameIndex(
            //     name: "IX_proveedor_id_direccion",
            //     table: "proveedor",
            //     newName: "IX_proveedor_id_domicilio");

            migrationBuilder.CreateTable(
                name: "Provincias",
                columns: table => new
                {
                    id_provincia = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("provincia_pkey", x => x.id_provincia);
                });

            migrationBuilder.CreateTable(
                name: "domicilio",
                columns: table => new
                {
                    id_domicilio = table.Column<short>(type: "smallint", nullable: false),
                    id_provincia = table.Column<short>(type: "smallint", nullable: false),
                    calle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    numero = table.Column<short>(type: "smallint", nullable: false),
                    piso = table.Column<short>(type: "smallint", nullable: true),
                    comentario = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("domicilio_pkey", x => x.id_domicilio);
                    table.ForeignKey(
                        name: "direccion_id_provincia_fkey",
                        column: x => x.id_provincia,
                        principalTable: "Provincias",
                        principalColumn: "id_provincia");
                });

            migrationBuilder.CreateIndex(
                name: "IX_domicilio_id_provincia",
                table: "domicilio",
                column: "id_provincia");

            migrationBuilder.CreateIndex(
                name: "unq_prov",
                table: "Provincias",
                column: "id_provincia",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "proveedor_id_domicilio_fkey",
                table: "proveedor",
                column: "id_domicilio",
                principalTable: "domicilio",
                principalColumn: "id_domicilio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "proveedor_id_domicilio_fkey",
                table: "proveedor");

            migrationBuilder.DropTable(
                name: "domicilio");

            migrationBuilder.DropTable(
                name: "Provincias");

            migrationBuilder.RenameTable(
                name: "Cuota",
                newName: "cuotas");

            migrationBuilder.RenameColumn(
                name: "id_domicilio",
                table: "proveedor",
                newName: "id_direccion");

            // migrationBuilder.RenameIndex(
            //     name: "IX_proveedor_id_domicilio",
            //     table: "proveedor",
            //     newName: "IX_proveedor_id_direccion");

            migrationBuilder.AddColumn<string>(
                name: "domicilio",
                table: "proveedor",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "provincia",
                columns: table => new
                {
                    id_provincia = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    provincia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("provincia_pkey", x => x.id_provincia);
                });

            migrationBuilder.CreateTable(
                name: "direccion",
                columns: table => new
                {
                    id_direccion = table.Column<short>(type: "smallint", nullable: false),
                    id_provincia = table.Column<short>(type: "smallint", nullable: false),
                    calle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    comentario = table.Column<string>(type: "text", nullable: true),
                    numero = table.Column<short>(type: "smallint", nullable: false),
                    piso = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("direccion_pkey", x => x.id_direccion);
                    table.ForeignKey(
                        name: "direccion_id_provincia_fkey",
                        column: x => x.id_provincia,
                        principalTable: "provincia",
                        principalColumn: "id_provincia");
                });

            migrationBuilder.CreateIndex(
                name: "IX_direccion_id_provincia",
                table: "direccion",
                column: "id_provincia");

            migrationBuilder.CreateIndex(
                name: "unq_prov",
                table: "provincia",
                column: "provincia",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "proveedor_id_direccion_fkey",
                table: "proveedor",
                column: "id_direccion",
                principalTable: "direccion",
                principalColumn: "id_direccion");
        }
    }
}
