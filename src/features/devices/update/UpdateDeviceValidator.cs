using FluentValidation;

namespace MonitoringHardApi.Features.Devices.Update;

public class UpdateDeviceValidator : AbstractValidator<UpdateDeviceRequest>
{
    public UpdateDeviceValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("O nome é obrigatório")
            .MaximumLength(100)
            .WithMessage("O nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Location)
            .NotEmpty()
            .WithMessage("A localização é obrigatória")
            .MaximumLength(200)
            .WithMessage("A localização deve ter no máximo 200 caracteres");
    }
}