using Boleiroffice.Application.DTOs.Desafios;
using FluentValidation;

namespace Boleiroffice.Application.Validators.Desafios;

public sealed class RegisterResultRequestValidator : AbstractValidator<RegisterResultRequest>
{
    public RegisterResultRequestValidator()
    {
        RuleFor(x => x.PlacarCriador).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PlacarDesafiante).GreaterThanOrEqualTo(0);
    }
}
