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
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.UI.Invoice.Resources;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class SalesStatisticsReportQueryResultVM : ModelBase
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
        private List<SalesStatisticsReportVM> m_ResultList;
        /// <summary>
        /// 查询结果列表
        /// </summary>
        public List<SalesStatisticsReportVM> ResultList
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

        private StatisticCollection<SalesStatisticsReportStatisticVM> m_StatisticList;
        /// <summary>
        /// 统计信息
        /// </summary>
        public StatisticCollection<SalesStatisticsReportStatisticVM> StatisticList
        {
            get { return m_StatisticList; }
            set { base.SetValue("StatisticList", ref m_StatisticList, value); }
        }
    }

    public class SalesStatisticsReportVM : ModelBase
    {
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

        private string m_ProductID;
        public string ProductID
        {
            get { return m_ProductID; }
            set { base.SetValue("ProductID", ref m_ProductID, value); }
        }

        private string m_ProductName;
        public string ProductName
        {
            get { return m_ProductName; }
            set { base.SetValue("ProductName", ref m_ProductName, value); }
        }

        private string m_C1Name;
        public string C1Name
        {
            get { return m_C1Name; }
            set { base.SetValue("C1Name", ref m_C1Name, value); }
        }

        private string m_C2Name;
        public string C2Name
        {
            get { return m_C2Name; }
            set { base.SetValue("C2Name", ref m_C2Name, value); }
        }

        private string m_C3Name;
        public string C3Name
        {
            get { return m_C3Name; }
            set { base.SetValue("C3Name", ref m_C3Name, value); }
        }

        private DateTime? m_BeginDate;
        public DateTime? BeginDate
        {
            get { return m_BeginDate; }
            set { base.SetValue("BeginDate", ref m_BeginDate, value); }
        }

        private DateTime? m_EndDate;
        public DateTime? EndDate
        {
            get { return m_EndDate; }
            set { base.SetValue("EndDate", ref m_EndDate, value); }
        }

        private string m_BrandName;
        public string BrandName
        {
            get { return m_BrandName; }
            set { base.SetValue("BrandName", ref m_BrandName, value); }
        }

        private string m_VendorName;
        public string VendorName
        {
            get { return m_VendorName; }
            set { base.SetValue("VendorName", ref m_VendorName, value); }
        }

        private string m_StockName;
        public string StockName
        {
            get { return m_StockName; }
            set { base.SetValue("StockName", ref m_StockName, value); }
        }

        private string m_BMCode;
        public string BMCode
        {
            get { return m_BMCode; }
            set { base.SetValue("BMCode", ref m_BMCode, value); }
        }

        private string m_ProductProperty1;
        public string ProductProperty1
        {
            get { return m_ProductProperty1; }
            set { base.SetValue("ProductProperty1", ref m_ProductProperty1, value); }
        }

        private string m_ProductProperty2;
        public string ProductProperty2
        {
            get { return m_ProductProperty2; }
            set { base.SetValue("ProductProperty2", ref m_ProductProperty2, value); }
        }

        private int? m_Quantity;
        public int? Quantity
        {
            get { return m_Quantity; }
            set { base.SetValue("Quantity", ref m_Quantity, value); }
        }

        private decimal? m_ProductCost;
        public decimal? ProductCost
        {
            get { return m_ProductCost; }
            set { base.SetValue("ProductCost", ref m_ProductCost, value); }
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

        private decimal? m_ProductSaleAmount;
        public decimal? ProductSaleAmount
        {
            get { return m_ProductSaleAmount; }
            set { base.SetValue("ProductSaleAmount", ref m_ProductSaleAmount, value); }
        }

        private string m_PayTypeName;
        public string PayTypeName
        {
            get { return m_PayTypeName; }
            set { base.SetValue("PayTypeName", ref m_PayTypeName, value); }
        }

        private decimal? m_ProductGrossMargin;
        public decimal? ProductGrossMargin
        {
            get { return m_ProductGrossMargin; }
            set { base.SetValue("ProductGrossMargin", ref m_ProductGrossMargin, value); }
        }

    }

    public class SalesStatisticsReportStatisticVM : ModelBase, IStatisticInfo
    {
        private decimal? m_ProductCost;
        public decimal? ProductCost
        {
            get { return m_ProductCost; }
            set { base.SetValue("ProductCost", ref m_ProductCost, value); }
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

        private decimal? m_ProductSaleAmount;
        public decimal? ProductSaleAmount
        {
            get { return m_ProductSaleAmount; }
            set { base.SetValue("ProductSaleAmount", ref m_ProductSaleAmount, value); }
        }

        private decimal? m_ProductGrossMargin;
        public decimal? ProductGrossMargin
        {
            get { return m_ProductGrossMargin; }
            set { base.SetValue("ProductGrossMargin", ref m_ProductGrossMargin, value); }
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
            return string.Format(ResSalesStatisticsReport.Message_StatisticInfo,
                        StatisticType.ToDescription(),
                        ConstValue.Invoice_ToCurrencyString(ProductCost),
                        ConstValue.Invoice_ToCurrencyString(ProductPriceAmount),
                        ConstValue.Invoice_ToCurrencyString(PromotionDiscountAmount),
                        ConstValue.Invoice_ToCurrencyString(ProductSaleAmount),
                        ConstValue.Invoice_ToCurrencyString(ProductGrossMargin));
        }
    }
}
