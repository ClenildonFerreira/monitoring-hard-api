using System.Text.Json.Serialization;

namespace MonitoringHardApi.Features.Devices.Update;

public class UpdateDeviceRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("location")]
    public required string Location { get; set; }
}