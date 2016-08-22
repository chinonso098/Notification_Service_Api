using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace SQNotificationService.Logging
{
    public class ApiEventSourceExceptionLogger : ExceptionLogger
    {

        public override void Log(ExceptionLoggerContext context)
        {
            if (context.Exception != null)
            {
                string error = string.Format("Unhandled exception for {0} to {1}: {2}", context.Request.Method, context.Request.RequestUri, context.Exception);

                ApiEventSource.Log.InternalError(error);
            }
        }
    }
}
