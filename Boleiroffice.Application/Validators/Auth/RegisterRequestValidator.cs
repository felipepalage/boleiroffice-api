using Boleiroffice.Application.Common.Validation;
using Boleiroffice.Application.DTOs.Auth;
using FluentValidation;

namespace Boleiroffice.Application.Validators.Auth;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(180);
        RuleFor(x => x.Senha).NotEmpty().MinimumLength(6).MaximumLength(100);
        RuleFor(x => x.EmpresaNome).NotEmpty().MaximumLength(120);
        RuleFor(x => x.EmpresaCnpj)
            .NotEmpty()
            .Must(CnpjHelper.IsValid)
            .WithMessage("CNPJ invalido.");
        RuleFor(x => x.EmpresaBairro).NotEmpty().MaximumLength(120);
        RuleFor(x => x.EmpresaCidade).NotEmpty().MaximumLength(120);
        RuleFor(x => x.EmpresaLogoUrl).MaximumLength(500);
    }
}