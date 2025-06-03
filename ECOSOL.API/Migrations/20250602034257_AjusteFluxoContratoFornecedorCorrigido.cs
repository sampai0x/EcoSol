using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECOSOL.API.Migrations
{
    /// <inheritdoc />
    public partial class AjusteFluxoContratoFornecedorCorrigido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_Clientes_ClienteId",
                table: "Contratos");

            migrationBuilder.DropIndex(
                name: "IX_Contratos_ClienteId",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Contratos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Contratos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_ClienteId",
                table: "Contratos",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_Clientes_ClienteId",
                table: "Contratos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
