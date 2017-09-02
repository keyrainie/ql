using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Customer;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品等级价格信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductRankPriceInfo
    {
        /// <summary>
        /// 顾客级别
        /// </summary>
        [DataMember]    
        public CustomerRank Rank { get; set; }

        /// <summary>
        /// 等级价格
        /// </summary>
        [DataMember]
        public decimal? RankPrice { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public ProductRankPriceStatus? Status { get; set; }


        public override bool Equals(object obj)
        {
            if (obj != null && obj is ProductRankPriceInfo)
            {
                var o = obj as ProductRankPriceInfo;
                return Rank == o.Rank && RankPrice == o.RankPrice && Status == o.Status;
            }
            return false;
        }
    }
}
