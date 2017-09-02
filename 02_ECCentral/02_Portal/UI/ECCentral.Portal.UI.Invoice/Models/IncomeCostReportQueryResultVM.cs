using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.UI.Invoice.Resources;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class IncomeCostReportQueryResultVM : ModelBase
    {
        /// <summary>
        /// 查询结果总记录数
        /// </summary>
        public int TotalCount
        {
            get;
            set;
        }

        // Swika注意: 在ViewModel，推荐使用ObservableCollection<T>，否则集合双向绑定会有问题，
        // 不过此处因为只是单向显示结果，所以不受影响。
        private List<IncomeCostReportVM> m_ResultList;
        /// <summary>
        /// 查询结果列表
        /// </summary>
        public List<IncomeCostReportVM> ResultList
        {
            get
            {
                return m_ResultList.DefaultIfNull();
            }
            set
            {
                base.SetValue("ResultList", ref m_ResultList, value);
            }
        }

        private StatisticCollection<IncomeCostReportStatisticVM> m_StatisticList;
        /// <summary>
        /// 统计信息
        /// </summary>
        public StatisticCollection<IncomeCostReportStatisticVM> StatisticList
        {
            get { return m_StatisticList; }
            set { base.SetValue("StatisticList", ref m_StatisticList, value); }
        }
    }

    public class IncomeCostReportVM : ModelBase
    {
        private string m_CustomerName;
        public string CustomerName
        {
            get { return m_CustomerName; }
            set { base.SetValue("CustomerName", ref m_CustomerName, value); }
        }

        private DateTime? m_OutTime;
        public DateTime? OutTime
        {
            get { return m_OutTime; }
            set { base.SetValue("OutTime", ref m_OutTime, value); }
        }

        private DateTime? m_OrderDate;
        public DateTime? OrderDate
        {
            get { return m_OrderDate; }
            set { base.SetValue("OrderDate", ref m_OrderDate, value); }
        }

        private decimal? m_SOSysNo;
        public decimal? SOSysNo
        {
            get { return m_SOSysNo; }
            set { base.SetValue("SOSysNo", ref m_SOSysNo, value); }
        }

        private string m_ChannelID;
        public string ChannelID
        {
            get { return m_ChannelID; }
            set { base.SetValue("ChannelID", ref m_ChannelID, value); }
        }

        private string m_ChannelName;
        public string ChannelName
        {
            get { return m_ChannelName; }
            set { base.SetValue("ChannelName", ref m_ChannelName, value); }
        }

        private int? m_PayTypeSysNo;
        public int? PayTypeSysNo
        {
            get { return m_PayTypeSysNo; }
            set { base.SetValue("PayTypeSysNo", ref m_PayTypeSysNo, value); }
        }

        private string m_PayTypeName;
        public string PayTypeName
        {
            get { return m_PayTypeName; }
            set { base.SetValue("PayTypeName", ref m_PayTypeName, value); }
        }

        private decimal? m_ShippingAmount;
        public decimal? ShippingAmount
        {
            get { return m_ShippingAmount; }
            set { base.SetValue("ShippingAmount", ref m_ShippingAmount, value); }
        }

        private decimal? m_ProductPriceAmount;
        public decimal? ProductPriceAmount
        {
            get { return m_ProductPriceAmount; }
            set { base.SetValue("ProductPriceAmount", ref m_ProductPriceAmount, value); }
        }

        private decimal? m_PromotionDiscountAmount;
        public decimal? PromotionDiscountAmount
        {
            get { return m_PromotionDiscountAmount; }
            set { base.SetValue("PromotionDiscountAmount", ref m_PromotionDiscountAmount, value); }
        }

        private decimal? m_PointPayAmount;
        public decimal? PointPayAmount
        {
            get { return m_PointPayAmount; }
            set { base.SetValue("PointPayAmount", ref m_PointPayAmount, value); }
        }

        private decimal? m_TariffAmount;
        public decimal? TariffAmount
        {
            get { return m_TariffAmount; }
            set { base.SetValue("TariffAmount", ref m_TariffAmount, value); }
        }

        private decimal? m_DiscountAmount;
        public decimal? DiscountAmount
        {
            get { return m_DiscountAmount; }
            set { base.SetValue("DiscountAmount", ref m_DiscountAmount, value); }
        }

        private decimal? m_PayAmount;
        public decimal? PayAmount
        {
            get { return m_PayAmount; }
            set { base.SetValue("PayAmount", ref m_PayAmount, value); }
        }

        private decimal? m_ShippingGrossMargin;
        public decimal? ShippingGrossMargin
        {
            get { return m_ShippingGrossMargin; }
            set { base.SetValue("ShippingGrossMargin", ref m_ShippingGrossMargin, value); }
        }

        private decimal? m_ProductCostAmount;
        public decimal? ProductCostAmount
        {
            get { return m_ProductCostAmount; }
            set { base.SetValue("ProductCostAmount", ref m_ProductCostAmount, value); }
        }

        private decimal? m_ProductIncome;
        public decimal? ProductIncome
        {
            get { return m_ProductIncome; }
            set { base.SetValue("ProductIncome", ref m_ProductIncome, value); }
        }

        private decimal? m_ProductGrossMargin;
        public decimal? ProductGrossMargin
        {
            get { return m_ProductGrossMargin; }
            set { base.SetValue("ProductGrossMargin", ref m_ProductGrossMargin, value); }
        }

        private decimal? m_SOIncome;
        public decimal? SOIncome
        {
            get { return m_SOIncome; }
            set { base.SetValue("SOIncome", ref m_SOIncome, value); }
        }

        private decimal? m_SOGrossMargin;
        public decimal? SOGrossMargin
        {
            get { return m_SOGrossMargin; }
            set { base.SetValue("SOGrossMargin", ref m_SOGrossMargin, value); }
        }

        private bool m_IsChecked;
        /// <summary>
        /// 记录是否被选中
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return m_IsChecked;
            }
            set
            {
                base.SetValue("IsChecked", ref m_IsChecked, value);
            }
        }

        private SOStatus? m_SOStatus;
        public SOStatus? SOStatus
        {
            get { return m_SOStatus; }
            set { base.SetValue("SOStatus", ref m_SOStatus, value); }
        }
       
    }

    public class IncomeCostReportStatisticVM : ModelBase, IStatisticInfo
    {
        private decimal? m_ShippingAmount;
        public decimal? ShippingAmount
        {
            get { return m_ShippingAmount; }
            set { base.SetValue("ShippingAmount", ref m_ShippingAmount, value); }
        }

        private decimal? m_ProductPriceAmount;
        public decimal? ProductPriceAmount
        {
            get { return m_ProductPriceAmount; }
            set { base.SetValue("ProductPriceAmount", ref m_ProductPriceAmount, value); }
        }

        private decimal? m_PromotionDiscountAmount;
        public decimal? PromotionDiscountAmount
        {
            get { return m_PromotionDiscountAmount; }
            set { base.SetValue("PromotionDiscountAmount", ref m_PromotionDiscountAmount, value); }
        }

        private decimal? m_PointPayAmount;
        public decimal? PointPayAmount
        {
            get { return m_PointPayAmount; }
            set { base.SetValue("PointPayAmount", ref m_PointPayAmount, value); }
        }

        private decimal? m_TariffAmount;
        public decimal? TariffAmount
        {
            get { return m_TariffAmount; }
            set { base.SetValue("TariffAmount", ref m_TariffAmount, value); }
        }

        private decimal? m_DiscountAmount;
        public decimal? DiscountAmount
        {
            get { return m_DiscountAmount; }
            set { base.SetValue("DiscountAmount", ref m_DiscountAmount, value); }
        }

        private decimal? m_PayAmount;
        public decimal? PayAmount
        {
            get { return m_PayAmount; }
            set { base.SetValue("PayAmount", ref m_PayAmount, value); }
        }

        private decimal? m_ShippingGrossMargin;
        public decimal? ShippingGrossMargin
        {
            get { return m_ShippingGrossMargin; }
            set { base.SetValue("ShippingGrossMargin", ref m_ShippingGrossMargin, value); }
        }

        private decimal? m_ProductCostAmount;
        public decimal? ProductCostAmount
        {
            get { return m_ProductCostAmount; }
            set { base.SetValue("ProductCostAmount", ref m_ProductCostAmount, value); }
        }

        private decimal? m_ProductIncome;
        public decimal? ProductIncome
        {
            get { return m_ProductIncome; }
            set { base.SetValue("ProductIncome", ref m_ProductIncome, value); }
        }

        private decimal? m_ProductGrossMargin;
        public decimal? ProductGrossMargin
        {
            get { return m_ProductGrossMargin; }
            set { base.SetValue("ProductGrossMargin", ref m_ProductGrossMargin, value); }
        }

        private decimal? m_SOIncome;
        public decimal? SOIncome
        {
            get { return m_SOIncome; }
            set { base.SetValue("SOIncome", ref m_SOIncome, value); }
        }

        private decimal? m_SOGrossMargin;
        public decimal? SOGrossMargin
        {
            get { return m_SOGrossMargin; }
            set { base.SetValue("SOGrossMargin", ref m_SOGrossMargin, value); }
        }


        /// <summary>
        /// 统计类型
        /// </summary>
        public StatisticType StatisticType
        {
            get;
            set;
        }

        public string ToStatisticText()
        {
            return string.Format(ResIncomeCostReport.Message_StatisticInfo,
                StatisticType.ToDescription(),
                ConstValue.Invoice_ToCurrencyString(ShippingAmount),
                ConstValue.Invoice_ToCurrencyString(ProductPriceAmount),
                ConstValue.Invoice_ToCurrencyString(PromotionDiscountAmount),
                ConstValue.Invoice_ToCurrencyString(PointPayAmount),
                ConstValue.Invoice_ToCurrencyString(TariffAmount),
                ConstValue.Invoice_ToCurrencyString(DiscountAmount),
                ConstValue.Invoice_ToCurrencyString(PayAmount),
                ConstValue.Invoice_ToCurrencyString(ShippingGrossMargin),
                ConstValue.Invoice_ToCurrencyString(ProductCostAmount),
                ConstValue.Invoice_ToCurrencyString(ProductIncome),
                ConstValue.Invoice_ToCurrencyString(ProductGrossMargin),
                ConstValue.Invoice_ToCurrencyString(SOIncome),
                ConstValue.Invoice_ToCurrencyString(SOGrossMargin));
        }
    }
}