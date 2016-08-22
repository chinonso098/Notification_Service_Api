using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace SQNotificationService.Logging
{
    /// <summary>
    /// Logging event source with methods representing strong-typed log messages, for semantic logging.
    /// </summary>
    [EventSource(Name = "NotificationService")]
    public class ApiEventSource : EventSource
    {
        public enum EventId
        {
            Informational = 1000 + EventLevel.Informational,
            Warning = 1000 + EventLevel.Warning,
            NonFatalBadRequest = 1000 + HttpStatusCode.BadRequest
        }

        static readonly Lazy<ApiEventSource> Instance = new Lazy<ApiEventSource>(() => new ApiEventSource());

        ApiEventSource() { }

        public static ApiEventSource Log { get { return Instance.Value; } }

        [Event((int)HttpStatusCode.Accepted, Level = EventLevel.Informational)]
        internal void SuccessResponse(string message)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent((int)HttpStatusCode.Accepted, message);
            }
        }

        [Event((int)HttpStatusCode.BadRequest, Level = EventLevel.Error)]
        internal void ErrorResponse(string message, string request)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent((int)HttpStatusCode.BadRequest, message, request);
            }
        }

        [Event((int)HttpStatusCode.InternalServerError, Level = EventLevel.Error)]
        internal void InternalError(string message)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent((int)HttpStatusCode.InternalServerError, message);
            }
        }

        [Event((int)HttpStatusCode.Conflict, Level = EventLevel.Error)]
        internal void EntityValidationError(string message)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent((int)HttpStatusCode.Conflict, message);
            }
        }

        [Event((int)EventId.Informational, Level = EventLevel.Informational)]
        internal void Info(string message)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent((int)EventId.Informational, message);
            }
        }

        [Event((int)EventId.Warning, Level = EventLevel.Warning)]
        internal void Warning(string message)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent((int)EventId.Warning, message);
            }
        }

        [Event((int)EventId.NonFatalBadRequest, Level = EventLevel.Error)]
        internal void NonFatalBadRequest(string message, string request)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent((int)EventId.NonFatalBadRequest, message, request);
            }
        }
    }
}
