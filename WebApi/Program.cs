using System.Runtime.CompilerServices;
using CodeBlocksMiddleware;
using NLog;
using NLog.Web;
using Microsoft.Extensions.Logging;


var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();

}
app.UseHttpsRedirection();
app.UseJsonExceptionMiddleware();
app.UseCors("corsapp");
app.UseAuthorization();
app.MapControllers();

app.Run();

