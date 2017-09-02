using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Product
{
    /// <summary>
    /// 商品销售信息
    /// </summary>
    public class SalesInfoModel
    {
        /// <summary>
        /// 市场价
        /// </summary>
        public decimal BasicPrice { get; set; }

        /// <summary>
        /// 卖价
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 返现
        /// </summary>
        public decimal CashRebate { get; set; }

        /// <summary>
        /// 关税
        /// </summary>
        public decimal TariffPrice { get; set; }

        /// <summary>
        /// 总价(CurrentPrice+CashRebate+TariffPrice(关税小于50可免))
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 是否免关税
        /// </summary>
        public bool FreeEntryTax { get; set; }

        /// <summary>
        /// 是否免运费
        /// </summary>
        public bool FreeShipping { get; set; }

        /// <summary>
        /// 每单最小订购数量
        /// </summary>
        public int MinCountPerOrder { get; set; }

        /// <summary>
        /// 每单最大订购数量
        /// </summary>
        public int MaxCountPerOrder { get; set; }

        /// <summary>
        /// 预计到货时间(单位：天)
        /// </summary>
        public int ETA { get; set; }

        /// <summary>
        /// 赠送积分数量
        /// </summary>
        public int PresentPoint { get; set; }

        /// <summary>
        /// 可销售数量
        /// </summary>
        public int OnlineQty { get; set; }

        /// <summary>
        /// 库存状态(有货，即将售完，已售罄),根据库存信息等计算机出来
        /// </summary>
        public string InventoryStatus { get; set; }

        /// <summary>
        /// 是否已加入收藏夹
        /// </summary>
        public bool IsWished { get; set; }

        /// <summary>
        /// 是否有赠品
        /// </summary>
        public bool IsHaveValidGift { get; set; }

        /// <summary>
        /// 是否是限时促销商品
        /// </summary>
        public bool IsCountDown { get; set; }

        /// <summary>
        /// 是否是新上架商品
        /// </summary>
        public bool IsNewProduct { get; set; }

        /// <summary>
        /// 是否是团购商品
        /// </summary>
        public bool IsGroupBuyProduct { get; set; }
    }
}