using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Member
{
    public class ProductPriceNotifyInfo
    {
        public int SysNo { get; set; }

        public int CustomerSysNo { get; set; }

        /// <summary>
        /// 即时价格
        /// </summary>
        public decimal InstantPrice { get; set; }

        /// <summary>
        /// 期望价
        /// </summary>
        public decimal ExpectedPrice { get; set; }

        /// <summary>
        /// 提醒状态 O,A,D,H,F
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductTitle { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        public string DefaultImage { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public int OnlineQty { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 返现金额
        /// </summary>
        public decimal CashRebate { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public decimal TariffRate { get; set; }

        /// <summary>
        /// 关税价格
        /// </summary>
        public decimal TaxPrice { get { return CurrentPrice * TariffRate; } }

        /// <summary>
        /// 真实价格
        /// </summary>
        public decimal RealPrice
        {
            get
            {
                if (TaxPrice <= 50)
                {
                    return CurrentPrice + CashRebate;
                }
                else
                {
                    return (CurrentPrice + TaxPrice + CashRebate);
                }
            }
        }
    }
}
