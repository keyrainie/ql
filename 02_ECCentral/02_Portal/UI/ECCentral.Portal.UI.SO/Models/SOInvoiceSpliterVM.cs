using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using System.Windows.Data;
using ECCentral.Portal.UI.SO.Resources;

namespace ECCentral.Portal.UI.SO.Models
{

    public class InvoiceProductVM : ModelBase
    {
        public int ProductSysNo
        {
            get;
            set;
        }
        public string ProductID
        {
            get;
            set;
        }
        public string ProductName
        {
            get;
            set;
        }
        public int Quantity
        {
            get;
            set;
        }
        public int InvoiceQuantity
        {
            get;
            set;
        }
        public decimal Price
        {
            get;
            set;
        }
        public string StockName
        {
            get;
            set;
        }
        public int? StockSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 是否是延保
        /// </summary>
        public bool? IsExtendWarrantyItem
        {
            get;
            set;
        }
        /// <summary>
        /// 如果是延保，对应的主商品编号
        /// </summary>
        public List<int> MasterProductSysNo
        {
            get;
            set;
        }

        public bool IsSplited
        {
            set;
            get;
        }

        /// <summary>
        /// 发票编号
        /// </summary>
        public int OldInvoiceNo
        {
            get;
            set;
        }

        private int invoiceNo;
        /// <summary>
        /// 发票编号
        /// </summary>
        public int InvoiceNo
        {
            get { return invoiceNo; }
            set
            {
                SetValue("InvoiceNo", ref invoiceNo, value);
            }
        }

        public bool InvoiceQuantityIsReadOnly
        {
            get
            {
                return Quantity == 1 || IsSplited;
            }
        }

        public Visibility IsShowSpliterButton
        {
            get
            {
                var result = Visibility.Collapsed;
                if (!IsSplited)
                {
                    result = Visibility.Visible;
                    if (IsShowDeleteButton == Visibility.Visible)
                    {
                        result = Visibility.Collapsed;
                    }
                }
                return result;
            }
        }

        public Visibility IsShowDeleteButton
        {
            get
            {
                var result = Visibility.Collapsed;
                if (!IsSplited)
                {
                    if (InvoiceNo != 1)
                    {
                        result = Visibility.Visible;
                    }
                }
                return result;
            }
        }

        private List<KeyValuePair<int, string>> invoiceNumberList;
        public List<KeyValuePair<int, string>> InvoiceNumberList
        {
            get { return invoiceNumberList; }
            set
            {
                SetValue("InvoiceNumberList", ref invoiceNumberList, value);
            }
        }
    }

    public class InvoiceSplitView : ModelBase
    {
        public SOBaseInfoVM SOBaseInfo
        {
            get;
            set;
        }

        public List<InvoiceProductVM> OriginalProductList { get; set; }

        private List<InvoiceProductVM> splitedProductList;
        public List<InvoiceProductVM> SplitedProductList
        {
            get { return splitedProductList; }
            set
            {
                SetValue("SplitedProductList", ref splitedProductList, value);
                if (splitedProductList != null)
                    splitedProductList.ForEach(item => { item.OldInvoiceNo = item.InvoiceNo; });

                GroupView = new PagedCollectionView(value);

                if (groupView.CanGroup == true)
                {
                    groupView.GroupDescriptions.Add(new PropertyGroupDescription("StockSysNo", new StockNameConverter()));//先根据仓库分组
                    groupView.GroupDescriptions.Add(new PropertyGroupDescription("InvoiceNo", new InvoiceNameConverter())); //再根据发票分组
                }
            }
        }

        private PagedCollectionView groupView;
        public PagedCollectionView GroupView
        {
            get { return groupView; }
            set
            {
                SetValue("GroupView", ref groupView, value);
            }
        }

        public InvoiceSplitView()
        {
            OriginalProductList = new List<InvoiceProductVM>();
            SplitedProductList = new List<InvoiceProductVM>();
        }
    }

    public class StockNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? null : String.Format("{0}{1}", ResSO.DataGrid_GroupTitle_Stock, value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InvoiceNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? null : String.Format("{0}{1}", ResSO.DataGrid_GroupTitle_Invoice, value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
