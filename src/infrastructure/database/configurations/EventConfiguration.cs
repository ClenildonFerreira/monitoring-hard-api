using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonitoringHardApi.Shared.Domain;

namespace MonitoringHardApi.Infrastructure.Database.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Temperature).IsRequired();
            builder.Property(x => x.Humidity).IsRequired();
            builder.Property(x => x.Timestamp).IsRequired();
        }
    }
}