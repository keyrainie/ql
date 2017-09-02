using System;
using System.Windows;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace ECCentral.BizEntity.Inventory
{
     [DataContract]
    public class ExperienceItemInfo : IIdentity, ICompany
    {
        public ExperienceItemInfo()
        {

        }

        [DataMember]
        public int? SysNo { get; set; }

        [DataMember]
        public int? ProductSysNo { get; set; }

        [DataMember]
        public string ProductID { get; set; }

        [DataMember]
        public string ProductName { get; set; }

        /// <summary>
        /// 调拨单号
        /// </summary>
        [DataMember]
        public int? AllocateSysNo { get; set; }

        [DataMember]
        public int? AllocateQty { get; set; }

        [DataMember]
        public string CompanyCode { get; set; }
    }
}
