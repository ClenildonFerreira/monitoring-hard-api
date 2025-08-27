using Carter;
using MonitoringHardApi.Infrastructure.Database;
using MonitoringHardApi.Infrastructure.Iot;

namespace MonitoringHardApi.Features.Devices.Delete;

public class DeleteDeviceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/devices/{id}", async (
            Guid id,
            ApplicationDbContext db,
            IIotClient iotClient) =>
        {
            var device = await db.Devices.FindAsync(id);

            if (device == null)
                return Results.NotFound("Dispositivo não encontrado");

            if (!string.IsNullOrWhiteSpace(device.IntegrationId))
            {
                try
                {
                    await iotClient.UnregisterDeviceAsync(device.IntegrationId!);
                }
                catch (Exception)
                {
                    return Results.Problem(
                        "Não foi possível desregistrar o dispositivo",
                        statusCode: StatusCodes.Status502BadGateway);
                }
            }

            db.Devices.Remove(device);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}