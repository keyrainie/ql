using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Customer;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.UI.Customer.UserControls;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Customer.Views.ShipTypeSMS
{
    [View]
    public partial class ShipTypeSMSQuery : PageBase
    {
        ShipTypeSMSQueryVM viewModel;
        ShipTypeSMSQueryFilter filter;
        CommonDataFacade commFacade;
        ShipTypeSMSQueryFacade facade;

        public ShipTypeSMSQuery()
        {
            viewModel = new ShipTypeSMSQueryVM();
            filter = new ShipTypeSMSQueryFilter();
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            commFacade = new CommonDataFacade(this);
            facade = new ShipTypeSMSQueryFacade(this);
            InitVM();
            this.DataContext = viewModel;
            viewModel.ChannelID = "0";
            base.OnPageLoad(sender, e);
            CheckRights();
        }

        private void InitVM()
        {
            CodeNamePairHelper.GetList("Customer", "SMSType", CodeNamePairAppendItemType.All, (s, arg) =>
            {
                if (arg.FaultsHandle())
                    return;
                foreach (var item in arg.Result)
                {
                    viewModel.SMSTypeList.Add(item);
                }
                cbSMSType.SelectedIndex = 0;
            });
            new CommonDataFacade(this).GetShippingTypeList(true, (s, arg) =>
            {
                if (arg.FaultsHandle())
                    return;
                viewModel.ShippingTypeList = arg.Result;
            });
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            dataGrid1.Bind();
        }

        private void dataGrid1_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            filter.ShipType = viewModel.ShipTypeSysNo;
            filter.ShipTypeSMSStatus = (int?)viewModel.Status;
            filter.SMSType = viewModel.SMSType;
            filter.WebChannelID = viewModel.ChannelID;
            filter.CompanyCode = viewModel.CompanyCode;
            filter.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            facade.Query(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                dataGrid1.ItemsSource = args.Result.Rows;
                dataGrid1.TotalCount = args.Result.TotalCount;
            });
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            ShipTypeSMSMaintain DialogPage = new ShipTypeSMSMaintain();
            DialogPage.Dialog = this.Window.ShowDialog(ECCentral.Portal.UI.Customer.Resources.ResShipTypeSMSQuery.Title_Add, DialogPage, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                    dataGrid1.Bind();
            });
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                ShipTypeSMSVM vm = DynamicConverter<ShipTypeSMSVM>.ConvertToVM(dataGrid1.SelectedItem, "SMSType");
                //渠道信息何用
                vm.ChannelID = "1";
                vm.SMSType = (dataGrid1.SelectedItem as dynamic).SMSTypeSysNo;
                ShipTypeSMSMaintain DialogPage = new ShipTypeSMSMaintain();
                DialogPage.viewModel.EntityVM = vm;
                DialogPage.Dialog = this.Window.ShowDialog(ECCentral.Portal.UI.Customer.Resources.ResShipTypeSMSQuery.Title_Edit, DialogPage, (obj, args) =>
                {
                    if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                        dataGrid1.Bind();
                });
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("请先选中一条数据");
            }
        }

        #region 按钮权限控制
        private void CheckRights()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_ShipTypeSMS_Add))
                this.btnNew.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_ShipTypeSMS_Edit))
                this.btnEdit.IsEnabled = false;
        }
        #endregion
    }


}