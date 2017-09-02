using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// 退款金额检测信息
    /// </summary>
    public class RefundAmountForCheck
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 主商品编号
        /// </summary>
        public string MasterProductSysNo { get; set; }
        /// <summary>
        /// 商品类型
        /// </summary>
        public ProductType? ProductType { get; set; }
        /// <summary>
        /// 销售金额
        /// </summary>
        public decimal SaleAmount { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal RefundAmount { get; set; }

        #region　要对应查询出来的字段

        public decimal RO { get; set; }

        public ProductType? SOItemType { get; set; }

        public decimal SO { get; set; }
        #endregion
    }
}
