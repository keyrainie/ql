using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 会员价
    /// </summary>
    [Serializable]
    public class ProductRankPriceInfo
    {
        private int id;
        private int customerRank;
        private decimal rankPrice;

        public int ProductID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public int CustomerRank
        {
            get { return this.customerRank; }
            set { this.customerRank = value; }
        }

        public decimal RankPrice
        {
            get { return this.rankPrice; }
            set { this.rankPrice = value; }
        }
    }
}
