using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.Basic.Components.UserControls.ProductPicker
{
    public partial class UCProductSearchForCombo : UserControl
    {
        public List<int> SelectedProductSysNoList
        {
            get
            {
                List<int> list = new List<int>();
                if (this.dataProduct.ItemsSource != null)
                {
                    dynamic rows = this.dataProduct.ItemsSource;
                    if (rows != null)
                    {
                        foreach (dynamic row in rows)
                        {
                            if (row.IsChecked)
                            {
                                list.Add(row.SysNo);
                            }
                        }
                    }
                }
                return list;
            }
        }

        public ProductQueryFilter Filter
        {
            get
            {
                return this.gridCondition.DataContext as ProductQueryFilter;
            }
            set
            {
                this.gridCondition.DataContext = value;
            }
        }

        public UCProductSearchForCombo()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(UCProductSearchForCombo_Loaded);
        }

        void UCProductSearchForCombo_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCProductSearchForCombo_Loaded);
            this.Filter = new ProductQueryFilter();

            //商品类型
            this.cmbType.ItemsSource = EnumConverter.GetKeyValuePairs<ProductType>(EnumConverter.EnumAppendItemType.All);
            this.cmbType.SelectedIndex = 0;

            //商品状态
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ProductStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedIndex = 0;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {            
            dataProduct.Bind();
        }

        private void dataProduct_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {           
            this.Filter.PagingInfo = new PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            //不读取作废商品
            this.Filter.IsNotAbandon = true;

            new ProductQueryFacade(CPApplication.Current.CurrentPage).QueryProduct(this.Filter, (s, args) =>
            {
                checkHidden.IsChecked = false;

                if (!args.FaultsHandle())
                {
                    dataProduct.TotalCount = args.Result.TotalCount;
                    Dictionary<string, object> changeColumns = new Dictionary<string, object>();
                    changeColumns.Add("IsChecked", false);                    
                    dataProduct.ItemsSource = args.Result.Rows.ToList(changeColumns);
                }
            });
        }

        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            dynamic rows = this.dataProduct.ItemsSource;
            if (rows != null)
            {
                foreach (dynamic row in rows)
                {
                    row.IsChecked = chk.IsChecked.Value ? true : false;
                }
            }
        }

        private void Hyperlink_ProductNumber_Click(object sender, RoutedEventArgs e)
        {
            var lnk = sender as FrameworkElement;
            if (null != lnk)
            {
                (CPApplication.Current.CurrentPage as PageBase).Window.Navigate(string.Format(ConstValue.IM_ProductMaintainUrlFormat, lnk.Tag.ToString()), null, true);                
            }
        }
    }
}
