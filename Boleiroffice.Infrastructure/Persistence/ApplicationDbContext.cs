using Boleiroffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Time> Times => Set<Time>();
    public DbSet<Jogador> Jogadores => Set<Jogador>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Desafio> Desafios => Set<Desafio>();
    public DbSet<ConfirmacaoPresenca> ConfirmacoesPresenca => Set<ConfirmacaoPresenca>();
    public DbSet<GolPartida> GolsPartida => Set<GolPartida>();
    public DbSet<VotoMvp> VotosMvp => Set<VotoMvp>();
    public DbSet<ReacaoFeed> ReacoesFeed => Set<ReacaoFeed>();
    public DbSet<DisponibilidadeTime> DisponibilidadesTime => Set<DisponibilidadeTime>();
    public DbSet<MensagemDesafio> MensagensDesafio => Set<MensagemDesafio>();
    public DbSet<Temporada> Temporadas => Set<Temporada>();
    public DbSet<FinanceiroItem> FinanceiroItens => Set<FinanceiroItem>();
    public DbSet<ComentarioFeed> ComentariosFeed => Set<ComentarioFeed>();
    public DbSet<PostMural> PostsMural => Set<PostMural>();
    public DbSet<Quadra> Quadras => Set<Quadra>();
    public DbSet<AvaliacaoQuadra> AvaliacoesQuadra => Set<AvaliacaoQuadra>();
    public DbSet<Notificacao> Notificacoes => Set<Notificacao>();
    public DbSet<Torneio> Torneios => Set<Torneio>();
    public DbSet<TorneioInscricao> TorneioInscricoes => Set<TorneioInscricao>();
    public DbSet<PartidaTorneio> PartidasTorneio => Set<PartidaTorneio>();
    public DbSet<JogadorAmistoso> JogadoresAmistoso => Set<JogadorAmistoso>();
    public DbSet<TimeAmistoso> TimesAmistoso => Set<TimeAmistoso>();
    public DbSet<TimeAmistosoJogador> TimesAmistosoJogadores => Set<TimeAmistosoJogador>();
    public DbSet<PartidaAmistoso> PartidasAmistoso => Set<PartidaAmistoso>();
    public DbSet<GolAmistoso> GolsAmistoso => Set<GolAmistoso>();
    public DbSet<RachaoEvento> RachaoEventos => Set<RachaoEvento>();
    public DbSet<RachaoConfirmacao> RachaoConfirmacoes => Set<RachaoConfirmacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.ToTable("empresas");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nome).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Cnpj).HasMaxLength(14).IsRequired();
            entity.Property(x => x.Bairro).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Cidade).HasMaxLength(120).IsRequired();
            entity.Property(x => x.LogoUrl).HasMaxLength(500).IsRequired(false);
            entity.Property(x => x.DataCriacao).IsRequired();
            entity.HasIndex(x => x.Nome).IsUnique();
            entity.HasIndex(x => x.Cnpj).IsUnique();
        });

        modelBuilder.Entity<Time>(entity =>
        {
            entity.ToTable("times");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nome).HasMaxLength(120).IsRequired();
            entity.Property(x => x.BairroBase).HasMaxLength(120).IsRequired();
            entity.Property(x => x.FotoUrl).HasMaxLength(500).IsRequired(false);
            entity.Property(x => x.Nivel).IsRequired();
            entity.Property(x => x.DataCriacao).IsRequired();
            entity.Property(x => x.PenalidadesCount).HasDefaultValue(0).IsRequired();
            entity.Property(x => x.BloqueioAte).IsRequired(false);
            entity.HasIndex(x => x.EmpresaId).HasDatabaseName("IX_Time_EmpresaId");
            entity.HasOne(x => x.Empresa)
                .WithMany(x => x.Times)
                .HasForeignKey(x => x.EmpresaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Jogador>(entity =>
        {
            entity.ToTable("jogadores");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nome).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Posicao).HasMaxLength(60).IsRequired();
            entity.Property(x => x.NumeroCamisa).IsRequired();
            entity.HasIndex(x => x.TimeId).HasDatabaseName("IX_Jogador_TimeId");
            entity.HasIndex(x => new { x.TimeId, x.NumeroCamisa }).IsUnique();
            entity.HasOne(x => x.Time)
                .WithMany(x => x.Jogadores)
                .HasForeignKey(x => x.TimeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nome).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(180).IsRequired();
            entity.Property(x => x.SenhaHash).HasMaxLength(255).IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();
            entity.HasOne(x => x.Empresa)
                .WithMany(x => x.Usuarios)
                .HasForeignKey(x => x.EmpresaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Desafio>(entity =>
        {
            entity.ToTable("desafios");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Local).HasMaxLength(180).IsRequired();
            entity.Property(x => x.Bairro).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Nivel).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
            entity.Property(x => x.DataCriacao).IsRequired();
            entity.Property(x => x.DataPropostaResultado).IsRequired(false);
            entity.Property(x => x.DataAceite).IsRequired(false);
            entity.Property(x => x.DataCancelamento).IsRequired(false);
            entity.Property(x => x.DataResultadoConfirmadoEm).IsRequired(false);
            entity.HasIndex(x => x.Status).HasDatabaseName("IX_Desafio_Status");
            entity.HasIndex(x => x.Bairro).HasDatabaseName("IX_Desafio_Bairro");
            entity.HasIndex(x => x.DataJogo).HasDatabaseName("IX_Desafio_DataJogo");
            entity.HasOne(x => x.TimeCriador)
                .WithMany(x => x.DesafiosCriados)
                .HasForeignKey(x => x.TimeCriadorId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.TimeDesafiante)
                .WithMany(x => x.DesafiosRecebidos)
                .HasForeignKey(x => x.TimeDesafianteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ConfirmacaoPresenca>(entity =>
        {
            entity.ToTable("confirmacoes_presenca");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.JogadorId, x.DesafioId }).IsUnique();
            entity.HasOne(x => x.Jogador)
                .WithMany(x => x.ConfirmacoesPresenca)
                .HasForeignKey(x => x.JogadorId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Desafio)
                .WithMany(x => x.ConfirmacoesPresenca)
                .HasForeignKey(x => x.DesafioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<VotoMvp>(entity =>
        {
            entity.ToTable("votos_mvp");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.DataVoto).IsRequired();
            entity.HasIndex(x => new { x.DesafioId, x.VotantePorEmpresaId }).IsUnique();
            entity.HasOne(x => x.Desafio)
                .WithMany()
                .HasForeignKey(x => x.DesafioId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.JogadorVotado)
                .WithMany()
                .HasForeignKey(x => x.JogadorVotadoId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.VotantePorEmpresa)
                .WithMany()
                .HasForeignKey(x => x.VotantePorEmpresaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ReacaoFeed>(entity =>
        {
            entity.ToTable("reacoes_feed");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Emoji).HasMaxLength(10).IsRequired();
            entity.Property(x => x.DataCriacao).IsRequired();
            entity.HasIndex(x => new { x.DesafioId, x.EmpresaId }).IsUnique();
            entity.HasOne(x => x.Desafio)
                .WithMany()
                .HasForeignKey(x => x.DesafioId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Empresa)
                .WithMany()
                .HasForeignKey(x => x.EmpresaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DisponibilidadeTime>(entity =>
        {
            entity.ToTable("disponibilidades_time");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.NomeLocal).HasMaxLength(200).IsRequired(false);
            entity.Property(x => x.Bairro).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Cidade).HasMaxLength(120).IsRequired();
            entity.Property(x => x.EnderecoCompleto).HasMaxLength(300).IsRequired(false);
            entity.HasIndex(x => new { x.TimeId, x.DiaSemana, x.Horario }).IsUnique();
            entity.HasOne(x => x.Time).WithMany().HasForeignKey(x => x.TimeId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MensagemDesafio>(entity =>
        {
            entity.ToTable("mensagens_desafio");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Conteudo).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.NomeEmpresa).HasMaxLength(120).IsRequired();
            entity.HasIndex(x => x.DesafioId).HasDatabaseName("IX_MensagemDesafio_DesafioId");
            entity.HasOne(x => x.Desafio).WithMany().HasForeignKey(x => x.DesafioId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Temporada>(entity =>
        {
            entity.ToTable("temporadas");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nome).HasMaxLength(120).IsRequired();
            entity.HasIndex(x => x.Nome).IsUnique();
        });

        modelBuilder.Entity<FinanceiroItem>(entity =>
        {
            entity.ToTable("financeiro_itens");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Descricao).HasMaxLength(300).IsRequired();
            entity.Property(x => x.Valor).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(x => x.Tipo).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Categoria).HasMaxLength(60).IsRequired(false);
            entity.HasOne(x => x.Time).WithMany().HasForeignKey(x => x.TimeId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ComentarioFeed>(entity =>
        {
            entity.ToTable("comentarios_feed");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Conteudo).HasMaxLength(500).IsRequired();
            entity.Property(x => x.NomeEmpresa).HasMaxLength(120).IsRequired();
            entity.HasIndex(x => x.DesafioId).HasDatabaseName("IX_ComentarioFeed_DesafioId");
            entity.HasOne(x => x.Desafio).WithMany().HasForeignKey(x => x.DesafioId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PostMural>(entity =>
        {
            entity.ToTable("posts_mural");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Titulo).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Conteudo).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.NomeAutor).HasMaxLength(120).IsRequired();
            entity.HasIndex(x => x.EmpresaId).HasDatabaseName("IX_PostMural_EmpresaId");
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Quadra>(entity =>
        {
            entity.ToTable("quadras");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Endereco).HasMaxLength(300).IsRequired();
            entity.Property(x => x.Bairro).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Cidade).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Estado).HasMaxLength(2).IsRequired(false);
            entity.Property(x => x.Cep).HasMaxLength(8).IsRequired(false);
            entity.HasIndex(x => x.Bairro).HasDatabaseName("IX_Quadra_Bairro");
        });

        modelBuilder.Entity<AvaliacaoQuadra>(entity =>
        {
            entity.ToTable("avaliacoes_quadra");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nota).IsRequired();
            entity.Property(x => x.Comentario).HasMaxLength(500).IsRequired(false);
            entity.HasIndex(x => new { x.QuadraId, x.EmpresaId }).IsUnique();
            entity.HasOne(x => x.Quadra).WithMany(x => x.Avaliacoes).HasForeignKey(x => x.QuadraId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Notificacao>(entity =>
        {
            entity.ToTable("notificacoes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Tipo).HasMaxLength(60).IsRequired();
            entity.Property(x => x.Titulo).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Mensagem).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Url).HasMaxLength(300).IsRequired(false);
            entity.HasIndex(x => new { x.EmpresaId, x.Lida }).HasDatabaseName("IX_Notificacao_Empresa_Lida");
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Torneio>(entity =>
        {
            entity.ToTable("torneios");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Descricao).HasMaxLength(1000).IsRequired(false);
            entity.HasOne(x => x.EmpresaOrganizadora).WithMany().HasForeignKey(x => x.EmpresaOrganizadoraId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TorneioInscricao>(entity =>
        {
            entity.ToTable("torneio_inscricoes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.GrupoLetra).HasMaxLength(1).IsRequired(false);
            entity.HasIndex(x => new { x.TorneioId, x.TimeId }).IsUnique();
            entity.HasOne(x => x.Torneio).WithMany(x => x.Inscricoes).HasForeignKey(x => x.TorneioId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Time).WithMany().HasForeignKey(x => x.TimeId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PartidaTorneio>(entity =>
        {
            entity.ToTable("partidas_torneio");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Fase).HasMaxLength(60).IsRequired();
            entity.Property(x => x.Local).HasMaxLength(200).IsRequired(false);
            entity.HasOne(x => x.Torneio).WithMany(x => x.Partidas).HasForeignKey(x => x.TorneioId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.TimeMandante).WithMany().HasForeignKey(x => x.TimeMandanteId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.TimeVisitante).WithMany().HasForeignKey(x => x.TimeVisitanteId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<GolPartida>(entity =>
        {
            entity.ToTable("gols_partida");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.NomeAutor).HasMaxLength(120).IsRequired();
            entity.Property(x => x.QuantidadeGols).IsRequired();
            entity.Property(x => x.DataCriacao).IsRequired();
            entity.HasIndex(x => x.DesafioId).HasDatabaseName("IX_GolPartida_DesafioId");
            entity.HasIndex(x => x.TimeId).HasDatabaseName("IX_GolPartida_TimeId");
            entity.HasOne(x => x.Desafio)
                .WithMany(x => x.Gols)
                .HasForeignKey(x => x.DesafioId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Time)
                .WithMany(x => x.GolsMarcados)
                .HasForeignKey(x => x.TimeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<JogadorAmistoso>(entity =>
        {
            entity.ToTable("jogadores_amistoso");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nome).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Ativo).HasDefaultValue(true).IsRequired();
            entity.Property(x => x.PagouMensalidade).HasDefaultValue(false).IsRequired();
            entity.Property(x => x.DataCriacao).IsRequired();
            entity.HasIndex(x => x.EmpresaId).HasDatabaseName("IX_JogadorAmistoso_EmpresaId");
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TimeAmistoso>(entity =>
        {
            entity.ToTable("times_amistoso");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nome).HasMaxLength(60).IsRequired();
            entity.Property(x => x.Ordem).IsRequired();
            entity.Property(x => x.DataSorteio).IsRequired();
            entity.HasIndex(x => x.EmpresaId).HasDatabaseName("IX_TimeAmistoso_EmpresaId");
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TimeAmistosoJogador>(entity =>
        {
            entity.ToTable("times_amistoso_jogadores");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nome).HasMaxLength(120).IsRequired();
            entity.HasIndex(x => x.TimeAmistosoId).HasDatabaseName("IX_TimeAmistosoJogador_TimeId");
            entity.HasOne(x => x.TimeAmistoso).WithMany(x => x.Jogadores).HasForeignKey(x => x.TimeAmistosoId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PartidaAmistoso>(entity =>
        {
            entity.ToTable("partidas_amistoso");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Time1Nome).HasMaxLength(60).IsRequired();
            entity.Property(x => x.Time2Nome).HasMaxLength(60).IsRequired();
            entity.Property(x => x.Time1Gols).IsRequired();
            entity.Property(x => x.Time2Gols).IsRequired();
            entity.Property(x => x.DataInicio).IsRequired();
            entity.Property(x => x.DataFim).IsRequired(false);
            entity.Property(x => x.DuracaoSegundos).IsRequired();
            entity.Property(x => x.Finalizada).HasDefaultValue(false).IsRequired();
            entity.HasIndex(x => new { x.EmpresaId, x.Finalizada }).HasDatabaseName("IX_PartidaAmistoso_Empresa_Finalizada");
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GolAmistoso>(entity =>
        {
            entity.ToTable("gols_amistoso");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TimeNumero).IsRequired();
            entity.Property(x => x.AutorNome).HasMaxLength(120).IsRequired();
            entity.Property(x => x.AssistenteNome).HasMaxLength(120).IsRequired(false);
            entity.Property(x => x.DataCriacao).IsRequired();
            entity.HasIndex(x => x.EmpresaId).HasDatabaseName("IX_GolAmistoso_EmpresaId");
            entity.HasIndex(x => x.PartidaAmistosoId).HasDatabaseName("IX_GolAmistoso_PartidaId");
            entity.HasOne(x => x.Partida).WithMany(x => x.Gols).HasForeignKey(x => x.PartidaAmistosoId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RachaoEvento>(entity =>
        {
            entity.ToTable("rachao_eventos");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Token).HasMaxLength(40).IsRequired();
            entity.Property(x => x.HorarioEvento).IsRequired();
            entity.Property(x => x.NumeroTimes).IsRequired();
            entity.Property(x => x.DataCriacao).IsRequired();
            entity.HasIndex(x => x.Token).IsUnique().HasDatabaseName("IX_RachaoEvento_Token");
            entity.HasIndex(x => x.EmpresaId).HasDatabaseName("IX_RachaoEvento_EmpresaId");
            entity.HasOne(x => x.Empresa).WithMany().HasForeignKey(x => x.EmpresaId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RachaoConfirmacao>(entity =>
        {
            entity.ToTable("rachao_confirmacoes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nome).HasMaxLength(120).IsRequired();
            entity.Property(x => x.DataCriacao).IsRequired();
            entity.HasIndex(x => x.RachaoEventoId).HasDatabaseName("IX_RachaoConfirmacao_EventoId");
            entity.HasOne(x => x.Evento).WithMany(x => x.Confirmacoes).HasForeignKey(x => x.RachaoEventoId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
