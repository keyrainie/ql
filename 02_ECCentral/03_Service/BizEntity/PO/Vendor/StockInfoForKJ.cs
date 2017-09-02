using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECCentral.BizEntity.PO.Vendor
{
    [Serializable]
    [DataContract]
    public class StockInfoForKJ
    {
        [DataMember]
        public string StockID
        {
            get;
            set;
        }

        [DataMember]
        public string StockName
        {
            get;
            set;
        }

        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }

        [DataMember]
        public string LanguageCode
        {
            get;
            set;
        }

        [DataMember]
        public string StoreCompanyCode
        {
            get;
            set;
        }

        [DataMember]
        public string CountryCode
        {
            get;
            set;
        }

        [DataMember]
        public int MerchantSysNo
        {
            get;
            set;
        }
        [DataMember]
        public int Status
        {
            get;
            set;
        }
    }
}
