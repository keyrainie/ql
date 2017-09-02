using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Member
{
    public class ProductNotifyInfo : EntityBase
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public NotifyStatus Status { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int CustomerSysNo { get; set; }

        /// <summary>
        /// 邮件
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// 通知时间
        /// </summary>
        public DateTime NotifyTime { get; set; }

        /// <summary>
        /// 产品编号
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 产品编号
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductTitle { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string ProductMode { get; set; }

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
