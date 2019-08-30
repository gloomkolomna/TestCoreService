using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TestCoreService.Processors;
using TestCoreService.Services;
using TestCoreService.Services.TaskQueue;

namespace TestCoreService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration(configBuilder =>
                {
                    configBuilder.AddJsonFile("config.json");
                    configBuilder.AddCommandLine(args);
                })
                .ConfigureLogging((configLogging) => 
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                })
                .ConfigureServices((service) => 
                {
                    service.AddHostedService<TaskSchedulerService>();
                    service.AddHostedService<WorkerService>();

                    service.AddSingleton<ISettingService, SettingsService>();
                    service.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
                    service.AddSingleton<ITaskProcessor, TaskProcessor>();
                });

            await builder.RunConsoleAsync();
        }
    }
}
