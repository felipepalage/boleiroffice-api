using Boleiroffice.Domain.Common;
using Boleiroffice.Domain.Enums;

namespace Boleiroffice.Domain.Entities;

public class Desafio : BaseEntity
{
    public Guid TimeCriadorId { get; set; }
    public Guid? TimeDesafianteId { get; set; }
    public DateOnly DataJogo { get; set; }
    public TimeOnly HoraJogo { get; set; }
    public string Local { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public DesafioStatus Status { get; set; } = DesafioStatus.Aberto;
    public int? PlacarCriador { get; set; }
    public int? PlacarDesafiante { get; set; }
    public int? PlacarCriadorProposto { get; set; }
    public int? PlacarDesafianteProposto { get; set; }
    public Guid? ResultadoPropostoPorTimeId { get; set; }
    public bool ResultadoConfirmadoPeloCriador { get; set; }
    public bool ResultadoConfirmadoPeloDesafiante { get; set; }
    public DateTime? DataPropostaResultado { get; set; }
    public DateTime? DataAceite { get; set; }
    public DateTime? DataCancelamento { get; set; }
    public DateTime? DataResultadoConfirmadoEm { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public Time? TimeCriador { get; set; }
    public Time? TimeDesafiante { get; set; }
    public ICollection<ConfirmacaoPresenca> ConfirmacoesPresenca { get; set; } = new List<ConfirmacaoPresenca>();
    public ICollection<GolPartida> Gols { get; set; } = new List<GolPartida>();

    public void Aceitar(Guid timeDesafianteId)
    {
        TimeDesafianteId = timeDesafianteId;
        Status = DesafioStatus.Aceito;
        DataAceite = DateTime.UtcNow;
    }

    public void Cancelar()
    {
        Status = DesafioStatus.Cancelado;
        DataCancelamento = DateTime.UtcNow;
    }

    public void ProporResultado(Guid timeId, int placarCriador, int placarDesafiante)
    {
        PlacarCriadorProposto = placarCriador;
        PlacarDesafianteProposto = placarDesafiante;
        ResultadoPropostoPorTimeId = timeId;
        ResultadoConfirmadoPeloCriador = timeId == TimeCriadorId;
        ResultadoConfirmadoPeloDesafiante = timeId == TimeDesafianteId;
        DataPropostaResultado = DateTime.UtcNow;
        DataResultadoConfirmadoEm = null;
        Status = DesafioStatus.ResultadoPendente;
    }

    public void ConfirmarResultado(Guid timeId)
    {
        if (timeId == TimeCriadorId)
        {
            ResultadoConfirmadoPeloCriador = true;
        }
        else if (timeId == TimeDesafianteId)
        {
            ResultadoConfirmadoPeloDesafiante = true;
        }

        if (ResultadoConfirmadoPeloCriador && ResultadoConfirmadoPeloDesafiante)
        {
            FinalizarResultado();
        }
    }

    public void FinalizarPorTimeout()
    {
        if (Status != DesafioStatus.ResultadoPendente) return;
        FinalizarResultado();
    }

    private void FinalizarResultado()
    {
        PlacarCriador = PlacarCriadorProposto;
        PlacarDesafiante = PlacarDesafianteProposto;
        PlacarCriadorProposto = null;
        PlacarDesafianteProposto = null;
        ResultadoPropostoPorTimeId = null;
        ResultadoConfirmadoPeloCriador = false;
        ResultadoConfirmadoPeloDesafiante = false;
        DataResultadoConfirmadoEm = DateTime.UtcNow;
        Status = DesafioStatus.Finalizado;
    }
}
