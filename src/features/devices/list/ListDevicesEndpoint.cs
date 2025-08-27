using Carter;
using Carter.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MonitoringHardApi.Infrastructure.Database;

namespace MonitoringHardApi.Features.Devices.List;

public class ListDevicesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/devices", async (
            [AsParameters] ListDevicesRequest request,
            ApplicationDbContext db) =>
        {
            var validator = new ListDevicesValidator();
            var validationResult = await validator.ValidateAsync(request);
            
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            var query = db.Devices.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(x => x.Name.Contains(request.Search));

            if (!string.IsNullOrWhiteSpace(request.Location))
                query = query.Where(x => x.Location.Contains(request.Location));

            if (request.FromDate.HasValue)
                query = query.Where(x => x.CreatedAt >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(x => x.CreatedAt <= request.ToDate.Value);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

            var devices = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Location,
                    x.IntegrationId,
                    x.CreatedAt,
                    x.UpdatedAt,
                    EventCount = x.Events.Count
                })
                .ToListAsync();

            return Results.Ok(new
            {
                Data = devices,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = request.Page,
                PageSize = request.PageSize
            });
        });
    }
}
