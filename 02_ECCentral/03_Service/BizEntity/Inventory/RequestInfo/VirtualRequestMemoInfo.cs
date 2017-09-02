using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.PO;

namespace ECCentral.BizEntity.Inventory.RequestInfo
{
    /// <summary>
    /// 商品库存单据-虚库申请单操作日志
    /// </summary>
    public class VirtualRequestMemoInfo : IIdentity
    {
        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        #endregion IIdentity Members

        /// <summary>
        /// 虚库单系统编号
        /// </summary>
        public int RequestSysNo { get; set; }

        /// <summary>
        /// 虚库单操作类型状态
        /// </summary>
        public VirtualRequestActionType? ActionType { get; set; }

        /// <summary>
        /// 虚库单状态
        /// </summary>
        public VirtualRequestStatus RequestStatus { get; set; }

        /// <summary>
        /// 调整虚库库存
        /// </summary>
        public int AdjustVirtualQty { get; set; }

        /// <summary>
        /// 原始虚库库存 
        /// </summary>
        public int VirtualQtyOrigin { get; set; }

        /// <summary>
        /// 虚库库存 
        /// </summary>
        public int VirtualQty { get; set; }

        /// <summary>
        /// 备注 
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public UserInfo CreateUser { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime? EditDate { get; set; }
    }
}
