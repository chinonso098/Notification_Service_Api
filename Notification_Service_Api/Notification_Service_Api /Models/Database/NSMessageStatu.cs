//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SQNotificationService.Models.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class NSMessageStatu
    {
        public string MsgSid { get; set; }
        public string ExternalReferenceID { get; set; }
        public string MessageStatus { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public Nullable<System.DateTime> ModifiedAt { get; set; }
    
        public virtual NSNotification NSNotification { get; set; }
    }
}
