using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPPOversea.Invoicemgmt.AutoSettledCommission.Model
{
    /// <summary>
    /// SO & RMA 商品信息
    /// </summary>
    [Serializable]
    public class OrderProductEntity
    {
        #region Table Field 对应表字段
        /// <summary>
        /// 去除三费、优惠券、捆绑优惠的金额[单个商品]
        /// </summary>
        [DataMapping("Price", DbType.Decimal)]
        public decimal Price { get; set; }


        /// <summary>
        /// 数量
        /// </summary>
        [DataMapping("Quantity", DbType.Int32)]
        public int Quantity { get; set; }

        /// <summary>
        /// C1SysNo
        /// </summary>
        [DataMapping("C1SysNo", DbType.Int32)]
        public int C1SysNo { get; set; }

        /// <summary>
        /// 商品对应的佣金比例
        /// </summary>
        [DataMapping("Percentage", DbType.Decimal)]
        public decimal Percentage { get; set; }
        #endregion Table Field 对应表字段
    }
    
}
