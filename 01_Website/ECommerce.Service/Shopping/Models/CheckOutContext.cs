using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECommerce.SOPipeline;

namespace ECommerce.Facade.Shopping
{
    [DataContract]
    public class CheckOutContext : ICloneable
    {
        /// <summary>
        /// 是否使用余额支付
        /// </summary>
        [DataMember]
        public int IsUsedPrePay { get; set; }

        /// <summary>
        /// 使用的积分
        /// </summary>
        [DataMember]
        public int PointPay { get; set; }

        /// <summary>
        /// 使用的网银积分
        /// </summary>
        public int BankPointPay { get; set; }

        /// <summary>
        /// 支付类型id
        /// </summary>
        [DataMember]
        public string PaymentCategoryID { get; set; }
        /// <summary>
        /// 支付方式id
        /// </summary>
        [DataMember]
        public int? PayTypeID { get; set; }

        /// <summary>
        /// 配送方式id
        /// </summary>
        [DataMember]
        public string ShipTypeID { get; set; }

        /// <summary>
        /// 配送地址id
        /// </summary>
        [DataMember]
        public int ShippingAddressID { get; set; }

        /// <summary>
        /// 优惠券
        /// </summary>
        [DataMember]
        public string PromotionCode { get; set; }

        /// <summary>
        /// 验证key
        /// </summary>
        [DataMember]
        public string ValidateKey { get; set; }

        /// <summary>
        /// 购买商品参数，如果购物车cookie为空，则购物车对象从该值构建
        /// </summary>
        [DataMember]
        public string ShoppingItemParam { get; set; }

        /// <summary>
        /// 虚拟团购订单使用的电话号码
        /// </summary>
        [DataMember]
        public string VirualGroupBuyOrderTel { get; set; }

        /// <summary>
        /// 使用的礼品卡列表(Json字符串)
        /// </summary>
        [DataMember]
        public string GiftCardParam { get; set; }

        public List<GiftCardContext> GiftCardList
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.GiftCardParam))
                {
                    return ECommerce.Utility.SerializationUtility.JsonDeserialize2<List<GiftCardContext>>(this.GiftCardParam);
                }
                return null;
            }
        }
        /// <summary>
        /// 订单备注
        /// </summary>
        public string OrderMemo { get; set; }

        /// <summary>
        /// 是否需要开发票
        /// </summary>
        public int NeedInvoice { get; set; }

        public CheckOutContext Clone()
        {
            ICloneable obj = this;
            return obj.Clone() as CheckOutContext;
        }

        /// <summary>
        /// 选中需要购买的单个商品编号，用逗号隔开
        /// </summary>
        public string PackageTypeSingleList { get; set; }

        /// <summary>
        /// 选中需要购买的捆绑商品的活动编号，用逗号隔开
        /// </summary>
        public string PackageTypeGroupList { get; set; }
        object ICloneable.Clone()
        {
            return new CheckOutContext()
            {
                IsUsedPrePay = this.IsUsedPrePay,
                PaymentCategoryID = this.PaymentCategoryID,
                ShippingAddressID = this.ShippingAddressID,
                PromotionCode = this.PromotionCode,
                ValidateKey = this.ValidateKey,
                ShoppingItemParam = this.ShoppingItemParam,
                PackageTypeSingleList = this.PackageTypeSingleList,
                PackageTypeGroupList = this.PackageTypeGroupList,
                PointPay = this.PointPay,
                BankPointPay=this.BankPointPay,
                VirualGroupBuyOrderTel = this.VirualGroupBuyOrderTel,
                GiftCardParam = this.GiftCardParam,
                ShipTypeID = this.ShipTypeID,
                NeedInvoice = this.NeedInvoice,
                OrderMemo = this.OrderMemo,
                PayTypeID=PayTypeID
            };
        }

        
    }

    [DataContract]
    public class GiftCardContext : ICloneable
    {
        /// <summary>
        /// 卡号
        /// </summary>
        [DataMember]
        public string Code { get; set; }
        /// <summary>
        /// 非用户绑定的礼品卡需要填写密码
        /// </summary>
        [DataMember]
        public string Password { get; set; }
        /// <summary>
        /// 是否已加密
        /// </summary>
        [DataMember]
        public string Crypto { get; set; }

        /// <summary>
        /// 是否已随机化混淆
        /// </summary>
        [DataMember]
        public string Random { get; set; }

        public GiftCardContext Clone()
        {
            ICloneable obj = this;
            return obj.Clone() as GiftCardContext;
        }

        object ICloneable.Clone()
        {
            return new GiftCardContext()
            {
                Code = this.Code,
                Password = this.Password
            };
        }
    }
}
