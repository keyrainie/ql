using System;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;


namespace ECCentral.Portal.UI.IM.Models
{
    public class RepeatPromotionQueryVM : ModelBase
    {
        /// <summary>
        /// 页面
        /// </summary>
        public PagingInfo PageInfo { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        private string _productSysNo;
        [Validate(ValidateType.Required)]
        public string ProductSysNo
        {
            get { return _productSysNo; }
            set { SetValue("ProductSysNo", ref _productSysNo, value); }
        }

        /// <summary>
        /// 商品ID
        /// </summary>
        private string _productId;
        [Validate(ValidateType.Required)]
        public string ProductId
        {
            get { return _productId; }
            set { SetValue("ProductId", ref _productId, value); }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 销售规则
        /// </summary>
        private string _saleRuleCount = "销售规则";
        public string SaleRuleCount
        {
            get
            {
                return _saleRuleCount;
            }
            set
            {
                value = "销售规则" + value;
                SetValue("SaleRuleCount", ref _saleRuleCount, value);
            }
        }

        /// <summary>
        /// 赠品
        /// </summary>
        private string _giftCount = "赠品";
        public string GiftCount
        {
            get
            {
                return _giftCount;
            }
            set
            {
                value = "赠品" + value;
                SetValue("GiftCount", ref _giftCount, value);
            }
        }

        /// <summary>
        /// 优惠券
        /// </summary>
        private string _couponCount = "优惠券";
        public string CouponCount
        {
            get
            {
                return _couponCount;
            }
            set
            {
                value = "优惠券" + value;
                SetValue("CouponCount", ref _couponCount, value);
            }
        }

        /// <summary>
        /// 限时抢购
        /// </summary>
        private string _saleCountDownCount = "限时抢购";
        public string SaleCountDownCount
        {
            get
            {
                return _saleCountDownCount;
            }
            set
            {
                value = "限时抢购" + value;
                SetValue("SaleCountDownCount", ref _saleCountDownCount, value);
            }
        }

        /// <summary>
        /// 促销计划
        /// </summary>
        private string _saleCountDownPlanCount = "促销计划";
        public string SaleCountDownPlanCount
        {
            get
            {
                return _saleCountDownPlanCount;
            }
            set
            {
                value = "促销计划" + value;
                SetValue("SaleCountDownPlanCount", ref _saleCountDownPlanCount, value);
            }
        }

        /// <summary>
        /// 团购
        /// </summary>
        private string _productGroupBuyingCount = "团购";
        public string ProductGroupBuyingCount
        {
            get
            {
                return _productGroupBuyingCount;
            }
            set
            {
                value = "团购" + value;
                SetValue("ProductGroupBuyingCount", ref _productGroupBuyingCount, value);
            }
        }
    }
}
