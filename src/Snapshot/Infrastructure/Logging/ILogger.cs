using System;

namespace Infrastructure.Logging
{
    public interface ILogger
    {
        void LogError(Exception ex, string contextualMessage = null);
        void LogInfo(string message);
    }
}