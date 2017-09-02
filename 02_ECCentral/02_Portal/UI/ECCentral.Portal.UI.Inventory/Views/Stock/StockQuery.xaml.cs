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
using Newegg.Oversea.Silverlight.Utilities.Validation;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.QueryFilter.Inventory;

using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Components.Models;

using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.UI.Inventory.UserControls;

//中蛋不存在多渠道 所以此页面不用
namespace ECCentral.Portal.UI.Inventory.Views
{    
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class StockQuery : PageBase
    {

        #region 属性
        StockQueryView PageView; 
        #endregion

        #region 初始化加载

        public StockQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            PageView = new StockQueryView();
            PageView.QueryInfo.CompanyCode = CPApplication.Current.CompanyCode;
            conditionExpander.DataContext = PageView.QueryInfo;
            dgStockQueryResult.DataContext = PageView;
            InitControl();
            btnStockNew.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_StockQuery_New);
        }

        /// <summary>
        /// 初始化控件状态
        /// </summary>
        private void InitControl()
        {
            LoadWarehouseList();
            txtSotckSysNo.Focus();
        }


        public void LoadWarehouseList()
        {
            WarehouseQueryFacade whFacade = new WarehouseQueryFacade();
            whFacade.GetWarehouseListByCompanyCode(CPApplication.Current.CompanyCode, (vmList) =>
            {
                WarehouseInfoVM blankInfo = new WarehouseInfoVM()
                {
                    SysNo = null,
                    WarehouseID = null,
                    WarehouseName = ResCommonEnum.Enum_All
                };
                vmList.Insert(0, blankInfo);
                PageView.QueryInfo.WarehouseList = vmList;

            });

        }

        #endregion

        #region 查询绑定
        private void btnStockSearch_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(conditionExpander);
            if (PageView.QueryInfo.HasValidationErrors)
            {
                return;
            }
            dgStockQueryResult.Bind();
        }

        private void dgStockQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            StockQueryFacade facade = new StockQueryFacade(this);

            PageView.QueryInfo.PagingInfo = new QueryFilter.Common.PagingInfo
             {
                 PageIndex = e.PageIndex,
                 PageSize = e.PageSize,
                 SortBy = e.SortField
             };

            facade.QueryStock(PageView.QueryInfo, (obj, args) =>
             {
                 if (args.Result != null && args.Result.Rows != null)
                 {
                     PageView.Result = args.Result.Rows.ToList(null);
                     PageView.TotalCount = args.Result.TotalCount;
                     this.dgStockQueryResult.ItemsSource = PageView.Result;
                 }
             });
        }
        #endregion

        #region 跳转

        //编辑渠道仓库
        private void hyperlinkStockID_Click(object sender, RoutedEventArgs e)
        {
            dynamic stock = (sender as HyperlinkButton).DataContext;
            if (stock != null)
            {
                Maintain(stock.SysNo);
            }
        }

        //新建渠道仓库
        private void btnStockNew_Click(object sender, RoutedEventArgs e)
        {
            Maintain(null);
        }

        private void Maintain(int? sysNo)
        {
            StockMaintain content = new StockMaintain()
            {
                Page = this,
                StockSysNo = sysNo
            };
            content.Saved += (o, e) =>
            {
                dgStockQueryResult.Bind();
            };
            IDialog dialog = this.Window.ShowDialog(sysNo.HasValue ? String.Format(ResStockQuery.UC_Title_Edit, sysNo) : ResStockQuery.UC_Title_Create, content, (obj, args) =>
            {
            });
            content.Dialog = dialog;
        }

        #endregion

        private void dgStockQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_StockQuery_ExportExcell))
            {
                Window.Alert("对不起，你没有权限进行此操作！");
                return;
            }

            if (this.dgStockQueryResult == null || this.dgStockQueryResult.TotalCount == 0)
            {
                Window.Alert("没有可供导出的数据!");
                return;
            }
            //导出全部:
            if (null != PageView.QueryInfo)
            {                
                PageView.QueryInfo.PagingInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };
                ColumnSet columnSet = new ColumnSet(dgStockQueryResult);
                new StockQueryFacade(this).ExportExcelForStockQuery(PageView.QueryInfo, new ColumnSet[] { columnSet });
            }
        }

    }
}
