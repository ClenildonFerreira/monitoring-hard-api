namespace MonitoringHardApi.Features.Devices.List;

public class ListDevicesRequest
{
    public string? Search { get; set; }
    public string? Location { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
