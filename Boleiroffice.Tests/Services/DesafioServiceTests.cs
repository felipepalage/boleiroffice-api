using AutoMapper;
using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Desafios;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Application.Mappings;
using Boleiroffice.Application.Services;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Boleiroffice.Tests.Services;

public sealed class DesafioServiceTests
{
    private readonly IDesafioRepository _desafioRepository;
    private readonly ITimeRepository _timeRepository;
    private readonly IMapper _mapper;
    private readonly INotificationService _notifications;
    private readonly DesafioService _sut;

    private static readonly Guid EmpresaCriadoraId = Guid.NewGuid();
    private static readonly Guid EmpresaDesafianteId = Guid.NewGuid();
    private static readonly Guid TimeCriadorId = Guid.NewGuid();
    private static readonly Guid TimeDesafianteId = Guid.NewGuid();

    public DesafioServiceTests()
    {
        _desafioRepository = Substitute.For<IDesafioRepository>();
        _timeRepository = Substitute.For<ITimeRepository>();
        _notifications = Substitute.For<INotificationService>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<ApplicationMappingProfile>());
        _mapper = config.CreateMapper();

        _sut = new DesafioService(_desafioRepository, _timeRepository, _mapper, _notifications);
    }

    private static Time MakeTime(Guid timeId, Guid empresaId) => new()
    {
        Id = timeId,
        Nome = "Time " + timeId.ToString()[..4],
        EmpresaId = empresaId,
        Nivel = 3,
        BairroBase = "Centro",
        Empresa = new Empresa { Id = empresaId, Nome = "Empresa " + empresaId.ToString()[..4] }
    };

    private static CurrentUser MakeUser(Guid empresaId) => new()
    {
        UsuarioId = Guid.NewGuid(),
        EmpresaId = empresaId,
        Nome = "Usuário Teste",
        Email = "teste@teste.com"
    };

    // ─── CreateAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ComDadosValidos_CriaDesafio()
    {
        var timeCriador = MakeTime(TimeCriadorId, EmpresaCriadoraId);
        var timeConvidado = MakeTime(TimeDesafianteId, EmpresaDesafianteId);
        var user = MakeUser(EmpresaCriadoraId);

        _timeRepository.GetByIdAsync(TimeCriadorId, default).Returns(timeCriador);
        _timeRepository.GetByIdAsync(TimeDesafianteId, default).Returns(timeConvidado);
        _desafioRepository.TimeHasConflictAsync(Arg.Any<Guid>(), Arg.Any<DateOnly>(), null, default).Returns(false);

        var request = new DesafioCreateRequest
        {
            TimeCriadorId = TimeCriadorId,
            TimeConvidadoId = TimeDesafianteId,
            DataJogo = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
            HoraJogo = new TimeOnly(15, 0),
            Local = "Arena Central",
            Bairro = "Centro",
            Nivel = 3
        };

        var result = await _sut.CreateAsync(request, user, default);

        result.Should().NotBeNull();
        result.Status.Should().Be(DesafioStatus.Aberto);
        result.TimeCriadorId.Should().Be(TimeCriadorId);
        await _desafioRepository.Received(1).AddAsync(Arg.Any<Desafio>(), default);
    }

    [Fact]
    public async Task CreateAsync_TimeNaoEncontrado_LancaNotFoundException()
    {
        var user = MakeUser(EmpresaCriadoraId);
        _timeRepository.GetByIdAsync(TimeCriadorId, default).ReturnsNull();

        var request = new DesafioCreateRequest
        {
            TimeCriadorId = TimeCriadorId,
            TimeConvidadoId = TimeDesafianteId,
            DataJogo = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
            HoraJogo = new TimeOnly(15, 0),
            Local = "Arena",
            Bairro = "Centro",
            Nivel = 3
        };

        await _sut.Invoking(s => s.CreateAsync(request, user, default))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("*criador*");
    }

    [Fact]
    public async Task CreateAsync_TimeCriadorDeOutraEmpresa_LancaBusinessException()
    {
        var timeCriador = MakeTime(TimeCriadorId, Guid.NewGuid()); // empresa diferente
        var user = MakeUser(EmpresaCriadoraId);

        _timeRepository.GetByIdAsync(TimeCriadorId, default).Returns(timeCriador);
        _timeRepository.GetByIdAsync(TimeDesafianteId, default).Returns(MakeTime(TimeDesafianteId, EmpresaDesafianteId));

        var request = new DesafioCreateRequest
        {
            TimeCriadorId = TimeCriadorId,
            TimeConvidadoId = TimeDesafianteId,
            DataJogo = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
            HoraJogo = new TimeOnly(15, 0),
            Local = "Arena",
            Bairro = "Centro",
            Nivel = 3
        };

        await _sut.Invoking(s => s.CreateAsync(request, user, default))
            .Should().ThrowAsync<BusinessException>()
            .WithMessage("*sua empresa*");
    }

    [Fact]
    public async Task CreateAsync_ConvidaMesmaEmpresa_LancaBusinessException()
    {
        var timeCriador = MakeTime(TimeCriadorId, EmpresaCriadoraId);
        var timeConvidado = MakeTime(TimeDesafianteId, EmpresaCriadoraId); // mesma empresa
        var user = MakeUser(EmpresaCriadoraId);

        _timeRepository.GetByIdAsync(TimeCriadorId, default).Returns(timeCriador);
        _timeRepository.GetByIdAsync(TimeDesafianteId, default).Returns(timeConvidado);

        var request = new DesafioCreateRequest
        {
            TimeCriadorId = TimeCriadorId,
            TimeConvidadoId = TimeDesafianteId,
            DataJogo = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
            HoraJogo = new TimeOnly(15, 0),
            Local = "Arena",
            Bairro = "Centro",
            Nivel = 3
        };

        await _sut.Invoking(s => s.CreateAsync(request, user, default))
            .Should().ThrowAsync<BusinessException>()
            .WithMessage("*outra empresa*");
    }

    [Fact]
    public async Task CreateAsync_TimeCriadorTemConflito_LancaBusinessException()
    {
        var timeCriador = MakeTime(TimeCriadorId, EmpresaCriadoraId);
        var timeConvidado = MakeTime(TimeDesafianteId, EmpresaDesafianteId);
        var user = MakeUser(EmpresaCriadoraId);
        var data = DateOnly.FromDateTime(DateTime.Today.AddDays(7));

        _timeRepository.GetByIdAsync(TimeCriadorId, default).Returns(timeCriador);
        _timeRepository.GetByIdAsync(TimeDesafianteId, default).Returns(timeConvidado);
        _desafioRepository.TimeHasConflictAsync(TimeCriadorId, data, null, default).Returns(true);

        var request = new DesafioCreateRequest
        {
            TimeCriadorId = TimeCriadorId,
            TimeConvidadoId = TimeDesafianteId,
            DataJogo = data,
            HoraJogo = new TimeOnly(15, 0),
            Local = "Arena",
            Bairro = "Centro",
            Nivel = 3
        };

        await _sut.Invoking(s => s.CreateAsync(request, user, default))
            .Should().ThrowAsync<BusinessException>()
            .WithMessage("*desafio marcado*");
    }

    // ─── CancelAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task CancelAsync_DesafioAberto_Cancela()
    {
        var desafio = BuildDesafio(DesafioStatus.Aberto);
        var user = MakeUser(EmpresaCriadoraId);

        _desafioRepository.GetByIdAsync(desafio.Id, default).Returns(desafio);

        var result = await _sut.CancelAsync(desafio.Id, new CancelDesafioRequest(), user, default);

        result.Status.Should().Be(DesafioStatus.Cancelado);
        await _desafioRepository.Received(1).UpdateAsync(desafio, default);
    }

    [Fact]
    public async Task CancelAsync_DesafioFinalizado_LancaBusinessException()
    {
        var desafio = BuildDesafio(DesafioStatus.Finalizado);
        var user = MakeUser(EmpresaCriadoraId);

        _desafioRepository.GetByIdAsync(desafio.Id, default).Returns(desafio);

        await _sut.Invoking(s => s.CancelAsync(desafio.Id, new CancelDesafioRequest(), user, default))
            .Should().ThrowAsync<BusinessException>()
            .WithMessage("*finalizado*");
    }

    [Fact]
    public async Task CancelAsync_DesafioNaoEncontrado_LancaNotFoundException()
    {
        _desafioRepository.GetByIdAsync(Arg.Any<Guid>(), default).ReturnsNull();
        var user = MakeUser(EmpresaCriadoraId);

        await _sut.Invoking(s => s.CancelAsync(Guid.NewGuid(), new CancelDesafioRequest(), user, default))
            .Should().ThrowAsync<NotFoundException>();
    }

    // ─── RegisterResultAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task RegisterResultAsync_PrimeiraProposta_StatusViraResultadoPendente()
    {
        var desafio = BuildDesafio(DesafioStatus.Aceito);
        var user = MakeUser(EmpresaCriadoraId);

        _desafioRepository.GetByIdAsync(desafio.Id, default).Returns(desafio);

        var result = await _sut.RegisterResultAsync(desafio.Id, new RegisterResultRequest { PlacarCriador = 2, PlacarDesafiante = 1 }, user, default);

        result.Status.Should().Be(DesafioStatus.ResultadoPendente);
    }

    [Fact]
    public async Task RegisterResultAsync_AmbosConcordam_StatusViraFinalizado()
    {
        var desafio = BuildDesafio(DesafioStatus.Aceito);

        // Criador propõe 2x1
        desafio.ProporResultado(TimeCriadorId, 2, 1);

        var userDesafiante = MakeUser(EmpresaDesafianteId);
        _desafioRepository.GetByIdAsync(desafio.Id, default).Returns(desafio);

        var result = await _sut.RegisterResultAsync(
            desafio.Id,
            new RegisterResultRequest { PlacarCriador = 2, PlacarDesafiante = 1 },
            userDesafiante,
            default);

        result.Status.Should().Be(DesafioStatus.Finalizado);
        result.PlacarCriador.Should().Be(2);
        result.PlacarDesafiante.Should().Be(1);
    }

    [Fact]
    public async Task RegisterResultAsync_DesafioNaoAceito_LancaBusinessException()
    {
        var desafio = BuildDesafio(DesafioStatus.Aberto);
        var user = MakeUser(EmpresaCriadoraId);

        _desafioRepository.GetByIdAsync(desafio.Id, default).Returns(desafio);

        await _sut.Invoking(s => s.RegisterResultAsync(desafio.Id, new RegisterResultRequest { PlacarCriador = 1, PlacarDesafiante = 0 }, user, default))
            .Should().ThrowAsync<BusinessException>();
    }

    // ─── AcceptAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task AcceptAsync_ComDadosValidos_AceitaDesafio()
    {
        var desafio = BuildDesafio(DesafioStatus.Aberto);
        var timeDesafiante = MakeTime(TimeDesafianteId, EmpresaDesafianteId);
        var user = MakeUser(EmpresaDesafianteId);

        _desafioRepository.GetByIdAsync(desafio.Id, default).Returns(desafio);
        _timeRepository.GetByIdAsync(TimeDesafianteId, default).Returns(timeDesafiante);
        _desafioRepository.TimeHasConflictAsync(TimeDesafianteId, desafio.DataJogo, desafio.Id, default).Returns(false);

        var result = await _sut.AcceptAsync(desafio.Id, new AcceptDesafioRequest { TimeDesafianteId = TimeDesafianteId }, user, default);

        result.Status.Should().Be(DesafioStatus.Aceito);
        await _desafioRepository.Received(1).UpdateAsync(desafio, default);
    }

    [Fact]
    public async Task AcceptAsync_DesafioJaAceito_LancaBusinessException()
    {
        var desafio = BuildDesafio(DesafioStatus.Aceito);
        var user = MakeUser(EmpresaDesafianteId);

        _desafioRepository.GetByIdAsync(desafio.Id, default).Returns(desafio);

        await _sut.Invoking(s => s.AcceptAsync(desafio.Id, new AcceptDesafioRequest { TimeDesafianteId = TimeDesafianteId }, user, default))
            .Should().ThrowAsync<BusinessException>()
            .WithMessage("*pendentes*");
    }

    // ─── GetByIdAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_IdInvalido_LancaBusinessException()
    {
        await _sut.Invoking(s => s.GetByIdAsync(Guid.Empty, default))
            .Should().ThrowAsync<BusinessException>();
    }

    [Fact]
    public async Task GetByIdAsync_NaoEncontrado_LancaNotFoundException()
    {
        _desafioRepository.GetByIdAsync(Arg.Any<Guid>(), default).ReturnsNull();

        await _sut.Invoking(s => s.GetByIdAsync(Guid.NewGuid(), default))
            .Should().ThrowAsync<NotFoundException>();
    }

    // ─── Helpers ───────────────────────────────────────────────────────────────

    private static Desafio BuildDesafio(DesafioStatus status)
    {
        var timeCriador = MakeTime(TimeCriadorId, EmpresaCriadoraId);
        var timeDesafiante = MakeTime(TimeDesafianteId, EmpresaDesafianteId);

        return new Desafio
        {
            Id = Guid.NewGuid(),
            TimeCriadorId = TimeCriadorId,
            TimeCriador = timeCriador,
            TimeDesafianteId = TimeDesafianteId,
            TimeDesafiante = timeDesafiante,
            DataJogo = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
            HoraJogo = new TimeOnly(15, 0),
            Local = "Arena Central",
            Bairro = "Centro",
            Nivel = 3,
            Status = status,
            DataCriacao = DateTime.UtcNow
        };
    }
}
