using MonitoringHardApi.Infrastructure.Database;
using MonitoringHardApi.Infrastructure.Iot;
using Microsoft.EntityFrameworkCore;
using Carter;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

builder.Services.AddCarter();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var iotBase = builder.Configuration.GetValue<string>("Iot:BaseUrl") ?? "http://localhost:5000/";
builder.Services.AddHttpClient<IIotClient, IotProviderClient>(client =>
{
    client.BaseAddress = new Uri(iotBase);
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddSignalR();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapCarter();
app.MapHub<MonitoringHardApi.Infrastructure.Signaling.EventHub>("/hubs/events");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var migrateOnStart = builder.Configuration.GetValue<bool?>("MigrateOnStart") ?? true;
    if (migrateOnStart)
        db.Database.Migrate();
}

app.Run();