using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Boleiroffice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "empresas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    Bairro = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Cidade = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_empresas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "quadras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Endereco = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Bairro = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Cidade = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Estado = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Capacidade = table.Column<int>(type: "integer", nullable: true),
                    Cep = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    TipoGrama = table.Column<int>(type: "integer", nullable: false),
                    Iluminacao = table.Column<bool>(type: "boolean", nullable: false),
                    Vestiario = table.Column<bool>(type: "boolean", nullable: false),
                    FotoUrl = table.Column<string>(type: "text", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quadras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "temporadas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    DataInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    DataFim = table.Column<DateOnly>(type: "date", nullable: false),
                    Ativa = table.Column<bool>(type: "boolean", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_temporadas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "notificacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Mensagem = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Url = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Lida = table.Column<bool>(type: "boolean", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notificacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notificacoes_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "posts_mural",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Conteudo = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    NomeAutor = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    DataPublicacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_posts_mural", x => x.Id);
                    table.ForeignKey(
                        name: "FK_posts_mural_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "times",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nivel = table.Column<int>(type: "integer", nullable: false),
                    BairroBase = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    FotoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    EscudoShape = table.Column<int>(type: "integer", nullable: false),
                    CorPrimaria = table.Column<string>(type: "text", nullable: false),
                    CorSecundaria = table.Column<string>(type: "text", nullable: false),
                    Cep = table.Column<string>(type: "text", nullable: true),
                    Cidade = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    PenalidadesCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    BloqueioAte = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_times", x => x.Id);
                    table.ForeignKey(
                        name: "FK_times_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "torneios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Formato = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DataInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    DataFim = table.Column<DateOnly>(type: "date", nullable: true),
                    EmpresaOrganizadoraId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_torneios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_torneios_empresas_EmpresaOrganizadoraId",
                        column: x => x.EmpresaOrganizadoraId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    SenhaHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_usuarios_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "avaliacoes_quadra",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuadraId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nota = table.Column<int>(type: "integer", nullable: false),
                    Comentario = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_avaliacoes_quadra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_avaliacoes_quadra_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_avaliacoes_quadra_quadras_QuadraId",
                        column: x => x.QuadraId,
                        principalTable: "quadras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "desafios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeCriadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeDesafianteId = table.Column<Guid>(type: "uuid", nullable: true),
                    DataJogo = table.Column<DateOnly>(type: "date", nullable: false),
                    HoraJogo = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Local = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    Bairro = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Nivel = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    PlacarCriador = table.Column<int>(type: "integer", nullable: true),
                    PlacarDesafiante = table.Column<int>(type: "integer", nullable: true),
                    PlacarCriadorProposto = table.Column<int>(type: "integer", nullable: true),
                    PlacarDesafianteProposto = table.Column<int>(type: "integer", nullable: true),
                    ResultadoPropostoPorTimeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ResultadoConfirmadoPeloCriador = table.Column<bool>(type: "boolean", nullable: false),
                    ResultadoConfirmadoPeloDesafiante = table.Column<bool>(type: "boolean", nullable: false),
                    DataPropostaResultado = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataAceite = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataCancelamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataResultadoConfirmadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_desafios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_desafios_times_TimeCriadorId",
                        column: x => x.TimeCriadorId,
                        principalTable: "times",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_desafios_times_TimeDesafianteId",
                        column: x => x.TimeDesafianteId,
                        principalTable: "times",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "disponibilidades_time",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DiaSemana = table.Column<int>(type: "integer", nullable: false),
                    Horario = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    NomeLocal = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Bairro = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Cidade = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    EnderecoCompleto = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_disponibilidades_time", x => x.Id);
                    table.ForeignKey(
                        name: "FK_disponibilidades_time_times_TimeId",
                        column: x => x.TimeId,
                        principalTable: "times",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "financeiro_itens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Categoria = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    DataVencimento = table.Column<DateOnly>(type: "date", nullable: false),
                    Pago = table.Column<bool>(type: "boolean", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_financeiro_itens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_financeiro_itens_times_TimeId",
                        column: x => x.TimeId,
                        principalTable: "times",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "jogadores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Posicao = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    NumeroCamisa = table.Column<int>(type: "integer", nullable: false),
                    TimeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jogadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_jogadores_times_TimeId",
                        column: x => x.TimeId,
                        principalTable: "times",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "partidas_torneio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TorneioId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeMandanteId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeVisitanteId = table.Column<Guid>(type: "uuid", nullable: true),
                    PlacarMandante = table.Column<int>(type: "integer", nullable: true),
                    PlacarVisitante = table.Column<int>(type: "integer", nullable: true),
                    Fase = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DataJogo = table.Column<DateOnly>(type: "date", nullable: true),
                    HoraJogo = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    Local = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_partidas_torneio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_partidas_torneio_times_TimeMandanteId",
                        column: x => x.TimeMandanteId,
                        principalTable: "times",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_partidas_torneio_times_TimeVisitanteId",
                        column: x => x.TimeVisitanteId,
                        principalTable: "times",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_partidas_torneio_torneios_TorneioId",
                        column: x => x.TorneioId,
                        principalTable: "torneios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "torneio_inscricoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TorneioId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeId = table.Column<Guid>(type: "uuid", nullable: false),
                    GrupoLetra = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    DataInscricao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_torneio_inscricoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_torneio_inscricoes_times_TimeId",
                        column: x => x.TimeId,
                        principalTable: "times",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_torneio_inscricoes_torneios_TorneioId",
                        column: x => x.TorneioId,
                        principalTable: "torneios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comentarios_feed",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DesafioId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    NomeEmpresa = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Conteudo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DataComentario = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comentarios_feed", x => x.Id);
                    table.ForeignKey(
                        name: "FK_comentarios_feed_desafios_DesafioId",
                        column: x => x.DesafioId,
                        principalTable: "desafios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comentarios_feed_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gols_partida",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DesafioId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeId = table.Column<Guid>(type: "uuid", nullable: false),
                    NomeAutor = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    QuantidadeGols = table.Column<int>(type: "integer", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gols_partida", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gols_partida_desafios_DesafioId",
                        column: x => x.DesafioId,
                        principalTable: "desafios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gols_partida_times_TimeId",
                        column: x => x.TimeId,
                        principalTable: "times",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "mensagens_desafio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DesafioId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    NomeEmpresa = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Conteudo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mensagens_desafio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mensagens_desafio_desafios_DesafioId",
                        column: x => x.DesafioId,
                        principalTable: "desafios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mensagens_desafio_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reacoes_feed",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DesafioId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Emoji = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reacoes_feed", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reacoes_feed_desafios_DesafioId",
                        column: x => x.DesafioId,
                        principalTable: "desafios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reacoes_feed_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "confirmacoes_presenca",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JogadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    DesafioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Confirmado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_confirmacoes_presenca", x => x.Id);
                    table.ForeignKey(
                        name: "FK_confirmacoes_presenca_desafios_DesafioId",
                        column: x => x.DesafioId,
                        principalTable: "desafios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_confirmacoes_presenca_jogadores_JogadorId",
                        column: x => x.JogadorId,
                        principalTable: "jogadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "votos_mvp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DesafioId = table.Column<Guid>(type: "uuid", nullable: false),
                    JogadorVotadoId = table.Column<Guid>(type: "uuid", nullable: false),
                    VotantePorEmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataVoto = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_votos_mvp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_votos_mvp_desafios_DesafioId",
                        column: x => x.DesafioId,
                        principalTable: "desafios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_votos_mvp_empresas_VotantePorEmpresaId",
                        column: x => x.VotantePorEmpresaId,
                        principalTable: "empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_votos_mvp_jogadores_JogadorVotadoId",
                        column: x => x.JogadorVotadoId,
                        principalTable: "jogadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_avaliacoes_quadra_EmpresaId",
                table: "avaliacoes_quadra",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_avaliacoes_quadra_QuadraId_EmpresaId",
                table: "avaliacoes_quadra",
                columns: new[] { "QuadraId", "EmpresaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComentarioFeed_DesafioId",
                table: "comentarios_feed",
                column: "DesafioId");

            migrationBuilder.CreateIndex(
                name: "IX_comentarios_feed_EmpresaId",
                table: "comentarios_feed",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_confirmacoes_presenca_DesafioId",
                table: "confirmacoes_presenca",
                column: "DesafioId");

            migrationBuilder.CreateIndex(
                name: "IX_confirmacoes_presenca_JogadorId_DesafioId",
                table: "confirmacoes_presenca",
                columns: new[] { "JogadorId", "DesafioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Desafio_Bairro",
                table: "desafios",
                column: "Bairro");

            migrationBuilder.CreateIndex(
                name: "IX_Desafio_DataJogo",
                table: "desafios",
                column: "DataJogo");

            migrationBuilder.CreateIndex(
                name: "IX_Desafio_Status",
                table: "desafios",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_desafios_TimeCriadorId",
                table: "desafios",
                column: "TimeCriadorId");

            migrationBuilder.CreateIndex(
                name: "IX_desafios_TimeDesafianteId",
                table: "desafios",
                column: "TimeDesafianteId");

            migrationBuilder.CreateIndex(
                name: "IX_disponibilidades_time_TimeId_DiaSemana_Horario",
                table: "disponibilidades_time",
                columns: new[] { "TimeId", "DiaSemana", "Horario" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_empresas_Cnpj",
                table: "empresas",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_empresas_Nome",
                table: "empresas",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_financeiro_itens_TimeId",
                table: "financeiro_itens",
                column: "TimeId");

            migrationBuilder.CreateIndex(
                name: "IX_GolPartida_DesafioId",
                table: "gols_partida",
                column: "DesafioId");

            migrationBuilder.CreateIndex(
                name: "IX_GolPartida_TimeId",
                table: "gols_partida",
                column: "TimeId");

            migrationBuilder.CreateIndex(
                name: "IX_Jogador_TimeId",
                table: "jogadores",
                column: "TimeId");

            migrationBuilder.CreateIndex(
                name: "IX_jogadores_TimeId_NumeroCamisa",
                table: "jogadores",
                columns: new[] { "TimeId", "NumeroCamisa" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MensagemDesafio_DesafioId",
                table: "mensagens_desafio",
                column: "DesafioId");

            migrationBuilder.CreateIndex(
                name: "IX_mensagens_desafio_EmpresaId",
                table: "mensagens_desafio",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacao_Empresa_Lida",
                table: "notificacoes",
                columns: new[] { "EmpresaId", "Lida" });

            migrationBuilder.CreateIndex(
                name: "IX_partidas_torneio_TimeMandanteId",
                table: "partidas_torneio",
                column: "TimeMandanteId");

            migrationBuilder.CreateIndex(
                name: "IX_partidas_torneio_TimeVisitanteId",
                table: "partidas_torneio",
                column: "TimeVisitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_partidas_torneio_TorneioId",
                table: "partidas_torneio",
                column: "TorneioId");

            migrationBuilder.CreateIndex(
                name: "IX_PostMural_EmpresaId",
                table: "posts_mural",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Quadra_Bairro",
                table: "quadras",
                column: "Bairro");

            migrationBuilder.CreateIndex(
                name: "IX_reacoes_feed_DesafioId_EmpresaId",
                table: "reacoes_feed",
                columns: new[] { "DesafioId", "EmpresaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reacoes_feed_EmpresaId",
                table: "reacoes_feed",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_temporadas_Nome",
                table: "temporadas",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Time_EmpresaId",
                table: "times",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_torneio_inscricoes_TimeId",
                table: "torneio_inscricoes",
                column: "TimeId");

            migrationBuilder.CreateIndex(
                name: "IX_torneio_inscricoes_TorneioId_TimeId",
                table: "torneio_inscricoes",
                columns: new[] { "TorneioId", "TimeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_torneios_EmpresaOrganizadoraId",
                table: "torneios",
                column: "EmpresaOrganizadoraId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Email",
                table: "usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_EmpresaId",
                table: "usuarios",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_votos_mvp_DesafioId_VotantePorEmpresaId",
                table: "votos_mvp",
                columns: new[] { "DesafioId", "VotantePorEmpresaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_votos_mvp_JogadorVotadoId",
                table: "votos_mvp",
                column: "JogadorVotadoId");

            migrationBuilder.CreateIndex(
                name: "IX_votos_mvp_VotantePorEmpresaId",
                table: "votos_mvp",
                column: "VotantePorEmpresaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "avaliacoes_quadra");

            migrationBuilder.DropTable(
                name: "comentarios_feed");

            migrationBuilder.DropTable(
                name: "confirmacoes_presenca");

            migrationBuilder.DropTable(
                name: "disponibilidades_time");

            migrationBuilder.DropTable(
                name: "financeiro_itens");

            migrationBuilder.DropTable(
                name: "gols_partida");

            migrationBuilder.DropTable(
                name: "mensagens_desafio");

            migrationBuilder.DropTable(
                name: "notificacoes");

            migrationBuilder.DropTable(
                name: "partidas_torneio");

            migrationBuilder.DropTable(
                name: "posts_mural");

            migrationBuilder.DropTable(
                name: "reacoes_feed");

            migrationBuilder.DropTable(
                name: "temporadas");

            migrationBuilder.DropTable(
                name: "torneio_inscricoes");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "votos_mvp");

            migrationBuilder.DropTable(
                name: "quadras");

            migrationBuilder.DropTable(
                name: "torneios");

            migrationBuilder.DropTable(
                name: "desafios");

            migrationBuilder.DropTable(
                name: "jogadores");

            migrationBuilder.DropTable(
                name: "times");

            migrationBuilder.DropTable(
                name: "empresas");
        }
    }
}
