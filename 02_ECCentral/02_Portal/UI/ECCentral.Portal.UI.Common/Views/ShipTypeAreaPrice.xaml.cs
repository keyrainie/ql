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

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.Common.Facades;
using ECCentral.Portal.UI.Common.Models;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.UI.Common.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Common.Resources;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Common.Views
{
    [View]
    public partial class ShipTypeAreaPrice : PageBase
    {
        public ShipTypeAreaPriceQueryFilterVM _viewModel;
        public ShipTypeFacade _facade;
        public ShipTypeAreaPrice()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            _viewModel = new ShipTypeAreaPriceQueryFilterVM();
            _facade = new ShipTypeFacade(this);
            BindComboBoxData();
            QueryFilter.DataContext = _viewModel;
        }

        private void Serch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QueryFilter))
            {
                // if (_filterVM.HasValidationErrors) return;
                QueryResult.Bind();
            }
        }

        private void BindComboBoxData()
        {
            //商户:
            this.Merchant.ItemsSource = EnumConverter.GetKeyValuePairs<CompanyCustomer>(EnumConverter.EnumAppendItemType.All);
            this.Merchant.SelectedIndex = 0;
        }

        private void QueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this._viewModel.PagingInfo.PageIndex = e.PageIndex;
            this._viewModel.PagingInfo.PageSize = e.PageSize;
            this._viewModel.PagingInfo.SortBy = e.SortField;

            _facade.QueryShipTypeAreaPriceList(_viewModel, (obj, args) =>
            {
                QueryResult.ItemsSource = args.Result.Rows.ToList("IsChecked", false);
                QueryResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ck = (CheckBox)sender;
            dynamic rows = QueryResult.ItemsSource;
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    row.IsChecked = ck.IsChecked.Value;
                }
            }
        }

        private void btnBatchVoid_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_ShipTypeAreaPrice_BatchDelete))
            {
                Window.Alert("您没有此功能的操作权限！");
                return;
            }
            List<int> sysnoList = new List<int>();
            dynamic rows = QueryResult.ItemsSource;
            if (rows == null)
            {
                Window.Alert("请选择需要删除的数据！");
                return;
            }

            foreach (var row in rows)
            {
                if (row.IsChecked)
                {
                    sysnoList.Add(row.SysNo);
                }
            }
            if (sysnoList.Count <= 0)
            {
                Window.Alert("请选择需要删除的数据!");
                return;
            }
            Window.Confirm("确定删除所选数据?", (o, s) =>
            {
                if (s.DialogResult == DialogResultType.OK)
                {
                    _facade.VoidShipTypeAreaPrice(sysnoList, (m, args) =>
                    {
                        Window.Alert("提示信息", "删除成功！", MessageType.Information, (g, ar) =>
                        {
                            QueryResult.Bind();
                        });
                    });
                }
            });
        }

        private void btnNewArea_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_ShipTypeAreaPrice_Add))
            {
                Window.Alert("您没有此功能的操作权限！");
                return;
            }
            Dialog(null);
        }

        private void Hyperlink_EditData_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_ShipTypeAreaPrice_Edit))
            {
                Window.Alert("您没有此功能的操作权限！");
                return;
            }
            ShipTypeAreaPriceInfoVM _infoView = new ShipTypeAreaPriceInfoVM();
            dynamic row = QueryResult.SelectedItem;
            if (row != null)
            {

                _infoView.AreaSysNo = row.AreaSysNo;
                _infoView.ShipTypeSysNo = row.ShipTypeSysNo;
                _infoView.BaseWeight = row.BaseWeight.ToString();
                _infoView.TopWeight = row.TopWeight.ToString();
                _infoView.UnitWeight = row.UnitWeight.ToString();
                _infoView.UnitPrice = row.UnitPrice.ToString();
                _infoView.MaxPrice = row.MaxPrice.ToString();
                _infoView.SysNo = row.SysNo;
                _infoView.ChannelID = _infoView.ListWebChannel[1].ChannelID;
                _infoView.VendorName = row.VendorName;
                _infoView.VendorSysNo = row.VendorSysNo;
                Dialog(_infoView);

            }
        }
        private void Dialog(ShipTypeAreaPriceInfoVM entity)
        {
            ShipTypeAreaPriceMaintain shipTypeAreaPriceMaintain = new ShipTypeAreaPriceMaintain(entity) { Page = this };
            shipTypeAreaPriceMaintain.DiolgHander = Window.ShowDialog(entity == null ? "新建配送方式-地区-价格" : "编辑配送方式-地区-价格", shipTypeAreaPriceMaintain, (s, args) =>
            {

                QueryResult.Bind();
            });
        }


        private void btnBatchCreate_Click(object sender, RoutedEventArgs e)
        {
            BatchCreateShipTypeAreaPrice ut = new BatchCreateShipTypeAreaPrice();
            ut.DiolgHander = Window.ShowDialog("批量新建配送方式-地区-价格", ut, (s, args) =>
                {
                    QueryResult.Bind();
                },new Size(600, 600));
        }
    }
}
