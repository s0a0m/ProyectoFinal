using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "condicion_pago",
                columns: table => new
                {
                    id_condicion_pago = table.Column<short>(type: "smallint", nullable: false),
                    dias_pago = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("condicion_pago_pkey", x => x.id_condicion_pago);
                });

            migrationBuilder.CreateTable(
                name: "provincia",
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
                name: "contado",
                columns: table => new
                {
                    id_condicion_pago = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("contado_pkey", x => x.id_condicion_pago);
                    table.ForeignKey(
                        name: "FK_contado_condicion_pago_id_condicion_pago",
                        column: x => x.id_condicion_pago,
                        principalTable: "condicion_pago",
                        principalColumn: "id_condicion_pago",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cuota",
                columns: table => new
                {
                    id_condicion_pago = table.Column<short>(type: "smallint", nullable: false),
                    cuotas = table.Column<short>(type: "smallint", nullable: false),
                    interes_porcentual = table.Column<decimal>(type: "numeric(6,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("cuota_pkey", x => x.id_condicion_pago);
                    table.ForeignKey(
                        name: "FK_cuota_condicion_pago_id_condicion_pago",
                        column: x => x.id_condicion_pago,
                        principalTable: "condicion_pago",
                        principalColumn: "id_condicion_pago",
                        onDelete: ReferentialAction.Cascade);
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
                    comentario = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("domicilio_pkey", x => x.id_domicilio);
                    table.ForeignKey(
                        name: "direccion_id_provincia_fkey",
                        column: x => x.id_provincia,
                        principalTable: "provincia",
                        principalColumn: "id_provincia");
                });

            migrationBuilder.CreateTable(
                name: "proveedor",
                columns: table => new
                {
                    id_proveedor = table.Column<short>(type: "smallint", nullable: false),
                    cuit = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    razon_social = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    id_condicion_pago_habitual = table.Column<short>(type: "smallint", nullable: false),
                    telefono = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    correo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    persona_responsable = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    saldo = table.Column<string>(type: "character varying", nullable: false),
                    id_domicilio = table.Column<short>(type: "smallint", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("proveedor_pkey", x => x.id_proveedor);
                    table.ForeignKey(
                        name: "proveedor_id_condicion_pago_habitual_fkey",
                        column: x => x.id_condicion_pago_habitual,
                        principalTable: "condicion_pago",
                        principalColumn: "id_condicion_pago");
                    table.ForeignKey(
                        name: "proveedor_id_domicilio_fkey",
                        column: x => x.id_domicilio,
                        principalTable: "domicilio",
                        principalColumn: "id_domicilio");
                });

            migrationBuilder.CreateIndex(
                name: "IX_domicilio_id_provincia",
                table: "domicilio",
                column: "id_provincia");

            migrationBuilder.CreateIndex(
                name: "IX_proveedor_id_condicion_pago_habitual",
                table: "proveedor",
                column: "id_condicion_pago_habitual");

            migrationBuilder.CreateIndex(
                name: "IX_proveedor_id_domicilio",
                table: "proveedor",
                column: "id_domicilio");

            migrationBuilder.CreateIndex(
                name: "unq_cuit",
                table: "proveedor",
                column: "cuit",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "unq_razon",
                table: "proveedor",
                column: "razon_social",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "unq_prov",
                table: "provincia",
                column: "id_provincia",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contado");

            migrationBuilder.DropTable(
                name: "cuota");

            migrationBuilder.DropTable(
                name: "proveedor");

            migrationBuilder.DropTable(
                name: "condicion_pago");

            migrationBuilder.DropTable(
                name: "domicilio");

            migrationBuilder.DropTable(
                name: "provincia");
        }
    }
}
