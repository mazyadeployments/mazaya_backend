using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.IO;

namespace MMA.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // NLog: setup the logger first to catch all errors
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                logger.Debug("Init Main");

                CreateWebHostBuilder(args, logger).Build().Run();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, NLog.Logger logger)
        {
            //logger.Trace("Creating Host");

            var host = WebHost.CreateDefaultBuilder(args);

            host.UseNLog();
           
           
            //logger.Trace("Configure logging and azure inisghts");

            host.ConfigureLogging(loggerBuilder =>
            {
                loggerBuilder.ClearProviders();

                loggerBuilder.AddConsole();

                loggerBuilder.SetMinimumLevel(LogLevel.Trace);

                loggerBuilder.AddApplicationInsights("08915b38-db37-41bb-a6b6-d6e8131a1329");
            });

            //logger.Trace("Use appsettings in json files");

            host.ConfigureAppConfiguration((hostContext, config) =>
            {
                config.Sources.Clear();
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                config.AddEnvironmentVariables();
                hostContext.Configuration = config.Build();

                //logger.Trace($"Using {hostContext.HostingEnvironment.EnvironmentName} environment");
            });
#if DEBUG
            //logger.Trace("Use kestrel for development");

            //host.UseKestrel(serverOptions =>
            //{
            //    //serverOptions.Limits.MaxRequestBodySize = 1024 * 100;
            //    serverOptions.ListenLocalhost(5000);
            //    serverOptions.AddServerHeader = false;
            //});
#endif
            //logger.Trace("Setup root");

            host.UseContentRoot(Directory.GetCurrentDirectory());

            //logger.Trace("Setup IIS integration");

            host.UseIISIntegration();

            //logger.Trace("Setup startup object");

            host.UseStartup<Startup>();

            //logger.Trace("Creating Host Completed");

            return host;
        }
    }
}
