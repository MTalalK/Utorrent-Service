using Microsoft.EntityFrameworkCore;
using WorkerServiceNew;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using Microsoft.Extensions.Hosting.WindowsServices;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
   .ConfigureServices((hostContext, services) =>
   {
       services.AddHostedService<Worker>();
       services.AddDbContext<TorrentContext>((options) =>
       options.UseSqlServer(hostContext.Configuration.GetConnectionString("Main"))
       , ServiceLifetime.Singleton);
   });
Log.Logger = new LoggerConfiguration()
            .WriteTo.File("app.txt") // Specify the file name
            .CreateLogger();
builder.UseSerilog();

builder.ConfigureAppConfiguration((hostContext, config) =>
{
    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    
});


IHost host = builder.Build();
await host.RunAsync();