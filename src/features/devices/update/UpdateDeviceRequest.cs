namespace MonitoringHardApi.Features.Devices.Update;

public class UpdateDeviceRequest
{
    public required string Name { get; set; }
    public required string Location { get; set; }
}