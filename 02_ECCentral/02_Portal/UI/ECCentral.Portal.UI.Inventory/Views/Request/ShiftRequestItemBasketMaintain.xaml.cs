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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.Inventory.Facades.Request;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.UI.Inventory.Models.Request;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Inventory.Views.Request
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class ShiftRequestItemBasketMaintain : PageBase
    {
        public ShiftRequestItemBasketFacade serviceFacade;
        public ShiftRequestItemBasketQueryFilter quertFilter;

        public ShiftRequestItemBasketMaintain()
        {
            InitializeComponent();
            quertFilter = new ShiftRequestItemBasketQueryFilter();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            serviceFacade = new ShiftRequestItemBasketFacade(this);

            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequestItemBasket_CreateShift))
            {
                btnCreateShift.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequestItemBasket_AddItem))
            {
                btnAdd.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequestItemBasket_Modify))
            {
                btnModify.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequestItemBasket_Delete))
            {
                btnDelete.IsEnabled = false;
            }
        }

        #region [Events]

        private void ckbSelectAll_Click(object sender, RoutedEventArgs e)
        {
            // 全选Row:
            CheckBox chk = sender as CheckBox;
            if (null != chk)
            {

                if (null != this.GridSearchResult.ItemsSource)
                {
                    foreach (var item in this.GridSearchResult.ItemsSource)
                    {
                        if (item is ShiftRequestItemBasketItemVM)
                        {
                            if (chk.IsChecked == true)
                            {
                                if (!((ShiftRequestItemBasketItemVM)item).IsChecked)
                                {
                                    ((ShiftRequestItemBasketItemVM)item).IsChecked = true;
                                }
                            }
                            else
                            {
                                if (((ShiftRequestItemBasketItemVM)item).IsChecked)
                                {
                                    ((ShiftRequestItemBasketItemVM)item).IsChecked = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            svStatisticInfo.Visibility = Visibility.Collapsed;  
            //查询 - 操作:
            quertFilter.CreateUserName = this.ucCreateUser.SelectedPMSysNo == null ? null : this.ucCreateUser.SelectedPMName;
            quertFilter.ShiftOutStockSysNo = this.ucSourceStock.SelectedStockSysNo == null ? null : this.ucSourceStock.SelectedStockSysNo.Value.ToString();
            quertFilter.ShiftInStockSysNo = this.ucTargetStock.SelectedStockSysNo == null ? null : this.ucTargetStock.SelectedStockSysNo.Value.ToString();

            this.GridSearchResult.Bind();
        }

        private void GridSearchResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            quertFilter.PagingInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            serviceFacade.QueryBasketList(quertFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                int getTotalCount = args.Result.TotalCount;
                this.GridSearchResult.TotalCount = getTotalCount;
                List<ShiftRequestItemBasketItemVM> viewVMList = DynamicConverter<ShiftRequestItemBasketItemVM>.ConvertToVMList(args.Result.Rows);
                this.GridSearchResult.TotalCount = getTotalCount;
                this.GridSearchResult.ItemsSource = viewVMList;
            });
        }

        private void btnCreateShift_Click(object sender, RoutedEventArgs e)
        {
            //创建移仓单 - 操作
            if (!ValidationManager.Validate(this.LayoutRoot))
            {
                return;
            }
            if (!CheckHasSelectItem())
            {
                Window.Alert("请选择后操作!");
                return;
            }
            Window.Confirm("是否将这些选中的商品创建为移仓单？", (objj, argss) =>
             {
                 if (argss.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                 {

                     //将数据按出仓入仓分组 ：
                     List<ShiftRequestItemInfo> itemList = BuildEntityList();
                     var master = from i in itemList
                                  group i by new { i.ShiftProduct.ProductConsignFlag, sourceStockSysNo = i.SourceStock.SysNo, targetStockSysNo = i.TargetStock.SysNo, i.ProductLineSysno } into g
                                  select new
                                  {
                                      sourceStockSysNo = g.Key.sourceStockSysNo,
                                      targetStockSysNo = g.Key.targetStockSysNo,
                                      ProductLineSysno = g.Key.ProductLineSysno,
                                      ProductConsignFlag = g.Key.ProductConsignFlag,
                                      result = g.ToList()
                                  };
                     //按分组创建移仓单列表 ：
                     List<ShiftRequestInfo> tfList = new List<ShiftRequestInfo>();
                     foreach (var item in master)
                     {
                         
                         tfList.Add(new ShiftRequestInfo()
                         {
                             SourceStock = new StockInfo() { SysNo = item.sourceStockSysNo },
                             TargetStock = new StockInfo() { SysNo = item.targetStockSysNo },
                             ShiftItemInfoList = item.result,
                             ProductLineSysno = item.ProductLineSysno.ToString()
                         });
                     }

                     tfList.ForEach(x =>
                     {
                         x.ShiftType = ShiftRequestType.Positive;
                         x.ShiftShippingType = "普通移仓-每周五陆运移出";
                     });

                     serviceFacade.BatchCreateShiftRequest(tfList, (obj, args) =>
                     {
                         if (args.FaultsHandle())
                         {
                             return;
                         }
                         if (!string.IsNullOrEmpty(args.Result))
                         {
                             Window.Alert("创建移仓单成功!");                                              
                             tbStatisticInfo.Text = args.Result;
                             svStatisticInfo.Visibility = Visibility.Visible;
                             this.GridSearchResult.Bind();
                         }
                     });
                 }
             });
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //添加商品 - 操作:
            //链接到备货中心页面:
            Window.Navigate(ConstValue.Inventory_ProductStockingCenterIndex, null, true);
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {          
            //修改 - 操作
            if (!ValidationManager.Validate(this.LayoutRoot))
            {
                return;
            }
            if (!CheckHasSelectItem())
            {
                Window.Alert("请选择后操作!");
                return;
            }
            Window.Confirm("是否保存当前表单？", (objj, argss) =>
             {
                 if (argss.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                 {
                     serviceFacade.BatchUpdateShiftBasketItem(BuildEntityList(), (obj, args) =>
                     {
                         if (args.FaultsHandle())
                         {
                             return;
                         }
                         Window.Alert("批量更新操作成功!");
                         this.GridSearchResult.Bind(); 
                         svStatisticInfo.Visibility = Visibility.Collapsed;   
                         return;
                     });
                 }
             });
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // 删除商品 - 操作:
            if (!CheckHasSelectItem())
            {
                Window.Alert("请选择后操作!");
                return;
            }
            Window.Confirm("是否删除当前选中的商品？", (objj, argss) =>
            {
                if (argss.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    serviceFacade.BatchDeleteShiftBasketItem(BuildEntityList(), (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert("批量删除操作成功!");
                        this.GridSearchResult.Bind();
                        svStatisticInfo.Visibility = Visibility.Collapsed;  
                        return;
                    });
                }
            });
        }
        #endregion

        /// <summary>
        /// 检查是否选择至少一条记录来操作:
        /// </summary>
        /// <returns></returns>
        private bool CheckHasSelectItem()
        {
            int selectedCount = 0;
            List<ShiftRequestItemBasketItemVM> viewVM = this.GridSearchResult.ItemsSource as List<ShiftRequestItemBasketItemVM>;
            if (null != viewVM)
            {
                viewVM.ForEach(x =>
                {
                    if (x.IsChecked)
                    {
                        selectedCount++;
                    }
                });
            }
            return selectedCount > 0;
        }

        private List<ShiftRequestItemInfo> BuildEntityList()
        {
            List<ShiftRequestItemInfo> returnList = new List<ShiftRequestItemInfo>();
            List<ShiftRequestItemBasketItemVM> getList = this.GridSearchResult.ItemsSource as List<ShiftRequestItemBasketItemVM>;
            if (null != getList)
            {
                getList = getList.Where(x => x.IsChecked == true).ToList();

                getList.ForEach(i =>
                {
                    ShiftRequestItemInfo newItem = new ShiftRequestItemInfo()
                    {
                        SysNo = i.SysNo,
                        ShiftQuantity = Convert.ToInt32(i.ShiftQty),
                        SourceStock = new StockInfo() { SysNo = i.OutStockSysNo },
                        TargetStock = new StockInfo() { SysNo = i.InStockSysNo },
                        ShiftProduct = new BizEntity.IM.ProductInfo() { SysNo = i.ProductSysNo.Value, ProductConsignFlag = i.IsConsign == 1 ? VendorConsignFlag.Consign : VendorConsignFlag.Gather },
                        ProductLineSysno =i.ProductLineSysNo.Value
                    };
                    returnList.Add(newItem);
                });
            }
            return returnList;
        }

        private void GridSearchResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequestItemBasket_ExportExcell))
            {
                Window.Alert("对不起，你没有权限进行此操作！");
                return;
            }

            if (this.GridSearchResult == null || this.GridSearchResult.TotalCount == 0)
            {
                Window.Alert("没有可供导出的数据!");
                return;
            }
            ColumnSet col = new ColumnSet(GridSearchResult);
            quertFilter.PagingInfo.PageSize = GridSearchResult.TotalCount;
            serviceFacade.ExportShiftRequestItemBasket(quertFilter, new ColumnSet[] { col });
        }
    }
}
