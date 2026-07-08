using AutoMapper;
using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.Common.Validation;
using Boleiroffice.Application.DTOs.Auth;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Security;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmailService _emailService;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IEmpresaRepository empresaRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IMapper mapper,
        IEmailService emailService)
    {
        _usuarioRepository = usuarioRepository;
        _empresaRepository = empresaRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _mapper = mapper;
        _emailService = emailService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        Guard.AgainstNullOrWhiteSpace(request.Nome, "Nome do usuario e obrigatorio.");
        Guard.AgainstNullOrWhiteSpace(request.Email, "Email e obrigatorio.");
        Guard.AgainstNullOrWhiteSpace(request.Senha, "Senha e obrigatoria.");
        Guard.AgainstNullOrWhiteSpace(request.EmpresaNome, "Nome da empresa e obrigatorio.");
        Guard.AgainstNullOrWhiteSpace(request.EmpresaCnpj, "CNPJ da empresa e obrigatorio.");
        Guard.AgainstNullOrWhiteSpace(request.EmpresaBairro, "Bairro da empresa e obrigatorio.");
        Guard.AgainstNullOrWhiteSpace(request.EmpresaCidade, "Cidade da empresa e obrigatoria.");

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedCnpj = CnpjHelper.Normalize(request.EmpresaCnpj);

        if (!CnpjHelper.IsValid(normalizedCnpj))
        {
            throw new BusinessException("CNPJ invalido.");
        }

        if (await _usuarioRepository.ExistsByEmailAsync(normalizedEmail, cancellationToken))
        {
            throw new BusinessException("Ja existe um usuario com esse email.");
        }

        var empresaExistente = await _empresaRepository.GetByCnpjAsync(normalizedCnpj, cancellationToken);
        if (empresaExistente is not null)
        {
            throw new BusinessException("Essa empresa já está cadastrada (CNPJ em uso). Faça login com a conta existente.");
        }

        var empresa = new Empresa
        {
            Nome = request.EmpresaNome.Trim(),
            Cnpj = normalizedCnpj,
            Bairro = request.EmpresaBairro.Trim(),
            Cidade = request.EmpresaCidade.Trim(),
            LogoUrl = NormalizeUrl(request.EmpresaLogoUrl),
            DataCriacao = DateTime.UtcNow,
            IndicadaPorEmpresaId = request.IndicadoPorEmpresaId
        };

        await _empresaRepository.AddAsync(empresa, cancellationToken);

        var usuario = new Usuario
        {
            Nome = request.Nome.Trim(),
            Email = normalizedEmail,
            SenhaHash = _passwordHasher.Hash(request.Senha),
            EmpresaId = empresa.Id,
            Empresa = empresa
        };

        await _usuarioRepository.AddAsync(usuario, cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(usuario, empresa.Nome);

        await _emailService.SendAsync(
            usuario.Email,
            "Bem-vindo ao Boleiroffice ⚽",
            $"<p>Olá {usuario.Nome},</p><p>Sua empresa <strong>{empresa.Nome}</strong> entrou no Boleiroffice. Crie seu time e comece a marcar amistosos com outras empresas!</p><p><a href=\"https://boleiroffice.com.br/app\">Acessar plataforma</a></p>",
            cancellationToken);

        return new AuthResponse
        {
            Token = token.Token,
            ExpiresAt = token.ExpiresAt,
            Usuario = _mapper.Map<AuthenticatedUserResponse>(usuario)
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var usuario = await _usuarioRepository.GetByEmailAsync(normalizedEmail, cancellationToken)
            ?? throw new BusinessException("Credenciais invalidas.");

        if (!_passwordHasher.Verify(request.Senha, usuario.SenhaHash))
        {
            throw new BusinessException("Credenciais invalidas.");
        }

        var empresaNome = usuario.Empresa?.Nome ?? string.Empty;
        var token = _jwtTokenGenerator.GenerateToken(usuario, empresaNome);

        return new AuthResponse
        {
            Token = token.Token,
            ExpiresAt = token.ExpiresAt,
            Usuario = _mapper.Map<AuthenticatedUserResponse>(usuario)
        };
    }

    private static string? NormalizeUrl(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}