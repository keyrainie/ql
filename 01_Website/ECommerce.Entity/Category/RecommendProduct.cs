using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Category
{
    [Serializable]
    [DataContract]
    public class RecommendProduct : EntityBase
    {
        [DataMember]
        public int SysNo { get; set; }
        [DataMember]
        public int BrandSysNo { get; set; }
        [DataMember]
        public string ProductID { get; set; }
        [DataMember]
        public int Priority { get; set; }
        [DataMember]
        public string BriefName { get; set; }
        [DataMember]
        public string DefaultImage { get; set; }

        [DataMember]
        public string ProductName { get; set; }
        [DataMember]
        public string ProductTitle { get; set; }
        [DataMember]
        public decimal CurrentPrice { get; set; }
        [DataMember]
        public decimal BasicPrice { get; set; }
        [DataMember]
        public decimal Discount { get; set; }
        [DataMember]
        public decimal TariffRate { get; set; }
        [DataMember]
        public decimal CashRebate { get; set; }
        [DataMember]
        public string PromotionTitle { get; set; }
        /// <summary>
        /// 标签名称
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 关税
        /// </summary>
        public decimal TariffPrice
        {
            get { return CurrentPrice * TariffRate; }
        }

        /// <summary>
        /// 实际的销售价格
        /// </summary>
        public decimal RealPrice
        {
            get
            {
                var tax = CurrentPrice * TariffRate;
                if (tax <= 50m)
                {
                    return CurrentPrice
                        + CashRebate;
                }
                else
                {
                    return CurrentPrice
                           + tax //关税
                           + CashRebate;//返现

                }

            }
        }
    }
}
