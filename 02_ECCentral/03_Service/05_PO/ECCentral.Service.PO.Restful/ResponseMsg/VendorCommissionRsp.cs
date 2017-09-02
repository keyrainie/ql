using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility.WCF;
using System.Runtime.Serialization;

namespace ECCentral.Service.PO.Restful.ResponseMsg
{
    [DataContract]
    public class VendorCommissionRsp : QueryResult
    {
        [DataMember]
        public decimal TotalAmt { get; set; } 
    }
}
