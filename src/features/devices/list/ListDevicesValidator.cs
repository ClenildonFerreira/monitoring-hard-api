using FluentValidation;

namespace MonitoringHardApi.Features.Devices.List;

public class ListDevicesValidator : AbstractValidator<ListDevicesRequest>
{
    public ListDevicesValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("A página deve ser maior que 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("O tamanho da página deve ser entre 1 e 100");

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("A data inicial deve ser menor ou igual à data final");
    }
}
