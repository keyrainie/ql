using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 商品销售信息：库存/价格/积分/经验值等
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductSalesInfo
    {
        public string ProductID { get; set; }

        public int ProductSysNo { get; set; }

        public string CategoryName { get; set; }
        public int ProductGroupSysNo { get; set; }
        /// <summary>
        /// 当前销售价
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 返现
        /// </summary>
        public decimal CashRebate { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 商品详情页商品总价
        /// </summary>
        public decimal TotalPrice
        {

            get
            {
                if (this.FreeEntryTax)
                {
                    return this.CurrentPrice + this.CashRebate;
                }
                else
                {
                    return this.CurrentPrice + this.CashRebate + this.CurrentPrice * TariffRate;
                }
            }
        }

        /// <summary>
        /// 支付类型
        /// </summary>
        public ProductPayType PointType { get; set; }
        /// <summary>
        /// 赠送积分
        /// </summary>
        public int Point { get; set; }

        /// <summary>
        /// 赠送经验值
        /// </summary>
        public int ExperienceValue { get; set; }

        /// 可用库存
        /// </summary>
        public int AvailQty { get; set; }

        /// <summary>
        /// 虚拟库存
        /// </summary>
        public int VirtualQty { get; set; }

        /// <summary>
        /// 财务库存
        /// </summary>
        public int ConsignQty { get; set; }

        /// <summary>
        /// 可销售库存（AvailQty+VirtualQty+ConsignQty）
        /// </summary>
        public int OnlineQty { get; set; }
        ///// <summary>
        ///// 渠道库存占比模式：每次下单百分比；与渠道独占库存模式互斥
        ///// </summary>
        //public decimal? ChannelQ4SPercent { get; set; }

        ///// <summary>
        ///// 渠道独占库存模式：优先于渠道占比模式
        ///// </summary>
        //public int? ChannelReservedQty { get; set; }

        ///// <summary>
        ///// 渠道临时占用库存，备用，可能会用于一些活动占用等
        ///// </summary>
        //public int? ChannelTempReservedQty { get; set; }

        ///// <summary>
        ///// 当前渠道的可售库存
        ///// </summary>
        //public int ChannelQ4S
        //{
        //    get
        //    {
        //        if (ChannelReservedQty.HasValue)
        //        {
        //            return ChannelReservedQty.Value;
        //        }
        //        else if (ChannelQ4SPercent.HasValue)
        //        {
        //            int q4s = (int)((TotalAvailQty + TotalVirtualQty) * ChannelQ4SPercent.Value);
        //            return q4s;
        //        }
        //        else
        //        {
        //            return (TotalAvailQty + TotalVirtualQty);
        //        }
        //    }
        //}

        /// <summary>
        /// 每单最小订购数量
        /// </summary>
        [DataMember]
        public int MinCountPerOrder { get; set; }

        /// <summary>
        /// 每单最大订购数量
        /// </summary>
        [DataMember]
        public int MaxCountPerOrder { get; set; }

        /// <summary>
        /// 成本价
        /// </summary>
        public decimal? UnitCostPrice { get; set; }

        /// <summary>
        /// 进口税（CurrentPrice*TrafficTax）
        /// </summary>
        public decimal? EntryTax { get; set; }

        /// <summary>
        /// 是否免收进口税
        /// </summary>
        public bool FreeEntryTax
        {
            get
            {
                return EntryTax <= 50M;
            }
        }

        public string ProductName
        {
            get;
            set;
        }

        public string DefaultImage
        {
            get;
            set;
        }

        public decimal RealPrice
        {
            get
            {
                return (CurrentPrice * TariffRate) <= 50 ? CurrentPrice : CurrentPrice + (CurrentPrice * TariffRate);
            }
        }

        public decimal TariffRate
        {
            get;
            set;
        }

        public string PromotionTitle { get; set; }

        public string ProductShowName
        {

            get
            {
                string temp = string.Empty;
                if (string.IsNullOrEmpty(PromotionTitle))
                {
                    temp = ProductTitle;
                }
                else
                {
                    temp = ProductTitle + "-" + PromotionTitle;
                }
                if (temp.Length <= 50)
                {
                    return temp;
                }
                else
                {
                    return temp.Substring(0, 50) + "...";
                }
            }
        }

        #region     为商品评论页以及咨询也所加SEO

        public string Keywords { get; set; }

        public string ProductDesc { get; set; }

        public string Title { get; set; }

        #endregion

        public int CategoryID { get; set; }

        public int BrandID { get; set; }

        public string BrandName { get; set; }


        public string ProductTitle { get; set; }
    }


    /// <summary>
    /// 商品会员价格信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductCustomerRankPrice
    {
        
        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 会员编号
        /// </summary>
        public int CustomerSysNo { get; set; }

        /// <summary>
        /// 会员等级
        /// </summary>
        public CustomerRankType CustomerRank { get; set; }

        /// <summary>
        /// 会员价
        /// </summary>
        public decimal RankPrice { get; set; }

        /// <summary>
        /// 会员总价(包含税费,不包含返现)
        /// </summary>
        public decimal TotalPrice
        {
            get;
            set;
        }

        /// <summary>
        /// 关税
        /// </summary>
        public decimal EntryTax { get; set; }

        /// <summary>
        /// 是否免收进口税
        /// </summary>
        public bool FreeEntryTax
        {
            get
            {
                return EntryTax <= 50M;
            }
        }
    }
}
