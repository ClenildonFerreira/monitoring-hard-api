using System;
using System.Collections.Generic;

namespace MonitoringHardApi.Shared.Domain
{
    public class Device
    {
        public Device()
        {
            Events = new List<Event>();
        }

        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Location { get; set; }
    public string? IntegrationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Event> Events { get; set; }
    }
}