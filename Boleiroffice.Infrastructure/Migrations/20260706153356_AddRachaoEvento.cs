using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Boleiroffice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRachaoEvento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rachao_eventos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    HorarioEvento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NumeroTimes = table.Column<int>(type: "integer", nullable: false),
                    SorteioFeito = table.Column<bool>(type: "boolean", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rachao_eventos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rachao_eventos_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rachao_confirmacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RachaoEventoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rachao_confirmacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rachao_confirmacoes_rachao_eventos_RachaoEventoId",
                        column: x => x.RachaoEventoId,
                        principalTable: "rachao_eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RachaoConfirmacao_EventoId",
                table: "rachao_confirmacoes",
                column: "RachaoEventoId");

            migrationBuilder.CreateIndex(
                name: "IX_RachaoEvento_EmpresaId",
                table: "rachao_eventos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_RachaoEvento_Token",
                table: "rachao_eventos",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rachao_confirmacoes");

            migrationBuilder.DropTable(
                name: "rachao_eventos");
        }
    }
}
