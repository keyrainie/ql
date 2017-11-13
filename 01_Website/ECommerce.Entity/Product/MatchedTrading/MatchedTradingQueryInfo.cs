using System;
using ECommerce.Utility;


namespace ECommerce.Entity.Product
{
    public class MatchedTradingQueryInfo
    {
        public MatchedTradingQueryInfo()
        {
            CustomerReply = true;
            KeyWord = "";
            MatchedTradingType = "";
        }

        public int MatchedTradingSysNo { get; set; }




        public string MatchedTradingType { get; set; }

        /// <summary>
        /// 是否包含网友回复
        /// </summary>
        public bool CustomerReply { get; set; }

        public int CustomerSysNo { get; set; }

        public string KeyWord { get; set; }

        // <summary>
        /// Gets or sets paging info
        /// </summary>
        public PageInfo PagingInfo { get; set; }

        /// <summary>
        /// 商品组编号
        /// </summary>
        public int ProductGroupSysNo { get; set; }

        public int ProductSysNo { get; set; }

        public MatchedTradingQueryType QueryType { get; set; }

        public SortingInfo SortingInfo { get; set; }
    }
}
