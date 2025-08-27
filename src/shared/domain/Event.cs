using System;

namespace MonitoringHardApi.Shared.Domain
{
    public class Event
    {
        public Guid Id { get; set; }
        public Guid DeviceId { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public bool IsAlarm { get; set; }
        public DateTime Timestamp { get; set; }
        public Device Device { get; set; }
    }
}
