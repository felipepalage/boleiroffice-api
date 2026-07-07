using AutoMapper;
using Boleiroffice.Application.DTOs.Auth;
using Boleiroffice.Application.DTOs.Convites;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Security;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Services;

public sealed class ConviteMembroService : IConviteMembroService
{
    private readonly IConviteMembroRepository _conviteRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;

    public ConviteMembroService(
        IConviteMembroRepository conviteRepository,
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IMapper mapper)
    {
        _conviteRepository = conviteRepository;
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _mapper = mapper;
    }

    public async Task<ConviteResponse> CriarAsync(Guid empresaId, CancellationToken cancellationToken)
    {
        var convite = new ConviteMembro
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            Token = Guid.NewGuid().ToString("N")[..12],
            DataCriacao = DateTime.UtcNow,
            ExpiraEm = DateTime.UtcNow.AddDays(7)
        };

        await _conviteRepository.AddAsync(convite, cancellationToken);
        return new ConviteResponse(convite.Token, convite.ExpiraEm);
    }

    public async Task<ConviteInfoResponse?> GetInfoAsync(string token, CancellationToken cancellationToken)
    {
        var convite = await _conviteRepository.GetValidoByTokenAsync(token, DateTime.UtcNow, cancellationToken);
        return convite is null ? null : new ConviteInfoResponse(convite.Empresa?.Nome ?? "Empresa");
    }

    public async Task<AuthResponse> AceitarAsync(string token, AceitarConviteRequest request, CancellationToken cancellationToken)
    {
        var convite = await _conviteRepository.GetValidoByTokenAsync(token, DateTime.UtcNow, cancellationToken)
            ?? throw new BusinessException("Convite inválido ou expirado.");

        var nome = (request.Nome ?? string.Empty).Trim();
        var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
        var senha = request.Senha ?? string.Empty;

        if (nome.Length < 2)
            throw new BusinessException("Informe seu nome.");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new BusinessException("Informe um e-mail válido.");
        if (senha.Length < 6)
            throw new BusinessException("A senha deve ter ao menos 6 caracteres.");

        if (await _usuarioRepository.ExistsByEmailAsync(email, cancellationToken))
            throw new BusinessException("Já existe um usuário com esse e-mail.");

        var usuario = new Usuario
        {
            Nome = nome,
            Email = email,
            SenhaHash = _passwordHasher.Hash(senha),
            EmpresaId = convite.EmpresaId,
            Empresa = convite.Empresa
        };

        await _usuarioRepository.AddAsync(usuario, cancellationToken);

        var empresaNome = convite.Empresa?.Nome ?? string.Empty;
        var token2 = _jwtTokenGenerator.GenerateToken(usuario, empresaNome);

        return new AuthResponse
        {
            Token = token2.Token,
            ExpiresAt = token2.ExpiresAt,
            Usuario = _mapper.Map<AuthenticatedUserResponse>(usuario)
        };
    }
}
