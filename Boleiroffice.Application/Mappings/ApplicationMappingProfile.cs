using AutoMapper;
using Boleiroffice.Application.DTOs.Auth;
using Boleiroffice.Application.DTOs.Desafios;
using Boleiroffice.Application.DTOs.Empresas;
using Boleiroffice.Application.DTOs.Jogadores;
using Boleiroffice.Application.DTOs.Times;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Mappings;

public sealed class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<Empresa, EmpresaResponse>();
        CreateMap<Empresa, EmpresaDetailsResponse>();
        CreateMap<Time, TimeLookupResponse>();
        CreateMap<Time, TimeResponse>()
            .ForMember(dest => dest.EmpresaNome, opt => opt.MapFrom(src => src.Empresa != null ? src.Empresa.Nome : string.Empty))
            .ForMember(dest => dest.EmpresaLogoUrl, opt => opt.MapFrom(src => src.Empresa != null ? src.Empresa.LogoUrl : null))
            .ForMember(dest => dest.TotalJogadores, opt => opt.MapFrom(src => src.Jogadores.Count));
        CreateMap<Time, TimeDetailsResponse>()
            .IncludeBase<Time, TimeResponse>();
        CreateMap<Jogador, JogadorResponse>()
            .ForMember(dest => dest.TimeNome, opt => opt.MapFrom(src => src.Time != null ? src.Time.Nome : string.Empty));
        CreateMap<Usuario, AuthenticatedUserResponse>()
            .ForMember(dest => dest.EmpresaNome, opt => opt.MapFrom(src => src.Empresa != null ? src.Empresa.Nome : string.Empty));
        CreateMap<GolPartida, GolPartidaResponse>()
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time != null ? src.Time.Nome : string.Empty));
        CreateMap<Desafio, DesafioResponse>()
            .ForMember(dest => dest.TimeCriador, opt => opt.MapFrom(src => src.TimeCriador != null ? src.TimeCriador.Nome : string.Empty))
            .ForMember(dest => dest.EmpresaCriadora, opt => opt.MapFrom(src => src.TimeCriador != null && src.TimeCriador.Empresa != null ? src.TimeCriador.Empresa.Nome : string.Empty))
            .ForMember(dest => dest.TimeCriadorFotoUrl, opt => opt.MapFrom(src => src.TimeCriador != null ? src.TimeCriador.FotoUrl : null))
            .ForMember(dest => dest.TimeCriadorEscudoShape, opt => opt.MapFrom(src => src.TimeCriador != null ? src.TimeCriador.EscudoShape : 1))
            .ForMember(dest => dest.TimeCriadorCorPrimaria, opt => opt.MapFrom(src => src.TimeCriador != null ? src.TimeCriador.CorPrimaria : "#DC2626"))
            .ForMember(dest => dest.TimeCriadorCorSecundaria, opt => opt.MapFrom(src => src.TimeCriador != null ? src.TimeCriador.CorSecundaria : "#111827"))
            .ForMember(dest => dest.TimeDesafiante, opt => opt.MapFrom(src => src.TimeDesafiante != null ? src.TimeDesafiante.Nome : null))
            .ForMember(dest => dest.EmpresaDesafiante, opt => opt.MapFrom(src => src.TimeDesafiante != null && src.TimeDesafiante.Empresa != null ? src.TimeDesafiante.Empresa.Nome : null))
            .ForMember(dest => dest.TimeDesafianteFotoUrl, opt => opt.MapFrom(src => src.TimeDesafiante != null ? src.TimeDesafiante.FotoUrl : null))
            .ForMember(dest => dest.TimeDesafianteEscudoShape, opt => opt.MapFrom(src => src.TimeDesafiante != null ? (int?)src.TimeDesafiante.EscudoShape : null))
            .ForMember(dest => dest.TimeDesafianteCorPrimaria, opt => opt.MapFrom(src => src.TimeDesafiante != null ? src.TimeDesafiante.CorPrimaria : null))
            .ForMember(dest => dest.TimeDesafianteCorSecundaria, opt => opt.MapFrom(src => src.TimeDesafiante != null ? src.TimeDesafiante.CorSecundaria : null))
            .ForMember(dest => dest.ResultadoPropostoPorTime, opt => opt.MapFrom(src =>
                src.ResultadoPropostoPorTimeId == src.TimeCriadorId
                    ? src.TimeCriador != null ? src.TimeCriador.Nome : null
                    : src.ResultadoPropostoPorTimeId == src.TimeDesafianteId
                        ? src.TimeDesafiante != null ? src.TimeDesafiante.Nome : null
                        : null));
    }
}
