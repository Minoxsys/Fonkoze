using Elmah;
using System;
using System.Web;

namespace Infrastructure.Logging
{
    public class ElmahLogger : ILogger
    {
        public void LogError(Exception ex, string contextualMessage = null)
        {
            try
            {
                // log error to Elmah
                if (contextualMessage != null)
                {
                    // log exception with contextual information that's visible when 
                    // clicking on the error in the Elmah log
                    var annotatedException = new Exception(contextualMessage, ex);
                    ErrorSignal.FromCurrentContext().Raise(annotatedException, HttpContext.Current);
                }
                else
                {
                    ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                }
            }
            catch
            {
                //just keep going
            }
        }

        public void LogInfo(string message)
        {
            throw new NotImplementedException();
        }
    }
}