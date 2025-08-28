using FluentValidation;

namespace MonitoringHardApi.Features.Devices.Update;

public class UpdateDeviceValidator : AbstractValidator<UpdateDeviceRequest>
{
    public UpdateDeviceValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100)
            .WithMessage("O nome deve ter no máximo 100 caracteres")
            .When(x => x.Name != null);

        RuleFor(x => x.Location)
            .MaximumLength(200)
            .WithMessage("A localização deve ter no máximo 200 caracteres")
            .When(x => x.Location != null);
    }
}