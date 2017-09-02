using ECommerce.Entity.Store.Vendor;
using ECommerce.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Invoice
{
    public class InvoiceMasterInfo
    {
        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNo
        {
            get;
            set;
        }

        /// <summary>
        /// 用户编号
        /// </summary>
        public int? CustomerSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string CustomerID
        {
            get;
            set;
        }

        /// <summary>
        /// 订单编号
        /// </summary>
        public int? SONumber
        {
            get;
            set;
        }

        /// <summary>
        /// 关联订单编号
        /// </summary>
        public int? ReferenceSONumber
        {
            get;
            set;
        }

        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal? InvoiceAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 分仓时间
        /// </summary>
        public DateTime? InvoiceDate
        {
            get;
            set;
        }

        /// <summary>
        /// 支付方式编号
        /// </summary>
        public int? PayTypeSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 配送方式名称
        /// </summary>
        public string PayTypeName
        {
            get;
            set;
        }

        /// <summary>
        /// 配送方式编号
        /// </summary>
        public int? ShipTypeSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// RMA编号
        /// </summary>
        public int? RMANumber
        {
            get;
            set;
        }

        /// <summary>
        /// 原始财务编号
        /// </summary>
        public int? OriginalInvoiceNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 财务备注
        /// </summary>
        public string InvoiceMemo
        {
            get;
            set;
        }

        /// <summary>
        /// 分仓编号
        /// </summary>
        public int? StockSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 下订单时间
        /// </summary>
        public DateTime? OrderDate
        {
            get;
            set;
        }

        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime? DeliveryDate
        {
            get;
            set;
        }

        /// <summary>
        /// 销售员编号
        /// </summary>
        public int? SalesManSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 是否是批发
        /// </summary>
        public bool? IsWholeSale
        {
            get;
            set;
        }

        /// <summary>
        /// 是否保价
        /// </summary>
        public bool? IsPremium
        {
            get;
            set;
        }

        /// <summary>
        /// 保价费
        /// </summary>
        public decimal? PremiumAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 运费
        /// </summary>
        public decimal? ShippingCharge
        {
            get;
            set;
        }

        /// <summary>
        /// 附加费
        /// </summary>
        public decimal? ExtraAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal? SOAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 折扣金额
        /// </summary>
        public decimal? DiscountAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 获得积分
        /// </summary>
        public int? GainPoint  //PointAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 现金支付金额
        /// </summary>
        public decimal? CashPaid
        {
            get;
            set;
        }

        /// <summary>
        /// 积分支付金额
        /// </summary>
        public decimal? PointPaid
        {
            get;
            set;
        }

        /// <summary>
        /// 预付款金额
        /// </summary>
        public decimal? PrepayAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 优惠券抵扣
        /// </summary>
        public decimal? PromotionAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 礼品卡抵扣
        /// </summary>
        public decimal? GiftCardPayAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 使用的优惠券编号
        /// </summary>
        public int? PromotionCodeSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 使用优惠券的用户系统编号
        /// </summary>
        public int? PromotionCustomerSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 商家编号
        /// </summary>
        public int? MerchantSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 发票备注
        /// </summary>
        public string InvoiceNote
        {
            get;
            set;
        }

        /// <summary>
        /// 财务备注
        /// </summary>
        public string FinanceNote
        {
            get;
            set;
        }

        /// <summary>
        /// 是否使用支票
        /// </summary>
        public bool? IsUseChequesPay
        {
            get;
            set;
        }

        /// <summary>
        /// 特别备注
        /// </summary>
        public string SpecialComment
        {
            get;
            set;
        }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        #region ShippingAddress

        /// <summary>
        /// 收货地址地区编号
        /// </summary>
        public int? ReceiveAreaSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string ReceiveContact
        {
            get;
            set;
        }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string ReceiveName
        {
            get;
            set;
        }

        /// <summary>
        /// 收货人联系电话
        /// </summary>
        public string ReceivePhone
        {
            get;
            set;
        }

        /// <summary>
        /// 收货人手机
        /// </summary>
        public string ReceiveCellPhone
        {
            get;
            set;
        }

        /// <summary>
        /// 收货地址
        /// </summary>
        public string ReceiveAddress
        {
            get;
            set;
        }

        /// <summary>
        /// 收货地址邮政编码
        /// </summary>
        public string ReceiveZip
        {
            get;
            set;
        }

        #endregion ShippingAddress

        #region BizPattern

        /// <summary>
        /// 仓储方式
        /// </summary>
        public ShippingStockType? StockType
        {
            get;
            set;
        }

        /// <summary>
        /// 配送方式
        /// </summary>
        public DeliveryType? ShippingType
        {
            get;
            set;
        }

        /// <summary>
        /// 开票方式
        /// </summary>
        public VendorInvoiceType? InvoiceType
        {
            get;
            set;
        }

        #endregion BizPattern

        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        #endregion IIdentity Members
    }
}
