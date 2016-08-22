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
using SQNotificationService.Delivery;
using SQNotificationService.Models;
using SQNotificationService.Models.Database;
using SQNotificationService.Security;

namespace SQNotificationService.Controllers
{

    public class NotificationController : ApiController
    {
        private NotificationServiceRepository repository = new NotificationServiceRepository();
        private CustomErrors error = new CustomErrors();
        private CustomErrors error1 = new CustomErrors();


        /// <summary>
        /// Return a notification object in Json format if it exists. Otherwise, return null
        /// </summary>
        /// 
        // GET: api/Notification/5
        [ResponseType(typeof(NSNotification))]
        public IHttpActionResult GetNSNotification(string id)
        {
            NSNotification nSNotification = repository.GetNotificationById(id);
            if (nSNotification == null)
            {
                return NotFound();
            }

            return Ok(nSNotification);
        }

        /// <summary>
        ///  No updates are allowed here. :)
        /// </summary>
        // PUT: api/Notification/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutNSNotification(string id, NSNotification notification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != notification.ExternalReferenceID)
            {
                return BadRequest();
            }

            try
            {
                repository.UpdateNotification(notification);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!repository.NotificationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }



        
        // POST: api/Notification
        /// <summary>
        ///  Create a new notification .
        /// </summary>
        /// <param name="Notifcation Object(jSon)"></param>
        /// <returns> externalReferenceid</returns>
        public String PostNSNotification(NSNotification notification)
        {
            NotificationSender sender = new NotificationSender();
            var validateNotification = NotificationValidator.Validate(notification);
            if (validateNotification.State.Equals(false))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                {
                    Content = new StringContent((error.GetErrorMessage(validateNotification.ErrorCode))),
                    StatusCode = HttpStatusCode.PreconditionFailed
                });
            }
            else 
            {
                repository.InsertNotification(notification);
                sender.Send(notification);
                return  notification.ExternalReferenceID ;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                repository.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}