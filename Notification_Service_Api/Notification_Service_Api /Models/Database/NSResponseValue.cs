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
    
    public partial class NSResponseValue
    {
        public int ID { get; set; }
        public string ExternalReferenceID { get; set; }
        public string PhoneNumber { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseCodeValue { get; set; }
    
        public virtual NSNotification NSNotification { get; set; }
    }
}
