using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using System.Xml.Serialization;

namespace ECCentral.BizEntity.RMA
{
    [XmlRoot("MessageHead")]
    public class SSBMessageHeader
    {
        //public SSBMessageHeader();

        public string Action { get; set; }
        public string CallbackAddress { get; set; }
        public string Comment { get; set; }
        public string CompanyCode { get; set; }
        public string CountryCode { get; set; }
        public string Description { get; set; }
        public string From { get; set; }
        public string FromSystem { get; set; }
        public string GlobalBusinessType { get; set; }
        public string Language { get; set; }
        public string Namespace { get; set; }
        public string OriginalGUID { get; set; }
        public string OriginalSender { get; set; }
        public string SellerID { get; set; }
        public string Sender { get; set; }
        public string Tag { get; set; }
        public string To { get; set; }
        public string ToSystem { get; set; }
        public string TransactionCode { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
    }
}