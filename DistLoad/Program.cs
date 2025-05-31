//using DistLoad.Metrics;
//using DistLoad.Models;
//using DistLoad.Services;
//using Prometheus;

//var builder = WebApplication.CreateBuilder(args);

//// ���� ���������� �������
//var servers = new List<ServerInstance>
//{
//    new() { Id = "1", Address = "http://localhost:5001" },
//    new() { Id = "2", Address = "http://localhost:5002" },
//    new() { Id = "3", Address = "http://localhost:5003" }
//};

//builder.Services.AddSingleton<List<ServerInstance>>(servers);
//builder.Services.AddSingleton<LoadBalancerManager>();
//builder.Services.AddSingleton<MetricsLogger>();

//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// ��������������
//app.UseMiddleware<LoadBalancerMiddleware>();

//// Prometheus-�������
//app.UseMetricServer();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
//app.UseAuthorization();
//app.MapControllers();

//// ����� �������
//var logger = app.Services.GetRequiredService<MetricsLogger>();

//app.Run();

//using DistLoad.Metrics;
using DistLoad.Models;
using DistLoad.Services;
using Prometheus;
using DistLoad.Metrics;

var builder = WebApplication.CreateBuilder(args);

var servers = new List<ServerInstance>
{
    new() { Id = "1", Address = "http://localhost:5001" },
    new() { Id = "2", Address = "http://localhost:5002" },
    new() { Id = "3", Address = "http://localhost:5003" }
};

builder.Services.AddSingleton(servers);
builder.Services.AddSingleton<LoadBalancerManager>();
builder.Services.AddSingleton<MetricsLogger>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var logger = app.Services.GetRequiredService<MetricsLogger>();
var manager = app.Services.GetRequiredService<LoadBalancerManager>();
manager.SetAlgorithm("adaptive");
app.UseMiddleware<LoadBalancerMiddleware>();
app.UseMetricServer();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ��������� ������� ������ ������
//var logger = app.Services.GetRequiredService<MetricsLogger>();
//var manager = app.Services.GetRequiredService<LoadBalancerManager>();
//manager.SetAlgorithm("adaptive");
Task.Run(() => logger.Loop());

app.Run();


