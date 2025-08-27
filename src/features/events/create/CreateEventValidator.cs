using FluentValidation;

namespace MonitoringHardApi.Features.Events.Create;

public class CreateEventValidator : AbstractValidator<CreateEventRequest>
{
    public CreateEventValidator()
    {
        RuleFor(x => x.IntegrationId)
            .NotEmpty()
            .WithMessage("O ID de integração é obrigatório");

        RuleFor(x => x.Temperature)
            .InclusiveBetween(-50, 100)
            .WithMessage("A temperatura deve estar entre -50°C e 100°C");

        RuleFor(x => x.Humidity)
            .InclusiveBetween(0, 100)
            .WithMessage("A umidade deve estar entre 0% e 100%");

        RuleFor(x => x.Timestamp)
            .NotEmpty()
            .WithMessage("O timestamp é obrigatório")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("O timestamp não pode ser no futuro");
    }
}