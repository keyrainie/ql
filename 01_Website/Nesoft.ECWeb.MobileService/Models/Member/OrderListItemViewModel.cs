using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity.Order;
using Nesoft.ECWeb.Entity.Shipping;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.MobileService.Models.Product;
using Nesoft.Utility;

namespace Nesoft.ECWeb.MobileService.Models.Member
{

    /// <summary>
    /// 订单列表详情
    /// </summary>
    public class OrderListItemViewModel
    {
        public string MemoForCustomer { get; set; }
        public int SoSysNo { get; set; }

        public SOStatus Status { get; set; }

        public string StatusText
        {
            get
            {
                if (Status == SOStatus.Original)
                {
                    if (IsNetPayed == 1)
                    {
                        return "已支付";
                    }
                    else
                    {
                        return "未支付";
                    }
                }
                else
                {
                    return EnumHelper.GetDescription(Status);
                }
            }
        }

        public int CustomerSysNo { get; set; }
        public string OrderDateString { get; set; }

        public int SOType { get; set; }

        public decimal SoAmt { get; set; }

        public int IsNet { get; set; }

        public bool IsNetPay { get; set; }

        public int IsPayWhenRecv { get; set; }

        public int IsNetPayed { get; set; }

        public int NetPayType { get; set; }

        public ShipTypeInfo ShipType { get; set; }

        public PaymentInfo Payment { get; set; }

        public string DeliveryDateString { get; set; }

        public int DeliveryTimeRange { get; set; }

        public string Memo { get; set; }

        public string DeliverySection { get; set; }

        public string ReceiveAreaSysNo { get; set; }

        public string ReceiveAreaName { get; set; }

        public string ReceiveContact { get; set; }

        public string ReceiveName { get; set; }

        public string ReceivePhone { get; set; }

        public string ReceiveCellPhone { get; set; }

        public string ReceiveAddress { get; set; }

        public string ReceiveZip { get; set; }

        public List<SOItemInfo> SOItemList { get; set; }

        public SOAmountInfo Amount { get; set; }
        public decimal TariffAmt { get; set; }

        public decimal PointAmt { get; set; }

        public decimal PromotionAmt { get; set; }

        /// <summary>
        /// 需要支付的金额
        /// </summary>
        public decimal RealPayAmt { get; set; }

         /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal SOTotalAmt { get; set; }

        public bool CanCancelOrder { get; set; }
        public bool CanPayOrder { get; set; }
        public bool IsShowVoid { get; set; }
        /// <summary>
        /// 该订单是否可以进行售后处理true处理，false不处理
        /// </summary>
        public bool CanRequest { get; set; }

        /*
         * OrderStatusProgressStep = 0 ，处于这种状态时，不会指向任何一个点.
         *OrderStatusProgressStep = 1 : 等待审核
OrderStatusProgressStep = 2 : 海关申报
OrderStatusProgressStep = 3 : 发往顾客
OrderStatusProgressStep = 4 : 订单完成
         * */
        public int OrderStatusProgressStep { get; set; }
    }

}