using System.Text.Json.Serialization;

namespace MonitoringHardApi.Features.Events.List;

public class ListEventsRequest
{
    [JsonPropertyName("deviceId")]
    public Guid? DeviceId { get; set; }
    
    [JsonPropertyName("fromDate")]
    public DateTime? FromDate { get; set; }
    
    [JsonPropertyName("toDate")]
    public DateTime? ToDate { get; set; }
    
    [JsonPropertyName("onlyAlarms")]
    public bool OnlyAlarms { get; set; }
    
    [JsonPropertyName("page")]
    public int Page { get; set; } = 1;
    
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; } = 10;
}