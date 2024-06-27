using MemoryMosaic.Scanner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Processory;
using Serilog;

namespace MemoryMosaic {
    public static class Program {

        private static void EntityMemoryServices(IServiceCollection services) {
            services.AddSingleton(_ => new ProcessoryClient("fm"));
            services.AddSingleton<IModuleScanner, ModuleScanner>();
        }


        public static void Main() {
            Startup.ConfigureSerilogLogger();

            //string debugVersion = AssemblyUtils.GetDebugVersionNumber();
            //Log.Information("Debug version: {DebugVersion}", debugVersion);

            //CsvDataLoader.LoadRowsFromCsv();
            using IHost host = CreateHostBuilder().Build();
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;

            try {
                Log.Information("Starting the application.");
                services.GetRequiredService<App>().Run();
                Log.Information("Application started successfully.");
            }
            catch (Exception e) {
                Log.Fatal(e, "An unhandled exception occurred while starting the application.");
            }
            finally {
                Log.Information("Shutting down the application.");
                Log.CloseAndFlush();
            }

            static IHostBuilder CreateHostBuilder() {
                return Host.CreateDefaultBuilder()
                    .ConfigureServices((_, services) => {
                        EntityMemoryServices(services);
                        services.AddSingleton<App>();
                    });
            }
        }
    }
}