using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 供应商退款信息
    /// </summary>
    public class VendorRefundInfo : IIdentity, ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 关联供应商系统编号
        /// </summary>
        public int? VendorSysNo { get; set; }


        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName { get; set; }


        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal? RefundCashAmt { get; set; }


        /// <summary>
        /// 付款方式
        /// </summary>
        public VendorRefundPayType? PayType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 创建人系统编号
        /// </summary>
        public int? CreateUserSysNo { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 作废人系统编号
        /// </summary>
        public int? AbandonUserSysNo { get; set; }

        /// <summary>
        /// 作废时间
        /// </summary>
        public DateTime? AbandonTime { get; set; }

        /// <summary>
        /// PM审核时间
        /// </summary>
        public DateTime? PMAuditTime { get; set; }

        /// <summary>
        /// PM系统编号
        /// </summary>
        public int? PMUserSysNo { get; set; }

        /// <summary>
        /// PM名称
        /// </summary>
        public string PMUserName { get; set; }

        /// <summary>
        /// PMD审核时间
        /// </summary>
        public DateTime? PMDAuditTime { get; set; }

        /// <summary>
        /// PMD系统编号
        /// </summary>
        public int? PMDUserSysNo { get; set; }

        /// <summary>
        /// PMD名称
        /// </summary>
        public string PMDUserName { get; set; }

        /// <summary>
        /// PMCC审核时间
        /// </summary>
        public DateTime? PMCCAuditTime { get; set; }

        /// <summary>
        /// PMCC系统编号
        /// </summary>
        public int? PMCCUserSysNo { get; set; }

        /// <summary>
        /// PMCC名称
        /// </summary>
        public string PMCCUserName { get; set; }

        /// <summary>
        /// PM备注
        /// </summary>
        public string PMMemo { get; set; }

        /// <summary>
        /// PMD备注
        /// </summary>
        public string PMDMemo { get; set; }

        /// <summary>
        /// PMCC备注
        /// </summary>
        public string PMCCMemo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 供应商退款单状态
        /// </summary>
        public VendorRefundStatus? Status { get; set; }

        /// <summary>
        /// 退款单商品列表
        /// </summary>
        public List<VendorRefundItemInfo> ItemList { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int ProductSysNo { get; set; }
        /// <summary>
        /// RMA退货仓库系统编号
        /// </summary>
        public int WarehouseSysNo { get; set; }
        /// <summary>
        /// RMA单件系统编号
        /// </summary>
        public int RegisterSysNo { get; set; }

        /// <summary>
        /// 审核人所在组名称（用于权限控制)
        /// </summary>
        public string UserRole { get; set; }

        /// <summary>
        /// 是否不为PM和PMD审核
        /// </summary>
        public bool? NotPMAndPMD { get; set; }

    }
}
