using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SQNotificationService.Delivery;
using SQNotificationService.Models.Twilio;
using SQNotificationService.Models;
using SQNotificationService.Models.Database;
using System.Threading.Tasks;


namespace SQNotificationService.Controllers
{
    [ValidateTwilioRequest]
    public class TwilioController : ApiController
    {
        NotificationServiceRepository repository = new NotificationServiceRepository();
        CustomErrors error = new CustomErrors();

        /// <summary>
        ///  Twilio calls the application using this api to call update the status of the message 
        /// </summary>
        [HttpPost]
        public void SmsStatusCallback(StatusCallback statusCallback)
        {
           // repository.UpdateSentMessageDetail(statusCallback);
        }

        /// <summary>
        /// Twilio calls the application using this api to call store recievers response in the database
        /// </summary>
        [HttpPost]
        public void SmsMessageResponse(MessageRequest msgRequest)
        {
            NotificationSender replyNotification = new NotificationSender();
            TimeSpan timeSpan = new TimeSpan();
            TimeSpan notificationDuration = new TimeSpan();

            int characterCount = msgRequest.Body.ToCharArray().Count(); int characterLimit = 4; 

            if (characterCount.Equals(characterLimit))
            {
                NotificationServiceDbContext dbContext = new NotificationServiceDbContext();
  
                // query the database using the body of the message and the phone number and use it to get the response value if it exists
                NSResponseValue responseValue = repository.GetResponseValueByResponse(msgRequest.From, msgRequest.Body);

                if (responseValue == null)
                {
                    replyNotification.SendSms(msgRequest.From, error.GetErrorMessage(8), false);
                }
                else 
                {
                    //NSNotification notification = repository.GetNotificationById(responseValue.ExternalReferenceID);

                    NSNotification notification = dbContext.NSNotifications.Find(responseValue.ExternalReferenceID);

                    if (!notification.EndsAt.HasValue) 
                    {
                        replyNotification.SendSms(msgRequest.From, error.NotAnErrorMessage(1), false);
                        repository.InsertOrUpdateSmsNotificationResponse(notification.ExternalReferenceID, msgRequest.From, responseValue.ResponseCodeValue);
                    }
                    else 
                    {
                        notificationDuration = Convert.ToDateTime(notification.EndsAt) - notification.CreatedAt;
                        timeSpan = Convert.ToDateTime(notification.EndsAt) - DateTime.UtcNow;

                        if (timeSpan.Minutes > notificationDuration.Minutes)
                        {
                            replyNotification.SendSms(msgRequest.From, notification.ErrorMessage, false);
                            repository.InsertOrUpdateSmsNotificationResponse(notification.ExternalReferenceID, msgRequest.From, responseValue.ResponseCodeValue);
                        }
                        else
                        {
                            replyNotification.SendSms(msgRequest.From, error.NotAnErrorMessage(1), false);
                            repository.InsertOrUpdateSmsNotificationResponse(notification.ExternalReferenceID, msgRequest.From, responseValue.ResponseCodeValue);
                        }
                    }
                }
                
            }
            else 
            {
                // character limit exceeded
                replyNotification.SendSms(msgRequest.From, error.GetErrorMessage(6), false);
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
