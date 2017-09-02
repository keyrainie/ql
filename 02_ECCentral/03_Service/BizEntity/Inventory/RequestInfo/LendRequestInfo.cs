using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.PO;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 商品库存单据-借货单
    /// </summary>        
    [DataContract]
    public class LendRequestInfo : IIdentity, ICompany
    {
        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        #endregion IIdentity Members

        #region ICompany Members

        /// <summary>
        /// 所属公司
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }

        #endregion ICompany Members

        /// <summary>
        /// 产品线编号
        /// </summary>
        [DataMember]
        public string ProductLineSysno { get; set; }
        /// <summary>
        /// 单据编号
        /// </summary>
        [DataMember]
        public string RequestID { get; set; }

        /// <summary>
        /// 借货人
        /// </summary>
        [DataMember]
        public UserInfo LendUser { get; set; }

        /// <summary>
        /// 借出渠道仓库
        /// </summary>
        [DataMember]
        public StockInfo Stock { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public UserInfo CreateUser { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [DataMember]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [DataMember]
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        [DataMember]
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        [DataMember]
        public UserInfo AuditUser { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        [DataMember]
        public DateTime? AuditDate { get; set; }

        /// <summary>
        /// 出库人
        /// </summary>
        [DataMember]
        public UserInfo OutStockUser { get; set; }

        /// <summary>
        /// 出库日期
        /// </summary>
        [DataMember]
        public DateTime? OutStockDate { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>
        [DataMember]
        public LendRequestStatus RequestStatus { get; set; }

        /// <summary>
        /// 代销标识
        /// </summary>
        [DataMember]
        public RequestConsignFlag ConsignFlag { get; set; }

        /// <summary>
        /// 单据备注
        /// </summary>
        [DataMember]
        public string Note { get; set; }

        /// <summary>
        /// 借出商品列表
        /// </summary>
        [DataMember]
        public List<LendRequestItemInfo> LendItemInfoList { get; set; }

        /// <summary>
        /// 归还商品列表
        /// </summary>
        [DataMember]
        public List<LendRequestReturnItemInfo> ReturnItemInfoList { get; set; }

    }
}
