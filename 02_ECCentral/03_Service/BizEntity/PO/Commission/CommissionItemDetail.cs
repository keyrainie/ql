using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 佣金分类商品信息明细
    /// </summary>
    public class CommissionItemDetail
    {

        public int? DetailSysNo { get; set; }
        /// <summary>
        /// 佣金单项编号
        /// </summary>
        public int? CommissionItemSysNo { get; set; }

        /// <summary>
        /// 单据编号
        /// </summary>
        public string ReferenceSysNo { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        public VendorCommissionReferenceType? ReferenceType { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品销售单价
        /// </summary>
        public decimal? SalePrice { get; set; }

        /// <summary>
        ///  商品数量
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        public int VendorManufacturerSysNo { get; set; }

        public int? Point { get; set; }

        public decimal? DiscountAmout { get; set; }

        public bool HaveAutoRMA { get; set; }

        public decimal PromotionDiscount { get; set; }

        public int? SOSysNo { get; set; }
    }
}
