using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace SQNotificationService.Logging
{
    /// <summary>
    ///  Exception filter to receive and log all unhandled exceptions.
    /// </summary>
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Overrides the OnException event and logs the exception. 
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                ApiEventSource.Log.InternalError(context.Exception.ToString());
            }
            base.OnException(context);
        }
    }
}