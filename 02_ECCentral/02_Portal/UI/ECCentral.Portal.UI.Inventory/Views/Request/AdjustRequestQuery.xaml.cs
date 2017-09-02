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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true)]
    public partial class AdjustRequestQuery : PageBase
    {
        AdjustRequestQueryView PageView;
        AdjustRequestQueryFacade QueryFacade; 
        public AdjustRequestQuery()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            QueryFacade = new AdjustRequestQueryFacade(this);
            PageView = new AdjustRequestQueryView();
            PageView.QueryInfo.CompanyCode = CPApplication.Current.CompanyCode;
            expanderCondition.DataContext = PageView.QueryInfo;
            dgAdjustQueryResult.DataContext = PageView;
            //创建损益单权限点
            btnAdjustRequestNew.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_AdjustRequestQuery_NavigateCreate);             
        }

        private void btnAdjustRequestSearch_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.expanderCondition);
            if (PageView.QueryInfo.HasValidationErrors)
            {
                return;
            }
            dgAdjustQueryResult.Bind();
        }

        private void dgAdjustRequestQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            PageView.QueryInfo.PagingInfo = new QueryFilter.Common.PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            PageView.QueryInfo.UserSysNo = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.UserSysNo;
            if (AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_SeniorPM_Query))
            {
                PageView.QueryInfo.PMQueryRightType = BizEntity.Common.PMQueryType.AllValid;
            }
            else if (AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_IntermediatePM_Query))
            {
                PageView.QueryInfo.PMQueryRightType = BizEntity.Common.PMQueryType.Team;
            }
            else if (AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_JuniorPM_Query))
            {
                PageView.QueryInfo.PMQueryRightType = BizEntity.Common.PMQueryType.Self;
            }
            else
            {
                PageView.QueryInfo.PMQueryRightType = null;
            }
            QueryFacade.QueryAdjustRequest(PageView.QueryInfo, (totalCount, vmList) =>
            {
                PageView.TotalCount = totalCount;
                PageView.Result = vmList;
                this.dgAdjustQueryResult.ItemsSource = PageView.Result;
                this.dgAdjustQueryResult.TotalCount = PageView.TotalCount;
            });
        }

        private void btnAdjustRequestNew_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(ConstValue.Inventory_AdjustRequestMaintainCreateFormat, null, true);
        }

        private void hyperlinkAdjustRequestID_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            Window.Navigate(String.Format(ConstValue.Inventory_AdjustRequestMaintainUrlFormat, btn.CommandParameter), null, true);
        }

        private void dgAdjustQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            //if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_AdjustRequestQuery_ExportExcell))
            //{
            //    Window.Alert("对不起，你没有权限进行此操作！");
            //    return;
            //}

            if (this.dgAdjustQueryResult == null || this.dgAdjustQueryResult.TotalCount == 0)
            {
                Window.Alert("没有可供导出的数据!");
                return;
            }
            //导出全部:
            if (null != PageView.QueryInfo)
            {
                AdjustRequestQueryVM exportQueryRequest = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<AdjustRequestQueryVM>(PageView.QueryInfo);
                exportQueryRequest.PagingInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };
                ColumnSet columnSet = new ColumnSet()
                .Add("RequestID", ResAdjustRequestQuery.Grid_AdjustID, 40)
                .Add("RequestStatus", ResAdjustRequestQuery.Grid_Status, 20)
                .Add("ConsignFlag", ResAdjustRequestQuery.Grid_ConsignFlag, 40)
                .Add("CreateDate", ResAdjustRequestQuery.Grid_CreateTime, 30)
                .Add("AuditDate", ResAdjustRequestQuery.Grid_AuditTime, 30)
                .Add("OutStockDate", ResAdjustRequestQuery.Grid_OutTime, 30);
                new AdjustRequestQueryFacade(this).ExportExcelForAdjustRequest(exportQueryRequest, new ColumnSet[] { columnSet });
            }
        }

    }

}
