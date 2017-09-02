using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Store.Vendor
{
    /// <summary>
    /// 供应商扩展信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class VendorExtendInfo
    {/// <summary>
        /// 公告
        /// </summary>
        [DataMember]
        public string Bulletin { get; set; }

        /// <summary>
        /// 商家简介
        /// </summary>
        [DataMember]
        public string BriefInfo { get; set; }
        /// <summary>
        /// 是否前台展示
        /// </summary>
        [DataMember]
        public bool IsShowStore { get; set; }

        /// <summary>
        /// 开票方式
        /// </summary>
        [DataMember]
        public VendorInvoiceType? InvoiceType { get; set; }

        /// <summary>
        /// 仓储方式
        /// </summary>
        [DataMember]
        public VendorStockType? StockType { get; set; }

        /// <summary>
        /// 配送方式
        /// </summary>
        [DataMember]
        public VendorShippingType? ShippingType { get; set; }

        [DataMember]
        public decimal? MerchantRate { get; set; }

        /// <summary>
        /// 默认送货分仓号
        /// </summary>
        [DataMember]
        public int? DefaultStock { get; set; }

        /// <summary>
        /// 货币编号
        /// </summary>
        [DataMember]
        public string CurrencyCode { get; set; }
    }
}
