var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpoints();
builder.Services.AddCarter();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapCarter();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.Run();
