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
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.UserControls;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.QueryFilter.Inventory;
using ECCentral.BizEntity.Inventory;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View]
    public partial class VirtualRequestQuery : PageBase
    {
        public VirtualRequestQueryFacade serviceFacade;
        public VirtualRequestQueryVM queryVM;
        public VirtualRequestQueryFilter queryFilter;

        public VirtualRequestQuery()
        {
            InitializeComponent();
            queryVM = new VirtualRequestQueryVM();
            queryFilter = new VirtualRequestQueryFilter();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            BindComboBoxData();
            serviceFacade = new VirtualRequestQueryFacade(this);
            this.DataContext = queryVM;
            serviceFacade.QueryVirtualRequestCreateUserList((totalCount, vmList) =>
            {
                vmList.Insert(0, new UserInfoVM()
                {
                    SysNo = null,
                    UserDisplayName = ResInventoryCommon.ComboItem_All
                });

                queryVM.CreateUserList = vmList;
            });

            btnCreateRequestSingle.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_VirtualRequestQuery_OperateVirtualRequestMaintain);
            btnCreateRequestBatch.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_VirtualRequestQuery_OperateVirtualRequestMaintainBatch);
        }

        private void BindComboBoxData()
        {
            // 虚库状态:
            this.cmbRequestStatusList.ItemsSource = EnumConverter.GetKeyValuePairs<VirtualRequestStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbRequestStatusList.SelectedIndex = 0;
            // 虚库类型：
            CodeNamePairHelper.GetList("Inventory", "VirtualRequestType", CodeNamePairAppendItemType.All, (obj, args) =>
           {
               if (args.FaultsHandle())
               {
                   return;
               }
               this.cmbVirtualTypeList.ItemsSource = args.Result;
               this.cmbVirtualTypeList.SelectedIndex = 0;
           });
        }

        private void btnCreateRequestSingle_Click(object sender, RoutedEventArgs e)
        {
            //单个申请虚库:
            Window.Navigate(ConstValue.Inventory_VirtualRequestMaintainCreate, null, true);
        }

        private void btnCreateRequestBatch_Click(object sender, RoutedEventArgs e)
        {
            //批量申请虚库:
            Window.Navigate(ConstValue.Inventory_VirtualRequestMaintainBatchCreateFormat, null, true);
        }

        private void btnApproveRequestBatch_Click(object sender, RoutedEventArgs e)
        {
            //批量同意虚库
            var requestList = new List<VirtualRequestVM>();
            var vmList = this.dgVirtualRequestQueryResult.ItemsSource as dynamic;
            if (vmList == null) return;
            foreach (var item in vmList)
            {
                if (item.IsChecked)
                {
                    requestList.Add(DynamicConverter<VirtualRequestVM>.ConvertToVM(item));
                }
            }

            if (requestList.Count == 0)
            {
                Window.Alert("请选择至少选择一条数据！");
                return;
            }
            else
            {
                serviceFacade.BatchApproveVirtualRequest(requestList, (msg) =>
                {
                    Window.Alert("提示", msg, MessageType.Information, (obj, args) =>
                    {
                        this.dgVirtualRequestQueryResult.Bind();
                    });
                });
            }
        }

        private void hyperlinkOperationView_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml getSelectedItem = this.dgVirtualRequestQueryResult.SelectedItem as DynamicXml;
            if (null != getSelectedItem)
            {
                //查询Item:
                Window.Navigate(string.Format("/ECCentral.Portal.UI.Inventory/VirtualRequestAudit/{0}", getSelectedItem["SysNo"].ToString()), null, true);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //查询:
            queryFilter = EntityConverter<VirtualRequestQueryVM, VirtualRequestQueryFilter>.Convert(queryVM);
            this.dgVirtualRequestQueryResult.Bind();
        }

        private void dgVirtualRequestQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            queryFilter.PagingInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };

            //if (!queryFilter.ProductSysNo.HasValue
            //    || !queryFilter.StartDate.HasValue)
            //{
            //    Window.Alert("商品编号和申请时间不能为空！");
            //    return;
            //}

            serviceFacade.QueryVirtualRequest(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var getList = args.Result.Rows.ToList("IsChecked", false);
                int totalCount = args.Result.TotalCount;
                this.dgVirtualRequestQueryResult.ItemsSource = getList;
                this.dgVirtualRequestQueryResult.TotalCount = totalCount;
            });
        }

        private void dgVirtualRequestQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_VirtualRequestQuery_ExportExcell))
            {
                Window.Alert("对不起，你没有权限进行此操作！");
                return;
            }

            if (this.dgVirtualRequestQueryResult == null || this.dgVirtualRequestQueryResult.TotalCount == 0)
            {
                Window.Alert("没有可供导出的数据!");
                return;
            }
            queryFilter.PagingInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };
            ColumnSet columnSet = new ColumnSet(dgVirtualRequestQueryResult);
            new VirtualRequestQueryFacade(this).ExportExcelForVirtualRequest(queryFilter, new ColumnSet[] { columnSet });
        }
    }
}
