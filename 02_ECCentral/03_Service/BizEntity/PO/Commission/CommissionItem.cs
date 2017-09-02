using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 佣金商品信息
    /// </summary>
    public class CommissionItem
    {
        /// <summary>
        /// 佣金类型（销售提成，订单提成，配送费用)
        /// </summary>
        public VendorCommissionItemType CommissionType { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? ItemSysNo { get; set; }

        public int? VendorManufacturerSysNo { get; set; }

        /// <summary>
        /// 佣金规则系统编号
        /// </summary>
        public int? RuleSysNo { get; set; }

        public decimal? TotalSaleAmt { get; set; }

        public decimal? TotalDeliveryFee { get; set; }

        public decimal? TotalOrderCommissionFee { get; set; }

        public int? TotalQty { get; set; }

        public decimal? SalesCommissionFee { get; set; }

        public decimal? OrderCommissionFee { get; set; }

        public int? OrderQty { get; set; }

        public decimal? DeliveryFee { get; set; }

        public int? DeliveryQty { get; set; }

        public decimal? RentFee { get; set; }

        /// <summary>
        /// 生产商名称
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 2级类别名称
        /// </summary>
        public string C2Name { get; set; }

        /// <summary>
        /// 3级类别名称
        /// </summary>
        public string C3Name { get; set; }

        /// <summary>
        /// 供应商佣金销售提成信息
        /// </summary>
        public VendorStagedSaleRuleEntity SaleRule { get; set; }
        /// <summary>
        /// 商品明细列表
        /// </summary>
        public List<CommissionItemDetail> DetailList { get; set; }

        public List<CommissionItemDetail> DetailOrderList { get; set; }

        public List<CommissionItemDetail> DetailDeliveryList { get; set; }

    }
}
