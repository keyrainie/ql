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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.UI.Inventory.Models;

namespace ECCentral.Portal.UI.Inventory.Views.Request
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class ShiftConfigInfoQuery : PageBase
    {
        StockShiftConfigFacade StockShiftConfigFacade;

        private StockShiftConfigView PageView;
        public ShiftConfigInfoQuery()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            PageView = new StockShiftConfigView();
            expanderCondition.DataContext = PageView.QueryInfo;
            dgStockShiftConfig.DataContext = PageView;
            StockShiftConfigFacade = new Facades.StockShiftConfigFacade(this);
            CodeNamePairHelper.GetList(ConstValue.DomainName_Inventory, ConstValue.Key_StockShiftConfigShippingType, CodeNamePairAppendItemType.All,
                (obj, args) =>
                {
                    if (!args.FaultsHandle() && args.Result != null)
                    {
                        PageView.QueryInfo.ShiftShippingTypeList = args.Result;
                    }
                });

            btnNew.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftConfigInfoQuery_New);
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.expanderCondition);
            if (PageView.QueryInfo.HasValidationErrors)
            {
                return;
            }

            dgStockShiftConfig.Bind();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            ECCentral.Portal.UI.Inventory.UserControls.StockShiftConfig content = new UserControls.StockShiftConfig();

            StockShiftConfigVM vm = new StockShiftConfigVM();
            vm.ShiftShippingTypeList = null;
            content.ConfigVM = vm;
            content.Page = this;

            content.Dialog = Window.ShowDialog("添加移仓配置信息", content, (obj, args) =>
            {
                dgStockShiftConfig.Bind();
            });
        }

        private void hlbtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftConfigInfoQuery_Modify))
            {
                Window.Alert("对不起，你没有权限进行此操作！");
                return;
            }

            HyperlinkButton btn = sender as HyperlinkButton;

            dynamic data = btn.DataContext;
            StockShiftConfigVM vm = DynamicConverter<StockShiftConfigVM>.ConvertToVM(btn.DataContext);
            ECCentral.Portal.UI.Inventory.UserControls.StockShiftConfig content = new UserControls.StockShiftConfig();
            vm.ShiftShippingTypeList = (from item in PageView.QueryInfo.ShiftShippingTypeList
                                        where item.Code != null
                                        select item).ToList();
            content.ConfigVM = vm;
            content.Page = this;

            content.Dialog = Window.ShowDialog("修改移仓配置信息", content, (obj, args) =>
             {
                 if (args.DialogResult == DialogResultType.OK)
                 {
                     StockShiftConfigVM tvm = args.Data as StockShiftConfigVM;
                     data.ShipInterval = tvm.ShipInterval;
                     data.SPLInterval = tvm.SPLInterval;
                     dgStockShiftConfig.Bind();
                 }
             });
        }

        private void dgStockShiftConfig_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            PageView.QueryInfo.PagingInfo = new QueryFilter.Common.PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            StockShiftConfigFacade.Query(PageView.QueryInfo, (totalCount, vmList) =>
            {
                PageView.TotalCount = totalCount;
                PageView.Result = vmList;
                dgStockShiftConfig.ItemsSource = PageView.Result;
                dgStockShiftConfig.TotalCount = PageView.TotalCount;
            });
        }

        private void dgStockShiftConfig_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftConfigInfoQuery_ExportExcell))
            {
                Window.Alert("对不起，你没有权限进行此操作！");
                return;
            }

            if (this.dgStockShiftConfig == null || this.dgStockShiftConfig.TotalCount == 0)
            {
                Window.Alert("没有可供导出的数据!");
                return;
            }
            StockShiftConfigQueryVM exportQueryRequest = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<StockShiftConfigQueryVM>(PageView.QueryInfo);
            exportQueryRequest.PagingInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };
            ColumnSet col = new ColumnSet(dgStockShiftConfig);            
            StockShiftConfigFacade.ExportShiftConfigInfo(exportQueryRequest, new ColumnSet[] { col });
        }
    }

}
