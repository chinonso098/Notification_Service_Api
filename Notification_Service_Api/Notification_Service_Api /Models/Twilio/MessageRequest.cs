using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQNotificationService.Models.Twilio
{
    public class MessageRequest
    {
        public String MessageSid { get; set; }
        public String SmsStatus { get; set; }
        public String SmsSid { get; set; }
        public DateTimeOffset DateCreated { get; set;}
        public DateTimeOffset DateSent { get; set; }
        public String AccountSid { get; set; }
        public String ApiVersion { get; set; }
        public String From { get; set; }
        public String To { get; set; }
        public String Uri { get; set; }
        public String Body { get; set; }
        int NumMedia { get; set; }


        
    }
}