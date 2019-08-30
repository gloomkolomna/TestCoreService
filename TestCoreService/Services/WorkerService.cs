using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestCoreService.Services.TaskQueue;

namespace TestCoreService.Services
{
    public class WorkerService : BackgroundService
    {
        private readonly ISettingService _setting;
        private readonly ILogger _logger;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        public WorkerService(ISettingService setting, ILogger<TaskSchedulerService> logger, IBackgroundTaskQueue backgroundTaskQueue)
        {
            _setting = setting;
            _logger = logger;
            _backgroundTaskQueue = backgroundTaskQueue;
        }

        protected async override Task ExecuteAsync(CancellationToken token)
        {
            var workersCount = _setting.WorkersCount;
            var workers = Enumerable.Range(0, workersCount)
                .Select(num => RunInstance(num, token));

            await Task.WhenAll(workers);
        }

        private async Task RunInstance(int num, CancellationToken token)
        {
            _logger.LogInformation($"#{num} is starting.");

            while (!token.IsCancellationRequested)
            {
                var workItem = await _backgroundTaskQueue.DequeuAsync(token);

                try
                {
                    _logger.LogInformation($"#{num}: Processing task. Queue size: {_backgroundTaskQueue.Size}.");
                    await workItem(token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"#{num}: Error task.");
                }
            }

            _logger.LogInformation($"#{num} is stopping.");
        }
    }
}
