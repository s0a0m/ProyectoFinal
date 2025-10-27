using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class AgregaTablaPermisosYUsuarioPermiso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "permiso",
                columns: table => new
                {
                    IdPermiso = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permiso", x => x.IdPermiso);
                });

            migrationBuilder.CreateTable(
                name: "usuario_permiso",
                columns: table => new
                {
                    IdUsuario = table.Column<short>(type: "smallint", nullable: false),
                    IdPermiso = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("usuario_permiso_pkey", x => new { x.IdPermiso, x.IdUsuario });
                    table.ForeignKey(
                        name: "usuario_permiso_id_permiso_fkey",
                        column: x => x.IdPermiso,
                        principalTable: "permiso",
                        principalColumn: "IdPermiso",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "usuario_permiso_id_usuario_fkey",
                        column: x => x.IdUsuario,
                        principalTable: "usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "unq_pm",
                table: "permiso",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_permiso_IdUsuario",
                table: "usuario_permiso",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "usuario_permiso");

            migrationBuilder.DropTable(
                name: "permiso");
        }
    }
}
