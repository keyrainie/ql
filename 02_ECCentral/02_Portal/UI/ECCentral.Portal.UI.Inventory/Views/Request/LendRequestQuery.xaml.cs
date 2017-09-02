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

using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.Basic.Components.UserControls.StockPicker;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class LendRequestQuery : PageBase
    {
        #region 属性                      
        
        LendRequestQueryView PageView;
        LendRequestQueryFacade QueryFacade;

        #endregion 属性

        #region 初始化加载

        public LendRequestQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            QueryFacade = new LendRequestQueryFacade(this);
            PageView = new LendRequestQueryView();
            PageView.QueryInfo.CompanyCode = CPApplication.Current.CompanyCode;
            this.SearchBuilder.DataContext = PageView.QueryInfo;
            dgLendRequestQueryResult.DataContext = PageView;
            svStatisticInfo.Visibility = Visibility.Collapsed;
            //创建借货单权限点        
            btnNewRequest.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_LendRequestQuery_NavigateCreate);
            btnStatByPM.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_LendRequestQuery_ExportExcell);
        }   

        #endregion 初始化加载

        #region 查询绑定

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.SearchBuilder);
            if (PageView.QueryInfo.HasValidationErrors)
            {
                return;
            }
            this.dgLendRequestQueryResult.Bind();            
        }   
 
        private void dgLendRequestQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
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
            QueryFacade.QueryLendRequest(PageView.QueryInfo, (totalCount, vmList) =>
            {
                PageView.TotalCount = totalCount;
                PageView.Result = vmList;
                this.dgLendRequestQueryResult.ItemsSource = PageView.Result;
                this.dgLendRequestQueryResult.TotalCount = PageView.TotalCount;

                #region 当前条件下总计
                QueryFacade.QueryLendCostbyStatus(PageView.QueryInfo,allCostInfo =>
                {                    
                    decimal AllPageOriginalCostTotal = 0.00M;
                    decimal AllPageAbandonCostTotal = 0.00M;
                    if (allCostInfo.Count>0)
                    {

                        var tempAllPageOriginalCostTotalEntity = allCostInfo.SingleOrDefault(x => { return x.Status == 1; });
                        var tempAllPageAbandonCostTotalEntity = allCostInfo.SingleOrDefault(x => { return x.Status == -1; });
                        if (tempAllPageOriginalCostTotalEntity!=null)
                        {
                            AllPageOriginalCostTotal = Math.Round(tempAllPageOriginalCostTotalEntity.Amount, 2);
                        }
                        if (tempAllPageAbandonCostTotalEntity != null)
                        {
                            AllPageAbandonCostTotal = Math.Round(tempAllPageAbandonCostTotalEntity.Amount, 2);
                        }                        
                    }                    
                    tbStatisticAllCostInfo.Text = string.Format("当前条件下总计：初始/作废 状态下的借货单成本金额总计分别是: 初始: ￥{0} 作废: ￥{1}。", AllPageOriginalCostTotal, AllPageAbandonCostTotal);
                });
                #endregion

                #region 当前页统计成本

                svStatisticInfo.Visibility = Visibility.Visible;
                decimal currentPageOriginalCostTotal = 0.00M;
                decimal currentPageAbandonCostTotal = 0.00M;
                if (vmList!=null&&vmList.Count>0)
                {
                    var currentPageOriginalInfoList = vmList.ToList().Where(x => { return x.RequestStatus == LendRequestStatus.Origin; });
                    foreach (var item in currentPageOriginalInfoList)
                    {
                        currentPageOriginalCostTotal += item.LendTotalCost;
                    }

                    var currentPageAbandonList = vmList.ToList().Where(x => { return x.RequestStatus == LendRequestStatus.Abandon; });
                    foreach (var item in currentPageAbandonList)
                    {
                        currentPageAbandonCostTotal += item.LendTotalCost;
                    }
                }               
                tbStatisticInfo.Text = string.Format("当前页中      ：初始/作废 状态下的借货单成本金额总计分别是: 初始: ￥{0} 作废: ￥{1}。", currentPageOriginalCostTotal, currentPageAbandonCostTotal);

                #endregion                
            });
        }

        #endregion  查询绑定

        #region 页面内按钮处理事件

        #region 界面事件   

        private void btnStatByPM_Click(object sender, RoutedEventArgs e)
        {        
            if (this.dgLendRequestQueryResult==null||this.dgLendRequestQueryResult.TotalCount == 0)
            {
                 Window.Alert("没有可供导出的数据!");
                return;
            }
            //按照PM导出全部:
            if (null != PageView.QueryInfo)
            {
                LendRequestQueryVM exportQueryRequest = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<LendRequestQueryVM>(PageView.QueryInfo);
                exportQueryRequest.PagingInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit , SortBy=""};
                ColumnSet columnSet = new ColumnSet()
                .Add("ProductCode", ResLendRequestQuery.Grid_ProductID, 40)
                .Add("ProductName", ResLendRequestQuery.Grid_ProductName, 20)
                .Add("LendCode", ResLendRequestQuery.Grid_RequestID, 40)
                .Add("LendUserName", ResLendRequestQuery.Grid_RequestLendUser, 40)
                .Add("LendTime",ResLendRequestQuery.Grid_RequestCreateDate, 30)
                .Add("LendQty", ResLendRequestQuery.Grid_LendQty, 30)
                .Add("LendCost", ResLendRequestQuery.Grid_LendUnitCost, 30)
                .Add("TotalLendCost", ResLendRequestQuery.Grid_LendUnitCostAll, 30)
                .Add("PMName", ResLendRequestQuery.Grid_PMName, 40);
                new LendRequestQueryFacade(this).ExportExcelForLendRequestByPM(exportQueryRequest, new ColumnSet[] { columnSet });
            }
        }

        private void dgLendRequestQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_LendRequestQuery_ExportExcell))
            {
                Window.Alert("对不起，你没有权限进行此操作！");
                return;
            }

            if (this.dgLendRequestQueryResult == null || this.dgLendRequestQueryResult.TotalCount == 0)
            {
                Window.Alert("没有可供导出的数据!");
                return;
            }
            //导出全部:
            if (null != PageView.QueryInfo)
            {
                LendRequestQueryVM exportQueryRequest = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<LendRequestQueryVM>(PageView.QueryInfo);
                exportQueryRequest.PagingInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };
                ColumnSet columnSet = new ColumnSet()
                .Add("RequestID", ResLendRequestQuery.Grid_RequestID, 40)
                .Add("RequestStatus", ResLendRequestQuery.Grid_RequestStatus, 20)
                .Add("LendUserName", ResLendRequestQuery.Grid_RequestLendUser, 40)
                .Add("LendTotalCost", ResLendRequestQuery.Grid_LendUnitCost, 30)
                .Add("LendTotalAmount", ResLendRequestQuery.Grid_LendTotalAmount, 30)
                .Add("CreateDate", ResLendRequestQuery.Grid_RequestCreateDate, 30)
                .Add("AuditDate", ResLendRequestQuery.Grid_RequestAuditDate, 30)
                .Add("OutStockDate", ResLendRequestQuery.Grid_RequestOutStockDate, 30);
                new LendRequestQueryFacade(this).ExportExcelForLendRequest(exportQueryRequest, new ColumnSet[] { columnSet });
            }
        }  

        #endregion

        #endregion

        #region 跳转    

        private void hyperlinkRequestID_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            Window.Navigate(String.Format(ConstValue.Inventory_LendRequestMaintainUrlFormat, btn.CommandParameter), null, true);
        }

        private void btnNewRequest_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(ConstValue.Inventory_LendRequestMaintainCreateFormat, null, true);
        }

        #endregion 跳转
     
    }
}
