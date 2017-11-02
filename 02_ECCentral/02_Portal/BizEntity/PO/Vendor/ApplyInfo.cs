using System;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.BizEntity.PO.Vendor
{
    [Serializable]
    [DataContract]
    public class ApplyInfo
    {
        [DataMember]
        public int? SysNo { get; set; }

        [DataMember]
        public int SellerSysNo { get; set; }

        public string Memo { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string ApplicationType { get; set; }

        [DataMember]
        public DateTime InDate { get; set; }

        [DataMember]
        public string InUser { get; set; }

        [DataMember]
        public DateTime EditDate { get; set; }

        [DataMember]
        public string EditUser { get; set; }
    }
}
