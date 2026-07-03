using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Boleiroffice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModoAmistoso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "jogadores_amistoso",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    PagouMensalidade = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jogadores_amistoso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_jogadores_amistoso_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "partidas_amistoso",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Time1Nome = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Time2Nome = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Time1Gols = table.Column<int>(type: "integer", nullable: false),
                    Time2Gols = table.Column<int>(type: "integer", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataFim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DuracaoSegundos = table.Column<int>(type: "integer", nullable: false),
                    Finalizada = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_partidas_amistoso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_partidas_amistoso_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "times_amistoso",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    DataSorteio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_times_amistoso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_times_amistoso_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gols_amistoso",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartidaAmistosoId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeNumero = table.Column<int>(type: "integer", nullable: false),
                    AutorNome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    AssistenteNome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gols_amistoso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gols_amistoso_partidas_amistoso_PartidaAmistosoId",
                        column: x => x.PartidaAmistosoId,
                        principalTable: "partidas_amistoso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "times_amistoso_jogadores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeAmistosoId = table.Column<Guid>(type: "uuid", nullable: false),
                    JogadorAmistosoId = table.Column<Guid>(type: "uuid", nullable: true),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_times_amistoso_jogadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_times_amistoso_jogadores_times_amistoso_TimeAmistosoId",
                        column: x => x.TimeAmistosoId,
                        principalTable: "times_amistoso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GolAmistoso_EmpresaId",
                table: "gols_amistoso",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_GolAmistoso_PartidaId",
                table: "gols_amistoso",
                column: "PartidaAmistosoId");

            migrationBuilder.CreateIndex(
                name: "IX_JogadorAmistoso_EmpresaId",
                table: "jogadores_amistoso",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_PartidaAmistoso_Empresa_Finalizada",
                table: "partidas_amistoso",
                columns: new[] { "EmpresaId", "Finalizada" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeAmistoso_EmpresaId",
                table: "times_amistoso",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeAmistosoJogador_TimeId",
                table: "times_amistoso_jogadores",
                column: "TimeAmistosoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gols_amistoso");

            migrationBuilder.DropTable(
                name: "jogadores_amistoso");

            migrationBuilder.DropTable(
                name: "times_amistoso_jogadores");

            migrationBuilder.DropTable(
                name: "partidas_amistoso");

            migrationBuilder.DropTable(
                name: "times_amistoso");
        }
    }
}
