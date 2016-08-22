using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

using SQNotificationService.Delivery;

namespace SQNotificationService.Models.Database
{
    /// <summary>
    /// Adds validation and metadata to the class.
    /// </summary>
    [MetadataType(typeof(NSNotificationMetadata))]
    public partial class NSNotification : IValidatableObject
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
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            Regex phonePattern = new Regex(@"(\+1\d{10})|(\d{10})$");
            Regex emailPattern = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            if ((this.EndsAt.HasValue && String.IsNullOrEmpty(this.ErrorMessage)) || (!this.EndsAt.HasValue && !String.IsNullOrEmpty(this.ErrorMessage)))
            {
                yield return new ValidationResult(CustomErrors.GetErrorMessage(9));
            }

            if ((this.EndsAt.HasValue && !String.IsNullOrEmpty(this.ErrorMessage)))
            {
                if (this.ResponseNeeded == false)
                {
                    yield return new ValidationResult(CustomErrors.GetErrorMessage(10));
                }
            }
            if (phonePattern.IsMatch(this.Receiver))
            {
                if (DeliveryTypes.Sms != this.DeliveryType)
                {
                    yield return new ValidationResult(CustomErrors.GetErrorMessage(1));

                }

                // Validate character count limit.
                // TODO,, characterLimit should be in configuration.
                int characterCount = this.MessageBody.ToCharArray().Count();
                int characterLimit = 1600;
                if (characterCount > characterLimit)
                {
                    yield return new ValidationResult(CustomErrors.GetErrorMessage(4));
                }

            }
            else if (emailPattern.IsMatch(this.Receiver))
            {
                if (DeliveryTypes.Email != this.DeliveryType)
                {
                    yield return new ValidationResult(CustomErrors.GetErrorMessage(2));
                }
            }
            else
            {
                yield return new ValidationResult(CustomErrors.GetErrorMessage(3));
            }
        }
    }

    /// <summary>
    /// Metadata for validation and serialization of the class.
    /// </summary>
    public class NSNotificationMetadata
    {
        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<NSMessageStatu> NSMessageStatus { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual NSResponse NSResponse { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<NSResponseValue> NSResponseValues { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<NSSentSmsMessage> NSSentSmsMessages { get; set; }
    }
}
