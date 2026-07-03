namespace Boleiroffice.Application.DTOs.Admin;

public record AdminStatsResponse(
    int TotalEmpresas,
    int TotalDesafios,
    int TotalUsuarios,
    int DesafiosSemana,
    int DesafiosMes);
