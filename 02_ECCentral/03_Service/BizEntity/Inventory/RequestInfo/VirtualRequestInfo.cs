using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.PO;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 商品库存单据-虚库申请单
    /// </summary>
    public class VirtualRequestInfo : IIdentity, ICompany
    {
        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        #endregion IIdentity Members

        #region ICompany Members

        /// <summary>
        /// 所属公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        #endregion ICompany Members


        /// <summary>
        /// 虚库商品
        /// </summary>
        public ProductInfo VirtualProduct { get; set; }

        /// <summary>
        /// 渠道仓库
        /// </summary>
        public StockInfo Stock { get; set; }

        /// <summary>
        /// 设置虚库数量
        /// </summary>
        public int VirtualQuantity { get; set; }

        /// <summary>
        /// 生效的虚库数量
        /// </summary>
        public int? ActiveVirtualQuantity { get; set; }

        /// <summary>
        /// 虚库类型
        /// </summary>
        public int? VirtualType { get; set; }

        /// <summary>
        /// 虚库单状态
        /// </summary>
        public VirtualRequestStatus RequestStatus { get; set; }

        /// <summary>
        /// 申请理由
        /// </summary>
        public string RequestNote { get; set; }
        
        /// <summary>
        /// 审批意见
        /// </summary>
        public string AuditNote { get; set; }

        /// <summary>
        /// 生效开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 过期日期
        /// </summary>
        public DateTime? EndDate { get; set; }        
                
        /// <summary>
        /// 创建人
        /// </summary>
        public UserInfo CreateUser { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public UserInfo AuditUser { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime? AuditDate { get; set; }

    }
}
