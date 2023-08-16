using System.Runtime.CompilerServices;
using CodeBlocksMiddleware;
using NLog;
using NLog.Web;
using Microsoft.Extensions.Logging;


var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Host.UseNLog();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

// Create the 'Logs' folder if it doesn't exist
var logsFolderPath = Path.Combine(builder.Environment.ContentRootPath, "Logs");
if (!Directory.Exists(logsFolderPath))
{
    Directory.CreateDirectory(logsFolderPath);
}

var app = builder.Build();

app.UseHttpsRedirection();
app.UseJsonExceptionMiddleware();
app.UseCors("corsapp");
app.UseAuthorization();
app.MapControllers();

app.Run();

