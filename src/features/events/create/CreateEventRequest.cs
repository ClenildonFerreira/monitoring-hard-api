using System.Text.Json.Serialization;

namespace MonitoringHardApi.Features.Events.Create;

public class CreateEventRequest
{
    [JsonPropertyName("deviceId")]
    public required string IntegrationId { get; set; }
    
    [JsonPropertyName("temperature")]
    public required double Temperature { get; set; }
    
    [JsonPropertyName("humidity")]
    public required double Humidity { get; set; }
    
    [JsonPropertyName("isAlarm")]
    public required bool IsAlarm { get; set; }
    
    [JsonPropertyName("timestamp")]
    public required DateTime Timestamp { get; set; }
}