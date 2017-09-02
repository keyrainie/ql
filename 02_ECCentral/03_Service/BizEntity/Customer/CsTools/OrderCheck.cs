using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 自动审单配置项
    /// </summary>
    public class OrderCheckMaster : IWebChannel, IIdentity
    {
        /// <summary>
        /// 自动审单项系统编号
        /// </summary>
        public int? SysNo { get; set; }
        /// <summary>
        /// 自动审单项编码
        /// </summary>
        public string CheckType { get; set; }
        /// <summary>
        /// 自动审单项描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 该审核项的状态（是否有效）
        /// </summary>
        public OrderCheckStatus? Status { get; set; }
        /// <summary>
        /// 该审核项对应的子配置项
        /// </summary>
        public List<OrderCheckItem> OrderCheckItemList { get; set; }
        #region IWebChannel Members
        /// <summary>
        /// 所属渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }
        #endregion

        #region ICompany Members
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode { get; set; }
        #endregion
    }
    /// <summary>
    /// 自动审单子项
    /// </summary>
    public class OrderCheckItem : IIdentity,IWebChannel
    {
        /// <summary>
        /// 自动审单子项系统编号
        /// </summary>
        public int? SysNo { get; set; }
        /// <summary>
        /// 自动审单子项Code
        /// </summary>
        public string ReferenceType { get; set; }
        /// <summary>
        /// 自动审单子项值 ReferenceContent和Description 都是存的值，请根据不同的子项类型，做相应的处理
        /// </summary>
        public string ReferenceContent { get; set; }
        /// <summary>
        /// 自动审单子项值
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 自动审单子项状态，看是否有效
        /// </summary>
        public OrderCheckStatus? Status { get; set; }
        /// <summary>
        /// 特殊的属性，用于BizProcessor中构建查询条件
        /// </summary>
        public string ReferenceTypeIn { get; set; }

        #region IWebChannel Members
        /// <summary>
        /// 所属渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }

        #endregion

        #region ICompany Members
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode { get; set; }

        public string SysNos { get; set; }

        #endregion
    }
}
