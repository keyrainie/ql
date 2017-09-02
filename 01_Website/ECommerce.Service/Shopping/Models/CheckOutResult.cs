using System.Collections.Generic;
using ECommerce.Entity.Payment;
using ECommerce.Entity.Promotion;
using ECommerce.Entity.Shipping;
using ECommerce.Entity.Shopping;
using ECommerce.SOPipeline;

namespace ECommerce.Facade.Shopping
{
    public class CheckOutResult
    {
        public bool HasSucceed { get; set; }

        private List<string>  m_ErrorMessages;

        public List<string> ErrorMessages
        {
            get
            {
                if (m_ErrorMessages == null)
                {
                    m_ErrorMessages = new List<string>();
                }
                return m_ErrorMessages;
            }
            set {
                m_ErrorMessages = value;
            }
        }

        public bool NeedLogin { get; set; }

        public OrderPipelineProcessResult OrderProcessResult { get; set; }

        /// <summary>
        ///  客户收货地址簿
        /// </summary>
        public List<ShippingContactInfo> ShippingAddressList { get; set; }

        /// <summary>
        /// 当前选中的配送方式ID
        /// </summary>
        public ShippingContactInfo SelShippingAddress { get; set; }

        /// <summary>
        /// 当前选中的支付方式
        /// </summary>
        public PayTypeInfo SelPayType { get; set; }

        /// <summary>
        /// 当前选中的配送方式
        /// </summary>
        public ShipTypeInfo SelShipType { get; set; }
        /// <summary>
        /// 当前购物车用户
        /// </summary>
        public CustomerInfo Customer { get; set; }
        /// <summary>
        /// 当前购物车用户实名认证信息
        /// </summary>
        public ECommerce.Entity.Member.CustomerAuthenticationInfo CustomerAuthenticationInfo { get; set; }
        /// <summary>
        ///  当前购物车用户发票信息
        /// </summary>
        public ECommerce.Entity.Member.CustomerInvoiceInfo CustomerInvoiceInfo { get; set; }

        /// <summary>
        /// 支付类别（在线支付、货到付款）
        /// </summary>
        public List<KeyValuePair<int, string>> PaymentCategoryList { get; set; }

        /// <summary>
        /// 当前选中的支付类别
        /// </summary>
        public int SelPaymentCategoryID { get; set; }

        //支付方式列表
        public List<PayTypeInfo> PayTypeList { get; set; }

        /// <summary>
        /// 配送方式列表
        /// </summary>
        public List<ShipTypeInfo> ShipTypeList { get; set; }

        /// <summary>
        /// 使用的礼品卡列表
        /// </summary>
        public List<GiftCardInfo> ApplyedGiftCardList { get; set; }

        /// <summary>
        /// 绑定的礼品卡列表
        /// </summary>
        public List<GiftCardInfo> BindingGiftCardList { get; set; }

        /// <summary>
        /// 礼品卡使用失败后的描述
        /// </summary>
        public string ApplyedGiftCardDesc { get; set; }

        /// <summary>
        /// 购买商品参数，如果购物车cookie为空，则购物车对象从该值构建
        /// </summary>
        public string ShoppingItemParam { get; set; }

        /// <summary>
        /// 应用的优惠券编码
        /// </summary>
        public string ApplyCouponCode { get; set; }

        /// <summary>
        /// 应用的优惠券名称 
        /// </summary>
        public string ApplyCouponName { get; set; }

        /// <summary>
        /// 优惠券应用失败后的描述
        /// </summary>
        public string ApplyedCouponDesc { get; set; }

        /// <summary>
        /// 使用的积分
        /// </summary>
        public int UsePointPay { get; set; }

        /// <summary>
        /// 使用的网银积分
        /// </summary>
        public int UseBankPointPay { get; set; }

        /// <summary>
        /// 本单最大使用积分
        /// </summary>
        public int MaxPointPay { get; set; }

        /// <summary>
        /// 积分支付失败后的描述
        /// </summary>
        public string UsePointPayDesc { get; set; }

        /// <summary>
        /// 是否是团购订单
        /// </summary>
        public bool IsGroupBuyOrder { get; set; }

        /// <summary>
        /// 选中需要购买的单个商品编号，用逗号隔开
        /// </summary>
        public string PackageTypeSingleList { get; set; }

        /// <summary>
        /// 选中需要购买的捆绑商品的活动编号，用逗号隔开
        /// </summary>
        public string PackageTypeGroupList { get; set; }
        
    }
}