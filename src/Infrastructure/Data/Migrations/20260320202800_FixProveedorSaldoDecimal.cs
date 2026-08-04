using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace src.Migrations
{
    /// <inheritdoc />
    public partial class FixProveedorSaldoDecimal : Migration
    {
        /// /// <inheritdoc />
        /// protected override void Up(MigrationBuilder migrationBuilder)
        /// {
        ///     migrationBuilder.AlterColumn<decimal>(
        ///         name: "saldo",
        ///         table: "proveedor",
        ///         type: "numeric(11,2)",
        ///         nullable: false,
        ///         oldClrType: typeof(string),
        ///         oldType: "character varying"
        ///     );
        /// }
        ///
        /// /// <inheritdoc />
        /// protected override void Down(MigrationBuilder migrationBuilder)
        /// {
        ///     migrationBuilder.AlterColumn<string>(
        ///         name: "saldo",
        ///         table: "proveedor",
        ///         type: "character varying",
        ///         nullable: false,
        ///         oldClrType: typeof(decimal),
        ///         oldType: "numeric(11,2)"
        ///     );
        /// }
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE proveedor ALTER COLUMN saldo TYPE numeric(11,2) USING saldo::numeric(11,2);"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE proveedor ALTER COLUMN saldo TYPE character varying USING saldo::character varying;"
            );
        }
    }
}
