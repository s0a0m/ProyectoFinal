using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class AgregarGruposDePermisos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "grupo_permisos",
                columns: table => new
                {
                    id_grupo_permiso = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grupo_permisos", x => x.id_grupo_permiso);
                });

            migrationBuilder.CreateTable(
                name: "grupo_permiso_permiso",
                columns: table => new
                {
                    id_grupo_permiso = table.Column<short>(type: "smallint", nullable: false),
                    id_permiso = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("grupo_permiso_permiso_pkey", x => new { x.id_grupo_permiso, x.id_permiso });
                    table.ForeignKey(
                        name: "gpp_id_grupo_permiso_fkey",
                        column: x => x.id_grupo_permiso,
                        principalTable: "grupo_permisos",
                        principalColumn: "id_grupo_permiso",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "gpp_id_permiso_fkey",
                        column: x => x.id_permiso,
                        principalTable: "permiso",
                        principalColumn: "IdPermiso",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuario_grupo_permisos",
                columns: table => new
                {
                    id_usuario = table.Column<short>(type: "smallint", nullable: false),
                    id_grupo_permiso = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("usuario_grupo_permisos_pkey", x => new { x.id_usuario, x.id_grupo_permiso });
                    table.ForeignKey(
                        name: "ugp_id_grupo_permiso_fkey",
                        column: x => x.id_grupo_permiso,
                        principalTable: "grupo_permisos",
                        principalColumn: "id_grupo_permiso",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ugp_id_usuario_fkey",
                        column: x => x.id_usuario,
                        principalTable: "usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_grupo_permiso_permiso_id_permiso",
                table: "grupo_permiso_permiso",
                column: "id_permiso");

            migrationBuilder.CreateIndex(
                name: "unq_gpm",
                table: "grupo_permisos",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_grupo_permisos_id_grupo_permiso",
                table: "usuario_grupo_permisos",
                column: "id_grupo_permiso");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "grupo_permiso_permiso");

            migrationBuilder.DropTable(
                name: "usuario_grupo_permisos");

            migrationBuilder.DropTable(
                name: "grupo_permisos");
        }
    }
}
