using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestCoreService.Processors
{
    public interface ITaskProcessor
    {
        Task RunAsync(int number, CancellationToken token);
    }
}
