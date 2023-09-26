using ApiMIddleware.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using NLog;
using NLog.Web;
using Serilog;


//added Nlog
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
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

//if you want to to write events through existing NLog infrastructure
//var logger = new LoggerConfiguration()
//    .WriteTo.NLog()
//    .CreateLogger();

//adding SerilogS
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console() //write logs to console
    .WriteTo.File("Logs/seriLogs-.txt", rollingInterval: RollingInterval.Day) //write logs to file 
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseJsonExceptionMiddleware();

app.UseLogExceptionMiddlevare();

app.UseCors("corsapp");

app.UseAuthorization();

app.MapControllers();

app.Run();
