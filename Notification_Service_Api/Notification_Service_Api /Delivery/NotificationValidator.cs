using System;
using System.Collections.Generic;
using System.Linq;
using SQNotificationService.Models.Database;
using SQNotificationService.Controllers;
using SQNotificationService.Delivery;
using System.Web;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using System.Net;

namespace SQNotificationService.Delivery
{

    public class NotificationValidator 
    {
        /// <summary>
        /// Validate entity state.
        ///
        /// This function ensures that the reciever information macthes the  message delivery type 
        /// To: XXX-XXX-XXXX Delivery type: SMS
        /// To: xxx@xxx.com  Delivery type: Email
        ///    
        /// if the notification is an sms, then a second validation is performed on the message
        /// body to ensure that it is no greater that 1600 character
        /// 
        /// ErrorCode: every error code expect for 00 has a specific error message
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public static ValidationOutput Validate(NSNotification notification)
        {
            ValidationOutput Result = new ValidationOutput();
            Regex phonePattern = new Regex(@"(\+1\d{10})|(\d{10})$");
            Regex emailPattern = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            if ((notification.EndsAt.HasValue && String.IsNullOrEmpty(notification.ErrorMessage)) || (!notification.EndsAt.HasValue && !String.IsNullOrEmpty(notification.ErrorMessage)))
            {
                Result.SetError(false, 9);
                return Result;
               // yield return new ValidationResult(CustomErrors.GetErrorMessage(9));
            }
            if ((notification.EndsAt.HasValue && !String.IsNullOrEmpty(notification.ErrorMessage)))
            {
                if (notification.ResponseNeeded == false)
                {
                    Result.SetError(false, 10);
                    return Result;
                }
            }
            if (phonePattern.IsMatch(notification.Receiver))
            {
                if (String.Compare(DeliveryTypes.Sms, notification.DeliveryType,true).Equals(1))
                {
                    Result.SetError(false, 1);
                    return Result;
                }
                // Validate character count limit.
                // TODO,, characterLimit should be in configuration.
                int characterCount = notification.MessageBody.ToCharArray().Count();
                int characterLimit = 1600;
                if (characterCount > characterLimit)
                {
                    Result.SetError(false, 4);
                    return Result;
                }
            }
            else if (emailPattern.IsMatch(notification.Receiver))
            {
                if (DeliveryTypes.Email != notification.DeliveryType)
                {
                    Result.SetError(false, 2);
                    return Result;
                }
            }
            else
            {
                Result.SetError(false, 3);
                return Result;
            }
            Result.SetError(true,0);
            return Result;

        }


    }

}