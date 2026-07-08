using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Boleiroffice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndicacaoEmpresa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IndicadaPorEmpresaId",
                table: "empresas",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empresa_IndicadaPor",
                table: "empresas",
                column: "IndicadaPorEmpresaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Empresa_IndicadaPor",
                table: "empresas");

            migrationBuilder.DropColumn(
                name: "IndicadaPorEmpresaId",
                table: "empresas");
        }
    }
}
