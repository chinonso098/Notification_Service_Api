using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQNotificationService.Models.Twilio
{
    public class ErrorMessages
    {
        public String GetErrorMessage(String errorCode) 
        {
            if (!String.IsNullOrEmpty(errorCode)) 
            {
                String result = errorCode.Remove(0, 3);

                int arryIndex = Convert.ToInt32(result);

                String[] meaningOfErrorCode = { "error 0","Queue overflow", 
                                    "Account suspended",
                                    "Unreachable destination handset",
                                    "Message blocked",
                                    "Unknown destination handset",
                                    "Landline or unreachable carrier","Carrier violation","Unknown error"};

                String errorMessage = meaningOfErrorCode[arryIndex];
                return errorMessage; 
            }
            else 
            {
                return null; 
            }


        }


        public String GetErrorDescription(String errorCode) 
        {
            if (!String.IsNullOrEmpty(errorCode))
            {
                String result = errorCode.Remove(0, 3);

                int arryIndex = Convert.ToInt32(result);

                String[] errorCodeDescription = { "error 0","You tried to send too many messages too quickly and your message queue overflowed. Try sending your message again after waiting some time.", 
                                    "Your account was suspended between the time of message send and delivery. Please contact Twilio.",
                                    "The destination handset you are trying to reach is switched off or otherwise unavailable.",
                                    "The destination number you are trying to reach is blocked from receiving this message (e.g. due to blacklisting).",
                                    "The destination number you are trying to reach is unknown and may no longer exist.",
                                    "The destination number is unable to receive this message. Potential reasons could include trying to reach a landline or, in the case of short codes, an unreachable carrier.",
                                    "Your message content was flagged as going against carrier guidelines.",
                                    "he error does not fit into any of the above categories."};

                String errorMessageDescription = errorCodeDescription[arryIndex];
                return errorMessageDescription;
            }
            else
            {
                return null;
            }
        }
    }
}