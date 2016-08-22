using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQNotificationService.Delivery
{
    public class ValidationOutput
    {
        public Boolean State { get; set; }
        public int ErrorCode { get; set; }

        public void SetError(Boolean state, int errorCode) 
        {
            State = state;
            ErrorCode = errorCode;
        }
    }
}