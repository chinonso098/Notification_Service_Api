using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SQNotificationService.Delivery
{
    public class CustomErrors
    {
        public String GetErrorMessage(int errorValue)
        {
            String[] errorDecription = { "error 0",
                                        "A notification intended for SMS delivery cannot be sent using an email delivery method", 
                                        "A notification intended for Email delivery cannot be sent using a sms delivery method",
                                        "phone or email pattern is not a match. For phone (+1xxxxxxxxxx) and for email(*@*.com)",
                                        "Sms character limit exceeded, Max of 1600",
                                        "Failed to stored message in database",
                                        "Sorry, our system only accepts a 4 character response code",
                                        "Sorry, you have exceeded the time frame to respond to this notification",
                                        "Sorry this code doesn't exist in our database, it is either incorrect, or this message doesn't require a response",
                                        "If there is an end date, then there must be an error message and vice versa",
                                        "If there  is and end date and a errormessage, then response must be set to true"
                                       };

            return errorDecription[errorValue];
        }

        public String NotAnErrorMessage(int succesValue)
        {
            String[] messageDecription = { "error 0",
                                           "Thank you, your respose has been noted", 
                                         };

            return messageDecription[succesValue];
        }


        //public HttpResponseMessage CustomHttpError(String ErrorMessage)
        //{
        //    HttpResponseMessage errorResponse = new HttpResponseMessage();
        //    var httpError = new HttpError(ErrorMessage);
        //    errorResponse = Request.CreateErrorResponse(HttpStatusCode.Conflict, httpError);
        //    return errorResponse;
        //}



    }
}