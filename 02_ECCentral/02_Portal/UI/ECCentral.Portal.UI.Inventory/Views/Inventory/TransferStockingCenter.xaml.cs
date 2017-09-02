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
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Facades;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Inventory;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.Inventory.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.UserControls.PMPicker;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class TransferStockingCenter : PageBase
    {
        /// <summary>
        /// 计数器：PM工作指标传入条件参数后，赋值的计数器。全部赋值完成后，执行查询操作
        /// </summary>
        private int reqParmLoadCompletedCount;
        /// <summary>
        /// PM工作指标 传入的条件参数总数 目前为8个 ，用于结合计数器判断传入的条件参数是否全部赋值完成
        /// 完成后则执行查询
        /// </summary>
        private int pmConditionCount = 8;

        #region ComboBox数据源
        public List<CodeNamePair> OutStockList { get; set; }
        public List<CodeNamePair> OutStockList_SH { get; set; }
        public List<CodeNamePair> OutStockList_BJ { get; set; }
        public List<CodeNamePair> OutStockList_WH { get; set; }
        public List<CodeNamePair> OutStockList_GZ { get; set; }
        public List<CodeNamePair> OutStockList_CD { get; set; }
        public List<CodeNamePair> OutStockList_KM { get; set; }
        public List<CodeNamePair> OutStockList_SZ { get; set; }

        public List<KeyValuePair<YNStatus?, string>> YNStatusList { get; set; }
        #endregion

        public InventoryTransferStockingQueryFilter queryFilter;
        public InventoryTransferStockingQueryVM queryVM;
        public InventoryTransferStockingFacade serviceFacade;
        public List<ProductCenterItemInfoVM> SearchList;

        public TransferStockingCenter()
        {
            InitializeComponent();
            queryFilter = new InventoryTransferStockingQueryFilter();
            queryVM = new InventoryTransferStockingQueryVM();
            SearchList = new List<ProductCenterItemInfoVM>();
        }

        private void HideColumnsForStock()
        {
            switch (this.queryVM.StockSysNo)
            {
                case "51":
                    //日本仓
                    SetColumnsVisibleForArea(39, 67, true);
                    //香港仓
                    SetColumnsVisibleForArea(68, 96, false);
                    //日本仓
                    SetColumnsVisibleForArea(97, 125, false);
                    break;

                case "52":
                    //日本仓
                    SetColumnsVisibleForArea(39, 67, false);
                    //香港仓
                    SetColumnsVisibleForArea(68, 96, true);
                    //日本仓
                    SetColumnsVisibleForArea(97, 125, false);
                    break;

                case "53":
                    //日本仓
                    SetColumnsVisibleForArea(39, 67, false);
                    //香港仓
                    SetColumnsVisibleForArea(68, 96, false);
                    //日本仓
                    SetColumnsVisibleForArea(97, 125, true);
                    break;

                default:
                    //日本仓
                    SetColumnsVisibleForArea(39, 67, true);
                    //香港仓
                    SetColumnsVisibleForArea(68, 96, true);
                    //日本仓
                    SetColumnsVisibleForArea(97, 125, true);
                    break;
            }
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            serviceFacade = new InventoryTransferStockingFacade(this);
            this.DataContext = queryVM;
            InitializeStocksData();

            LoadUrlParm();

            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_TransferStockingCenter_OperatePurchase))
            {
                btnPurchase.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnPurchase.Visibility = Visibility.Visible;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_TransferStockingCenter_OperateShift))
            {
                btnShift.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnShift.Visibility = Visibility.Visible;
            }
        }

        void TransferStockingCenter_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUrlParm();
        }

        private void InitializeStocksData()
        {
            OutStockList = new List<CodeNamePair>();
            OutStockList_SH = new List<CodeNamePair>();
            OutStockList_BJ = new List<CodeNamePair>();
            OutStockList_WH = new List<CodeNamePair>();
            OutStockList_GZ = new List<CodeNamePair>();
            OutStockList_CD = new List<CodeNamePair>();
            OutStockList_KM = new List<CodeNamePair>();
            OutStockList_SZ = new List<CodeNamePair>();
            CodeNamePairHelper.GetList("Inventory", "StockInfo", (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var list = args.Result;
                list.ForEach(x =>
                {
                    OutStockList.Add(x);
                    OutStockList_SH.Add(x);
                    OutStockList_BJ.Add(x);
                    OutStockList_WH.Add(x);
                    OutStockList_GZ.Add(x);
                    OutStockList_CD.Add(x);
                    OutStockList_KM.Add(x);
                    OutStockList_SZ.Add(x);
                });

                OutStockList_SH.RemoveAll(x => x.Code == "51" || x.Code == "59");
                OutStockList_BJ.RemoveAll(x => x.Code == "52" || x.Code == "59");
                OutStockList_WH.RemoveAll(x => x.Code == "55" || x.Code == "59");
                OutStockList_GZ.RemoveAll(x => x.Code == "53" || x.Code == "59");
                OutStockList_CD.RemoveAll(x => x.Code == "54" || x.Code == "59");
                OutStockList_KM.RemoveAll(x => x.Code == "60" || x.Code == "59");
                OutStockList_SZ.RemoveAll(x => x.Code == "61" || x.Code == "59");
            });

            YNStatusList = new List<KeyValuePair<YNStatus?, string>>();
            YNStatusList = EnumConverter.GetKeyValuePairs<YNStatus>();
        }

        private void SetColumnsVisibleForArea(int startColumnIndex, int endColumnIndex, bool isVisible)
        {
            for (int i = startColumnIndex; i <= endColumnIndex; ++i)
            {
                this.GridSearchResult.Columns[i].Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 验证Grid输入:
        /// </summary>
        /// <returns></returns>
        private bool ValidateGridInput()
        {
            return ValidationManager.Validate(this.GridSearchResult);
        }

        /// <summary>
        ///   刷新查询条件显示
        /// </summary>
        private void RefreshSearchConditionText()
        {
            StringBuilder sbSearchConditionText = new StringBuilder();
            sbSearchConditionText.Append("当前查询条件:\r\n");
            if (!string.IsNullOrEmpty(queryVM.ProductID))
            {
                sbSearchConditionText.AppendFormat("【{0} = {1}】 ", "商品编号", queryVM.ProductID);
            }
            if (!string.IsNullOrEmpty(queryVM.ProductName))
            {
                sbSearchConditionText.AppendFormat("【{0} = {1}】 ", "商品名称", queryVM.ProductName);
            }
            if (queryVM.StockSysNo != "-999")
            {
                CodeNamePair obj = this.ucSearchCondition.cmbStock.SelectedItem as CodeNamePair;
                sbSearchConditionText.AppendFormat("【{0} = {1}】 ", "分仓", obj.Name);
                sbSearchConditionText.AppendFormat("【{0} = {1}】 ", "采购分仓", obj.Name);
                sbSearchConditionText.AppendFormat("【{0} = {1}】 ", "可销售天数分仓", obj.Name);
            }
            if (!string.IsNullOrEmpty(queryVM.Category1SysNo))
            {
                sbSearchConditionText.AppendFormat("【{0} = {1}】 ", "大类", this.ucSearchCondition.ucCategory.Category1Name);
            }
            if (!string.IsNullOrEmpty(queryVM.Category2SysNo))
            {
                sbSearchConditionText.AppendFormat("【{0} = {1}】 ", "中类", this.ucSearchCondition.ucCategory.Category2Name);
            }
            if (!string.IsNullOrEmpty(queryVM.Category3SysNo))
            {
                sbSearchConditionText.AppendFormat("【{0} = {1}】 ", "小类", this.ucSearchCondition.ucCategory.Category3Name);
            }
            if (!string.IsNullOrEmpty(queryVM.PMSysNo))
            {
                sbSearchConditionText.AppendFormat("【{0} = {1}】 ", "PM", this.ucSearchCondition.ucPM.SelectedPMName);
            }
            if (!string.IsNullOrEmpty(queryVM.ProductConsignFlag))
            {
                CodeNamePair obj = this.ucSearchCondition.cmbConsign.SelectedItem as CodeNamePair;
                sbSearchConditionText.AppendFormat("【{0} = {1}】 ", "代销属性", obj.Name);
            }
            if (!string.IsNullOrEmpty(queryVM.SysNO))
            {
                sbSearchConditionText.AppendFormat("【{0} = {1}】 ", "系统编号", queryVM.SysNO);
            }
            if (queryVM.ProductStatus.HasValue)
            {
                KeyValuePair<ProductStatus?, string> getSelectedItem = (KeyValuePair<ProductStatus?, string>)this.ucSearchCondition.cmbStatus.SelectedItem;
                sbSearchConditionText.AppendFormat("【{0} {2} {1}】 ", "状态", getSelectedItem.Value, queryVM.ProductStatusCompareCode);
            }
            else
            {
                sbSearchConditionText.AppendFormat("【{0} {2} {1}】 ", "状态", "所有", queryVM.ProductStatusCompareCode);
            }

            if (!string.IsNullOrEmpty(queryVM.DaySalesCount))
            {
                sbSearchConditionText.AppendFormat("【{0} {2} {1}】 ", "日均销量", queryVM.DaySalesCount, queryVM.DaySalesCountCompareCode);
            }
            if (!string.IsNullOrEmpty(queryVM.AvailableSaleDays))
            {
                sbSearchConditionText.AppendFormat("【{0} {2} {1}】 ", "可销售天数", queryVM.AvailableSaleDays, queryVM.AvailableSaleDaysCompareCode);
            }
            if (!string.IsNullOrEmpty(queryVM.RecommendBackQty))
            {
                sbSearchConditionText.AppendFormat("【{0} {2} {1}】 ", "建议备货量", queryVM.RecommendBackQty, queryVM.RecommendBackQtyCompareCode);
            }

            //生产商:
            if (!string.IsNullOrEmpty(queryVM.ManufacturerName))
            {
                sbSearchConditionText.AppendFormat("【{0} = {1}】 ", "生产商", queryVM.ManufacturerName);
            }
            if (!string.IsNullOrEmpty(queryVM.VendorName))
            {
                sbSearchConditionText.AppendFormat("【{0} = {1}】 ", "供应商", queryVM.VendorName);
            }
            if (!string.IsNullOrEmpty(queryVM.BrandName))
            {
                sbSearchConditionText.AppendFormat("【{0} = {1}】 ", "品牌", queryVM.BrandName);
            }
            //价格和积分
            if (!string.IsNullOrEmpty(queryVM.AverageUnitCost))
            {
                sbSearchConditionText.AppendFormat("【{0} {2} {1}】 ", "平均成本", queryVM.AverageUnitCost, queryVM.AverageUnitCostCompareCode);
            }
            if (!string.IsNullOrEmpty(queryVM.SalePrice))
            {
                sbSearchConditionText.AppendFormat("【{0} {2} {1}】 ", "价格", queryVM.AverageUnitCost, queryVM.SalePrice, queryVM.SalePriceCompareCode);
            }
            if (!string.IsNullOrEmpty(queryVM.Point))
            {
                sbSearchConditionText.AppendFormat("【{0} {2} {1}】 ", "积分", queryVM.AverageUnitCost, queryVM.Point, queryVM.SalePrice, queryVM.PointCompareCode);
            }
            //库存:
            if (!string.IsNullOrEmpty(queryVM.FinanceQty))
            {
                sbSearchConditionText.AppendFormat("【{0} {2} {1}】 ", "财务库存", queryVM.FinanceQty, queryVM.FinanceQtyCompareCode);
            }
            if (!string.IsNullOrEmpty(queryVM.AvailableQty))
            {
                sbSearchConditionText.AppendFormat("【{0} {2} {1}】 ", "可用库存", queryVM.AvailableQty, queryVM.AvailableQtyCompareCode);
            }
            if (!string.IsNullOrEmpty(queryVM.OrderedQty))
            {
                sbSearchConditionText.AppendFormat("【{0} {2} {1}】 ", "被订购数", queryVM.OrderedQty, queryVM.OrderedQtyCompareCode);
            }
            if (!string.IsNullOrEmpty(queryVM.ConsignQty))
            {
                sbSearchConditionText.AppendFormat("【{0} {2} {1}】 ", "代销库存", queryVM.ConsignQty, queryVM.ConsignQtyCompareCode);
            }
            if (!string.IsNullOrEmpty(queryVM.OccupiedQty))
            {
                sbSearchConditionText.AppendFormat("【{0} {2} {1}】 ", "被占用库存", queryVM.OccupiedQty, queryVM.OccupiedQtyCompareCode);
            }
            if (!string.IsNullOrEmpty(queryVM.OnlineQty))
            {
                sbSearchConditionText.AppendFormat("【{0} {2} {1}】 ", "Online库存", queryVM.OnlineQty, queryVM.OnlineQtyCompareCode);
            }
            if (!string.IsNullOrEmpty(queryVM.VirtualQty))
            {
                sbSearchConditionText.AppendFormat("【{0} {2} {1}】 ", "虚库数量", queryVM.VirtualQty, queryVM.VirtualQtyCompareCode);
            }
            if (!string.IsNullOrEmpty(queryVM.PurchaseQty))
            {
                sbSearchConditionText.AppendFormat("【{0} {2} {1}】 ", "采购在途", queryVM.PurchaseQty, queryVM.PurchaseQtyCompareCode);
            }
            if (queryVM.IsAsyncStock.HasValue)
            {
                KeyValuePair<YNStatus?, string> obj = (KeyValuePair<YNStatus?, string>)this.ucSearchCondition.cmbIsSync.SelectedItem;
                sbSearchConditionText.AppendFormat("【{0} = {1}】 ", "库存同步", obj.Value);
            }
            if (queryVM.IsLarge.HasValue)
            {
                KeyValuePair<YNStatus?, string> obj = (KeyValuePair<YNStatus?, string>)this.ucSearchCondition.cmbIsLarge.SelectedItem;
                sbSearchConditionText.AppendFormat("【{0} = {1}】 ", "是否为大货", obj.Value);
            }

            //排序字段和升序，降序
            List<CodeNamePair> getSelectSortItems = ucSearchCondition.cmbOrderBy.ItemsSource as List<CodeNamePair>;
            if (null != getSelectSortItems)
            {
                string sortByString = string.Format("【排序字段={0} {1}】", getSelectSortItems.SingleOrDefault(x => x.Code == queryVM.SortByField).Name, queryVM.IsSortByAsc == true ? "升序" : "降序");
                sbSearchConditionText.Append(sortByString);
            }
            this.lblSearchConditionResult.Text = sbSearchConditionText.ToString();
        }

        #region [Events]

        private void GridSearchResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            //绑定查询的数据源: 
            queryFilter.PageInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize
            };

            #region [Ray.L.Xing 注销旧需求（因为新需求 取消了PM 查询PM的权限 改为通过商品的产品线来过滤单据）]

            //string pmList = string.Empty;
            //if (this.ucSearchCondition.ucPM.itemList != null && this.ucSearchCondition.ucPM.itemList.Count > 0)
            //{
            //    foreach (var item in this.ucSearchCondition.ucPM.itemList)
            //    {
            //       pmList +=","+item.SysNo;
            //    }
            //    queryFilter.AuthorizedPMsSysNumber = pmList.Remove(0, 2);
            //}    

            //通过 AuthorizedPMsSysNumber 来标记 当前PM的权限 高级权限可以访问所有商品
            if (AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_SeniorPM_Query))
            {
                queryFilter.AuthorizedPMsSysNumber = "Senior";
            }

            #endregion
            serviceFacade.QueryInventoryTransferStockingList(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                int getTotalCount = args.Result.TotalCount;
                this.GridSearchResult.TotalCount = getTotalCount;

                if (args.Result.ResultList == null || args.Result.ResultList.Count == 0)
                {
                    this.GridSearchResult.ItemsSource = null;
                    this.GridSearchResult.ItemsSource = args.Result.ResultList;
                    return;
                }
                SearchList = new List<ProductCenterItemInfoVM>();
                foreach (var item in args.Result.ResultList)
                {
                    ProductCenterItemInfoVM itemVM = new ProductCenterItemInfoVM();
                    itemVM = item.Convert<ProductCenterItemInfo, ProductCenterItemInfoVM>();
                    SearchList.Add(itemVM);
                }

                for (int i = 0; i < SearchList.Count; i++)
                {
                    decimal shPrice = 0.00M;
                    SearchList[i].OutStockList_SH = OutStockList_SH;
                    SearchList[i].OutStockList_BJ = OutStockList_BJ;
                    SearchList[i].OutStockList_WH = OutStockList_WH;
                    SearchList[i].OutStockList_GZ = OutStockList_GZ;
                    SearchList[i].OutStockList_CD = OutStockList_CD;
                    SearchList[i].OutStockList_KM = OutStockList_KM;
                    SearchList[i].OutStockList_SZ = OutStockList_SZ;
                    SearchList[i].YNStatusList = YNStatusList;
                    SearchList[i].SuggestTransferStocks.ForEach(x =>
                    {
                        x.Parent = SearchList[i];
                        x.NeedBufferEnable = YNStatus.No;
                        x.NeedBufferVisible = true;
                        if (x.Lastintime.HasValue && (SearchList[i].VFType == "U" || SearchList[i].VFType == "L")) //是库存同步商品 所有分仓显示 库存同步商品的采购价格
                        {
                            //同步商品虚库数量调用规则为'直接调用'或'限制数量上限'显示同步采购价格                                      
                            shPrice = SearchList[i].PurchasePrice.Value;
                            x.Price = SearchList[i].PurchasePrice.Value.ToString("f2");
                        }
                        else if (SearchList[i].VirtualPrice != 0)//存在正常采购价格  所有分仓显示 正常采购价格
                        {
                            shPrice = SearchList[i].VirtualPrice.Value;
                            x.Price = SearchList[i].VirtualPrice.Value.ToString("f2");
                        }
                        else // 保持原先逻辑
                        {
                            if (x.WareHouseNumber == "51")
                            {
                                shPrice = x.LastPrice.Value;
                                x.Price = x.LastPrice.Value.ToString("f2");
                            }
                            else
                            {
                                x.Price = x.LastPrice.Value > 0 ? x.LastPrice.Value.ToString("f2") : shPrice.ToString("f2");
                            }
                        }
                    });

                    //日本仓 商品的最近一次采购入库分仓为宝山仓，则该商品的建议备货数量默认加载在宝山仓采购数量填充栏中，嘉定仓留空；商品的最近一次采购入库分仓为嘉定仓，则该商品的建议备货数量默认加载在嘉定仓采购数量填充栏中，宝山仓留空；
                    DateTime? S51 = null;
                    S51 = SearchList[i].SuggestTransferStocks.Where(x => x.WareHouseNumber == "51").First().Lastintime;
                    DateTime? S59 = null;
                    S59 = SearchList[i].SuggestTransferStocks.Where(x => x.WareHouseNumber == "59").First().Lastintime;
                    if (!S51.HasValue)
                    {
                        S51 = DateTime.MinValue;
                    }
                    if (!S59.HasValue)
                    {
                        S59 = DateTime.MinValue;
                    }
                    if (S51.Value >= S59.Value)
                    {
                        SearchList[i].SuggestTransferStocks.Where(x => x.WareHouseNumber == "59").First().PurchaseQty = "0";
                        SearchList[i].SuggestTransferStocks.Where(x => x.WareHouseNumber == "51").First().PurchaseQty = SearchList[i].SuggestTransferStocks.Where(x => x.WareHouseNumber == "51").First().SuggestQty.ToString();
                    }
                    else
                    {
                        SearchList[i].SuggestTransferStocks.Where(x => x.WareHouseNumber == "51").First().PurchaseQty = "0";
                        SearchList[i].SuggestTransferStocks.Where(x => x.WareHouseNumber == "59").First().PurchaseQty = SearchList[i].SuggestTransferStocks.Where(x => x.WareHouseNumber == "51").First().SuggestQty.ToString();
                    }
                }
                this.GridSearchResult.ItemsSource = null;
                this.GridSearchResult.ItemsSource = SearchList;
                HideColumnsForStock();
                #region 注销

                //SearchList = EntityConverter<List<ProductCenterItemInfo>, List<ProductCenterItemInfoVM>>.Convert(args.Result.ResultList, (s, t) =>
                //{
                //    decimal shPrice = 0.00M;
                //    for (int i = 0; i < s.Count; ++i)
                //    {
                //        t[i].OutStockList_SH = OutStockList_SH;
                //        t[i].OutStockList_BJ = OutStockList_BJ;
                //        t[i].OutStockList_WH = OutStockList_WH;
                //        t[i].OutStockList_GZ = OutStockList_GZ;
                //        t[i].OutStockList_CD = OutStockList_CD;
                //        t[i].YNStatusList = YNStatusList;
                //        t[i].SuggestTransferStocks.ForEach(x =>
                //        {
                //            x.Parent = t[i];                        
                //            if (x.Lastintime.HasValue && (t[i].VFType == "U" || t[i].VFType == "L")) //是库存同步商品 所有分仓显示 库存同步商品的采购价格
                //            {
                //                //同步商品虚库数量调用规则为'直接调用'或'限制数量上限'显示同步采购价格                                      
                //                shPrice = t[i].PurchasePrice.Value;
                //                x.Price = t[i].PurchasePrice.Value.ToString("f2");
                //            }
                //            else if (t[i].VirtualPrice != 0)//存在正常采购价格  所有分仓显示 正常采购价格
                //            {
                //                shPrice = t[i].VirtualPrice.Value;
                //                x.Price = t[i].VirtualPrice.Value.ToString("f2");
                //            }
                //            else // 保持原先逻辑
                //            {
                //                if (x.WareHouseNumber == "51")
                //                {
                //                    shPrice = x.LastPrice.Value;
                //                    x.Price = x.LastPrice.Value.ToString("f2");
                //                }
                //                else
                //                {
                //                    x.Price = x.LastPrice.Value > 0 ? x.LastPrice.Value.ToString("f2") : shPrice.ToString("f2");
                //                }
                //            }                
                //        });

                //        //日本仓 商品的最近一次采购入库分仓为宝山仓，则该商品的建议备货数量默认加载在宝山仓采购数量填充栏中，嘉定仓留空；商品的最近一次采购入库分仓为嘉定仓，则该商品的建议备货数量默认加载在嘉定仓采购数量填充栏中，宝山仓留空；
                //        DateTime? S51 = null;
                //        S51 = t[i].SuggestTransferStocks.Where(x => x.WareHouseNumber == "51").First().Lastintime;
                //        DateTime? S59 = null;
                //        S59 = t[i].SuggestTransferStocks.Where(x => x.WareHouseNumber == "59").First().Lastintime;
                //        if (!S51.HasValue)
                //        {
                //            S51 = DateTime.MinValue;
                //        }
                //        if (!S59.HasValue)
                //        {
                //            S59 = DateTime.MinValue;
                //        }
                //        if (S51.Value >= S59.Value)
                //        {
                //            t[i].SuggestTransferStocks.Where(x => x.WareHouseNumber == "59").First().PurchaseQty = "0";
                //            t[i].SuggestTransferStocks.Where(x => x.WareHouseNumber == "51").First().PurchaseQty = t[i].SuggestTransferStocks.Where(x => x.WareHouseNumber == "51").First().SuggestQty.ToString();
                //        }
                //        else
                //        {
                //            t[i].SuggestTransferStocks.Where(x => x.WareHouseNumber == "51").First().PurchaseQty = "0";
                //            t[i].SuggestTransferStocks.Where(x => x.WareHouseNumber == "59").First().PurchaseQty = t[i].SuggestTransferStocks.Where(x => x.WareHouseNumber == "51").First().SuggestQty.ToString();
                //        }                                           
                //    }
                //});
                #endregion

            });
        }

        private void GridSearchResult_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ProductCenterItemInfoVM rowData = e.Row.DataContext as ProductCenterItemInfoVM;
            switch (rowData.ProductStatus)
            {
                case -1:
                    e.Row.FontStyle = FontStyles.Italic;
                    e.Row.Background = new SolidColorBrush(Colors.LightGray);
                    e.Row.Foreground = new SolidColorBrush(Colors.Gray);
                    break;
                case 1:
                    e.Row.FontStyle = FontStyles.Normal;
                    e.Row.Background = new SolidColorBrush(Colors.White);
                    e.Row.Foreground = new SolidColorBrush(Colors.Black);
                    break;
                default:
                    e.Row.FontStyle = FontStyles.Normal;
                    e.Row.Background = new SolidColorBrush(Colors.White);
                    e.Row.Foreground = new SolidColorBrush(Colors.Red);
                    break;
            }

            if (rowData.InstockDays > 180) //库龄为超过180天的商品记录显示白色背景颜色
            {
                e.Row.Background = new SolidColorBrush(Colors.Red);
            }
            else if (rowData.InstockDays >= 121 && rowData.InstockDays <= 180)//库龄为121天-180天的商品记录显示红色背景颜色
            {
                e.Row.Background = new SolidColorBrush(Colors.Orange);
            }
            else if (rowData.InstockDays >= 91 && rowData.InstockDays <= 120)//库龄为91-120天的商品记录显示红色背景颜色；
            {
                e.Row.Background = new SolidColorBrush(Colors.Orange);
            }
            else if (rowData.InstockDays >= 61 && rowData.InstockDays <= 90)//库龄为61-90天的商品记录显示橙色背景颜色
            {
                e.Row.Background = new SolidColorBrush(Colors.Yellow);
            }
            else if (rowData.InstockDays >= 31 && rowData.InstockDays <= 60)//库龄为31-60天的商品记录显示黄背景颜色；
            {
                e.Row.Background = new SolidColorBrush(Color.FromArgb(255, 102, 255, 102));
            }
            else if (rowData.InstockDays >= 0 && rowData.InstockDays <= 30)
            {
                e.Row.Background = new SolidColorBrush(Colors.White);
            }
        }

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
                        if (item is ProductCenterItemInfoVM)
                        {
                            if (chk.IsChecked == true)
                            {
                                if (!((ProductCenterItemInfoVM)item).IsChecked)
                                {
                                    ((ProductCenterItemInfoVM)item).IsChecked = true;
                                }
                            }
                            else
                            {
                                if (((ProductCenterItemInfoVM)item).IsChecked)
                                {
                                    ((ProductCenterItemInfoVM)item).IsChecked = false;
                                }
                            }

                        }
                    }
                }
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchExecute();
        }

        private void SearchExecute()
        {
            int getCurrentTabIndex = ucSearchCondition.tabSearchCondition.SelectedIndex;
            RefreshSearchConditionText();
            //搜索:
            if (!ValidationManager.Validate(this.ucSearchCondition.tabSearchCondition))
            {
                return;
            }
            ucSearchCondition.tabSearchCondition.SelectedIndex = 1;
            this.UpdateLayout();
            if (!ValidationManager.Validate(this.ucSearchCondition.tabSearchCondition))
            {
                return;
            }
            ucSearchCondition.tabSearchCondition.SelectedIndex = 2;
            this.UpdateLayout();
            if (!ValidationManager.Validate(this.ucSearchCondition.tabSearchCondition))
            {
                return;
            }
            ucSearchCondition.tabSearchCondition.SelectedIndex = 3;
            this.UpdateLayout();
            if (!ValidationManager.Validate(this.ucSearchCondition.tabSearchCondition))
            {
                return;
            }
            ucSearchCondition.tabSearchCondition.SelectedIndex = 4;
            this.UpdateLayout();
            if (!ValidationManager.Validate(this.ucSearchCondition.tabSearchCondition))
            {
                return;
            }
            ucSearchCondition.tabSearchCondition.SelectedIndex = getCurrentTabIndex;

            this.queryFilter = EntityConverter<InventoryTransferStockingQueryVM, InventoryTransferStockingQueryFilter>.Convert(queryVM, (s, t) =>
            {
                t.BackDay = !string.IsNullOrEmpty(s.UserDefinedBackDay) ? s.UserDefinedBackDay : s.BackDay;
            });

            //检查:类别跟PM选项不能同时为空:
            if (CheckEmptySearchCondition())
            {
                this.lblSearchConditionResult.Text = "为了避免查询数据量太大，类别跟PM选项不能同时为空!";
                return;
            }
            this.GridSearchResult.Bind();
            //根据仓库，隐藏相关列:
            HideColumnsForStock();
        }

        private bool CheckEmptySearchCondition()
        {
            return (
                   string.IsNullOrEmpty(queryVM.ProductID)
                && string.IsNullOrEmpty(queryVM.SysNO)
                && string.IsNullOrEmpty(queryVM.ProductName)
                && string.IsNullOrEmpty(queryVM.Category1SysNo)
                && string.IsNullOrEmpty(queryVM.Category2SysNo)
                && string.IsNullOrEmpty(queryVM.Category3SysNo)
                && string.IsNullOrEmpty(queryVM.PMSysNo)
                && string.IsNullOrEmpty(queryVM.ManufacturerName)
                && string.IsNullOrEmpty(queryVM.VendorName)
                && string.IsNullOrEmpty(queryVM.AverageUnitCost)
                && string.IsNullOrEmpty(queryVM.SalePrice)
                && string.IsNullOrEmpty(queryVM.Point)
                && string.IsNullOrEmpty(queryVM.AvailableQty)
                && string.IsNullOrEmpty(queryVM.ConsignQty)
                && string.IsNullOrEmpty(queryVM.FinanceQty)
                && string.IsNullOrEmpty(queryVM.OccupiedQty)
                && string.IsNullOrEmpty(queryVM.OnlineQty)
                && string.IsNullOrEmpty(queryVM.OrderedQty)
                && string.IsNullOrEmpty(queryVM.PurchaseQty)
                && string.IsNullOrEmpty(queryVM.SubStockQty)
                && string.IsNullOrEmpty(queryVM.VirtualQty)
                && queryVM.StockSysNo == "-999"
                && !queryVM.IsAsyncStock.HasValue
                && !queryVM.IsLarge.HasValue
                );
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            //重置查询条件:
            this.queryVM = new InventoryTransferStockingQueryVM();
            this.DataContext = queryVM;
            this.lblSearchConditionResult.Text = string.Empty;
        }

        private void btnTransferStockingAlert_Click(object sender, RoutedEventArgs e)
        {
            //当日备货提醒:
            UCBackOrderAlertForToday alertTodayCtrl = new UCBackOrderAlertForToday();
            alertTodayCtrl.Dialog = Window.ShowDialog("当日需备货供应商", alertTodayCtrl, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK && args.Data != null)
                {
                    //
                    this.queryVM = new InventoryTransferStockingQueryVM();
                    this.lblSearchConditionResult.Text = string.Empty;
                    string getVendorSysNoList = args.Data.ToString();
                    if (getVendorSysNoList.IndexOf('|') >= 0)
                    {
                        queryVM.VendorSysNoList.Clear();
                        string[] sysNoArr = getVendorSysNoList.Split('|');
                        sysNoArr.ForEach(x => queryVM.VendorSysNoList.Add(Convert.ToInt32(x)));
                    }
                    else
                    {
                        queryVM.VendorSysNoList.Add(Convert.ToInt32(getVendorSysNoList));
                    }
                    this.GridSearchResult.PageIndex = 0;
                    this.queryFilter = EntityConverter<InventoryTransferStockingQueryVM, InventoryTransferStockingQueryFilter>.Convert(queryVM, (s, t) =>
                    {
                        t.BackDay = !string.IsNullOrEmpty(s.UserDefinedBackDay) ? s.UserDefinedBackDay : s.BackDay;
                    });
                    this.DataContext = queryVM;
                    this.GridSearchResult.Bind();
                    queryVM.VendorSysNoList.Clear();
                }
            }, new Size(700, 500));
        }

        private void btnPurchase_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.LayoutRoot))
            {
                return;
            }
            //采购:
            if (SearchList.Count <= 0)
            {
                Window.Alert("没有查询到任何数据！");
                return;
            }
            var getSelectedProductList = SearchList.Where(x => x.IsChecked == true).ToList();
            if (null == SearchResult || getSelectedProductList.Count <= 0)
            {
                Window.Alert("你没有选择要采购的商品！");
                return;
            }

            bool checkPurchaseQtyZero = false;
            getSelectedProductList.ForEach(x =>
            {
                x.SuggestTransferStocks.ForEach(y =>
                {
                    if (Convert.ToDecimal(y.Price) > 0)
                    {
                        checkPurchaseQtyZero = true;
                    }

                });
            });
            if (!checkPurchaseQtyZero)
            {
                Window.Alert("你没有要选择的商品,或者商品信息 （采购数量，采购价格）设置不正确!");
                return;
            }

            List<ProductCenterItemInfo> infoList = EntityConverter<List<ProductCenterItemInfoVM>, List<ProductCenterItemInfo>>.Convert(getSelectedProductList);
            serviceFacade.CreateBasketItemsForPrepare(infoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                Window.Alert("预购成功!");
                return;
            });
        }

        private void btnShift_Click(object sender, RoutedEventArgs e)
        {
            var getSelectedProductList = SearchList.Where(x => x.IsChecked == true).ToList();

            #region Check操作:
            if (!ValidationManager.Validate(this.LayoutRoot))
            {
                return;
            }
            //移仓:
            if (SearchList.Count <= 0)
            {
                Window.Alert("没有查询到任何数据！");
                return;
            }
            if (null == SearchResult || getSelectedProductList.Count <= 0)
            {
                Window.Alert("你没有选择要采购的商品！");
                return;
            }

            bool checkShiftQtyZero = false;
            getSelectedProductList.ForEach(x =>
            {
                x.SuggestTransferStocks.ForEach(y =>
                {
                    if (Convert.ToInt32(y.ShiftQty) > 0)
                    {
                        checkShiftQtyZero = true;
                    }

                });
            });
            if (!checkShiftQtyZero)
            {
                Window.Alert("你没有要选择的商品,或者商品信息 （移仓出库仓，移仓数量）设置不正确!");
                return;
            }

            #region 检查移仓数量是否小于等于可移库存:
            string shiftQtyErrorMsg = string.Empty;
            getSelectedProductList.ForEach(x =>
            {
                x.SuggestTransferStocks.ForEach(y =>
                {
                    if (Convert.ToInt32(y.ShiftQty) > y.OutStockShiftQtyDisplay)
                    {
                        shiftQtyErrorMsg = "[" + OutStockList.Single(z => z.Code == y.WareHouseNumber).Name + "]移仓数量不能大于[" + OutStockList.Single(z => z.Code == y.OutStockWareHouseNumber).Name + "]可移数量 !";
                        return;
                    }
                });
            });
            if (!string.IsNullOrEmpty(shiftQtyErrorMsg))
            {
                Window.Alert(shiftQtyErrorMsg);
                return;
            }
            #endregion


            string NoSuccessProductSysno = string.Empty;

            foreach (var item in getSelectedProductList)
            {
                if (item.IsBatch == "Y")
                {
                    NoSuccessProductSysno += item.ItemSysNumber.Value.ToString() + " ";
                }
            }

            if (!string.IsNullOrEmpty(NoSuccessProductSysno))
            {
                Window.Alert("商品编号： " + NoSuccessProductSysno + " 采用批号管理，仅限入库分仓当地销售，目前暂不支持分仓间库存的调拨 !");
                return;
            }
            #endregion

            ShiftRequestItemBasket newBasketInfo = new ShiftRequestItemBasket() { ShiftItemInfoList = new List<ShiftRequestItemInfo>() };
            getSelectedProductList.ForEach(x =>
            {
                x.SuggestTransferStocks.ForEach(y =>
                {
                    if (Convert.ToInt32(y.ShiftQty) > 0)
                    {
                        ShiftRequestItemInfo newItem = new ShiftRequestItemInfo()
                        {
                            ShiftProduct = new ProductInfo() { SysNo = x.ItemSysNumber.Value },
                            InStockQuantity = y.OutStockShiftQtyDisplay,
                            ShiftQuantity = Convert.ToInt32(y.ShiftQty),
                            SourceStock = new StockInfo() { SysNo = Convert.ToInt32(y.OutStockWareHouseNumber), StockName = OutStockList.Single(z => z.Code == y.OutStockWareHouseNumber).Name },
                            TargetStock = new StockInfo() { SysNo = Convert.ToInt32(y.WareHouseNumber), StockName = OutStockList.Single(z => z.Code == y.WareHouseNumber).Name }
                        };
                        newBasketInfo.ShiftItemInfoList.Add(newItem);
                    }
                });
            });

            serviceFacade.CreateShiftBasket(newBasketInfo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                Window.Alert("加入移仓篮成功!");
                return;
            });

        }

        private void btnViewBasket_Click(object sender, RoutedEventArgs e)
        {
            //查看采购篮:
            Window.Navigate("/ECCentral.Portal.UI.PO/BasketQuery", null, true);
        }

        private void btnViewShift_Click(object sender, RoutedEventArgs e)
        {
            //查看移仓篮：
            Window.Navigate("/ECCentral.Portal.UI.Inventory/ShiftRequestItemBasketMaintain", null, true);

        }

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            //立即同步 ：
            if (SearchList.Count <= 0)
            {
                Window.Alert("没有查询到任何数据！");
                return;
            }
            //TODO:同步库存，调用第三方接口：

            List<ProductCenterItemInfoVM> sourceList = this.GridSearchResult.ItemsSource as List<ProductCenterItemInfoVM>;
            if (null != sourceList)
            {
                sourceList.ForEach(x =>
                {
                    if (x.IsSynProduct == 1)
                    {
                        //调用第三方接口:
                    }
                });
            }
            Window.Alert("同步库存成功！");

        }

        private void btnClearPurchase_Click(object sender, RoutedEventArgs e)
        {
            //清空采购数量:
            if (SearchList.Count <= 0)
            {
                Window.Alert("没有查询到任何数据！");
                return;
            }
            SearchList.ForEach(x =>
            {
                x.SuggestTransferStocks.ForEach(y =>
                {
                    y.PurchaseQty = "0";
                });
            });
        }

        private void GridSearchResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_TransferStockingCenter_ExportExcellAll))
            {
                Window.Alert("对不起，你没有权限进行此操作！");
                return;
            }


            List<ProductCenterItemInfoVM> sourceList = this.GridSearchResult.ItemsSource as List<ProductCenterItemInfoVM>;
            if (null == sourceList || sourceList.Count == 0)
            {
                Window.Alert("没有可供导出的数据！");
                return;
            }

            //导出Excel:
            //TODO:导出excel权限控制:
            if (null != queryFilter)
            {
                InventoryTransferStockingQueryFilter exportQueryRequest = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<InventoryTransferStockingQueryFilter>(queryFilter);
                exportQueryRequest.PageInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };
                ColumnSet columnSet = new ColumnSet();

                #region [构建Excel列]

                columnSet.Add("ItemCode", "Item#", 30);
                columnSet.Add("ItemName", "商品名称", 40);
                columnSet.Add("ItemSysNumber", "系统编号", 20);
                columnSet.Add("ManufacturerName", "生产商", 30);
                columnSet.Add("Brand", "品牌", 20);
                columnSet.Add("AllAvailableQty", "总可用库存", 20);
                columnSet.Add("ConsignQty", "总代销库存", 20);
                columnSet.Add("AllStockAVGDailySales", "整网日均销量", 20);
                columnSet.Add("AllStockAvailableSalesDays", "整网可销售天数", 20);
                columnSet.Add("AllOutStockQuantity", "当天出库量", 20);
                columnSet.Add("VirtualQty", "总虚库库存", 20);
                columnSet.Add("InventoryQty", "库存同步数量", 20);
                columnSet.Add("PurchasePrice", "同步采购价格", 20);
                columnSet.Add("OrderQty", "总被订购库存", 20);
                columnSet.Add("TransferStockQty", "中转仓库存", 20);
                columnSet.Add("PurchaseQty", "所有仓采购单据数量", 20);
                columnSet.Add("UnmarketableQty", "滞销库存", 20);
                columnSet.Add("SuggestQtyAll", "建议备货总数", 20);
                columnSet.Add("VirtualPrice", "正常采购价格", 20);
                columnSet.Add("UnitCost", "成本", 20);
                columnSet.Add("GrossProfitRate", "毛利率", 20);
                columnSet.Add("D1", "D1", 20);
                columnSet.Add("D2", "D2", 20);
                columnSet.Add("D3", "D3", 20);
                columnSet.Add("D4", "D4", 20);
                columnSet.Add("D5", "D5", 20);
                columnSet.Add("D6", "D6", 20);
                columnSet.Add("D7", "D7", 20);
                columnSet.Add("W1", "W1", 20);
                columnSet.Add("W2", "W2", 20);
                columnSet.Add("W3", "W3", 20);
                columnSet.Add("W4", "W4", 20);
                columnSet.Add("M1", "M1", 20);
                columnSet.Add("M2", "M2", 20);
                columnSet.Add("M3", "M3", 20);
                columnSet.Add("PO_Memo", "采购价格备注", 40);
                columnSet.Add("CurrentPrice", "泰隆优选价格", 20);
                columnSet.Add("JDPrice", "京东价格", 20);
                #region 构建各个分仓库存:
                //显示日本仓Columns:
                if (queryFilter.StockSysNo == "51" || queryFilter.StockSysNo == "-999")
                {
                    columnSet.Add("SuggestTransferStocks_0_AvailableQty", "日本可用库存", 20);
                    columnSet.Add("SuggestTransferStocks_0_VirtualQty", "日本虚库库存", 20);
                    columnSet.Add("SuggestTransferStocks_0_OrderQty", "日本仓被订购数量", 20);
                    columnSet.Add("SuggestTransferStocks_0_ConsignQty", "日本仓代销库存数量", 20);
                    columnSet.Add("SuggestTransferStocks_0_AvailableSalesDays", "日本仓可销售天数", 20);
                    columnSet.Add("SuggestTransferStocks_0_PurchaseInQty", "日本采购单据数量", 20);
                    columnSet.Add("SuggestTransferStocks_0_ShiftInQty", "日本移仓在途数量", 20);
                    columnSet.Add("SuggestTransferStocks_0_D1", "日本D1", 20);
                    columnSet.Add("SuggestTransferStocks_0_D2", "日本D2", 20);
                    columnSet.Add("SuggestTransferStocks_0_D3", "日本D3", 20);
                    columnSet.Add("SuggestTransferStocks_0_D4", "日本D4", 20);
                    columnSet.Add("SuggestTransferStocks_0_D5", "日本D5", 20);
                    columnSet.Add("SuggestTransferStocks_0_D6", "日本D6", 20);
                    columnSet.Add("SuggestTransferStocks_0_D7", "日本D7", 20);
                    columnSet.Add("SuggestTransferStocks_0_W1", "日本W1", 20);
                    columnSet.Add("SuggestTransferStocks_0_W2", "日本W2", 20);
                    columnSet.Add("SuggestTransferStocks_0_W3", "日本W3", 20);
                    columnSet.Add("SuggestTransferStocks_0_W4", "日本W4", 20);
                    columnSet.Add("SuggestTransferStocks_0_M1", "日本M1", 20);
                    columnSet.Add("SuggestTransferStocks_0_M2", "日本M2", 20);
                    columnSet.Add("SuggestTransferStocks_0_M3", "日本M3", 20);
                    columnSet.Add("SuggestTransferStocks_0_W1RegionSalesQty", "日本仓覆盖地区W1", 20);
                    columnSet.Add("SuggestTransferStocks_0_M1RegionSalesQty", "日本仓覆盖地区M1", 20);
                    columnSet.Add("SuggestTransferStocks_0_AVGDailySales", "日本仓覆盖区域日均销量", 20);
                    columnSet.Add("SuggestTransferStocks_0_SuggestQty", "日本仓建议备货数量", 20);
                    columnSet.Add("SuggestTransferStocks_0_Lastintime", "日本最后一次采购时间", 20);
                    columnSet.Add("SuggestTransferStocks_0_LastPrice", "日本最后一次采购价格", 20);
                }
                //香港仓库存:
                if (queryFilter.StockSysNo == "52" || queryFilter.StockSysNo == "-999")
                {
                    columnSet.Add("SuggestTransferStocks_1_AvailableQty", "香港可用库存", 20);
                    columnSet.Add("SuggestTransferStocks_1_VirtualQty", "香港虚库库存", 20);
                    columnSet.Add("SuggestTransferStocks_1_OrderQty", "香港仓被订购数量", 20);
                    columnSet.Add("SuggestTransferStocks_1_ConsignQty", "香港仓代销库存数量", 20);
                    columnSet.Add("SuggestTransferStocks_1_AvailableSalesDays", "香港仓可销售天数", 20);
                    columnSet.Add("SuggestTransferStocks_1_PurchaseInQty", "香港采购单据数量", 20);
                    columnSet.Add("SuggestTransferStocks_1_ShiftInQty", "香港移仓在途数量", 20);
                    columnSet.Add("SuggestTransferStocks_1_D1", "香港D1", 20);
                    columnSet.Add("SuggestTransferStocks_1_D2", "香港D2", 20);
                    columnSet.Add("SuggestTransferStocks_1_D3", "香港D3", 20);
                    columnSet.Add("SuggestTransferStocks_1_D4", "香港D4", 20);
                    columnSet.Add("SuggestTransferStocks_1_D5", "香港D5", 20);
                    columnSet.Add("SuggestTransferStocks_1_D6", "香港D6", 20);
                    columnSet.Add("SuggestTransferStocks_1_D7", "香港D7", 20);
                    columnSet.Add("SuggestTransferStocks_1_W1", "香港W1", 20);
                    columnSet.Add("SuggestTransferStocks_1_W2", "香港W2", 20);
                    columnSet.Add("SuggestTransferStocks_1_W3", "香港W3", 20);
                    columnSet.Add("SuggestTransferStocks_1_W4", "香港W4", 20);
                    columnSet.Add("SuggestTransferStocks_1_M1", "香港M1", 20);
                    columnSet.Add("SuggestTransferStocks_1_M2", "香港M2", 20);
                    columnSet.Add("SuggestTransferStocks_1_M3", "香港M3", 20);
                    columnSet.Add("SuggestTransferStocks_1_W1RegionSalesQty", "香港仓覆盖地区W1", 20);
                    columnSet.Add("SuggestTransferStocks_1_M1RegionSalesQty", "香港仓覆盖地区M1", 20);
                    columnSet.Add("SuggestTransferStocks_1_AVGDailySales", "香港仓覆盖区域日均销量", 20);
                    columnSet.Add("SuggestTransferStocks_1_SuggestQty", "香港仓建议备货数量", 20);
                    columnSet.Add("SuggestTransferStocks_1_Lastintime", "香港最后一次采购时间", 20);
                    columnSet.Add("SuggestTransferStocks_1_LastPrice", "香港最后一次采购价格", 20);
                }
                //上海仓库存:
                if (queryFilter.StockSysNo == "53" || queryFilter.StockSysNo == "-999")
                {
                    columnSet.Add("SuggestTransferStocks_2_AvailableQty", "上海可用库存", 20);
                    columnSet.Add("SuggestTransferStocks_2_VirtualQty", "上海虚库库存", 20);
                    columnSet.Add("SuggestTransferStocks_2_OrderQty", "上海仓被订购数量", 20);
                    columnSet.Add("SuggestTransferStocks_2_ConsignQty", "上海仓代销库存数量", 20);
                    columnSet.Add("SuggestTransferStocks_2_AvailableSalesDays", "上海仓可销售天数", 20);
                    columnSet.Add("SuggestTransferStocks_2_PurchaseInQty", "上海采购单据数量", 20);
                    columnSet.Add("SuggestTransferStocks_2_ShiftInQty", "上海移仓在途数量", 20);
                    columnSet.Add("SuggestTransferStocks_2_D1", "上海D1", 20);
                    columnSet.Add("SuggestTransferStocks_2_D2", "上海D2", 20);
                    columnSet.Add("SuggestTransferStocks_2_D3", "上海D3", 20);
                    columnSet.Add("SuggestTransferStocks_2_D4", "上海D4", 20);
                    columnSet.Add("SuggestTransferStocks_2_D5", "上海D5", 20);
                    columnSet.Add("SuggestTransferStocks_2_D6", "上海D6", 20);
                    columnSet.Add("SuggestTransferStocks_2_D7", "上海D7", 20);
                    columnSet.Add("SuggestTransferStocks_2_W1", "上海W1", 20);
                    columnSet.Add("SuggestTransferStocks_2_W2", "上海W2", 20);
                    columnSet.Add("SuggestTransferStocks_2_W3", "上海W3", 20);
                    columnSet.Add("SuggestTransferStocks_2_W4", "上海W4", 20);
                    columnSet.Add("SuggestTransferStocks_2_M1", "上海M1", 20);
                    columnSet.Add("SuggestTransferStocks_2_M2", "上海M2", 20);
                    columnSet.Add("SuggestTransferStocks_2_M3", "上海M3", 20);
                    columnSet.Add("SuggestTransferStocks_2_W1RegionSalesQty", "上海仓覆盖地区W1", 20);
                    columnSet.Add("SuggestTransferStocks_2_M1RegionSalesQty", "上海仓覆盖地区M1", 20);
                    columnSet.Add("SuggestTransferStocks_2_AVGDailySales", "上海仓覆盖区域日均销量", 20);
                    columnSet.Add("SuggestTransferStocks_2_SuggestQty", "上海仓建议备货数量", 20);
                    columnSet.Add("SuggestTransferStocks_2_Lastintime", "上海最后一次采购时间", 20);
                    columnSet.Add("SuggestTransferStocks_2_LastPrice", "上海最后一次采购价格", 20);
                }
                ////成都仓库存:
                //if (queryFilter.StockSysNo == "54" || queryFilter.StockSysNo == "-999")
                //{
                //    columnSet.Add("SuggestTransferStocks_3_AvailableQty", "成都可用库存", 20);
                //    columnSet.Add("SuggestTransferStocks_3_VirtualQty", "成都虚库库存", 20);
                //    columnSet.Add("SuggestTransferStocks_3_OrderQty", "成都仓被订购数量", 20);
                //    columnSet.Add("SuggestTransferStocks_3_ConsignQty", "成都仓代销库存数量", 20);
                //    columnSet.Add("SuggestTransferStocks_3_AvailableSalesDays", "成都仓可销售天数", 20);
                //    columnSet.Add("SuggestTransferStocks_3_PurchaseInQty", "成都采购单据数量", 20);
                //    columnSet.Add("SuggestTransferStocks_3_ShiftInQty", "成都移仓在途数量", 20);
                //    columnSet.Add("SuggestTransferStocks_3_D1", "成都D1", 20);
                //    columnSet.Add("SuggestTransferStocks_3_D2", "成都D2", 20);
                //    columnSet.Add("SuggestTransferStocks_3_D3", "成都D3", 20);
                //    columnSet.Add("SuggestTransferStocks_3_D4", "成都D4", 20);
                //    columnSet.Add("SuggestTransferStocks_3_D5", "成都D5", 20);
                //    columnSet.Add("SuggestTransferStocks_3_D6", "成都D6", 20);
                //    columnSet.Add("SuggestTransferStocks_3_D7", "成都D7", 20);
                //    columnSet.Add("SuggestTransferStocks_3_W1", "成都W1", 20);
                //    columnSet.Add("SuggestTransferStocks_3_W2", "成都W2", 20);
                //    columnSet.Add("SuggestTransferStocks_3_W3", "成都W3", 20);
                //    columnSet.Add("SuggestTransferStocks_3_W4", "成都W4", 20);
                //    columnSet.Add("SuggestTransferStocks_3_M1", "成都M1", 20);
                //    columnSet.Add("SuggestTransferStocks_3_M2", "成都M2", 20);
                //    columnSet.Add("SuggestTransferStocks_3_M3", "成都M3", 20);
                //    columnSet.Add("SuggestTransferStocks_3_W1RegionSalesQty", "成都仓覆盖地区W1", 20);
                //    columnSet.Add("SuggestTransferStocks_3_M1RegionSalesQty", "成都仓覆盖地区M1", 20);
                //    columnSet.Add("SuggestTransferStocks_3_AVGDailySales", "成都仓覆盖区域日均销量", 20);
                //    columnSet.Add("SuggestTransferStocks_3_SuggestQty", "成都仓建议备货数量", 20);
                //    columnSet.Add("SuggestTransferStocks_3_Lastintime", "成都最后一次采购时间", 20);
                //    columnSet.Add("SuggestTransferStocks_3_LastPrice", "成都最后一次采购价格", 20);
                //}
                ////武汉仓库存:
                //if (queryFilter.StockSysNo == "54" || queryFilter.StockSysNo == "-999")
                //{
                //    columnSet.Add("SuggestTransferStocks_4_AvailableQty", "武汉可用库存", 20);
                //    columnSet.Add("SuggestTransferStocks_4_VirtualQty", "武汉虚库库存", 20);
                //    columnSet.Add("SuggestTransferStocks_4_OrderQty", "武汉仓被订购数量", 20);
                //    columnSet.Add("SuggestTransferStocks_4_ConsignQty", "武汉仓代销库存数量", 20);
                //    columnSet.Add("SuggestTransferStocks_4_AvailableSalesDays", "武汉仓可销售天数", 20);
                //    columnSet.Add("SuggestTransferStocks_4_PurchaseInQty", "武汉采购单据数量", 20);
                //    columnSet.Add("SuggestTransferStocks_4_ShiftInQty", "武汉移仓在途数量", 20);
                //    columnSet.Add("SuggestTransferStocks_4_D1", "武汉D1", 20);
                //    columnSet.Add("SuggestTransferStocks_4_D2", "武汉D2", 20);
                //    columnSet.Add("SuggestTransferStocks_4_D3", "武汉D3", 20);
                //    columnSet.Add("SuggestTransferStocks_4_D4", "武汉D4", 20);
                //    columnSet.Add("SuggestTransferStocks_4_D5", "武汉D5", 20);
                //    columnSet.Add("SuggestTransferStocks_4_D6", "武汉D6", 20);
                //    columnSet.Add("SuggestTransferStocks_4_D7", "武汉D7", 20);
                //    columnSet.Add("SuggestTransferStocks_4_W1", "武汉W1", 20);
                //    columnSet.Add("SuggestTransferStocks_4_W2", "武汉W2", 20);
                //    columnSet.Add("SuggestTransferStocks_4_W3", "武汉W3", 20);
                //    columnSet.Add("SuggestTransferStocks_4_W4", "武汉W4", 20);
                //    columnSet.Add("SuggestTransferStocks_4_M1", "武汉M1", 20);
                //    columnSet.Add("SuggestTransferStocks_4_M2", "武汉M2", 20);
                //    columnSet.Add("SuggestTransferStocks_4_M3", "武汉M3", 20);
                //    columnSet.Add("SuggestTransferStocks_4_W1RegionSalesQty", "武汉仓覆盖地区W1", 20);
                //    columnSet.Add("SuggestTransferStocks_4_M1RegionSalesQty", "武汉仓覆盖地区M1", 20);
                //    columnSet.Add("SuggestTransferStocks_4_AVGDailySales", "武汉仓覆盖区域日均销量", 20);
                //    columnSet.Add("SuggestTransferStocks_4_SuggestQty", "武汉仓建议备货数量", 20);
                //    columnSet.Add("SuggestTransferStocks_4_Lastintime", "武汉最后一次采购时间", 20);
                //    columnSet.Add("SuggestTransferStocks_4_LastPrice", "武汉最后一次采购价格", 20);
                //}
                ////昆明仓库存:
                //if (queryFilter.StockSysNo == "60" || queryFilter.StockSysNo == "-999")
                //{
                //    columnSet.Add("SuggestTransferStocks_5_AvailableQty", "昆明可用库存", 20);
                //    columnSet.Add("SuggestTransferStocks_5_VirtualQty", "昆明虚库库存", 20);
                //    columnSet.Add("SuggestTransferStocks_5_OrderQty", "昆明仓被订购数量", 20);
                //    columnSet.Add("SuggestTransferStocks_5_ConsignQty", "昆明仓代销库存数量", 20);
                //    columnSet.Add("SuggestTransferStocks_5_AvailableSalesDays", "昆明仓可销售天数", 20);
                //    columnSet.Add("SuggestTransferStocks_5_PurchaseInQty", "昆明采购单据数量", 20);
                //    columnSet.Add("SuggestTransferStocks_5_ShiftInQty", "昆明移仓在途数量", 20);
                //    columnSet.Add("SuggestTransferStocks_5_D1", "昆明D1", 20);
                //    columnSet.Add("SuggestTransferStocks_5_D2", "昆明D2", 20);
                //    columnSet.Add("SuggestTransferStocks_5_D3", "昆明D3", 20);
                //    columnSet.Add("SuggestTransferStocks_5_D4", "昆明D4", 20);
                //    columnSet.Add("SuggestTransferStocks_5_D5", "昆明D5", 20);
                //    columnSet.Add("SuggestTransferStocks_5_D6", "昆明D6", 20);
                //    columnSet.Add("SuggestTransferStocks_5_D7", "昆明D7", 20);
                //    columnSet.Add("SuggestTransferStocks_5_W1", "昆明W1", 20);
                //    columnSet.Add("SuggestTransferStocks_5_W2", "昆明W2", 20);
                //    columnSet.Add("SuggestTransferStocks_5_W3", "昆明W3", 20);
                //    columnSet.Add("SuggestTransferStocks_5_W4", "昆明W4", 20);
                //    columnSet.Add("SuggestTransferStocks_5_M1", "昆明M1", 20);
                //    columnSet.Add("SuggestTransferStocks_5_M2", "昆明M2", 20);
                //    columnSet.Add("SuggestTransferStocks_5_M3", "昆明M3", 20);
                //    columnSet.Add("SuggestTransferStocks_5_W1RegionSalesQty", "昆明仓覆盖地区W1", 20);
                //    columnSet.Add("SuggestTransferStocks_5_M1RegionSalesQty", "昆明仓覆盖地区M1", 20);
                //    columnSet.Add("SuggestTransferStocks_5_AVGDailySales", "昆明仓覆盖区域日均销量", 20);
                //    columnSet.Add("SuggestTransferStocks_5_SuggestQty", "昆明仓建议备货数量", 20);
                //    columnSet.Add("SuggestTransferStocks_5_Lastintime", "昆明最后一次采购时间", 20);
                //    columnSet.Add("SuggestTransferStocks_5_LastPrice", "昆明最后一次采购价格", 20);
                //}
                ////深圳仓库存:
                //if (queryFilter.StockSysNo == "61" || queryFilter.StockSysNo == "-999")
                //{
                //    columnSet.Add("SuggestTransferStocks_6_AvailableQty", "深圳可用库存", 20);
                //    columnSet.Add("SuggestTransferStocks_6_VirtualQty", "深圳虚库库存", 20);
                //    columnSet.Add("SuggestTransferStocks_6_OrderQty", "深圳仓被订购数量", 20);
                //    columnSet.Add("SuggestTransferStocks_6_ConsignQty", "深圳仓代销库存数量", 20);
                //    columnSet.Add("SuggestTransferStocks_6_AvailableSalesDays", "深圳仓可销售天数", 20);
                //    columnSet.Add("SuggestTransferStocks_6_PurchaseInQty", "深圳采购单据数量", 20);
                //    columnSet.Add("SuggestTransferStocks_6_ShiftInQty", "深圳移仓在途数量", 20);
                //    columnSet.Add("SuggestTransferStocks_6_D1", "深圳D1", 20);
                //    columnSet.Add("SuggestTransferStocks_6_D2", "深圳D2", 20);
                //    columnSet.Add("SuggestTransferStocks_6_D3", "深圳D3", 20);
                //    columnSet.Add("SuggestTransferStocks_6_D4", "深圳D4", 20);
                //    columnSet.Add("SuggestTransferStocks_6_D5", "深圳D5", 20);
                //    columnSet.Add("SuggestTransferStocks_6_D6", "深圳D6", 20);
                //    columnSet.Add("SuggestTransferStocks_6_D7", "深圳D7", 20);
                //    columnSet.Add("SuggestTransferStocks_6_W1", "深圳W1", 20);
                //    columnSet.Add("SuggestTransferStocks_6_W2", "深圳W2", 20);
                //    columnSet.Add("SuggestTransferStocks_6_W3", "深圳W3", 20);
                //    columnSet.Add("SuggestTransferStocks_6_W4", "深圳W4", 20);
                //    columnSet.Add("SuggestTransferStocks_6_M1", "深圳M1", 20);
                //    columnSet.Add("SuggestTransferStocks_6_M2", "深圳M2", 20);
                //    columnSet.Add("SuggestTransferStocks_6_M3", "深圳M3", 20);
                //    columnSet.Add("SuggestTransferStocks_6_W1RegionSalesQty", "深圳仓覆盖地区W1", 20);
                //    columnSet.Add("SuggestTransferStocks_6_M1RegionSalesQty", "深圳仓覆盖地区M1", 20);
                //    columnSet.Add("SuggestTransferStocks_6_AVGDailySales", "深圳仓覆盖区域日均销量", 20);
                //    columnSet.Add("SuggestTransferStocks_6_SuggestQty", "深圳仓建议备货数量", 20);
                //    columnSet.Add("SuggestTransferStocks_6_Lastintime", "深圳最后一次采购时间", 20);
                //    columnSet.Add("SuggestTransferStocks_6_LastPrice", "深圳最后一次采购价格", 20);
                //}
                #endregion

                #endregion

                serviceFacade.ExportExcelForTransferStockingList(exportQueryRequest, new ColumnSet[] { columnSet });
            }
        }

        #endregion

        #region [移仓 ComboxBox]
        private void cmbShiftOutStock_BS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductCenterItemInfoVM selectVM = this.GridSearchResult.SelectedItem as ProductCenterItemInfoVM;
            ComboBox cmb = sender as ComboBox;
            if (null != cmb && null != selectVM)
            {
                if (cmb.SelectedValue.ToString() == "-999")
                {
                    cmb.Tag = 0;
                }
                else
                {
                    foreach (var stock in selectVM.SuggestTransferStocks)
                    {
                        if (stock.WareHouseNumber.ToString() == cmb.SelectedValue.ToString())
                        {
                            cmb.Tag = stock.OutStockShiftQty;
                            break;
                        }
                    }
                }
            }
        }

        private void cmbShiftOutStock_JD_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductCenterItemInfoVM selectVM = this.GridSearchResult.SelectedItem as ProductCenterItemInfoVM;
            ComboBox cmb = sender as ComboBox;
            if (null != cmb && null != selectVM)
            {
                if (cmb.SelectedValue.ToString() == "-999")
                {
                    cmb.Tag = 0;
                }
                else
                {
                    foreach (var stock in selectVM.SuggestTransferStocks)
                    {
                        if (stock.WareHouseNumber.ToString() == cmb.SelectedValue.ToString())
                        {
                            cmb.Tag = stock.OutStockShiftQty;
                            break;
                        }
                    }
                }
            }
        }

        private void cmbShiftOutStock_BJ_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductCenterItemInfoVM selectVM = this.GridSearchResult.SelectedItem as ProductCenterItemInfoVM;
            ComboBox cmb = sender as ComboBox;
            if (null != cmb && null != selectVM)
            {
                if (cmb.SelectedValue.ToString() == "-999")
                {
                    cmb.Tag = 0;
                }
                else
                {
                    foreach (var stock in selectVM.SuggestTransferStocks)
                    {
                        if (stock.WareHouseNumber.ToString() == cmb.SelectedValue.ToString())
                        {
                            cmb.Tag = stock.OutStockShiftQty;
                            break;
                        }
                    }
                }
            }
        }

        private void cmbShiftOutStock_GZ_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductCenterItemInfoVM selectVM = this.GridSearchResult.SelectedItem as ProductCenterItemInfoVM;
            ComboBox cmb = sender as ComboBox;
            if (null != cmb && null != selectVM)
            {
                if (cmb.SelectedValue.ToString() == "-999")
                {
                    cmb.Tag = 0;
                }
                else
                {
                    foreach (var stock in selectVM.SuggestTransferStocks)
                    {
                        if (stock.WareHouseNumber.ToString() == cmb.SelectedValue.ToString())
                        {
                            cmb.Tag = stock.OutStockShiftQty;
                            break;
                        }
                    }
                }
            }
        }

        private void cmbShiftOutStock_CD_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductCenterItemInfoVM selectVM = this.GridSearchResult.SelectedItem as ProductCenterItemInfoVM;
            ComboBox cmb = sender as ComboBox;
            if (null != cmb && null != selectVM)
            {
                if (cmb.SelectedValue.ToString() == "-999")
                {
                    cmb.Tag = 0;
                }
                else
                {
                    foreach (var stock in selectVM.SuggestTransferStocks)
                    {
                        if (stock.WareHouseNumber.ToString() == cmb.SelectedValue.ToString())
                        {
                            cmb.Tag = stock.OutStockShiftQty;
                            break;
                        }
                    }
                }
            }
        }

        private void cmbShiftOutStock_WH_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductCenterItemInfoVM selectVM = this.GridSearchResult.SelectedItem as ProductCenterItemInfoVM;
            ComboBox cmb = sender as ComboBox;
            if (null != cmb && null != selectVM)
            {
                if (cmb.SelectedValue.ToString() == "-999")
                {
                    cmb.Tag = 0;
                }
                else
                {
                    foreach (var stock in selectVM.SuggestTransferStocks)
                    {
                        if (stock.WareHouseNumber.ToString() == cmb.SelectedValue.ToString())
                        {
                            cmb.Tag = stock.OutStockShiftQty;
                            break;
                        }
                    }
                }
            }
        }

        private void cmbShiftOutStock_KM_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ProductCenterItemInfoVM selectVM = this.GridSearchResult.SelectedItem as ProductCenterItemInfoVM;
            ComboBox cmb = sender as ComboBox;
            if (null != cmb && null != selectVM)
            {
                if (cmb.SelectedValue.ToString() == "-999")
                {
                    cmb.Tag = 0;
                }
                else
                {
                    foreach (var stock in selectVM.SuggestTransferStocks)
                    {
                        if (stock.WareHouseNumber.ToString() == cmb.SelectedValue.ToString())
                        {
                            cmb.Tag = stock.OutStockShiftQty;
                            break;
                        }
                    }
                }
            }
        }

        private void cmbShiftOutStock_SZ_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ProductCenterItemInfoVM selectVM = this.GridSearchResult.SelectedItem as ProductCenterItemInfoVM;
            ComboBox cmb = sender as ComboBox;
            if (null != cmb && null != selectVM)
            {
                if (cmb.SelectedValue.ToString() == "-999")
                {
                    cmb.Tag = 0;
                }
                else
                {
                    foreach (var stock in selectVM.SuggestTransferStocks)
                    {
                        if (stock.WareHouseNumber.ToString() == cmb.SelectedValue.ToString())
                        {
                            cmb.Tag = stock.OutStockShiftQty;
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        private void cmbNeedBufferEnable_BJ_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductCenterItemInfoVM selectVM = this.GridSearchResult.SelectedItem as ProductCenterItemInfoVM;
            ComboBox cmb = sender as ComboBox;
            if (null != cmb && null != selectVM)
            {
                foreach (var stock in selectVM.SuggestTransferStocks)
                {
                    if (stock.WareHouseNumber.ToString() == "52")
                    {
                        if (cmb.SelectedValue.ToString() == "Y")//是中转 则 重新计算 分仓的建议备货数量 和总仓的建议备货数量
                        {
                            stock.SuggestQtyDisplay = stock.SuggestQtyZhongZhuan;
                            selectVM.SuggestQtyAllDisplay = selectVM.SuggestQtyAllZhongZhuan;
                        }
                        else
                        {
                            stock.SuggestQtyDisplay = stock.SuggestQty;
                            selectVM.SuggestQtyAllDisplay = selectVM.SuggestQtyAll;
                        }
                        stock.PurchaseQty = stock.SuggestQtyDisplay.ToString();//采购数量默认加载为分仓建议备货数量
                        break;
                    }
                }
            }
        }

        private void cmbNeedBufferEnable_WH_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductCenterItemInfoVM selectVM = this.GridSearchResult.SelectedItem as ProductCenterItemInfoVM;
            ComboBox cmb = sender as ComboBox;
            if (null != cmb && null != selectVM)
            {
                foreach (var stock in selectVM.SuggestTransferStocks)
                {
                    if (stock.WareHouseNumber.ToString() == "55")
                    {
                        if (cmb.SelectedValue.ToString() == "Y")//是中转 则 重新计算 分仓的建议备货数量 和总仓的建议备货数量
                        {
                            stock.SuggestQtyDisplay = stock.SuggestQtyZhongZhuan;
                            selectVM.SuggestQtyAllDisplay = selectVM.SuggestQtyAllZhongZhuan;
                        }
                        else
                        {
                            stock.SuggestQtyDisplay = stock.SuggestQty;
                            selectVM.SuggestQtyAllDisplay = selectVM.SuggestQtyAll;
                        }
                        stock.PurchaseQty = stock.SuggestQtyDisplay.ToString();//采购数量默认加载为分仓建议备货数量
                        break;
                    }
                }
            }
        }

        private void cmbNeedBufferEnable_CD_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductCenterItemInfoVM selectVM = this.GridSearchResult.SelectedItem as ProductCenterItemInfoVM;
            ComboBox cmb = sender as ComboBox;
            if (null != cmb && null != selectVM)
            {
                foreach (var stock in selectVM.SuggestTransferStocks)
                {
                    if (stock.WareHouseNumber.ToString() == "54")
                    {
                        if (cmb.SelectedValue.ToString() == "Y")//是中转 则 重新计算 分仓的建议备货数量 和总仓的建议备货数量
                        {
                            stock.SuggestQtyDisplay = stock.SuggestQtyZhongZhuan;
                            selectVM.SuggestQtyAllDisplay = selectVM.SuggestQtyAllZhongZhuan;
                        }
                        else
                        {
                            stock.SuggestQtyDisplay = stock.SuggestQty;
                            selectVM.SuggestQtyAllDisplay = selectVM.SuggestQtyAll;
                        }
                        stock.PurchaseQty = stock.SuggestQtyDisplay.ToString();//采购数量默认加载为分仓建议备货数量
                        break;
                    }
                }
            }
        }

        private void cmbNeedBufferEnable_GZ_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductCenterItemInfoVM selectVM = this.GridSearchResult.SelectedItem as ProductCenterItemInfoVM;
            ComboBox cmb = sender as ComboBox;
            if (null != cmb && null != selectVM)
            {
                foreach (var stock in selectVM.SuggestTransferStocks)
                {
                    if (stock.WareHouseNumber.ToString() == "53")
                    {
                        if (cmb.SelectedValue.ToString() == "Y")//是中转 则 重新计算 分仓的建议备货数量 和总仓的建议备货数量
                        {
                            stock.SuggestQtyDisplay = stock.SuggestQtyZhongZhuan;
                            selectVM.SuggestQtyAllDisplay = selectVM.SuggestQtyAllZhongZhuan;
                        }
                        else
                        {
                            stock.SuggestQtyDisplay = stock.SuggestQty;
                            selectVM.SuggestQtyAllDisplay = selectVM.SuggestQtyAll;
                        }
                        stock.PurchaseQty = stock.SuggestQtyDisplay.ToString();//采购数量默认加载为分仓建议备货数量
                        break;
                    }
                }
            }
        }

        private void cmbNeedBufferEnable_KM_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ProductCenterItemInfoVM selectVM = this.GridSearchResult.SelectedItem as ProductCenterItemInfoVM;
            ComboBox cmb = sender as ComboBox;
            if (null != cmb && null != selectVM)
            {
                foreach (var stock in selectVM.SuggestTransferStocks)
                {
                    if (stock.WareHouseNumber.ToString() == "60")
                    {
                        if (cmb.SelectedValue.ToString() == "Y")//是中转 则 重新计算 分仓的建议备货数量 和总仓的建议备货数量
                        {
                            stock.SuggestQtyDisplay = stock.SuggestQtyZhongZhuan;
                            selectVM.SuggestQtyAllDisplay = selectVM.SuggestQtyAllZhongZhuan;
                        }
                        else
                        {
                            stock.SuggestQtyDisplay = stock.SuggestQty;
                            selectVM.SuggestQtyAllDisplay = selectVM.SuggestQtyAll;
                        }
                        stock.PurchaseQty = stock.SuggestQtyDisplay.ToString();//采购数量默认加载为分仓建议备货数量
                        break;
                    }
                }
            }
        }

        private void cmbNeedBufferEnable_SZ_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ProductCenterItemInfoVM selectVM = this.GridSearchResult.SelectedItem as ProductCenterItemInfoVM;
            ComboBox cmb = sender as ComboBox;
            if (null != cmb && null != selectVM)
            {
                foreach (var stock in selectVM.SuggestTransferStocks)
                {
                    if (stock.WareHouseNumber.ToString() == "61")
                    {
                        if (cmb.SelectedValue.ToString() == "Y")//是中转 则 重新计算 分仓的建议备货数量 和总仓的建议备货数量
                        {
                            stock.SuggestQtyDisplay = stock.SuggestQtyZhongZhuan;
                            selectVM.SuggestQtyAllDisplay = selectVM.SuggestQtyAllZhongZhuan;
                        }
                        else
                        {
                            stock.SuggestQtyDisplay = stock.SuggestQty;
                            selectVM.SuggestQtyAllDisplay = selectVM.SuggestQtyAll;
                        }
                        stock.PurchaseQty = stock.SuggestQtyDisplay.ToString();//采购数量默认加载为分仓建议备货数量
                        break;
                    }
                }
            }
        }

        #region 加载PM工作指标页面跳转过来的参数  Jack 2012/8/17

        /// <summary>
        /// 加载PM工作指标页面跳转过来的参数  Jack
        /// </summary>
        private void LoadUrlParm()
        {
            if (!string.IsNullOrEmpty(Request.Param))
            {
                reqParmLoadCompletedCount = 0;

                string[] requestParm = this.Request.Param.Split(',');
                if (requestParm != null && requestParm.Length != 0)
                {
                    //queryVM.Category1SysNo = string.IsNullOrEmpty(requestParm[0]) ? null : requestParm[0];
                    //queryVM.Category2SysNo = string.IsNullOrEmpty(requestParm[1]) ? null : requestParm[1];
                    //queryVM.PMSysNo = string.IsNullOrEmpty(requestParm[2]) ? null : requestParm[2];
                    //queryVM.StockSysNo = string.IsNullOrEmpty(requestParm[3]) ? null : requestParm[3];
                    //queryVM.AvailableSaleDaysCompareCode = string.IsNullOrEmpty(requestParm[4]) ? null : requestParm[4];
                    queryVM.AvailableSaleDays = string.IsNullOrEmpty(requestParm[5]) ? null : requestParm[5];
                    //queryVM.DaySalesCountCompareCode = string.IsNullOrEmpty(requestParm[6]) ? null : requestParm[6];
                    queryVM.DaySalesCount = string.IsNullOrEmpty(requestParm[7]) ? null : requestParm[7];
                    reqParmLoadCompletedCount = 2;

                    ucSearchCondition.SetStockDefaultValueHandler = new UCInventoryStockingCenterSearch.SetDefaultValue(ucSearchCondition_SetDefaultValue);
                    ucSearchCondition.ucPM.SetDefaultValueHandler = new Basic.Components.UserControls.PMPicker.UCPMPicker.SetDefaultValue(ucPM_PMSetDefaultValue);
                    ucSearchCondition.ucCategory.SetDefaultValueHandler = new Basic.Components.UserControls.CategoryPicker.UCCategoryPicker.SetDefaultValue(ucCategory_SetDefaultValue);
                    ucSearchCondition.SetCompareDefaultValueHandler = new UCInventoryStockingCenterSearch.SetDefaultValue(ucCompare_SetDefaultValue);
                }
            }
        }

        void SearchReqAuto()
        {
            reqParmLoadCompletedCount++;

            if (reqParmLoadCompletedCount == pmConditionCount)
                SearchExecute();
        }

        void ucPM_PMSetDefaultValue(object sender)
        {
            ucSearchCondition.ucPM.SetDefaultValueHandler = null;
            string tStr = GetRequestParm(2);
            if (!string.IsNullOrEmpty(tStr))
                ucSearchCondition.ucPM.SelectedPMSysNo = int.Parse(tStr);

            SearchReqAuto();
        }

        void ucSearchCondition_SetDefaultValue(object sender)
        {
            ucSearchCondition.SetStockDefaultValueHandler = null;
            string tStr = GetRequestParm(3);
            if (!string.IsNullOrEmpty(tStr))
                ucSearchCondition.cmbStock.SelectedValue = tStr;

            SearchReqAuto();
        }

        void ucCategory_SetDefaultValue(params object[] senders)
        {
            ucSearchCondition.ucCategory.SetDefaultValueHandler = null;
            string tStr = GetRequestParm(0);
            if (!string.IsNullOrEmpty(tStr))
                (senders[0] as Combox).SelectedValue = tStr;
            SearchReqAuto();

            tStr = GetRequestParm(1);
            if (!string.IsNullOrEmpty(tStr))
                (senders[1] as Combox).SelectedValue = tStr;
            SearchReqAuto();
        }

        void ucCompare_SetDefaultValue(params object[] senders)
        {
            ucSearchCondition.SetCompareDefaultValueHandler = null;
            string tStr = GetRequestParm(4);
            if (!string.IsNullOrEmpty(tStr))
                (senders[0] as ComboBox).SelectedValue = tStr;
            SearchReqAuto();

            tStr = GetRequestParm(6);
            if (!string.IsNullOrEmpty(tStr))
                (senders[1] as ComboBox).SelectedValue = tStr;
            SearchReqAuto();
        }

        private string GetRequestParm(int parmIndex)
        {
            string[] requestParm = this.Request.Param.Split(',');

            if (requestParm != null && (requestParm.Length - 1) >= parmIndex)
            {
                return requestParm[parmIndex];
            }
            return string.Empty;
        }

        #endregion

        #region 相关链接

        private void hlbtnItemCode_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            //Ocean.20130514, Move to ControlPanelConfiguration
            string urlFormat = this.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_WebSiteProductPreviewUrl);
            string url = String.Format(urlFormat, btn.CommandParameter);
            this.Window.Navigate(url, null, true);
        }

        private void hlbtn_ItemName_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            string url = String.Format(ConstValue.IM_ProductMaintainUrlFormat, btn.CommandParameter);
            this.Window.Navigate(url, null, true);
        }

        private void hlbtn_AllStockAVGDailySales_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            string url = String.Format(ConstValue.Inventory_InventoryQueryFormat, btn.CommandParameter);
            this.Window.Navigate(url, null, true);
        }

        private void hlbtn_AllStockAvailableSalesDays_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            string url = String.Format(ConstValue.Inventory_InventoryQueryFormat, btn.CommandParameter);
            this.Window.Navigate(url, null, true);
        }

        private void hlbtn_VirtualQty_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            string url = String.Format(ConstValue.Inventory_VirtualRequestMaintainFormat, btn.CommandParameter);
            this.Window.Navigate(url, null, true);
        }

        private void hlbtn_PurchaseQty_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            string url = String.Format(ConstValue.PO_PurchaseOrderQuery, btn.CommandParameter);
            this.Window.Navigate(url, null, true);
        }

        private void hlbtn_VirtualPrice_Click(object sender, RoutedEventArgs e)
        {
            //链接到PO - 查看商品采购历史 页面 ：          
            var info = (sender as HyperlinkButton).DataContext as ProductCenterItemInfoVM;
            string url = string.Format(ConstValue.IM_ProductPurchaseHistoryUrlFormat, info.ItemSysNumber + "|" + info.ItemCode);
            Window.Navigate(url, null, true);
        }

        private void hlbtn_JDPrice_Click(object sender, RoutedEventArgs e)
        {
            var info = (sender as HyperlinkButton).DataContext as ProductCenterItemInfoVM;
            //Ocean.20130514, Move to ControlPanelConfiguration
            string urlFormat = this.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_JingDongPriceUrl);
            Window.Navigate(string.Format(urlFormat, info.JDItemNumber), null, true);
        }

        private void hlbtn_CurrentPrice_Click(object sender, RoutedEventArgs e)
        {
            var info = (sender as HyperlinkButton).DataContext as ProductCenterItemInfoVM;
            string url = string.Format(ConstValue.IM_ProductMaintainUrlFormat, info.ItemSysNumber);
            Window.Navigate(url, null, true);
        }

        private void hlbtn_UnmarketableQty_Click(object sender, RoutedEventArgs e)
        {
            var info = (sender as HyperlinkButton).DataContext as ProductCenterItemInfoVM;
            string url = string.Format(ConstValue.Inventory_UnmarketableInventoryQuery, info.ItemSysNumber);
            Window.Navigate(url, null, true);
        }

        #endregion
    }
}
