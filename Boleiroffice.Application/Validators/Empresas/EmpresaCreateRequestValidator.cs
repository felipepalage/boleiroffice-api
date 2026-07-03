using Boleiroffice.Application.Common.Validation;
using Boleiroffice.Application.DTOs.Empresas;
using FluentValidation;

namespace Boleiroffice.Application.Validators.Empresas;

public sealed class EmpresaCreateRequestValidator : AbstractValidator<EmpresaCreateRequest>
{
    public EmpresaCreateRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Cnpj)
            .NotEmpty()
            .Must(CnpjHelper.IsValid)
            .WithMessage("CNPJ invalido.");
        RuleFor(x => x.Bairro).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Cidade).NotEmpty().MaximumLength(120);
        RuleFor(x => x.LogoUrl).MaximumLength(500);
    }
}