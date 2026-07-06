using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Boleiroffice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmpresaChaveUnicaRachao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChaveUnica",
                table: "rachao_confirmacoes",
                type: "character varying(260)",
                maxLength: 260,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Empresa",
                table: "rachao_confirmacoes",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            // Backfill não-destrutivo: gera uma chave a partir do nome já existente
            // (linhas antigas não têm empresa) para não colidirem no default "" antes do índice único.
            migrationBuilder.Sql(
                "UPDATE rachao_confirmacoes SET \"ChaveUnica\" = lower(trim(\"Nome\")) WHERE \"ChaveUnica\" = '';");

            migrationBuilder.CreateIndex(
                name: "IX_RachaoConfirmacao_Evento_ChaveUnica",
                table: "rachao_confirmacoes",
                columns: new[] { "RachaoEventoId", "ChaveUnica" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RachaoConfirmacao_Evento_ChaveUnica",
                table: "rachao_confirmacoes");

            migrationBuilder.DropColumn(
                name: "ChaveUnica",
                table: "rachao_confirmacoes");

            migrationBuilder.DropColumn(
                name: "Empresa",
                table: "rachao_confirmacoes");
        }
    }
}
