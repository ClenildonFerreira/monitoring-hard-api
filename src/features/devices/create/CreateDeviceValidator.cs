using FluentValidation;

namespace MonitoringHardApi.Features.Devices.Create;

public class CreateDeviceValidator : AbstractValidator<CreateDeviceRequest>
{
    public CreateDeviceValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Nome do dispositivo é obrigatório e deve ter no máximo 100 caracteres");

        RuleFor(x => x.Location)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Localização é obrigatória e deve ter no máximo 200 caracteres");
    }
}