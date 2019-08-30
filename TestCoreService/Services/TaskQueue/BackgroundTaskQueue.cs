using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace TestCoreService.Services.TaskQueue
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<Func<CancellationToken, Task>> _workItems = new ConcurrentQueue<Func<CancellationToken, Task>>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public int Size => _workItems.Count;

        public async Task<Func<CancellationToken, Task>> DequeuAsync(CancellationToken token)
        {
            await _signal.WaitAsync(token);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }

        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(workItem);
            _signal.Release();
        }
    }
}
