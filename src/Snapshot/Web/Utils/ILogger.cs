using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Utils
{
    public interface ILogger
    {
        void LogError(Exception ex, string contextualMessage = null);
    }
}