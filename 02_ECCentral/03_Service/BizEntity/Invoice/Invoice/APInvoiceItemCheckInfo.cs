using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.BizEntity.Invoice.Invoice
{
    /// <summary>
    /// 用于创建POitems，InvoiceItems，作为加载未录入的POItems的条件以及保存APInvoiceMaster的DTO
    /// 创建，加载Items需要保留原有页面中的Items
    /// </summary>
    public class APInvoiceItemInputEntity
    {
        /// <summary>
        /// 供应商编号
        /// </summary>
        public int? VendorSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName
        {
            get;
            set;
        }

        //public decimal? TaxRate { get; set; }
        /// <summary>
        /// item编号列表（三种方式：单一、以'.'分隔，以'-'分隔）
        /// </summary>
        public string ItemsNoList
        {
            get;
            set;
        }
        /// <summary>
        /// 发票日期
        /// </summary>
        public DateTime? InvoiceDate
        {
            get;
            set;
        }
        /// <summary>
        /// 发票金额
        /// </summary>
        public decimal? InvoiceAmt
        {
            get;
            set;
        }
        /// <summary>
        /// PO单据类型
        /// </summary>
        public PayableOrderType? OrderType
        {
            get;
            set;
        }
        /// <summary>
        /// PO单起始日期
        /// </summary>
        public DateTime? PODateFrom
        {
            get;
            set;
        }
        /// <summary>
        /// PO列表
        /// </summary>
        public List<APInvoicePOItemInfo> POItemList
        {
            get;
            set;
        }
        /// <summary>
        /// Invoice列表
        /// </summary>
        public List<APInvoiceInvoiceItemInfo> InvoiceItemList
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }
    }

    /// <summary>
    /// POItem验证
    /// </summary>
    public class POItemCheckEntity
    {
        /// <summary>
        /// PO编号
        /// </summary>
        public int PONo
        {
            get;
            set;
        }
        /// <summary>
        /// 供应商编号
        /// </summary>
        public int VendorSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 仓库状态，RMA单则为RMAVendorRefundStatus
        /// </summary>
        public int? StockStatus
        {
            get;
            set;
        }
        /// <summary>
        /// 订单类型
        /// </summary>
        public PayableOrderType? OrderType
        {
            get;
            set;
        }
        /// <summary>
        /// 发票状态
        /// </summary>
        public PayableInvoiceStatus? InvoiceStatus
        {
            get;
            set;
        }
        /// <summary>
        /// APInvoice_PO_Item.Status
        /// </summary>
        public APInvoiceItemStatus? POItemStatus
        {
            get;
            set;
        }
        /// <summary>
        /// APInvoice_Master.Status
        /// </summary>
        public APInvoiceMasterStatus? MasterStatus
        {
            get;
            set;
        }
        /// <summary>
        /// 批次号
        /// </summary>
        public int? BatchNumber
        {
            get;
            set;
        }
    }

    public class POItemKeyEntity
    {
        public int PONo
        {
            get;
            set;
        }

        public PayableOrderType? OrderType
        {
            get;
            set;
        }

        public int? BatchNumber
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 发票号验证
    /// </summary>
    public class InvoiceItemCheckEntity
    {
        /// <summary>
        /// 要录入的发票号码
        /// </summary>
        public List<string> originalList
        {
            get;
            set;
        }
        /// <summary>
        /// 无效的发票号码
        /// </summary>
        public List<string> invalidList
        {
            get;
            set;
        }
    }

    public class APInvoiceInputPOItemComparer : IEqualityComparer<APInvoicePOItemInfo>
    {
        public bool Equals(APInvoicePOItemInfo x, APInvoicePOItemInfo y)
        {
            return x.PoNo == y.PoNo
                && x.OrderType == y.OrderType.Value
                && x.BatchNumber == y.BatchNumber;
        }

        public int GetHashCode(APInvoicePOItemInfo obj)
        {
            return base.GetHashCode() ^ 2;
        }
    }

    public class POItemKeyComparer : IEqualityComparer<POItemKeyEntity>
    {
        public bool Equals(POItemKeyEntity x, POItemKeyEntity y)
        {
            return x.PONo == y.PONo
                && x.OrderType == y.OrderType
                && x.BatchNumber == y.BatchNumber;
        }

        public int GetHashCode(POItemKeyEntity obj)
        {
            return base.GetHashCode() ^ 2;
        }
    }

    //同样的单据类型，仅判断No即可
    public class POItemCheckComparer : IEqualityComparer<POItemCheckEntity>
    {
        public bool Equals(POItemCheckEntity x, POItemCheckEntity y)
        {
            return x.PONo == y.PONo
                && x.BatchNumber == y.BatchNumber;
        }

        public int GetHashCode(POItemCheckEntity obj)
        {
            return base.GetHashCode() ^ 2;
        }
    }
}
