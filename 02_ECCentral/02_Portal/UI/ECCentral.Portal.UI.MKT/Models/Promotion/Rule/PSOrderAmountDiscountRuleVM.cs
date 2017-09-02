using System;
using System.Collections.ObjectModel;

using ECCentral.BizEntity.MKT;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class PSOrderAmountDiscountRuleViewModel : ModelBase
    {
        private string m_OrderMaxDiscount;
        /// <summary>
        /// 每单折扣上限
        /// </summary>
        [Validate(ValidateType.Regex, @"^[1-9]\d*\.?[0-9]{0,2}$", ErrorMessage = "必须是大于等于1的数字")]
        public string OrderMaxDiscount
        {
            get { return this.m_OrderMaxDiscount; }
            set { this.SetValue("OrderMaxDiscount", ref m_OrderMaxDiscount, value); }
        }

        private ObservableCollection<OrderAmountDiscountRankViewModel> m_OrderAmountDiscountRank;
        public ObservableCollection<OrderAmountDiscountRankViewModel> OrderAmountDiscountRank
        {
            get { return this.m_OrderAmountDiscountRank; }
            set { this.SetValue("OrderAmountDiscountRank", ref m_OrderAmountDiscountRank, value); }
        }
    }

   public class OrderAmountDiscountRankViewModel : ModelBase
   {
       private Decimal? m_OrderMinAmount;
       public Decimal? OrderMinAmount
       {
           get { return this.m_OrderMinAmount; }
           set { this.SetValue("OrderMinAmount", ref m_OrderMinAmount, value); }
       }

       private PSDiscountTypeForOrderAmount? m_DiscountType;
       public PSDiscountTypeForOrderAmount? DiscountType
       {
           get { return this.m_DiscountType; }
           set { this.SetValue("DiscountType", ref m_DiscountType, value); }
       }

       private Decimal? m_DiscountValue;
       public Decimal? DiscountValue
       {
           get { return this.m_DiscountValue; }
           set { this.SetValue("DiscountValue", ref m_DiscountValue, value); }
       }

       public string DiscountValueStr
       {
           get
           {
               return DiscountType == PSDiscountTypeForOrderAmount.OrderAmountDiscount ? DiscountValue.ToString() : DiscountValue.Value.ToString("p");
           }
       }
   }
}
