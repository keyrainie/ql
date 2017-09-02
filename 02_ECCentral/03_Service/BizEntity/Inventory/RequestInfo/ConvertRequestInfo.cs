using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.PO;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 商品库存单据-转换单
    /// </summary>
    public class ConvertRequestInfo : IIdentity, ICompany
    {
        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        #endregion IIdentity Members

        #region ICompany Members

        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        #endregion ICompany Members
        /// <summary>
        /// 产品线编号
        /// </summary>
        public string ProductLineSysno { get; set; }
        /// <summary>
        /// 单据编号
        /// </summary>
        public string RequestID { get; set; }

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

        /// <summary>
        /// 审核人
        /// </summary>
        public UserInfo AuditUser { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime? AuditDate { get; set; }

        /// <summary>
        /// 出库人
        /// </summary>
        public UserInfo OutStockUser { get; set; }

        /// <summary>
        /// 出库日期
        /// </summary>
        public DateTime? OutStockDate { get; set; }

        /// <summary>
        /// 源渠道仓库
        /// </summary>
        public StockInfo Stock { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>
        public ConvertRequestStatus RequestStatus { get; set; }

        /// <summary>
        /// 代销标识
        /// </summary>
        public RequestConsignFlag ConsignFlag { get; set; }

        /// <summary>
        /// 单据备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 转换商品列表
        /// </summary>
        public List<ConvertRequestItemInfo> ConvertItemInfoList { get; set; }

        public ConvertRequestInfo()
        {
            Stock = new StockInfo(); 
        }
    }
}
