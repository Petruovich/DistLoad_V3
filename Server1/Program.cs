﻿//using Prometheus;
//using Server1.Services;

//var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddSingleton<ServerMetricsService>();
//builder.Services.AddControllers();

//var app = builder.Build();

//var metricsService = app.Services.GetRequiredService<ServerMetricsService>();

//app.MapGet("/", async () =>
//{
//    metricsService.IncreaseRequests();
//    await Task.Delay(Random.Shared.Next(50, 300));
//    metricsService.DecreaseRequests();
//    return Results.Ok("Response from Server 1");
//});

//app.MapControllers();


//app.UseMetricServer();
//app.Run("http://localhost:5001");
//Program.cs(Server1, 2, 3)


using Server1.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ServerMetricsService>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run("http://localhost:5001"); //, 5002, 5003 соответственно
