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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.Common.Models;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Common.Facades;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Common.Resources;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Enum.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.Common.Views
{
    [View]
    public partial class ShipTypeProduct : PageBase
    {
        public ShipTypeProductQueryFilterVM _viewMode;
        public ShipTypeFacade _shipTypeProcuctFacade;
        public ShipTypeProduct()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            _viewMode = new ShipTypeProductQueryFilterVM();
            _shipTypeProcuctFacade = new ShipTypeFacade(this);
            BindComboBoxData();
            this.QueryFilter.DataContext = _viewMode;
        }

        private void QueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this._viewMode.PagingInfo.PageIndex = e.PageIndex;
            this._viewMode.PagingInfo.PageSize = e.PageSize;
            this._viewMode.PagingInfo.SortBy = e.SortField;
             
            _shipTypeProcuctFacade.QueryShipTypeProductList(_viewMode, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                QueryResult.ItemsSource = args.Result.Rows.ToList("IsChecked", false);
                QueryResult.TotalCount =  args.Result.TotalCount;

            });
        }

        private void BindComboBoxData()
        {
            //商户:
            this.Merchant.ItemsSource = EnumConverter.GetKeyValuePairs<CompanyCustomer>(EnumConverter.EnumAppendItemType.All);
            this.Merchant.SelectedIndex = 0;
        }

        private void Serch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QueryFilter))
            {
                // if (_filterVM.HasValidationErrors) return;
                QueryResult.Bind();
            }
        }

        private void ItemRange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ItemRange.SelectedIndex)
            {
                case 0: 
                    GoodsPanel.Visibility = Visibility.Collapsed;
                    _viewMode.ProductID=null;
                    _viewMode.ProductSysNo = null;
                    CategoryPanel.Visibility = Visibility.Collapsed;
                    _viewMode.Category1 = null;
                    break;
                case 1:
                    GoodsPanel.Visibility = Visibility.Visible;
                    _viewMode.ProductID = null;
                    _viewMode.ProductSysNo = null;
                    CategoryPanel.Visibility = Visibility.Collapsed;
                    _viewMode.Category1 = null;
                    break;
                case 2:
                    GoodsPanel.Visibility = Visibility.Collapsed;
                    _viewMode.ProductID = null;
                    _viewMode.ProductSysNo = null;
                    CategoryPanel.Visibility = Visibility.Visible;
                    _viewMode.Category1 = null;
                    break;
            }
        }

        private void DataGridCheckBoxAllCode_Click(object sender, RoutedEventArgs e)
        {

            CheckBox chk = (CheckBox)sender;
            dynamic rows=QueryResult.ItemsSource;
            foreach (dynamic row in rows)
            {
                row.IsChecked = chk.IsChecked.Value;
            }
        }

        private void btnBatchVoid_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_ShipTypeProduct_BatchDelete))
            {
                Window.Alert("您没有此功能的操作权限！");
                return;
            }
            dynamic rows = QueryResult.ItemsSource;
            List<int?> sysNoList = new List<int?>();
            if (rows == null)
            {
                Window.Alert("请至少勾选一条数据!");
                return;
            }
            foreach (dynamic row in rows)
            {
                if (row.IsChecked)
                {
                    sysNoList.Add(row.SysNo);
                }
            }
            if (sysNoList.Count == 0)
            {
                Window.Alert("请至少勾选一条数据!");
                return;
            }
            Window.Confirm("确定删除所选数据?",(o,s) =>
            {
                if (s.DialogResult == DialogResultType.OK)
                {
                    _shipTypeProcuctFacade.VoidShipTypeProduct(sysNoList, (obj, args) =>
                    {
                        Window.Alert("提示信息", "删除数据成功!", MessageType.Information, (f, ar) =>
                        {
                            QueryResult.Bind();
                        });

                    });
                }
            });
            
        }

        private void btnNewArea_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_ShipTypeProduct_Add))
            {
                Window.Alert("您没有此功能的操作权限！");
                return;
            }
            this.Window.Navigate(ConstValue.Common_ShipTypeProductAddnewUrlFormat,null,true);
        }
    }
}
