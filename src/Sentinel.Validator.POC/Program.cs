using Sentinel.Validator.POC.BackgroundServices;
using Sentinel.Validator.POC.Repo;
using Sentinel.Validator.POC.ValidationReaders;
using Sentinel.Core.K8s.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<IValidationReader, JsonValidationReader>();

builder.Services.AddSingleton<ValidationRepo>();
builder.Services.AddControllers();

builder.Services.AddSingleton<IServiceCollection>(builder.Services);
builder.Services.AddSingleton<IServiceProvider>(builder.Services.BuildServiceProvider());
builder.Services.AddSingleton<ValidationBackgroundServiceFactory>();

builder.Services.AddCoreK8s(builder.Configuration);

builder.Services.AddHostedService<K8NonGenericWatcher>();

var q = new ValidationBackgroundServiceFactory(builder.Services);

//builder.Services.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// var fac = app.Services.GetService<ValidationBackgroundServiceFactory>();
//var fac = app.Services.GetService<ValidationBackgroundServiceFactory();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
