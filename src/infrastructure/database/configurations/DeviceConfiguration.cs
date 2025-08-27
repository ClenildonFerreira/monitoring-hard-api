using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonitoringHardApi.Shared.Domain;

namespace MonitoringHardApi.Infrastructure.Database.Configurations
{
    public class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Location).IsRequired();
            builder.Property(x => x.IntegrationId).IsRequired();
            
            builder.HasMany(x => x.Events)
                .WithOne(x => x.Device)
                .HasForeignKey(x => x.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}