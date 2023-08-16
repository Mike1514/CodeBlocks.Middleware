In order to add the middleware to your project,
check the nlog.config configuration file,
where the data logging files are specified in the Logs folder.
Аfter that, create a logger variable in the program.ts file:
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger(); 
in the builder add: 
builder.Logging.ClearProviders();
builder.Host.UseNLog();
also need to add app.UseJsonExceptionMiddleware();
in the program build pipeline
That's all! 