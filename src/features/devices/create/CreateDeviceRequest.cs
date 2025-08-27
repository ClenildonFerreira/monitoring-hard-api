using System.ComponentModel.DataAnnotations;

namespace MonitoringHardApi.Features.Devices.Create;

public record CreateDeviceRequest
{
    public required string Name { get; init; }
    public required string Location { get; init; }
}