using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TestCoreService.Processors;
using TestCoreService.Services.TaskQueue;

namespace TestCoreService.Services
{
    public class TaskSchedulerService : IHostedService, IDisposable
    {
        private Timer _timer;

        private readonly ISettingService _setting;
        private readonly ILogger _logger;
        private readonly ITaskProcessor _taskProcessor;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        private readonly object _sync = new object();
        private readonly Random _random = new Random();

        public TaskSchedulerService(ISettingService setting, ILogger<TaskSchedulerService> logger,
            ITaskProcessor taskProcessor, IBackgroundTaskQueue backgroundTaskQueue)
        {
            _setting = setting;
            _logger = logger;
            _taskProcessor = taskProcessor;
            _backgroundTaskQueue = backgroundTaskQueue;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var interval = _setting?.RunInterval ?? 0;
            if (interval == 0)
            {
                _logger.LogWarning("Interval is not defined in settings. Set to default: 60 sec.");
                interval = 60;
            }

            _timer = new Timer((e) => ProcessTask(), null, TimeSpan.Zero, TimeSpan.FromSeconds(interval));

            return Task.CompletedTask;
        }

        private void ProcessTask()
        {
            if (Monitor.TryEnter(_sync))
            {
                _logger.LogInformation("Process started.");

                for (var i = 0; i < 20; i++) DoWork();

                _logger.LogInformation("Process finished.");
                Monitor.Exit(_sync);
            }
            else
            {
                _logger.LogWarning("Processing is currenly in progress. Skipped.");
            }
        }

        private void DoWork()
        {
            var number = _random.Next(20);
            _backgroundTaskQueue.QueueBackgroundWorkItem(token =>
            {
                return _taskProcessor.RunAsync(number, token);
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
