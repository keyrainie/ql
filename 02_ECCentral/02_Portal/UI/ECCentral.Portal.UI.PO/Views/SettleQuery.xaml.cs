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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.QueryFilter.PO;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class SettleQuery : PageBase
    {
        public SettleQueryVM settleQueryVM;
        public SettleFacade settleFacade;
        public ECCentral.QueryFilter.PO.SettleQueryFilter queryRequest;

        public SettleQuery()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            settleQueryVM = new SettleQueryVM();

            settleFacade = new SettleFacade(this);

            queryRequest = new ECCentral.QueryFilter.PO.SettleQueryFilter();

            BindComboBoxData();
            this.DataContext = settleQueryVM;
            SetAccessControl();            
        }

        private void SetAccessControl()
        {
            ////权限控制:
            ////新增供应商:
            //if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Vendor_EditAndCreateVendor))
            //{
            //    this.btnNewVendor.IsEnabled = false;
            //}
            ////搜索可用状态的供应商
            //if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Vendor_SearchValidVendor))
            //{
            //    (this.cmbStatus.ItemsSource as List<KeyValuePair<VendorStatus?, string>>).RemoveAt(0);
            //    (this.cmbStatus.ItemsSource as List<KeyValuePair<VendorStatus?, string>>).RemoveAt(1);
            //    settleQueryVM.Status = VendorStatus.UnAvailable;
            //}
        }

        #region [Methods]

        private void BindComboBoxData()
        {
            //状态:
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<POSettleStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedIndex = 0;
        }

        #endregion

        #region [Events]

        private void btnNewVendor_Click(object sender, RoutedEventArgs e)
        {
            VendorNew VendorNewPage = new VendorNew();
           // Window.Navigate("/ECCentral.Portal.UI.PO/VendorNew", null, true);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.StackPanelCondition))
            {
                int sysno = 0;
                if (!string.IsNullOrEmpty(settleQueryVM.SettleSysNo) && !int.TryParse(settleQueryVM.SettleSysNo, out sysno))
                {
                    Window.Alert("结算单编号输入有误");
                    settleQueryVM.SettleSysNo = string.Empty;
                    return;
                }

                if (int.TryParse(settleQueryVM.SettleSysNo, out sysno) && sysno <= 0)
                {
                    Window.Alert("结算单编号输入有误");
                    settleQueryVM.SettleSysNo = string.Empty;
                    return;
                }

                this.queryRequest = EntityConverter<SettleQueryVM, ECCentral.QueryFilter.PO.SettleQueryFilter>.Convert(settleQueryVM);
                QueryResultGrid.Bind();
            }
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryRequest.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = QueryResultGrid.PageSize,
                PageIndex = QueryResultGrid.PageIndex,
                SortBy = e.SortField
            };
            queryRequest.PageInfo.SortBy = e.SortField;
            settleFacade.QuerySettle(queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var settleList = args.Result.Rows;
                int totalCount = args.Result.TotalCount;
                QueryResultGrid.TotalCount = totalCount;
                QueryResultGrid.ItemsSource = settleList;
            });
        }

        /// <summary>
        /// 编辑，加载供应商信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_EditVendor_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (null != getSelectedItem)
            {
                Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/SettleDetailView/{0}", getSelectedItem["SysNo"]), null, true);
            }
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            dynamic viewList = this.QueryResultGrid.ItemsSource as dynamic;
            if (viewList != null)
            {
                foreach (var view in viewList)
                {
                    view.IsCheck = ckb.IsChecked.Value ? true : false;
                }
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
                Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/SettleOrderCreate"), null, true);
        }

        #endregion

    }

}
