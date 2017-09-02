using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using ECCentral.Portal.Basic.Converters;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.Controls.Data;
using System.Linq;
using DataGridTemplateColumn = Newegg.Oversea.Silverlight.Controls.Data.DataGridTemplateColumn;
using DataGridTextColumn = Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls.Components;


namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductQueryGridView : UserControl
    {
        #region 自定义EventHandler
        public event EventHandler GridView_LoadingDataSource;
        public event EventHandler GridView_ExportAllDataSource;
        public event RoutedEventHandler GridView_HyperlinkProductKey_Click;
        public event RoutedEventHandler GridView_HyperlinkProductNameKey_Click;
        public event RoutedEventHandler GridView_HyperlinkImageId_Click;
        public event RoutedEventHandler GridView_HyperlinkGroupID_Click;
        public event RoutedEventHandler GridView_HyperlinkAccountQtyKey_Click;
        public event RoutedEventHandler GridView_HyperlinkAvailableQtyKey_Click;
        public event RoutedEventHandler GridView_HyperlinkVirtualQtyKey_Click;
        public event RoutedEventHandler GridView_HyperlinkSaleDaysKey_Click;
        public event RoutedEventHandler GridView_HyperlinkDMSValueKey_Click;
        public event RoutedEventHandler GridView_HyperlinkPurchaseQtyKey_Click;
        public event RoutedEventHandler GridView_HyperlinkUnmarketableProductQuantityKey_Click;
        public event RoutedEventHandler GridView_HyperlinkProductNotifyTimesKey_Click;
        public event RoutedEventHandler GridView_linkProducId_Click;
        public event RoutedEventHandler GridView_linkCurrentPrice_Click;
        public event RoutedEventHandler GridView_HyperlinkMultiLanguageEdit_Click;
        #endregion

        #region 初始化
        public ProductQueryGridView()
        {
            InitializeComponent();
            this.Loaded+=new RoutedEventHandler(ProductQueryGridView_Loaded);
        }

        void ProductQueryGridView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ProductQueryGridView_Loaded;

        }
        #endregion
     
        #region 内部事件处理
        private void dgProductQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            if (GridView_LoadingDataSource != null)
            {
                GridView_LoadingDataSource(sender, e);
            }
        }

        private void hyperlinkProductSysNo_Click(object sender, RoutedEventArgs e)
        {
            if (GridView_HyperlinkProductKey_Click != null)
            {
                GridView_HyperlinkProductKey_Click(sender, e);
            }
        }

        private void hyperlinkImageId_Click(object sender, RoutedEventArgs e)
        {
            if (GridView_HyperlinkImageId_Click != null)
            {
                GridView_HyperlinkImageId_Click(sender, e);
            }
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var ckb = sender as CheckBox;
            if (ckb == null) return;
            var viewList = dgProductQueryResult.ItemsSource as dynamic;
            if (viewList == null) return;
            foreach (var view in viewList)
            {
                view.IsChecked = ckb.IsChecked != null && ckb.IsChecked.Value;
            }
        }
        #endregion

        /// <summary>
        /// 商品名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hyperlinkProductName_Click(object sender, RoutedEventArgs e)
        {
            if (GridView_HyperlinkProductNameKey_Click != null)
            {
                GridView_HyperlinkProductNameKey_Click(sender, e);
            }
        }

        /// <summary>
        /// 财务库存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hyperlinkAccountQty_Click(object sender, RoutedEventArgs e)
        {
            if (GridView_HyperlinkAccountQtyKey_Click != null)
            {
                GridView_HyperlinkAccountQtyKey_Click(sender, e);
            }
        }

        /// <summary>
        /// 被占用库存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hyperlinkAvailableQty_Click(object sender, RoutedEventArgs e)
        {
            if (GridView_HyperlinkAvailableQtyKey_Click != null)
            {
                GridView_HyperlinkAvailableQtyKey_Click(sender, e);
            }
        }

        /// <summary>
        /// 虚库数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hyperlinkVirtualQty_Click(object sender, RoutedEventArgs e)
        {
            if (GridView_HyperlinkVirtualQtyKey_Click != null)
            {
                GridView_HyperlinkVirtualQtyKey_Click(sender, e);
            }
        }

        /// <summary>
        /// 可买天数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hyperlinkSaleDays_Click(object sender, RoutedEventArgs e)
        {
            if (GridView_HyperlinkSaleDaysKey_Click != null)
            {
                GridView_HyperlinkSaleDaysKey_Click(sender, e);
            }
        }

        /// <summary>
        /// 商品多语言编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkMultiLanguageEdit_Click(object sender, RoutedEventArgs e)
        {
            if (GridView_HyperlinkMultiLanguageEdit_Click != null)
            {
                GridView_HyperlinkMultiLanguageEdit_Click(sender, e);
            }
        }

        /// <summary>
        /// 可买天数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hyperlinkDMSValue_Click(object sender, RoutedEventArgs e)
        {
            if (GridView_HyperlinkDMSValueKey_Click != null)
            {
                GridView_HyperlinkDMSValueKey_Click(sender, e);
            }
        }

        /// <summary>
        /// 待入库数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hyperlinkPurchaseQty_Click(object sender, RoutedEventArgs e)
        {
            if (GridView_HyperlinkPurchaseQtyKey_Click != null)
            {
                GridView_HyperlinkPurchaseQtyKey_Click(sender, e);
            }
        }

        private void hyperlinkUnmarketableProductQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (GridView_HyperlinkUnmarketableProductQuantityKey_Click != null)
            {
                GridView_HyperlinkUnmarketableProductQuantityKey_Click(sender, e);
            }
        }

        /// <summary>
        /// 到货通知 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hyperlinkProductNotifyTimes_Click(object sender, RoutedEventArgs e)
        {
            if (GridView_HyperlinkProductNotifyTimesKey_Click != null)
            {
                GridView_HyperlinkProductNotifyTimesKey_Click(sender, e);
            }
        }

        /// <summary>
        /// 商品组
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hyperlinkGroupID_Click(object sender, RoutedEventArgs e)
        {
            if (GridView_HyperlinkGroupID_Click != null)
            {
                GridView_HyperlinkGroupID_Click(sender, e);
            }
        }

        private void linkProducId_Click(object sender, RoutedEventArgs e)
        {
            if (GridView_linkProducId_Click != null)
            {
                GridView_linkProducId_Click(sender, e);
            }
        }

        private void linkCurrentPrice_Click(object sender, RoutedEventArgs e)
        {
            if (GridView_linkCurrentPrice_Click != null)
            {
                GridView_linkCurrentPrice_Click(sender, e);
            }
        }

        private void linkIngramInventoryQty_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 导出全部。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgProductQueryResult_ExportAllClick(object sender, EventArgs e)
        {

            if (this.dgProductQueryResult.TotalCount >= ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit)
            {
                var mainPage = CPApplication.Current.CurrentPage;
                mainPage.Context.Window.Alert("超出可以导出的数据量，最大可以导出10000条！", MessageType.Error);
                return;
            }
            if (GridView_ExportAllDataSource != null)
            {
                GridView_ExportAllDataSource(sender, e);
            }
        }
    }
    /// <summary>
    /// 将null转换成0
    /// </summary>
    public class NullConvertToZreo : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
            if (value != null)
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    return 0;
                }
                return value;
            }
            return 0;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
