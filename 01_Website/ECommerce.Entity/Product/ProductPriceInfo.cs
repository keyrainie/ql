using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    public class ProductPriceInfo
    {
        #region [ Fields ]

        private decimal basicPrice;
        private decimal currentPrice;
        private decimal cashRebate;
        private decimal maxPriceWeek;
        private ProductPayType pointType;
        private int maxPerOrder;
        private int isWholeSale;
        private int q1;
        private decimal p1;
        private int q2;
        private decimal p2;
        private int q3;
        private decimal p3;
        private int point;
        private int clearanceSale;
        private int isExistRankPrice;
        private int? priceCompareSysNo;
        private int productSysNo;
        private decimal secondhandDiscount;
        private int minCountPerOrder;
        private decimal commissionPrice;

        private List<ProductRankPriceInfo> rankPriceInfoList;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Basic price
        /// </summary>
        public decimal BasicPrice
        {
            get { return this.basicPrice; }
            set { this.basicPrice = value; }
        }

        /// <summary>
        /// Current price
        /// </summary>
        public decimal CurrentPrice
        {
            get { return this.currentPrice; }
            set { this.currentPrice = value; }
        }

        /// <summary>
        /// Cash rebate
        /// </summary>
        public decimal CashRebate
        {
            get { return this.cashRebate; }
            set { this.cashRebate = value; }
        }

        /// <summary>
        /// MaxPriceWeek
        /// </summary>
        public decimal MaxPriceWeek
        {
            get { return this.maxPriceWeek; }
            set { this.maxPriceWeek = value; }
        }


        public ProductPayType PointType
        {
            get { return this.pointType; }
            set { this.pointType = value; }
        }


        public int MaxPerOrder
        {
            get { return this.maxPerOrder; }
            set { this.maxPerOrder = value; }
        }


        public int IsWholeSale
        {
            get { return this.isWholeSale; }
            set { this.isWholeSale = value; }
        }


        public int Q1
        {
            get { return this.q1; }
            set { this.q1 = value; }
        }


        public decimal P1
        {
            get { return this.p1; }
            set { this.p1 = value; }
        }


        public int Q2
        {
            get { return this.q2; }
            set { this.q2 = value; }
        }


        public decimal P2
        {
            get { return this.p2; }
            set { this.p2 = value; }
        }


        public int Q3
        {
            get { return this.q3; }
            set { this.q3 = value; }
        }

    
        public decimal P3
        {
            get { return this.p3; }
            set { this.p3 = value; }
        }


        public int Point
        {
            get { return this.point; }
            set { this.point = value; }
        }


        public int ClearanceSale
        {
            get { return this.clearanceSale; }
            set { this.clearanceSale = value; }
        }


        public int IsExistRankPrice
        {
            get { return this.isExistRankPrice; }
            set { this.isExistRankPrice = value; }
        }


        public int? PriceCompareSysNo
        {
            get { return this.priceCompareSysNo; }
            set { this.priceCompareSysNo = value; }
        }


        public int ProductSysNo
        {
            get { return this.productSysNo; }
            set { this.productSysNo = value; }
        }

 
        public List<ProductRankPriceInfo> RankPriceInfoList
        {
            get { return this.rankPriceInfoList; }
            set { this.rankPriceInfoList = value; }
        }

        /// <summary>
        /// 折扣(currentPrice+cashRebate-basicPrice)
        /// </summary>
        public decimal SecondhandDiscount
        {
            get { return this.secondhandDiscount; }
            set { this.secondhandDiscount = value; }
        }

        /// <summary>
        /// 最小订购量
        /// </summary>
        public int MinCountPerOrder
        {
            get { return this.minCountPerOrder; }
            set { this.minCountPerOrder = value; }
        }

        /// <summary>
        /// 佣金价格
        /// </summary>
        public decimal CommissionPrice
        {
            get { return commissionPrice; }
            set { commissionPrice = value; }
        }
        #endregion
    }
}
