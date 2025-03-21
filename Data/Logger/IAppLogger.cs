using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeterChangeApi.Data.Logger
{
    public interface IAppLogger
    {
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(Exception ex, string message, params object[] args);
    }
}