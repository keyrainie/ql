using System;
using System.Net;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.PO
{
    [Serializable]
    [DataContract]
    public class VendorDeductInfo
    {
        [DataMember]
        public int DeductSysNo
        {
            get;
            set;
        }

       [DataMember]
        public VendorCalcType CalcType
        {
            get;
            set;
        }
        [DataMember]
        public decimal DeductPercent
        {
            get;
            set;
        }

       [DataMember]
        public decimal FixAmt
        {
            get;
            set;
        }

        [DataMember]
        public decimal MaxAmt
        {
            get;
            set;
        }   
    }
}
