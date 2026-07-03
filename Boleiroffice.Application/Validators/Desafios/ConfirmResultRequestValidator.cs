using Boleiroffice.Application.DTOs.Desafios;
using FluentValidation;

namespace Boleiroffice.Application.Validators.Desafios;

public sealed class ConfirmResultRequestValidator : AbstractValidator<ConfirmResultRequest>
{
    public ConfirmResultRequestValidator()
    {
        RuleFor(x => x.PlacarCriador).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PlacarDesafiante).GreaterThanOrEqualTo(0);
    }
}
