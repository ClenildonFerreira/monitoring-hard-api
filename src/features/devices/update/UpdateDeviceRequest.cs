using System.Text.Json.Serialization;

namespace MonitoringHardApi.Features.Devices.Update;

public class UpdateDeviceRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("location")]
    public string? Location { get; set; }
}