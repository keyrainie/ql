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
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls.Components;

using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.UI.Inventory.UserControls;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class WarehouseOwnerQuery : PageBase
    {

        #region 属性
        WarehouseOwnerQueryView PageView;
        #endregion

        #region 初始化加载

        public WarehouseOwnerQuery()
        {
            InitializeComponent();
        }  

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            PageView = new WarehouseOwnerQueryView();
            PageView.QueryInfo.CompanyCode = CPApplication.Current.CompanyCode;
            conditionExpander.DataContext = PageView.QueryInfo;
            dgWarehouseOwnerQueryResult.DataContext = PageView;
            btnOwnerNew.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_WarehouseOwnerQuery_OwnerNew);
            cmbOwnerTypeList.SelectedIndex = 1;
        }

        #endregion

        #region 查询绑定

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dgWarehouseOwnerQueryResult.Bind();
        }

        private void dgWarehouseOwnerQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            WarehouseOwnerQueryFacade facade = new WarehouseOwnerQueryFacade(this);

            PageView.QueryInfo.PagingInfo = new QueryFilter.Common.PagingInfo
             {
                 PageIndex = e.PageIndex,
                 PageSize = e.PageSize,
                 SortBy = e.SortField
             };

            facade.QueryWarehouseOwner(PageView.QueryInfo, (obj, args) =>
             {
                 if (args.Result != null && args.Result.Rows != null)
                 {
                     PageView.Result = args.Result.Rows.ToList(null);
                     PageView.TotalCount = args.Result.TotalCount;
                     this.dgWarehouseOwnerQueryResult.ItemsSource = PageView.Result;
                 }
             });
        }
      
        #endregion        

        #region 跳转
  
        //编辑仓库所有者
        private void hyperlinkOwnerSysNo_Click(object sender, RoutedEventArgs e)
        {
            dynamic owner = (sender as HyperlinkButton).DataContext;
            if (owner != null)
            {
                Maintain(owner.OwnerSysNo);
            }
        }

        //新建仓库所有者         
        private void btnOwnerNew_Click(object sender, RoutedEventArgs e)
        {
            Maintain(null);
        }

        private void Maintain(int? sysNo)
        {
            WarehouseOwnerMaintain content = new WarehouseOwnerMaintain()
            {
                Page = this,
                OwnerSysNo = sysNo
            };
            content.Saved += (o, e) =>
            {
                dgWarehouseOwnerQueryResult.Bind();
            };
            IDialog dialog = this.Window.ShowDialog(ResWarehouseOwnerMaintain.Expander_BasicInfo, content, (obj, args) =>
            {
            });
            content.Dialog = dialog;
        }
                   
        #endregion

        private void dgWarehouseOwnerQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_WarehouseOwnerQuery_ExportExcell))
            {
                Window.Alert("对不起，你没有权限进行此操作！");
                return;
            }

            if (this.dgWarehouseOwnerQueryResult == null || this.dgWarehouseOwnerQueryResult.TotalCount == 0)
            {
                Window.Alert("没有可供导出的数据!");
                return;
            }
            //导出全部:
            if (null != PageView.QueryInfo)
            {                
                PageView.QueryInfo.PagingInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };
                ColumnSet columnSet = new ColumnSet(dgWarehouseOwnerQueryResult);
                new WarehouseOwnerQueryFacade(this).ExportExcelForWarehouseOwnerQuery(PageView.QueryInfo, new ColumnSet[] { columnSet });
            }
        }
    }
}
