using System;
using System.Net;
using System.Text;
using System.Data;
using System.Net.Mail;
using System.Configuration;
using System.Collections;
using Twilio;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SQNotificationService.Models.Twilio;
using SQNotificationService.Models;
using System.Collections.Generic;
using SQNotificationService.Models.Database;
using SQNotificationService.Delivery;
using System.Threading;

namespace SQNotificationService.Delivery
{
    public class NotificationSender
    {
        public void Send(NSNotification notification)
        {
            //String.Compare(DeliveryTypes.Email, notification.DeliveryType, true).Equals(1)
            if (DeliveryTypes.Email.Equals(notification.DeliveryType,StringComparison.CurrentCultureIgnoreCase))
            {
                SendEMail(notification.Receiver, notification.MessageBody, notification.ExternalReferenceID, notification.MessageTitle, notification.ResponseNeeded);
                // TODO,, Should this add a NsMessageStatus record indicating whether the message was sent or not?
                 // If jeff see any use for emails. in this application
            }
            else if (DeliveryTypes.Sms.Equals(notification.DeliveryType, StringComparison.CurrentCultureIgnoreCase))
            {
                var sendSmsResult = SendSms(notification.Receiver, notification.MessageBody, notification.ResponseNeeded);

                if (sendSmsResult.Succeeded)
                {
                    using (NotificationServiceRepository notificationServiceRepository = new NotificationServiceRepository())
                    {
                        notificationServiceRepository.InsertSentMessageDetail(notification.ExternalReferenceID, sendSmsResult.MsgSid, sendSmsResult.MsgStatus, notification.Receiver);

                        if (notification.ResponseNeeded==true)
                        {
                            notificationServiceRepository.InsertResponseValue(notification.ExternalReferenceID, notification.Receiver, sendSmsResult.YesResponseValue, sendSmsResult.NoResponseValue);
                        }
                    }
                }

            }
        }

        public void SendEMail(String emailAddress, String notificationContent, String externalReferenceID, String subject, Boolean msgResponse)
        {
            String mailOrigin = ConfigurationManager.AppSettings["sendingEmailAddress"];
            String passWord = ConfigurationManager.AppSettings["emailAccountPassword"];
            String basePath = ConfigurationManager.AppSettings["basePath"];


            if (msgResponse == true)
            {
                String Yes = String.Empty;
                String No = String.Empty;

                // TODO,, Should probably still use response value codes to prevent spoofing.
                Yes = HttpUtility.UrlPathEncode(basePath + "api/Notification/GetEmailResponse?emailAddress=" + emailAddress + "&extRefID=" + externalReferenceID + "&response=" + 1);
                No = HttpUtility.UrlPathEncode(basePath + "api/Notification/GetEmailResponse?emailAddress=" + emailAddress + "&extRefID=" + externalReferenceID + "&response=" + 0);
                notificationContent += (" Please <a href='" + Yes + "'>click here for yes</a>  or");
                notificationContent += (" <a href='" + No + "'>click here for no</a>  Have a wonderful day. \nThanks!");
            }


            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(mailOrigin);
                msg.To.Add(new MailAddress(emailAddress));
                msg.Subject = subject;
                msg.Body += notificationContent;
                msg.IsBodyHtml = true;

                var smtp = new System.Net.Mail.SmtpClient();
                {
                    smtp.Host = ConfigurationManager.AppSettings["SMTPServerHost"];
                    smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPSeverPort"]);
                    smtp.EnableSsl = false;
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential(mailOrigin, passWord);
                    smtp.Timeout = 20000;
                }
                smtp.Send(msg);
            }
            catch (Exception err)
            {
                String Result = err.ToString();
            }

        }

        // TODO,, What happens if send to twilio times out or returns an error?
        public SendSmsResult SendSms(String toPhoneNumber, String notificationContent, Boolean msgResponse)
        {
            String accountSid = ConfigurationManager.AppSettings["accountSid"];
            String authToken = ConfigurationManager.AppSettings["TwilioAuthToken"];
            String fromPhoneNumber = ConfigurationManager.AppSettings["fromPhoneNumber"];
            String basePath = ConfigurationManager.AppSettings["basePath"];
            String statusCallback = HttpUtility.UrlPathEncode(basePath + "api/Twilio/SmsStatusCallback");

            String yes = String.Empty; String no = String.Empty; String msgSid = String.Empty; String msgStatus = String.Empty;

            if (msgResponse == true)
            {
                yes = UniqueIds.NewMessageCode();
                no = UniqueIds.NewMessageCode();
                notificationContent += ("Reply-->" + yes + " for Yes  or " + no + " for No");
            }
            var twilio = new TwilioRestClient(accountSid, authToken);
            var sms = twilio.SendSmsMessage(fromPhoneNumber, toPhoneNumber, notificationContent, statusCallback);

            return new SendSmsResult() { MsgSid = sms.Sid, MsgStatus = sms.Status, YesResponseValue = yes, NoResponseValue = no};
        }
    }
}