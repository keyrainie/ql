using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 仓库24小时服务设置
    /// </summary>
    [Serializable]
    [DataContract]
    public class AreaDeliveryInfo:IIdentity,ICompany
    {
        [DataMember]
        public int? SysNo { get; set; }

        [DataMember]
        public int? WHArea { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string DeliveryScope { get; set; }

        [DataMember]
        public int? Priority { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string CompanyCode { get; set; }

        [DataMember]
        public DateTime InDate { get; set; }

        [DataMember]
        public string InUser { get; set; }

        [DataMember]
        public DateTime EditDate { get; set; }

        [DataMember]
        public string EditUser { get; set; }

        [DataMember]
        public string WarehouseName { get; set; }
    }
}
