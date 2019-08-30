using System;
using System.Collections.Generic;
using System.Text;

namespace TestCoreService.Services
{
    public interface ISettingService
    {
        int WorkersCount { get; }
        int RunInterval { get; }
        string ResultPath { get; }
        string InstanceName { get; }
    }
}
