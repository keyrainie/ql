using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 虚库采购单信息
    /// </summary>
    public class VirtualStockPurchaseOrderInfo : IIdentity, ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        /// <summary>
        /// 销售单号
        /// </summary>
        public int? SOSysNo { get; set; }

        /// <summary>
        ///  销售单Item编号
        /// </summary>
        public int? SOItemSysNo { get; set; }


        /// <summary>
        /// 产品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 产品编号
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 虚库采购单状态
        /// </summary>
        public VirtualPurchaseOrderStatus? Status { get; set; }

        /// <summary>
        /// 申请采购数量
        /// </summary>
        public int? PurchaseQty { get; set; }

        /// <summary>
        /// 估计到达时间
        /// </summary>
        public DateTime? EstimateArriveTime { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 创建人系统编号
        /// </summary>
        public int? CreateUserSysNo { get; set; }

        /// <summary>
        /// 实际入库时间
        /// </summary>
        public string InStockTime { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// CS备注
        /// </summary>
        public string CSMemo { get; set; }

        /// <summary>
        /// PM系统编号
        /// </summary>
        public int? PMUserSysNo { get; set; }

        /// <summary>
        /// PM备注
        /// </summary>
        public string PMMemo { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        public VirtualPurchaseInStockOrderType? InStockOrderType { get; set; }

        /// <summary>
        /// 单据编号
        /// </summary>
        public int? InStockOrderSysNo { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>
        public InStockStatus? InStockStatus { get; set; }

        /// <summary>
        /// 销售单虚库单数量
        /// </summary>
        public int? SOVirtualCount { get; set; }

    }
}
