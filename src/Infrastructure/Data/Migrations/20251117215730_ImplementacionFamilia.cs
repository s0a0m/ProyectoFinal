using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class ImplementacionFamilia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "id_familia",
                table: "categoria",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "familia",
                columns: table => new
                {
                    id_familia = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_familia", x => x.id_familia);
                });

            migrationBuilder.CreateIndex(
                name: "IX_categoria_id_familia",
                table: "categoria",
                column: "id_familia");

            migrationBuilder.AddForeignKey(
                name: "FK_categoria_familia_id_familia",
                table: "categoria",
                column: "id_familia",
                principalTable: "familia",
                principalColumn: "id_familia",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categoria_familia_id_familia",
                table: "categoria");

            migrationBuilder.DropTable(
                name: "familia");

            migrationBuilder.DropIndex(
                name: "IX_categoria_id_familia",
                table: "categoria");

            migrationBuilder.DropColumn(
                name: "id_familia",
                table: "categoria");
        }
    }
}
