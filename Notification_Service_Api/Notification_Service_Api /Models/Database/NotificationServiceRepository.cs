using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQNotificationService.Models.Twilio;
using SQNotificationService.Delivery;
using System.Data.Entity;

namespace SQNotificationService.Models.Database
{
    public class NotificationServiceRepository : IDisposable
    {
        ValidationOutput Error = new ValidationOutput();
        ErrorMessages errorMessage = new ErrorMessages();
        NotificationServiceDbContext dbContext = new NotificationServiceDbContext();

        public void InsertNotification(NSNotification notification)
        {
            notification.ExternalReferenceID = UniqueIds.NewExternalReferenceId();
            notification.CreatedAt = DateTime.Now;
            notification.Receiver = notification.Receiver.Replace(@"+1", String.Empty);

            dbContext.NSNotifications.Add(notification);
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Find the NSNotification with the given ExternalReferenceID.
        /// </summary>
        /// <param name="externalReferenceId"></param>
        /// <returns>The NSNotification, or null if not found.</returns>
        public NSNotification GetNotificationById(string externalReferenceId)
        {
            return dbContext.NSNotifications.Find(externalReferenceId);
        }

        public bool NotificationExists(string id)
        {
            return dbContext.NSNotifications.Count(e => e.ExternalReferenceID == id) > 0;
        }

        /// <summary>
        /// Updates an NsNotification entity in the database. 
        /// </summary>
        /// <param name="notification"></param>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateConcurrencyException"></exception>
        public void UpdateNotification(NSNotification notification)
        {
            dbContext.Entry(notification).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public NSMessageStatu GetMessageStatusById(String externalReferenceId)
        {
            NSMessageStatu messageStatus = new NSMessageStatu();

            NotificationServiceDbContext dbContext = new NotificationServiceDbContext();

            messageStatus = (from msgStatus in dbContext.NSMessageStatus
                                                  where msgStatus.ExternalReferenceID == externalReferenceId
                                                  orderby msgStatus.ModifiedAt descending
                                                  select msgStatus).FirstOrDefault();
            return messageStatus;
        }

        public NSResponse GetMessageResponseById(String id)
        {
            return dbContext.NSResponses.Find(id);
        }

        public void InsertSentMessageDetail(String externalRefID, String msgSid, String msgStatus, String phoneNumber)
        {
            NSSentSmsMessage sentMessage = new NSSentSmsMessage()
            {
                ExternalReferenceID = externalRefID,
                MsgSid = msgSid,
                PhoneNumber = phoneNumber.Replace(@"+1", String.Empty),
                SentAt = System.DateTime.UtcNow
            };

            NSMessageStatu currentMsgStatus = new NSMessageStatu()
            {
                MsgSid = msgSid,
                ExternalReferenceID = externalRefID,
                MessageStatus = msgStatus
            };

            dbContext.NSSentSmsMessages.Add(sentMessage);
            dbContext.NSMessageStatus.Add(currentMsgStatus);

            dbContext.SaveChanges();
        }

        public void UpdateSentMessageDetail(StatusCallback statusCallback)
        {
            NSMessageStatu gottenFromDb = dbContext.NSMessageStatus.Find(statusCallback.MessageSid);

            gottenFromDb.ErrorCode = statusCallback.ErrorCode;
            gottenFromDb.MsgSid = statusCallback.MessageSid;
            gottenFromDb.MessageStatus = statusCallback.MessageStatus;
            gottenFromDb.ExternalReferenceID = gottenFromDb.ExternalReferenceID;
            gottenFromDb.ErrorMessage = errorMessage.GetErrorMessage(statusCallback.ErrorCode);
            gottenFromDb.ErrorDescription = errorMessage.GetErrorDescription(statusCallback.ErrorCode);
            gottenFromDb.ModifiedAt = System.DateTime.UtcNow;

            dbContext.Entry(gottenFromDb).State = EntityState.Modified;

            dbContext.SaveChanges();
        }

        public void InsertEmailNotificationResponse(String externalReferenceID, Boolean response, String responderInfo)
        {
            var validationResult = ValidateEmailResponseTime(externalReferenceID, response);

            if (validationResult == true)
            {
                NSResponse notificationResponse = new NSResponse()
                {
                    ExternalReferenceID = externalReferenceID,
                    ResponderInfo = responderInfo,
                    Response = response,
                    RespondedAt = DateTime.UtcNow
                };

                dbContext.NSResponses.Add(notificationResponse);

                dbContext.SaveChanges();
            }
        }

        // validate response time
        // TODO,, This should either be in IValidatableObject.Validate or in Core or controller logic.
        public static Boolean ValidateEmailResponseTime(String externalReferenceID, Boolean response)
        {

            NotificationServiceDbContext dbContext = new NotificationServiceDbContext();
            TimeSpan timeSpan = new TimeSpan();
            TimeSpan notificationDuration = new TimeSpan();

            String errorMessage = String.Empty; String successMessage = String.Empty;


            NSNotification notification = dbContext.NSNotifications.Find(externalReferenceID);

            if (!notification.EndsAt.HasValue)
            {
                return true;
            }
            else
            {
                notificationDuration = Convert.ToDateTime(notification.EndsAt) - notification.CreatedAt;
                timeSpan = Convert.ToDateTime(notification.EndsAt) - DateTime.UtcNow;

                if (timeSpan.Minutes > notificationDuration.Minutes)
                {
                    errorMessage = notification.ErrorMessage;
                    return true;
                }
                else
                {
                    return true;
                }
            }
        }

        public void InsertOrUpdateSmsNotificationResponse(string externalReferenceID, string responseFrom, string responseCodeValue)
        {
            NSResponse checkForAResponse = dbContext.NSResponses.Find(externalReferenceID);

            if (checkForAResponse == null) //data doesn't exist in the db
            {
                NSResponse notificationResponse = new NSResponse()
                {
                    ExternalReferenceID = externalReferenceID,
                    ResponderInfo = responseFrom.Replace(@"+1", String.Empty),
                    Response = Convert.ToBoolean(Convert.ToInt32(responseCodeValue)),
                    RespondedAt = DateTime.Now
                };

                dbContext.NSResponses.Add(notificationResponse);
            }
            else
            {
                checkForAResponse.Response = Convert.ToBoolean(Convert.ToInt32(responseCodeValue));
                checkForAResponse.ResponderInfo = responseFrom.Replace(@"+1", String.Empty);
                checkForAResponse.RespondedAt = DateTime.Now;

                dbContext.Entry(checkForAResponse).State = EntityState.Modified;
            }

            dbContext.SaveChanges();
        }

        public void InsertResponseValue(String externalReferenceID, String phoneNumber, String yes, String no)
        {
            String yesCodeValue = "1"; String noCodeValue = "0";

            NSResponseValue yesResponse = new NSResponseValue()
            {
                ExternalReferenceID = externalReferenceID,
                PhoneNumber = phoneNumber.Replace(@"+1", String.Empty),
                ResponseCode = yes,
                ResponseCodeValue = yesCodeValue
            };

            NSResponseValue noResponse = new NSResponseValue()
            {
                ExternalReferenceID = externalReferenceID,
                PhoneNumber = phoneNumber.Replace(@"+1", String.Empty),
                ResponseCode = no,
                ResponseCodeValue = noCodeValue
            };

            dbContext.NSResponseValues.Add(yesResponse);
            dbContext.NSResponseValues.Add(noResponse);
            dbContext.SaveChanges();



        }

        public NSResponseValue GetResponseValueByResponse(string phoneNumber, string responseCode)
        {
            // Find an NSResponseValue that matches the body of the message. 
            var responseValueQuery = from msgValues in dbContext.NSResponseValues
                                     where msgValues.PhoneNumber == phoneNumber.Replace(@"+1", String.Empty) && msgValues.ResponseCode == responseCode
                                     select msgValues;

            return responseValueQuery.FirstOrDefault();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbContext.Dispose();
            }
        }
    }
}