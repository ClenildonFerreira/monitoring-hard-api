using System;
using System.Collections.Generic;

namespace MonitoringHardApi.Shared.Domain
{
    public class Device
    {
        public  Iin id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string IntegrationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Event> Events { get; set; }
    }
}
