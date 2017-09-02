using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Inventory
{
    public class AdjustRequestInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }
        /// <summary>
        /// 产品线编号
        /// </summary>
        public string ProductLineSysno { get; set; }
        /// <summary>
        /// 单据编号
        /// </summary>
        public string RequestID { get; set; }


        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateDate { get; set; }


        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime? AuditDate { get; set; }

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
        public AdjustRequestStatus RequestStatus { get; set; }

        /// <summary>
        /// 代销标识
        /// </summary>
        public RequestConsignFlag ConsignFlag { get; set; }

        /// <summary>
        /// 单据备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 损益单类型
        /// </summary> 
        public AdjustRequestProperty AdjustProperty { get; set; }

        /// <summary>
        /// 损益商品明细表
        /// </summary> 
        public List<AdjustRequestItemInfo> AdjustItemInfoList { get; set; }

        /// <summary>
        /// 损益单发票
        /// </summary> 
        public AdjustRequestInvoiceInfo InvoiceInfo { get; set; }
    }

    /// <summary>
    /// 损益单所包含的商品损益信息
    /// </summary>
    public class AdjustRequestItemInfo
    {
        public int? SysNo
        {
            get;
            set;
        }
        public int? RequestSysNo { get; set; }
        public int? ProductSysNo { get; set; }
        public int? AdjustQuantity { get; set; }
        public decimal? AdjustCost { get; set; }
        ///// <summary>
        ///// 损益商品
        ///// </summary>
        //public ProductInfo AdjustProduct { get; set; }

        ///// <summary>
        ///// 损益数量
        ///// </summary>
        //public int AdjustQuantity { get; set; }

        ///// <summary>
        ///// 损益成本
        ///// </summary>
        //public decimal AdjustCost { get; set; }

        ///// <summary>
        ///// 商品批次信息列表
        ///// </summary>        
        //public List<InventoryBatchDetailsInfo> BatchDetailsInfoList { get; set; }
    }


    /// <summary>
    /// 损益单发票
    /// </summary>
    public class AdjustRequestInvoiceInfo
    {
        /// <summary>
        /// 发票系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string ReceiveName { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        public string ContactAddress { get; set; }

        /// <summary>
        /// 收件地址
        /// </summary>
        public string ContactShippingAddress { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 发票编号
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

    }
}
