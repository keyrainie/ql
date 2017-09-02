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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Utilities.Validation;

using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.UserControls;
using ECCentral.QueryFilter.Inventory;
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.UI.Inventory.Resources;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class ShiftRequestMemoQuery : PageBase
    {
        public ShiftRequestQueryFacade QueryFacade;
        public ShiftRequestMemoQueryView PageView;        

        public ShiftRequestMemoQuery()
        {
            InitializeComponent();           
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            QueryFacade = new ShiftRequestQueryFacade(this);
            PageView = new ShiftRequestMemoQueryView();
            PageView.QueryInfo.CompanyCode = CPApplication.Current.CompanyCode;
            PageView.QueryInfo.MemoStatus = ShiftRequestMemoStatus.FollowUp;
            expanderCondition.DataContext = PageView.QueryInfo;
            dgShiftRequestMemoQueryResult.DataContext = PageView;            
        }       

        private void dgShiftRequestMemoQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            PageView.QueryInfo.PagingInfo = new QueryFilter.Common.PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            QueryFacade.QueryShiftRequestMemo(PageView.QueryInfo, (totalCount, vmList) =>
            {
                PageView.TotalCount = totalCount;
                PageView.Result = vmList;               
                dgShiftRequestMemoQueryResult.ItemsSource = PageView.Result;
                dgShiftRequestMemoQueryResult.TotalCount = PageView.TotalCount;
            });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {            
            ValidationManager.Validate(this.expanderCondition);
            if (PageView.QueryInfo.HasValidationErrors)
            {
                return;
            }
            this.dgShiftRequestMemoQueryResult.Bind();
        }

        private void hyperlinkMaintainMemo_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            Window.Navigate(String.Format(ConstValue.Inventory_ShiftRequestMemoMaintainUrlFormat, btn.CommandParameter), null, true);
        }

        private void hyperlinkRequestInfo_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            Window.Navigate(String.Format(ConstValue.Inventory_ShiftRequestMaintainUrlFormat, btn.CommandParameter), null, true);
        }

        private void dgShiftRequestMemoQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequestMemoQuery_ExportExcell))
            {
                Window.Alert("对不起，你没有权限进行此操作！");
                return;
            }

            if (this.dgShiftRequestMemoQueryResult == null || this.dgShiftRequestMemoQueryResult.TotalCount == 0)
            {
                Window.Alert("没有可供导出的数据!");
                return;
            }
            //导出全部:
            if (null != PageView.QueryInfo)
            {              
                ShiftRequestMemoQueryVM exportQueryRequest = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ShiftRequestMemoQueryVM>(PageView.QueryInfo);
                exportQueryRequest.PagingInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };
                ColumnSet columnSet = new ColumnSet()
                .Add("RequestSysNo", ResShiftRequestMemoMaintain.Grid_ShiftRequestSysNo, 40)
                .Add("Content", ResShiftRequestMemoMaintain.Grid_MemoContent, 40)
                .Add("MemoStatus", ResShiftRequestMemoMaintain.Grid_MemoStatus, 40)
                .Add("CreateUserName", ResShiftRequestMemoMaintain.Grid_CreateUser, 40)
                .Add("CreateDate", ResShiftRequestMemoMaintain.Grid_CreateDate, 40)
                .Add("EditDate", ResShiftRequestMemoMaintain.Grid_CloseTime, 40)
                .Add("RemindTime", ResShiftRequestMemoMaintain.Grid_RemindTime, 40)
                .Add("Note", ResShiftRequestMemoMaintain.Grid_MemoNote, 40);
                new ShiftRequestQueryFacade(this).ExportRequestMemo(exportQueryRequest, new ColumnSet[] { columnSet });
            }
        }

    }
}
