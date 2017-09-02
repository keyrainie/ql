using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 供应商锁定关联PM信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class VendorHoldPMInfo
    {
        /// <summary>
        /// 是否被选中
        /// </summary>
      [DataMember]
        public bool IsChecked { get; set; }

        /// <summary>
        /// 供应商系统编号
        /// </summary>
      [DataMember]
      public int? VendorSysNo { get; set; }

        /// <summary>
        /// PM系统编号
        /// </summary>
      [DataMember]
      public int? PMSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// PM ID
        /// </summary>
      [DataMember]
      public string PMID
        {
            get;
            set;
        }

        /// <summary>
        /// PM名称
        /// </summary>
      [DataMember]
      public string PMName
        {
            get;
            set;
        }

        /// <summary>
        /// 创建人名称
        /// </summary>
      [DataMember]
      public string InUser { get; set; }
    }
}
