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
using ECCentral.QueryFilter.Customer;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.UserControls;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Customer.Views.CSToolBox
{
    [View]
    public partial class CSSet : PageBase
    {
        public CSQueryFilter queryRequest;
        public CSSetVM vm;
        CSFacade facade;
        public CSSet()
        {
            InitializeComponent();
            userName.KeyDown += new KeyEventHandler(userName_KeyDown);
        }

        void userName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Search();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            queryRequest = new CSQueryFilter();
            vm = new CSSetVM();
            this.DataContext = vm;
            facade = new CSFacade(this);
            queryRequest.PagingInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = CSGrid.PageSize,
                PageIndex = 0,
                SortBy = string.Empty
            };
            vm.RoleList = EnumConverter.GetKeyValuePairs<CSRole>(EnumConverter.EnumAppendItemType.All);
            Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn role = this.CSGrid.Columns[1] as Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn;
            role.Binding.ConverterParameter = typeof(CSRole);
            base.OnPageLoad(sender, e);
            CheckRights();
        }
        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryRequest.Name = vm.Name;
            queryRequest.Role = vm.Role;
            queryRequest.IsGetUnderling = vm.IsGetUnderling;
            queryRequest.PagingInfo.SortBy = e.SortField;
            queryRequest.PagingInfo.PageSize = e.PageSize;
            queryRequest.PagingInfo.PageIndex = e.PageIndex;
            facade.Query(queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<CSVM> list = DynamicConverter<CSVM>.ConvertToVMList(args.Result.Rows);
                if (list != null)
                {
                    CSGrid.ItemsSource = list;
                    CSGrid.TotalCount = args.Result.TotalCount;
                }
            });
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            CSMaintain csmCrl = new CSMaintain();
            csmCrl.Dialog = Window.ShowDialog(ECCentral.Portal.UI.Customer.Resources.ResCSSet.Dialog_AddCS, csmCrl, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    CSGrid.Bind();
                }
            }, new Size(500, 287));
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (CSGrid.SelectedItem != null)
            {
                CSMaintain csmCrl = new CSMaintain();
                csmCrl.Dialog = Window.ShowDialog(ECCentral.Portal.UI.Customer.Resources.ResCSSet.Dialog_EditCS, csmCrl, (s, args) =>
                {
                    if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                    {
                        CSGrid.Bind();
                    }
                }, new Size(500, 287));
                CSMaintainVM maintainvm = new CSMaintainVM();
                maintainvm.csvm = CSGrid.SelectedItem as CSVM;
                csmCrl.viewModel = maintainvm;
                csmCrl.IsAdd = false;
            }
            else
            {
                Window.Alert("请选中一条数据");
            }
        }
        private void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }
        private void Search()
        {
            vm.IsGetUnderling = false;
            CSGrid.Bind();
        }
        private void hlbGetUnderlings_Click(object sender, RoutedEventArgs e)
        {
            CSVM entityvm = CSGrid.SelectedItem as CSVM;
            vm.Name = entityvm.UserName;
            vm.Role = entityvm.Role;
            vm.IsGetUnderling = true;
            CSGrid.Bind();
        }

        #region 按钮权限控制
        private void CheckRights()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CSSet_Add))
                this.btnNew.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CSSet_Edit))
                this.btnUpdate.IsEnabled = false;
        }
        #endregion
    }

}
