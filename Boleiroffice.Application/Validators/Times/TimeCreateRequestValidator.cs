using Boleiroffice.Application.DTOs.Times;
using FluentValidation;

namespace Boleiroffice.Application.Validators.Times;

public sealed class TimeCreateRequestValidator : AbstractValidator<TimeCreateRequest>
{
    public TimeCreateRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(120);
        RuleFor(x => x.EmpresaId).NotEmpty();
        RuleFor(x => x.BairroBase).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Nivel).InclusiveBetween(1, 5);
        RuleFor(x => x.FotoUrl).MaximumLength(500);
    }
}