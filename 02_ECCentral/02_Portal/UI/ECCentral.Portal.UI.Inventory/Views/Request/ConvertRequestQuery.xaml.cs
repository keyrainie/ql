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

using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.Basic.Components.UserControls.StockPicker;
using ECCentral.Portal.Basic.Converters;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class ConvertRequestQuery : PageBase
    {
        private ConvertRequestQueryView PageView;
        private ConvertRequestQueryFacade QueryFacade;
        #region 初始化加载

        public ConvertRequestQuery()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            PageView = new ConvertRequestQueryView();
            QueryFacade = new ConvertRequestQueryFacade(this);
            PageView.QueryInfo.CompanyCode = CPApplication.Current.CompanyCode;
            expanderCondition.DataContext = PageView.QueryInfo;
            dgRequestConvertQueryResult.DataContext = PageView;

            //创建转换单权限点
            btnNewRequest.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ConvertRequestQuery_NavigateCreate);  
        }

        #endregion

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.expanderCondition);
            if (PageView.QueryInfo.HasValidationErrors)
            {
                return;
            }
            dgRequestConvertQueryResult.Bind();
        }

        private void dgRequestConvertQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
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
            QueryFacade.QueryConvertRequest(PageView.QueryInfo, (totalCount, vmList) =>
            {
                PageView.TotalCount = totalCount;
                PageView.Result = vmList;
                dgRequestConvertQueryResult.ItemsSource = PageView.Result;
                dgRequestConvertQueryResult.TotalCount = PageView.TotalCount;
            });
        }

        private void btnNewRequest_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(ConstValue.Inventory_ConvertRequestMaintainCreateFormat, null, true);
        }

        private void hlbtnEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            Window.Navigate(String.Format(ConstValue.Inventory_ConvertRequestMaintainUrlFormat, btn.CommandParameter), null, true);
        }

        private void dgRequestConvertQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            //if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ConvertRequestQuery_ExportExcell))
            //{
            //    Window.Alert("对不起，你没有权限进行此操作！");
            //    return;
            //}

            if (this.dgRequestConvertQueryResult == null || this.dgRequestConvertQueryResult.TotalCount == 0)
            {
                Window.Alert("没有可供导出的数据!");
                return;
            }
            //导出全部:
            if (null != PageView.QueryInfo)
            {
                ConvertRequestQueryVM exportQueryRequest = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ConvertRequestQueryVM>(PageView.QueryInfo);
                exportQueryRequest.PagingInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };
                ColumnSet columnSet = new ColumnSet()
                .Add("RequestID", ResAdjustRequestQuery.Grid_AdjustID, 40)
                .Add("RequestStatus", ResAdjustRequestQuery.Grid_Status, 20)            
                .Add("CreateDate", ResAdjustRequestQuery.Grid_CreateTime, 30)
                .Add("AuditDate", ResAdjustRequestQuery.Grid_AuditTime, 30)
                .Add("OutStockDate", ResAdjustRequestQuery.Grid_OutTime, 30);
                new ConvertRequestQueryFacade(this).ExportExcelForConvertRequest(exportQueryRequest, new ColumnSet[] { columnSet });
            }
        }
    }
}
