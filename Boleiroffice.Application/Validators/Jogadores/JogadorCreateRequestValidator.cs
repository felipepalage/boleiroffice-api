using Boleiroffice.Application.DTOs.Jogadores;
using FluentValidation;

namespace Boleiroffice.Application.Validators.Jogadores;

public sealed class JogadorCreateRequestValidator : AbstractValidator<JogadorCreateRequest>
{
    public JogadorCreateRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Posicao).NotEmpty().MaximumLength(60);
        RuleFor(x => x.NumeroCamisa).InclusiveBetween(1, 99);
        RuleFor(x => x.TimeId).NotEmpty();
    }
}
