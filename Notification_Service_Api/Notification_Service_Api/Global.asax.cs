using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Configuration;

using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Sinks;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;
using System.Diagnostics.Tracing;

using SQNotificationService.Logging;

namespace SQNotificationService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        // SQL Logging sink.
        static ObservableEventListener listener;
        static SinkSubscription<SqlDatabaseSink> sqlSubscription;

        protected void Application_Start()
        {
            //qConfigureLog4Net();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Configure logging.
            listener = new ObservableEventListener();
            listener.EnableEvents(ApiEventSource.Log, EventLevel.LogAlways, Keywords.All);

            sqlSubscription = listener.LogToSqlDatabase("SQNotificationService", Properties.Settings.Default.LoggingConnectionString, "Traces", new TimeSpan(0, 0, 10), 1000, null, 30000);
        }

        void Application_End(object sender, EventArgs e)
        {
            if (sqlSubscription != null)
            {
                sqlSubscription.Dispose();
            }

            listener.DisableEvents(ApiEventSource.Log);
            listener.Dispose();
        }

    }
}
