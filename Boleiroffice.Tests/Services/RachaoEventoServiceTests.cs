using Boleiroffice.Application.DTOs.Rachao;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Application.Services;
using Boleiroffice.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace Boleiroffice.Tests.Services;

public sealed class RachaoEventoServiceTests
{
    private readonly IRachaoEventoRepository _repo;
    private readonly IAmistosoRepository _amistosoRepo;
    private readonly INotificationService _notifier;
    private readonly RachaoEventoService _sut;

    private static readonly Guid EmpresaId = Guid.NewGuid();
    private const string ValidCpf = "52998224725";

    public RachaoEventoServiceTests()
    {
        _repo = Substitute.For<IRachaoEventoRepository>();
        _amistosoRepo = Substitute.For<IAmistosoRepository>();
        _notifier = Substitute.For<INotificationService>();

        _sut = new RachaoEventoService(_repo, _amistosoRepo, _notifier);
    }

    private static RachaoEvento MakeEvento(int jogadoresPorTime = 6, bool sorteioFeito = false) => new()
    {
        Id = Guid.NewGuid(),
        EmpresaId = EmpresaId,
        Token = "tok123",
        HorarioEvento = DateTime.UtcNow,
        JogadoresPorTime = jogadoresPorTime,
        SorteioFeito = sorteioFeito,
        Confirmacoes = new List<RachaoConfirmacao>()
    };

    private static RachaoConfirmacao MakeConfirmacao(RachaoEvento evento, string nome, bool goleiro = false) => new()
    {
        Id = Guid.NewGuid(),
        RachaoEventoId = evento.Id,
        Nome = nome,
        Empresa = null,
        Cpf = ValidCpf,
        Goleiro = goleiro,
        DataCriacao = DateTime.UtcNow
    };

    // ─── ConfirmarAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task ConfirmarAsync_ComCpfInvalido_LancaBusinessException()
    {
        var evento = MakeEvento();
        _repo.GetByTokenAsync(evento.Token, default).Returns(evento);

        var request = new ConfirmarPresencaRequest("Joao Silva", "Zitec", "11111111111", false);

        var act = () => _sut.ConfirmarAsync(evento.Token, request, default);

        await act.Should().ThrowAsync<BusinessException>();
        await _repo.DidNotReceive().AddConfirmacaoAsync(Arg.Any<RachaoConfirmacao>(), default);
    }

    [Fact]
    public async Task ConfirmarAsync_ComCpfJaConfirmadoPorOutroNome_LancaBusinessException()
    {
        var evento = MakeEvento();
        evento.Confirmacoes.Add(MakeConfirmacao(evento, "Joao Silva"));
        _repo.GetByTokenAsync(evento.Token, default).Returns(evento);

        var request = new ConfirmarPresencaRequest("Joao S.", "Zitec", ValidCpf, false);

        var act = () => _sut.ConfirmarAsync(evento.Token, request, default);

        await act.Should().ThrowAsync<BusinessException>();
        await _repo.DidNotReceive().AddConfirmacaoAsync(Arg.Any<RachaoConfirmacao>(), default);
    }

    [Fact]
    public async Task ConfirmarAsync_ComCpfValido_SalvaConfirmacaoComGoleiro()
    {
        var evento = MakeEvento();
        _repo.GetByTokenAsync(evento.Token, default).Returns(evento);

        var request = new ConfirmarPresencaRequest("Joao Silva", "Zitec", "529.982.247-25", true);

        var response = await _sut.ConfirmarAsync(evento.Token, request, default);

        response.Should().NotBeNull();
        response!.Confirmados.Should().ContainSingle(c => c.Nome == "Joao Silva" && c.Goleiro);

        await _repo.Received(1).AddConfirmacaoAsync(
            Arg.Is<RachaoConfirmacao>(c => c.Cpf == ValidCpf && c.Goleiro),
            default);
    }

    // ─── SortearPendentesAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task SortearPendentesAsync_ExcluiGoleirosDoSorteioMasContaSoJogadoresDeLinha()
    {
        var evento = MakeEvento(jogadoresPorTime: 6);
        for (var i = 1; i <= 12; i++)
            evento.Confirmacoes.Add(MakeConfirmacao(evento, $"Linha{i}"));
        evento.Confirmacoes.Add(MakeConfirmacao(evento, "Goleiro1", goleiro: true));
        evento.Confirmacoes.Add(MakeConfirmacao(evento, "Goleiro2", goleiro: true));

        _repo.GetPendentesSorteioAsync(Arg.Any<DateTime>(), default).Returns(new List<RachaoEvento> { evento });
        _amistosoRepo.GetTimesAsync(EmpresaId, default).Returns(new List<TimeAmistoso>());

        var total = await _sut.SortearPendentesAsync(default);

        total.Should().Be(1);
        evento.SorteioFeito.Should().BeTrue();

        await _amistosoRepo.Received(1).ReplaceTimesAsync(
            EmpresaId,
            Arg.Is<IReadOnlyList<TimeAmistoso>>(times =>
                times.Count == 2 &&
                times.SelectMany(t => t.Jogadores).Count() == 12 &&
                times.SelectMany(t => t.Jogadores).All(j => j.Nome.StartsWith("Linha"))),
            default);
    }

    [Fact]
    public async Task SortearPendentesAsync_SoComGoleiros_NaoFormaTime()
    {
        var evento = MakeEvento(jogadoresPorTime: 6);
        evento.Confirmacoes.Add(MakeConfirmacao(evento, "Goleiro1", goleiro: true));
        evento.Confirmacoes.Add(MakeConfirmacao(evento, "Goleiro2", goleiro: true));

        _repo.GetPendentesSorteioAsync(Arg.Any<DateTime>(), default).Returns(new List<RachaoEvento> { evento });

        await _sut.SortearPendentesAsync(default);

        evento.SorteioFeito.Should().BeTrue();
        await _amistosoRepo.DidNotReceive().ReplaceTimesAsync(Arg.Any<Guid>(), Arg.Any<IReadOnlyList<TimeAmistoso>>(), default);
    }

    // ─── GetPublicoAsync / MapPublicoAsync ─────────────────────────────────────

    [Fact]
    public async Task GetPublicoAsync_ComGoleiroConfirmado_NaoAparaceComoExcedente()
    {
        var evento = MakeEvento(jogadoresPorTime: 6, sorteioFeito: true);
        var linhas = Enumerable.Range(1, 6).Select(i => $"Linha{i}").ToList();
        foreach (var nome in linhas)
            evento.Confirmacoes.Add(MakeConfirmacao(evento, nome));
        evento.Confirmacoes.Add(MakeConfirmacao(evento, "Goleiro1", goleiro: true));

        var time = new TimeAmistoso
        {
            Id = Guid.NewGuid(),
            EmpresaId = EmpresaId,
            Nome = "Time 1",
            Ordem = 1,
            Jogadores = linhas.Select(nome => new TimeAmistosoJogador { Id = Guid.NewGuid(), Nome = nome }).ToList()
        };

        _repo.GetByTokenAsync(evento.Token, default).Returns(evento);
        _amistosoRepo.GetTimesAsync(EmpresaId, default).Returns(new List<TimeAmistoso> { time });

        var response = await _sut.GetPublicoAsync(evento.Token, default);

        response.Should().NotBeNull();
        response!.Confirmados.Should().HaveCount(7);
        response.Confirmados.Should().ContainSingle(c => c.Nome == "Goleiro1" && c.Goleiro);
        response.Times.Should().ContainSingle(t => t.Jogadores.Count == 6);
        response.Excedentes.Should().BeEmpty();
    }
}
