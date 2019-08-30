using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestCoreService.Services;

namespace TestCoreService.Processors
{
    public class TaskProcessor : ITaskProcessor
    {
        private readonly ILogger _logger;
        private readonly ISettingService _setting;
        public TaskProcessor(ILogger<TaskProcessor> logger, ISettingService setting)
        {
            _logger = logger;
            _setting = setting;
        }

        public async Task RunAsync(int number, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            Func<int, int> fibonacci = null;
            fibonacci = (num) =>
            {
                if (num < 2) return 1;
                else return fibonacci(num - 1) + fibonacci(num - 2);
            };

            var result = await Task.Run(async () =>
            {
                await Task.Delay(1000);
                return Enumerable.Range(0, number).Select(n => fibonacci(n));
            }, token);

            var output = string.Join(" ", result);
            using (var writer = new StreamWriter(_setting.ResultPath, true, Encoding.UTF8))
            {
                writer.WriteLine($"{DateTime.Now.ToString()}: {output}");
            }

            _logger.LogInformation($"Task finished. Result: {output}");
        }
    }
}
