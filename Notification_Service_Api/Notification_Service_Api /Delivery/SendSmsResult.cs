using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQNotificationService.Delivery
{
    public class SendSmsResult
    {
        public string MsgSid { get; set; }
        public string MsgStatus { get; set; }
        public string YesResponseValue { get; set; }
        public string NoResponseValue { get; set; }
        public bool Succeeded { get { return !string.IsNullOrEmpty(MsgSid);  } }
    }
}