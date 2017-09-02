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

using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.UserControls;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.QueryFilter.Inventory;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View]
    public partial class VirtualRequestMemoQuery : PageBase
    {
        public VirtualRequestQueryFacade QueryFacade;
        public VirtualRequestQueryView PageView;
        //public VirtualRequestQueryFilter QueryFilter;

        public VirtualRequestMemoQuery()
        {
            InitializeComponent();            
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);            
            PageView = new VirtualRequestQueryView();
            QueryFacade = new VirtualRequestQueryFacade(this);
            PageView.QueryInfo.CompanyCode = CPApplication.Current.CompanyCode;
            expanderCondition.DataContext = PageView.QueryInfo;
            dgVirtualRequestMemoQueryResult.DataContext = PageView;            
            BindComboBoxData();           
        }

        private void BindComboBoxData()
        {            
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

           QueryFacade.QueryVirtualRequestMemoCreateUserList((totalCount, vmList) =>
           {
               vmList.Insert(0, new UserInfoVM()
               {
                   SysNo = null,
                   UserDisplayName = ResInventoryCommon.ComboItem_All
               });

               PageView.QueryInfo.CreateUserList = vmList;                               
           });
        }
   
        private void dgVirtualRequestMemoQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            PageView.QueryInfo.PagingInfo = new PagingInfo()
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };

            if (!PageView.QueryInfo.ProductSysNo.HasValue 
                || !PageView.QueryInfo.StartDate.HasValue)
            {
                Window.Alert("商品编号和申请时间不能为空！");
                return;
            }


            QueryFacade.QueryVirtualRequestMemo(PageView.QueryInfo, (totalCount, vmList) =>
            {
                PageView.TotalCount = totalCount;
                PageView.Result = vmList;
                dgVirtualRequestMemoQueryResult.ItemsSource = PageView.Result;
                dgVirtualRequestMemoQueryResult.TotalCount = PageView.TotalCount;
            });           
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //查询:
            //queryFilter = EntityConverter<VirtualRequestQueryVM, VirtualRequestQueryFilter>.Convert(queryVM);
            this.dgVirtualRequestMemoQueryResult.Bind();
        }

        private void dgVirtualRequestMemoQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_VirtualRequestMemoQuery_ExportExcell))
            {
                Window.Alert("对不起，你没有权限进行此操作！");
                return;
            }

            if (this.dgVirtualRequestMemoQueryResult == null || this.dgVirtualRequestMemoQueryResult.TotalCount == 0)
            {
                Window.Alert("没有可供导出的数据!");
                return;
            }
            //导出全部:
            if (null != PageView.QueryInfo)
            {
                VirtualRequestQueryVM exportQueryRequest = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<VirtualRequestQueryVM>(PageView.QueryInfo);
                exportQueryRequest.PagingInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };
                ColumnSet columnSet = new ColumnSet()
                .Add("ProductID", ResVirtualRequestMemoQuery.Grid_ProductID, 40)
                .Add("ProductName", ResVirtualRequestMemoQuery.Grid_ProductName, 40)
                .Add("VirtualQuantity", ResVirtualRequestMemoQuery.Grid_AdjustVirtualQty, 40)
                .Add("VirtualType", ResVirtualRequestMemoQuery.Grid_VirtualType, 40)
                .Add("CreateUserName", ResVirtualRequestMemoQuery.Grid_OperationUser, 40)
                .Add("CreateDate", ResVirtualRequestMemoQuery.Grid_OperationDate, 30)
                .Add("Note", ResVirtualRequestMemoQuery.Grid_ActionMemo, 30);
                new VirtualRequestQueryFacade(this).ExportExcelForVirtualRequestMemo(exportQueryRequest, new ColumnSet[] { columnSet });
            }
        }

    }
}
