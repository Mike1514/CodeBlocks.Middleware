using NLog;
using NLog.Web;
using CodeBlocksMiddleware;



var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<JsonExceptionMiddleware>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();

}

app.UseHttpsRedirection();

app.UseMiddleware<JsonExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();