using Boleiroffice.Application.DTOs.Desafios;
using FluentValidation;

namespace Boleiroffice.Application.Validators.Desafios;

public sealed class DesafioCreateRequestValidator : AbstractValidator<DesafioCreateRequest>
{
    public DesafioCreateRequestValidator()
    {
        RuleFor(x => x.TimeCriadorId).NotEmpty();
        RuleFor(x => x.TimeConvidadoId).NotEmpty();
        RuleFor(x => x.TimeConvidadoId)
            .NotEqual(x => x.TimeCriadorId)
            .WithMessage("O time convidado precisa ser diferente do time criador.");
        RuleFor(x => x.DataJogo).GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.Date));
        RuleFor(x => x.Local).NotEmpty().MaximumLength(180);
        RuleFor(x => x.Bairro).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Nivel).InclusiveBetween(1, 5);
    }
}
