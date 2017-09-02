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

using ECCentral.QueryFilter.Inventory;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class VirtualRequestAudit : PageBase
    {
        public string getVirtualRequestSysNo;
        public VirtualRequestQueryFacade serviceFacade;
        public VirtualRequestMaintainFacade serviceMaintainFacade;
        public VirtualRequestQueryFilter queryFilter;
        public VirtualRequestQueryFilter logQueryFilter;
        public VirtualRequestVM viewVM;

        public VirtualRequestAudit()
        {
            InitializeComponent();
            viewVM = new VirtualRequestVM();
            queryFilter = new VirtualRequestQueryFilter();
            logQueryFilter = new VirtualRequestQueryFilter();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            serviceFacade = new VirtualRequestQueryFacade(this);
            serviceMaintainFacade = new VirtualRequestMaintainFacade(this);
            getVirtualRequestSysNo = this.Request.Param;
            if (!string.IsNullOrEmpty(getVirtualRequestSysNo))
            {
                int virtualRequestSysNo = 0;
                if (!int.TryParse(getVirtualRequestSysNo, out virtualRequestSysNo))
                {
                    return;
                }
                LoadVirtualRequestInfo(virtualRequestSysNo);
                LoadVirtualRequestCloseLog(virtualRequestSysNo);
            }

            Button_Approve.IsEnabled = Button_Decline.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_VirtualRequestQuery_Audit);
        }

        private void LoadVirtualRequestCloseLog(int virtualRequestSysNo)
        {
            //加载虚库单关闭日志List:
            logQueryFilter.SysNo = virtualRequestSysNo;
            this.dgVirtualQtyChangeInfo.Bind();
        }

        private void LoadVirtualRequestInfo(int virtualRequestSysNo)
        {
            //加载虚库单信息:
            queryFilter.SysNo = virtualRequestSysNo;
            queryFilter.PagingInfo = new QueryFilter.Common.PagingInfo()
            {
                PageSize = 1,
                PageIndex = 0
            };
            serviceFacade.QueryVirtualRequest(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                int totalCount = args.Result.TotalCount;
                if (totalCount == 1)
                {
                    List<VirtualRequestVM> vmList = new List<VirtualRequestVM>();
                    vmList = DynamicConverter<VirtualRequestVM>.ConvertToVMList(args.Result.Rows);
                    viewVM = vmList[0];
                    this.DataContext = viewVM;

                    if (viewVM.RequestStatus != VirtualRequestStatus.Origin)
                    {
                        this.Button_Approve.Visibility = Visibility.Collapsed;
                        this.Button_Decline.Visibility = Visibility.Collapsed;
                        this.tbDeclineReason.IsReadOnly = true;
                    }

                    if (viewVM != null && viewVM.RequestStatus == VirtualRequestStatus.Origin)
                    {
                        queryFilter.ProductSysNo = viewVM.ProductSysNo;
                        queryFilter.StockSysNo = viewVM.StockSysNo;
                        queryFilter.SysNo = viewVM.SysNo;
                        serviceFacade.QueryNeedCloseRequestCount(queryFilter, (innerObj, innerArgs) =>
                         {
                             if (args.FaultsHandle())
                             {
                                 return;
                             }
                             if (innerArgs != null && innerArgs.Result != 0)
                             {
                                 txtMessage.Text = "提示：该商品已有有效状态的虚库设置，数量是" + innerArgs.Result + "，如果新纪录生效，当前记录将会被自动作废";
                             }
                             else
                             {
                                 txtMessage.Text = string.Empty;
                             }
                         });
                    }
                }

            });
        }

        private void dgVirtualQtyChangeInfo_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            logQueryFilter.PagingInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            serviceFacade.QueryVirtualRequestCloseLog(logQueryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                int totalCount = args.Result.TotalCount;
                var getList = args.Result.Rows;
                this.dgVirtualQtyChangeInfo.ItemsSource = getList;
                this.dgVirtualQtyChangeInfo.TotalCount = totalCount;
            });
        }

        private void Button_Approve_Click(object sender, RoutedEventArgs e)
        {
            //同意操作:
            Window.Confirm("确认要进行审核同意操作？", (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    serviceMaintainFacade.ApproveVirtualRequest(viewVM, (obj1, args1) =>
                    {
                        if (args1.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert("审核成功!");
                        Window.Refresh();
                    });
                }
            });
        }

        private void Button_Decline_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.tbDeclineReason.Text))
            {
                Window.Alert("拒绝理由不能为空!");
                return;
            }
            //拒绝操作 :
            Window.Confirm("确认要进行审核拒绝操作？", (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    serviceMaintainFacade.RejectVirtualRequest(viewVM, (obj1, args1) =>
                    {
                        if (args1.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert("拒绝成功!");
                        Window.Refresh();
                    });
                }
            });
        }

    }
}
