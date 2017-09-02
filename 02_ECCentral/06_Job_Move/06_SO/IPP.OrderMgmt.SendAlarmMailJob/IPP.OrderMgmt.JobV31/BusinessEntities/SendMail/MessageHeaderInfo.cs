using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.SendMail
{
    [Serializable]
    public class MessageHeaderInfo
    {
        public string Action { get; set; }

        public string CallbackAddress { get; set; }

        public string CompanyCode { get; set; }

        public string CountryCode { get; set; }

        public string Description { get; set; }

        public string From { get; set; }

        public string FromSystem { get; set; }

        public int GlobalBusinessType { get; set; }

        public string Language { get; set; }

        public string NameSpace { get; set; }

        public OperationUserInfo OperationUser { get; set; }

        public string OriginalGUID { get; set; }

        public string OriginalSender { get; set; }

        //public List<Preferences> Preferences { get; set; }

        public string Sender { get; set; }

        public string StoreCompanyCode { get; set; }

        public string Tag { get; set; }

        public string TimeZone { get; set; }

        public string To { get; set; }

        public string ToSystem { get; set; }

        public string TransactionCode { get; set; }

        public string Type { get; set; }

        public string Version { get; set; }
    }

    [Serializable]
    public class OperationUserInfo
    {
        public string CompanyCode { get; set; }

        public string FullName { get; set; }

        public string LogUserName { get; set; }

        public string SourceDirectoryKey { get; set; }

        public string SourceUserName { get; set; }

        public string UniqueUserName { get; set; }
    }
}
