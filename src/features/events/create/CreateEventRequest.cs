namespace MonitoringHardApi.Features.Events.Create;

public class CreateEventRequest
{
    public required string IntegrationId { get; set; }
    public required double Temperature { get; set; }
    public required double Humidity { get; set; }
    public required bool IsAlarm { get; set; }
    public required DateTime Timestamp { get; set; }
}