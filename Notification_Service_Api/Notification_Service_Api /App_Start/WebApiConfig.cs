using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;

using System.Net.Http;
using System.Web;
using System.Web.Http.ExceptionHandling;
using SQNotificationService.Logging;

namespace SQNotificationService
{
    public static class WebApiConfig
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = System.Web.Http.RouteParameter.Optional }
            );

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            // Add a filter to receive and log unhandleed exceptions thrown by WebAPI controllers.
            config.Filters.Add(new SQNotificationService.Logging.ExceptionHandlingAttribute());

            // Add a filter to log ALL unhandleed exceptions.
            //config.Services.Add(typeof(System.Web.Http.ExceptionHandling.IExceptionLogger), new ApiEventSourceExceptionLogger());

        }
    }
}
