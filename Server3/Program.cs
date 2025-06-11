using Server3.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ServerMetricsService>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run("http://localhost:5003");