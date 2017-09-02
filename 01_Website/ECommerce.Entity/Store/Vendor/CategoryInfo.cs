using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Store.Vendor
{
    /// <summary>
    /// 类别信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class CategoryInfo 
    {
        /// <summary>
        /// 父级类别SysNo
        /// </summary>
        [DataMember]
        public int? ParentSysNumber { get; set; }
        /// <summary>
        /// 类别名称
        /// </summary>
        [DataMember]
        public LanguageContent CategoryName { get; set; }

        /// <summary>
        /// 类别状态
        /// </summary>
        [DataMember]
        public CategoryStatus Status { get; set; }

        /// <summary>
        /// 基于类别设置指标
        /// </summary>
        //[DataMember]
        //public CategorySetting CategorySetting { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 类别ID
        /// </summary>
        [DataMember]
        public string CategoryID { get; set; }

        /// <summary>
        /// 操作类型 
        /// </summary>
        [DataMember]
        public int OperationType { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [DataMember]
        public BusinessType? BusinessType { get; set; }

        /// <summary>
        /// 销售策略
        /// </summary>
        [DataMember]
        public SalesPolicyType? SalesPolicyType { get; set; }

        /// <summary>
        /// 前台UI展示模式
        /// </summary>
        //[DataMember]
        //public UIModeType? UIModeType { get; set; }

    }
}
