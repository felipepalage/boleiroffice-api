using AutoMapper;
using Boleiroffice.Application.Common.Security;
using Boleiroffice.Application.DTOs.Auth;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Security;
using Boleiroffice.Application.Mappings;
using Boleiroffice.Application.Services;
using Boleiroffice.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Boleiroffice.Tests.Services;

public sealed class AuthServiceTests
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;
    private readonly AuthService _sut;

    private const string ValidCnpj = "11222333000181";
    private const string FormattedCnpj = "11.222.333/0001-81";

    public AuthServiceTests()
    {
        _usuarioRepository = Substitute.For<IUsuarioRepository>();
        _empresaRepository = Substitute.For<IEmpresaRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<ApplicationMappingProfile>());
        _mapper = config.CreateMapper();

        _sut = new AuthService(_usuarioRepository, _empresaRepository, _passwordHasher, _jwtTokenGenerator, _mapper);
    }

    private RegisterRequest BuildValidRegisterRequest(string? cnpj = null) => new()
    {
        Nome = "Felipe Teste",
        Email = "felipe@empresa.com",
        Senha = "senha123",
        EmpresaNome = "Empresa XPTO",
        EmpresaCnpj = cnpj ?? ValidCnpj,
        EmpresaBairro = "Centro",
        EmpresaCidade = "São Paulo"
    };

    private void SetupTokenGenerator()
    {
        _jwtTokenGenerator.GenerateToken(Arg.Any<Usuario>(), Arg.Any<string>())
            .Returns(new TokenResult { Token = "fake.jwt.token", ExpiresAt = DateTime.UtcNow.AddHours(2) });
    }

    // ─── RegisterAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task RegisterAsync_ComDadosValidos_CriaUsuarioEEmpresaNovos()
    {
        SetupTokenGenerator();
        _usuarioRepository.ExistsByEmailAsync(Arg.Any<string>(), default).Returns(false);
        _empresaRepository.GetByCnpjAsync(Arg.Any<string>(), default).ReturnsNull();
        _passwordHasher.Hash(Arg.Any<string>()).Returns("hash123");

        var result = await _sut.RegisterAsync(BuildValidRegisterRequest(), default);

        result.Should().NotBeNull();
        result.Token.Should().Be("fake.jwt.token");
        await _empresaRepository.Received(1).AddAsync(Arg.Any<Empresa>(), default);
        await _usuarioRepository.Received(1).AddAsync(Arg.Any<Usuario>(), default);
    }

    [Fact]
    public async Task RegisterAsync_EmpresaJaExiste_UsaEmpresaExistente()
    {
        SetupTokenGenerator();
        var empresaExistente = new Empresa { Id = Guid.NewGuid(), Nome = "Empresa XPTO", Cnpj = ValidCnpj };

        _usuarioRepository.ExistsByEmailAsync(Arg.Any<string>(), default).Returns(false);
        _empresaRepository.GetByCnpjAsync(Arg.Any<string>(), default).Returns(empresaExistente);
        _passwordHasher.Hash(Arg.Any<string>()).Returns("hash123");

        await _sut.RegisterAsync(BuildValidRegisterRequest(), default);

        await _empresaRepository.DidNotReceive().AddAsync(Arg.Any<Empresa>(), default);
        await _usuarioRepository.Received(1).AddAsync(Arg.Any<Usuario>(), default);
    }

    [Fact]
    public async Task RegisterAsync_EmailJaExiste_LancaBusinessException()
    {
        _usuarioRepository.ExistsByEmailAsync(Arg.Any<string>(), default).Returns(true);

        await _sut.Invoking(s => s.RegisterAsync(BuildValidRegisterRequest(), default))
            .Should().ThrowAsync<BusinessException>()
            .WithMessage("*email*");
    }

    [Fact]
    public async Task RegisterAsync_CnpjInvalido_LancaBusinessException()
    {
        _usuarioRepository.ExistsByEmailAsync(Arg.Any<string>(), default).Returns(false);

        await _sut.Invoking(s => s.RegisterAsync(BuildValidRegisterRequest("00000000000000"), default))
            .Should().ThrowAsync<BusinessException>()
            .WithMessage("*CNPJ*");
    }

    [Fact]
    public async Task RegisterAsync_CnpjFormatado_NormalizaAntesDeValidar()
    {
        SetupTokenGenerator();
        _usuarioRepository.ExistsByEmailAsync(Arg.Any<string>(), default).Returns(false);
        _empresaRepository.GetByCnpjAsync(Arg.Any<string>(), default).ReturnsNull();
        _passwordHasher.Hash(Arg.Any<string>()).Returns("hash123");

        var result = await _sut.RegisterAsync(BuildValidRegisterRequest(FormattedCnpj), default);

        result.Should().NotBeNull();
        await _empresaRepository.Received(1)
            .GetByCnpjAsync(Arg.Is<string>(c => !c.Contains('.')), default);
    }

    [Fact]
    public async Task RegisterAsync_EmpresaComNomeDiferente_LancaBusinessException()
    {
        var empresaExistente = new Empresa { Id = Guid.NewGuid(), Nome = "Nome Diferente", Cnpj = ValidCnpj };
        _usuarioRepository.ExistsByEmailAsync(Arg.Any<string>(), default).Returns(false);
        _empresaRepository.GetByCnpjAsync(Arg.Any<string>(), default).Returns(empresaExistente);

        await _sut.Invoking(s => s.RegisterAsync(BuildValidRegisterRequest(), default))
            .Should().ThrowAsync<BusinessException>()
            .WithMessage("*CNPJ*");
    }

    [Fact]
    public async Task RegisterAsync_SemNome_LancaBusinessException()
    {
        var request = new RegisterRequest
        {
            Nome = "",
            Email = "felipe@empresa.com",
            Senha = "senha123",
            EmpresaNome = "Empresa XPTO",
            EmpresaCnpj = ValidCnpj,
            EmpresaBairro = "Centro",
            EmpresaCidade = "São Paulo"
        };

        await _sut.Invoking(s => s.RegisterAsync(request, default))
            .Should().ThrowAsync<BusinessException>();
    }

    // ─── LoginAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task LoginAsync_CredenciaisValidas_RetornaToken()
    {
        SetupTokenGenerator();
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Felipe",
            Email = "felipe@empresa.com",
            SenhaHash = "hash123",
            EmpresaId = Guid.NewGuid()
        };

        _usuarioRepository.GetByEmailAsync("felipe@empresa.com", default).Returns(usuario);
        _passwordHasher.Verify("senha123", "hash123").Returns(true);

        var result = await _sut.LoginAsync(new LoginRequest { Email = "felipe@empresa.com", Senha = "senha123" }, default);

        result.Should().NotBeNull();
        result.Token.Should().Be("fake.jwt.token");
    }

    [Fact]
    public async Task LoginAsync_EmailNaoEncontrado_LancaBusinessException()
    {
        _usuarioRepository.GetByEmailAsync(Arg.Any<string>(), default).ReturnsNull();

        await _sut.Invoking(s => s.LoginAsync(new LoginRequest { Email = "nao@existe.com", Senha = "qualquer" }, default))
            .Should().ThrowAsync<BusinessException>()
            .WithMessage("*Credenciais*");
    }

    [Fact]
    public async Task LoginAsync_SenhaErrada_LancaBusinessException()
    {
        var usuario = new Usuario { Id = Guid.NewGuid(), Email = "felipe@empresa.com", SenhaHash = "hash123" };
        _usuarioRepository.GetByEmailAsync("felipe@empresa.com", default).Returns(usuario);
        _passwordHasher.Verify("senhaErrada", "hash123").Returns(false);

        await _sut.Invoking(s => s.LoginAsync(new LoginRequest { Email = "felipe@empresa.com", Senha = "senhaErrada" }, default))
            .Should().ThrowAsync<BusinessException>()
            .WithMessage("*Credenciais*");
    }

    [Fact]
    public async Task LoginAsync_EmailNormalizaParaMinusculas()
    {
        SetupTokenGenerator();
        var usuario = new Usuario { Id = Guid.NewGuid(), Email = "felipe@empresa.com", SenhaHash = "hash" };
        _usuarioRepository.GetByEmailAsync("felipe@empresa.com", default).Returns(usuario);
        _passwordHasher.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        await _sut.LoginAsync(new LoginRequest { Email = "FELIPE@EMPRESA.COM", Senha = "123" }, default);

        await _usuarioRepository.Received(1).GetByEmailAsync("felipe@empresa.com", default);
    }
}
