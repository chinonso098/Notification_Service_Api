using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQNotificationService.Models.Twilio
{
    public class StatusCallback: MessageRequest
    {
        public String MessageStatus { get; set; }
        public String ErrorCode { get; set; }
        public new String  MessageSid { get; set; }

    }
}