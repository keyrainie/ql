using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity.Shipping;
using Nesoft.ECWeb.Entity.Payment;
using Nesoft.ECWeb.SOPipeline;

namespace Nesoft.ECWeb.MobileService.Models.Order
{
    public class CheckOutResultModel
    {
        public bool HasSucceed { get; set; }

        public List<string> ErrorMessages { get; set; }

        public OrderInfoModel ReturnData { get; set; }

        /// <summary>
        ///  客户收货地址簿
        /// </summary>
        public List<ShippingContactInfo> ShippingAddressList { get; set; }

        /// <summary>
        /// 当前选中的配送方式ID
        /// </summary>
        public ShippingContactInfo SelShippingAddress { get; set; }

        /// <summary>
        /// 当前选中的支付方式ID
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

        //支付方式列表
        public List<PayTypeInfo> PayTypeList { get; set; }
        /// <summary>
        /// 配送方式列表
        /// </summary>
        public List<ShipTypeInfo> ShipTypeList { get; set; }

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
    }
}