using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.ControlPannel;
using ECommerce.Entity.Store.Vendor;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    public class PurchaseOrderInfo : EntityBase
    {
        public PurchaseOrderInfo()
        {
            PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo()
            {
                PurchaseOrderStatus = PurchaseOrderStatus.Origin,
                StockInfo = new StockInfo(),
                ETATimeInfo = new PurchaseOrderETATimeInfo(),
                MemoInfo = new PurchaseOrderMemoInfo()
            };
            POItems = new List<PurchaseOrderItemInfo>();
        }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 采购单基本信息
        /// </summary>
        public PurchaseOrderBasicInfo PurchaseOrderBasicInfo { get; set; }

        /// <summary>
        /// 供应商信息
        /// </summary>
        public VendorBasicInfo VendorInfo { get; set; }
        /// <summary>
        /// 采购单商品列表
        /// </summary>
        public List<PurchaseOrderItemInfo> POItems { get; set; }
        /// <summary>
        /// 采购单收货信息
        /// </summary>
        public List<PurchaseOrderReceivedInfo> ReceivedInfoList { get; set; }

    }
}
