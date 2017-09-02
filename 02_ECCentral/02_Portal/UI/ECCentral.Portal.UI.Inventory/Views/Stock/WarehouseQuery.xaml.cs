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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class WarehouseQuery : PageBase
    {
        #region 属性
        WarehouseQueryView PageView;
        #endregion

        #region 初始化加载

        public WarehouseQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            PageView = new WarehouseQueryView();
            PageView.QueryInfo.CompanyCode = CPApplication.Current.CompanyCode;
            base.OnPageLoad(sender, e);
            spConditions.DataContext = PageView.QueryInfo;
            dgWarehouseQueryResult.DataContext = PageView;

            //new WarehouseOwnerQueryFacade().GetWarehouseOwnerByCompanyCode(CPApplication.Current.CompanyCode, (ownerList) =>
            //{
            //    ownerList.Insert(0, new WarehouseOwnerInfoVM
            //    {
            //        SysNo = null,
            //        OwnerName = ECCentral.BizEntity.Enum.Resources.ResCommonEnum.Enum_All
            //    });
            //    PageView.QueryInfo.OwnerList = ownerList;
            //});
            btnNew.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_WarehouseQuery_New);
            btnRefresh.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_WarehouseQuery_Refresh);
            //cmbWarehouseTypeList.SelectedIndex = 1;//中蛋定制化 只获取中蛋的信息
        }

        #endregion

        #region 查询绑定

        private void btnWarehouseSearch_Click(object sender, RoutedEventArgs e)
        {
            dgWarehouseQueryResult.Bind();
        }

        private void dgWarehouseQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            WarehouseQueryFacade facade = new WarehouseQueryFacade(this);

            PageView.QueryInfo.PageInfo = new QueryFilter.Common.PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            facade.QueryWarehouse(PageView.QueryInfo, (totalCount, queryResult) =>
            {
                PageView.TotalCount = totalCount;
                PageView.Result = queryResult;
                this.dgWarehouseQueryResult.ItemsSource = PageView.Result;
            });
        }

        #endregion

        #region 跳转

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {

        }

        //编辑仓库
        private void hyperlinkWarehouseID_Click(object sender, RoutedEventArgs e)
        {
            WarehouseInfoVM warehouse = (sender as HyperlinkButton).DataContext as WarehouseInfoVM;
            if (warehouse != null)
            {
                Maintain(warehouse.SysNo);
            }
        }

        //新建仓库
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Maintain(null);
        }

        private void Maintain(int? sysNo)
        {
            WarehouseMaintain content = new WarehouseMaintain()
            {
                Page = this,
                WarehouseSysNo = sysNo
            };
            content.Saved += (o, e) =>
            {
                dgWarehouseQueryResult.Bind();
            };
            IDialog dialog = this.Window.ShowDialog(sysNo.HasValue ? String.Format(ResWarehouseQuery.UC_Title_WH_Edit, sysNo) : ResWarehouseQuery.UC_Title_WH_Create, content, (obj, args) =>
            {

            }, new Size(660,500));
            content.Dialog = dialog;
        }

        #endregion

        private void dgWarehouseQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_WarehouseQuery_ExportExcell))
            {
                Window.Alert("对不起，你没有权限进行此操作！");
                return;
            }

            if (this.dgWarehouseQueryResult == null || this.dgWarehouseQueryResult.TotalCount == 0)
            {
                Window.Alert("没有可供导出的数据!");
                return;
            }
            //导出全部:
            if (null != PageView.QueryInfo)
            {                
                PageView.QueryInfo.PageInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };
                ColumnSet columnSet = new ColumnSet(dgWarehouseQueryResult);
                new WarehouseQueryFacade(this).ExportExcelForWarehouseQuery(PageView.QueryInfo, new ColumnSet[] { columnSet });
            }
        }
    }
}
