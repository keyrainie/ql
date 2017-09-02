using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Inventory
{
    [DataContract]
    public class ExperienceInfo : IIdentity
    {
        public ExperienceInfo()
        {

        }

        #region 属性

        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        ///  仓库编号
        /// </summary>
        [DataMember]
        public string StockSysNo { get; set; }

        /// <summary>
        /// 调拨类型
        /// </summary>
        [DataMember]
        public AllocateType AllocateType { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public int? InUser { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [DataMember]
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [DataMember]
        public int? EditUser { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        [DataMember]
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        [DataMember]
        public int? AuditUser { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        [DataMember]
        public DateTime? AuditDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public ExperienceHallStatus Status { get; set; }

        [DataMember]
        public string Meno { get; set; }

        [DataMember]
        public List<ExperienceItemInfo> ExperienceItemInfoList { get; set; }

        #endregion 属性
    }
}
