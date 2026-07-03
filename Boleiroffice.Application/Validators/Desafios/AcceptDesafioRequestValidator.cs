using Boleiroffice.Application.DTOs.Desafios;
using FluentValidation;

namespace Boleiroffice.Application.Validators.Desafios;

public sealed class AcceptDesafioRequestValidator : AbstractValidator<AcceptDesafioRequest>
{
    public AcceptDesafioRequestValidator()
    {
        RuleFor(x => x.TimeDesafianteId).NotEmpty();
    }
}
