using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SQNotificationService.Models;
using SQNotificationService.Models.Database;
using SQNotificationService.Security;

namespace SQNotificationService.Controllers
{
    [Authorize]
    [ImpersonationScopeRequired]
    public class NotificationResponseController : ApiController
    {
        private NotificationServiceDbContext db = new NotificationServiceDbContext();

        // GET: api/NotificationResponse?notificationId=5
        /// <summary>
        /// Get the response to a notification, if one exists. Otherwise, returns "null".
        /// </summary>
        /// <param name="externalReferenceId">The NSNotification.ExternalReferenceId of the notification.</param>
        /// <returns>An NSReponse or "null".</returns>
        public NSResponse GetNSResponse(string externalReferenceId)
        {
            NSResponse nSResponse = db.NSResponses.Find(externalReferenceId);
            if (nSResponse == null) 
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("No Response matching this referenceId was found"),
                    StatusCode = HttpStatusCode.NotFound
                });
            }

            return nSResponse;
        }

        // GET: api/NotificationResponse?afterDateTime={afterDateTime}
        /// <summary>
        /// Get ALL responses received after the specified time. Does not filter by user, phone number, etc.
        /// </summary>
        /// <param name="afterDateTime"></param>
        /// <returns></returns>
        public IEnumerable<NSResponse> GetByRespondedAfter(string afterDateTime)
        {
            DateTime convertedDate = DateTime.ParseExact(afterDateTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            return db.NSResponses.Where(n => n.RespondedAt > convertedDate);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NSResponseExists(string id)
        {
            return db.NSResponses.Count(e => e.ExternalReferenceID == id) > 0;
        }
    }
}