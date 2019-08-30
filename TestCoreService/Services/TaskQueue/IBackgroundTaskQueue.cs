using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestCoreService.Services.TaskQueue
{
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// Размер очереди
        /// </summary>
        int Size { get; }
        /// <summary>
        /// Принимает задачу в очередь
        /// </summary>
        /// <param name="workItem"></param>
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);
        /// <summary>
        /// Извлекает из очереди задачу
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Func<CancellationToken, Task>> DequeuAsync(CancellationToken token);
    }
}
