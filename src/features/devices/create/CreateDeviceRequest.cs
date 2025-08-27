using System.Text.Json.Serialization;

namespace MonitoringHardApi.Features.Devices.Create;

public record CreateDeviceRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    
    [JsonPropertyName("location")]
    public required string Location { get; init; }
}