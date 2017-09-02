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
using System.Windows.Navigation;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.UserControls;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductPriceCompareQuery : PageBase
    {
        private ProductPriceCompareQueryVM _queryVM;

        public ProductPriceCompareQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            //绑定查询区域中的渠道列表
            var channelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.lstChannelList.ItemsSource = channelList;

            _queryVM = new ProductPriceCompareQueryVM();
            this.GridFilter.DataContext = _queryVM;
            this.lstChannelList.SelectedIndex = 0;

            this.lstStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ProductPriceCompareStatus>(EnumConverter.EnumAppendItemType.All);
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            this.DataGrid.Bind();
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            //1.初始化查询条件
            //2.请求服务查询
            PagingInfo p = new PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            ProductPriceCompareFacade facade = new ProductPriceCompareFacade(this);
            _queryVM.C1SysNo = ucCategoryPicker.ChooseCategory1SysNo;
            _queryVM.C2SysNo = ucCategoryPicker.ChooseCategory2SysNo;
            _queryVM.C3SysNo = ucCategoryPicker.ChooseCategory3SysNo;
            facade.Query(_queryVM, p, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                this.DataGrid.ItemsSource = args.Result.Rows;
                this.DataGrid.TotalCount = args.Result.TotalCount;
            });
        }

        private void OnMaintainDialogClosed(object sender, ResultEventArgs e)
        {
            if (e.DialogResult == DialogResultType.OK)
            {
                this.DataGrid.Bind();
            }
        }

        private void btnRenew_Click(object sender, RoutedEventArgs e)
        {
            AssureRowSelected((row) =>
            {
                Window.Confirm(ResProductPriceCompare.Confirm_Renew, (cs, cr) =>
                {
                    if (cr.DialogResult == DialogResultType.OK)
                    {

                        new ProductPriceCompareFacade(this).Recover(row.SysNo, new Action(() =>
                        {
                            Window.Alert(ResProductPriceCompare.Info_RenewSuccess);
                            this.DataGrid.Bind();
                        }));
                    }
                });
            });
        }

        private void btnAuditPass_Click(object sender, RoutedEventArgs e)
        {
            AssureRowSelected((row) =>
            {
                Window.Confirm(ResProductPriceCompare.Confirm_AuditPass, (cs, cr) =>
                {
                    if (cr.DialogResult == DialogResultType.OK)
                    {

                        new ProductPriceCompareFacade(this).AuditPass(row.SysNo, new Action(() =>
                        {
                            Window.Alert(ResProductPriceCompare.Info_AuditPassSuccess);
                            this.DataGrid.Bind();
                        }));
                    }
                });
            });
        }

        private void btnAuditDecline_Click(object sender, RoutedEventArgs e)
        {
            AssureRowSelected((row) =>
            {
                UCProductPriceCompareMaintain uc = new UCProductPriceCompareMaintain(row.SysNo);
                uc.DialogHandle = this.Window.ShowDialog(ResProductPriceCompare.Info_AuditDeclineTitle, uc, OnMaintainDialogClosed);
            });
        }

        private void AssureRowSelected(Action<dynamic> cb)
        {
            var row = this.DataGrid.SelectedItem as dynamic;
            if (row == null)
            {
                this.Window.Alert(ResProductPriceCompare.Info_PleaseSelectRow);
                return;
            }
            if (cb != null)
            {
                cb(row);
            }
        }

        /// <summary>
        /// 点击ProductID跳转到顾客维护页面
        /// </summary>
        private void Hyperlink_ProductID_Click(object sender, RoutedEventArgs e)
        {
            var lnk = sender as FrameworkElement;
            if (null != lnk)
            {
                this.Window.Navigate(string.Format(ConstValue.IM_ProductMaintainUrlFormat, lnk.Tag.ToString()), null, true);
            }
        }
    }

}
