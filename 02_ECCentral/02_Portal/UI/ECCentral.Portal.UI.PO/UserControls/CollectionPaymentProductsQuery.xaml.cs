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
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.PO.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Service.PO.Restful.RequestMsg;
using ECCentral.QueryFilter.PO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class CollectionPaymentProductsQuery : UserControl
    {
        public IDialog Dialog { get; set; }

        public ConsignSettlementFacade serviceFacade;
        public int? SettleSysNo;
        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public IPage CurrentPage
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        public List<CollectionPaymentItemInfoVM> listProductsVM;
        public List<CollectionPaymentItemInfoVM> OldItemVM;
        public ConsignSettlementProductsQueryFilter RequestMsg { get; set; }
        public ConsignQueryVM queryVM;

        public CollectionPaymentProductsQuery(int? settleSysNo, int? stockSysNo, string stockID, int? vendorSysNo, string vendorName, List<CollectionPaymentItemInfoVM> itemVM)
        {
            InitializeComponent();
            queryVM = new ConsignQueryVM();
            listProductsVM = new List<CollectionPaymentItemInfoVM>();
            OldItemVM = new List<CollectionPaymentItemInfoVM>();

            this.SettleSysNo = settleSysNo;
            queryVM.StockSysNo = stockSysNo;
            queryVM.VendorSysNo = vendorSysNo;
            queryVM.VendorName = vendorName;
            OldItemVM = itemVM;
            this.txtVendorName.Text = vendorName;
            this.txtVendorSysNo.Text = vendorSysNo.Value.ToString();
            this.Loaded += new RoutedEventHandler(SettledProductsQuery_Loaded);
        }

        void SettledProductsQuery_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= SettledProductsQuery_Loaded;
            this.DataContext = queryVM;
            serviceFacade = new ConsignSettlementFacade(CurrentPage);
            RequestMsg = new ConsignSettlementProductsQueryFilter();
        }

        #region [Events]

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //查询Product:
            BuildRequestMessage();
            this.QueryResultGrid.Bind();
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            RequestMsg.PageInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            listProductsVM.Clear();
            serviceFacade.GetConsignSettlmentProductList(RequestMsg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var productList = args.Result.Rows;
                int totalCount = args.Result.TotalCount;

                this.QueryResultGrid.TotalCount = totalCount;
                int i = 0;
                if (null != productList)
                {
                    listProductsVM = DynamicConverter<CollectionPaymentItemInfoVM>.ConvertToVMList(productList);
                    foreach (var item in productList)
                    {

                        listProductsVM[i].VendorInfo.VendorBasicInfo.VendorNameLocal = item["VendorInfo.VendorBasicInfo.VendorNameLocal"].ToString();
                        listProductsVM[i].ConsignToAccLogInfo.LogSysNo = item["ConsignToAccLogInfo.LogSysNo"];
                        listProductsVM[i].POConsignToAccLogSysNo = item["ConsignToAccLogInfo.LogSysNo"];
                        listProductsVM[i].ConsignToAccLogInfo.SettleType = item["SettleType"];
                        listProductsVM[i].SettlePercentage = item["ConsignToAccLogInfo.SettlePercentage"] == null ? null : item["ConsignToAccLogInfo.SettlePercentage"].ToString();
                        listProductsVM[i].ConsignToAccLogInfo.SalePrice = item["ConsignToAccLogInfo.SalePrice"];
                        listProductsVM[i].ConsignToAccLogInfo.Point = item["ConsignToAccLogInfo.Point"];
                        listProductsVM[i].ConsignToAccLogInfo.MinCommission = item["ConsignToAccLogInfo.MinCommission"];
                        listProductsVM[i].ConsignToAccLogInfo.Cost = item["ConsignToAccLogInfo.Cost"] == null ? null : item["ConsignToAccLogInfo.Cost"].ToString();
                        listProductsVM[i].ConsignToAccLogInfo.CreateCost = item["ConsignToAccLogInfo.CreateCost"];
                        listProductsVM[i].ConsignToAccLogInfo.ProductQuantity = item["ConsignToAccLogInfo.ProductQuantity"];
                        //listProductsVM[i].ConsignToAccLogInfo.RateMargin = item["ConsignToAccLogInfo.RateMargin"];
                        //listProductsVM[i].ConsignToAccLogInfo.RateMarginTotal = item["ConsignToAccLogInfo.RateMarginTotal"];
                        //listProductsVM[i].ConsignToAccLogInfo.CountMany = item["ConsignToAccLogInfo.CountMany"];
                        listProductsVM[i].ConsignToAccLogInfo.ConsignToAccStatus = item["ConsignToAccLogInfo.ConsignToAccStatus"];

                        listProductsVM[i].ConsignToAccLogInfo.StockName = item["ConsignToAccLogInfo.StockName"];
                        listProductsVM[i].ConsignToAccLogInfo.StockSysNo = item["ConsignToAccLogInfo.StockSysNo"];
                        listProductsVM[i].StockSysNo = item["ConsignToAccLogInfo.StockSysNo"];
                        ++i;
                    }
                }
                this.QueryResultGrid.ItemsSource = listProductsVM;
            });
        }

        private void btnAddSettleItems_Click(object sender, RoutedEventArgs e)
        {
            //添加结算商品:
            int selectCount = 0;
            listProductsVM.ForEach(x =>
            {
                if (x.IsCheckedItem)
                {
                    selectCount++;
                }
            });
            if (selectCount <= 0)
            {
                CurrentWindow.Alert(ResSettledProductsRuleQuery.InfoMsg_CheckProducts);
                return;
            }
            if (CheckMaxinumItemsCount())
            {
                this.listProductsVM.Where(i => i.IsCheckedItem == true).ToList().ForEach(x =>
                {
                    x.IsCheckedItem = false;
                    if (SettleSysNo.HasValue)
                    {
                        x.SettleSysNo = SettleSysNo.Value;
                    }
                    //添加进现有的List中(不重复添加:):
                    CollectionPaymentItemInfoVM getExistItem = OldItemVM.SingleOrDefault(i => i.ConsignToAccLogInfo.LogSysNo == x.ConsignToAccLogInfo.LogSysNo);
                    if (getExistItem == null)
                    {
                        x.IsSettleCostTextBoxReadOnly = x.SettleType == SettleType.O ? false : true;
                        x.IsSettlePercentageTextBoxReadOnly = x.SettleType == SettleType.P ? false : true;
                        x.SettlePercentageTextBoxVisibility = x.SettleType == SettleType.P ? Visibility.Visible : Visibility.Collapsed;
                        //毛利:
                        x.ConsignToAccLogInfo.RateMargin = x.ConsignToAccLogInfo.SalePrice.ToDecimal() - x.ConsignToAccLogInfo.Cost.ToDecimal();
                        //毛利总额 ：
                        x.ConsignToAccLogInfo.RateMarginTotal = x.ConsignToAccLogInfo.RateMargin.ToDecimal() * x.ConsignToAccLogInfo.ProductQuantity.ToInteger();
                        OldItemVM.Add(x);
                    }
                    else
                    {
                        if (getExistItem.SettleSysNo == -1)
                        {
                            getExistItem.SettleSysNo = null;
                            getExistItem.Cost = string.IsNullOrEmpty(x.ConsignToAccLogInfo.Cost) ? (decimal?)null : x.ConsignToAccLogInfo.Cost.ToDecimal();
                            getExistItem.SettleRuleSysNo = x.SettleRuleSysNo;
                            getExistItem.SettleRuleName = x.SettleRuleName;
                        }
                    }
                });

                this.Dialog.ResultArgs.Data = OldItemVM;
                this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                Dialog.Close(true);
            }
        }

        private void btnAddAllSettleItems_Click(object sender, RoutedEventArgs e)
        {
            //全选添加结算商品:
            RequestMsg.PageInfo = new QueryFilter.Common.PagingInfo()
           {
               PageIndex = 0,
               PageSize = int.MaxValue
           };
            listProductsVM.Clear();
            serviceFacade.GetConsignSettlmentProductList(RequestMsg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var productList = args.Result.Rows;
                int i = 0;
                if (null != productList)
                {
                    listProductsVM = DynamicConverter<CollectionPaymentItemInfoVM>.ConvertToVMList(productList);
                    foreach (var item in productList)
                    {

                        listProductsVM[i].VendorInfo.VendorBasicInfo.VendorNameLocal = item["VendorInfo.VendorBasicInfo.VendorNameLocal"].ToString();
                        listProductsVM[i].ConsignToAccLogInfo.LogSysNo = item["ConsignToAccLogInfo.LogSysNo"];
                        listProductsVM[i].POConsignToAccLogSysNo = item["ConsignToAccLogInfo.LogSysNo"];
                        listProductsVM[i].ConsignToAccLogInfo.SettleType = item["SettleType"];
                        listProductsVM[i].SettlePercentage = item["ConsignToAccLogInfo.SettlePercentage"] == null ? null : item["ConsignToAccLogInfo.SettlePercentage"].ToString();
                        listProductsVM[i].ConsignToAccLogInfo.SalePrice = item["ConsignToAccLogInfo.SalePrice"];
                        listProductsVM[i].ConsignToAccLogInfo.Point = item["ConsignToAccLogInfo.Point"];
                        listProductsVM[i].ConsignToAccLogInfo.MinCommission = item["ConsignToAccLogInfo.MinCommission"];
                        listProductsVM[i].ConsignToAccLogInfo.Cost = item["ConsignToAccLogInfo.Cost"] == null ? null : item["ConsignToAccLogInfo.Cost"].ToString();
                        listProductsVM[i].ConsignToAccLogInfo.CreateCost = item["ConsignToAccLogInfo.CreateCost"];
                        listProductsVM[i].ConsignToAccLogInfo.ProductQuantity = item["ConsignToAccLogInfo.ProductQuantity"];
                        listProductsVM[i].ConsignToAccLogInfo.RateMargin = item["ConsignToAccLogInfo.RateMargin"];
                        listProductsVM[i].ConsignToAccLogInfo.RateMarginTotal = item["ConsignToAccLogInfo.RateMarginTotal"];
                        listProductsVM[i].ConsignToAccLogInfo.CountMany = item["ConsignToAccLogInfo.CountMany"];
                        listProductsVM[i].ConsignToAccLogInfo.ConsignToAccStatus = item["ConsignToAccLogInfo.ConsignToAccStatus"];

                        listProductsVM[i].ConsignToAccLogInfo.StockName = item["ConsignToAccLogInfo.StockName"];
                        listProductsVM[i].ConsignToAccLogInfo.StockSysNo = item["ConsignToAccLogInfo.StockSysNo"];
                        listProductsVM[i].StockSysNo = item["ConsignToAccLogInfo.StockSysNo"];
                        ++i;
                    }
                }
                listProductsVM.ForEach(x => { x.IsCheckedItem = true; });
                btnAddSettleItems_Click(null, null);
            });

        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (null != chk)
            {

                if (null != this.QueryResultGrid.ItemsSource)
                {
                    foreach (var item in this.QueryResultGrid.ItemsSource)
                    {
                        if (item is CollectionPaymentItemInfoVM)
                        {
                            if (chk.IsChecked == true)
                            {
                                if (!((CollectionPaymentItemInfoVM)item).IsCheckedItem)
                                {
                                    ((CollectionPaymentItemInfoVM)item).IsCheckedItem = true;
                                }
                            }
                            else
                            {
                                if (((CollectionPaymentItemInfoVM)item).IsCheckedItem)
                                {
                                    ((CollectionPaymentItemInfoVM)item).IsCheckedItem = false;
                                }
                            }

                        }
                    }
                }
            }
        }
        #endregion

        private void BuildRequestMessage()
        {
            RequestMsg.ProductSysNo = string.IsNullOrEmpty(ucProduct.ProductSysNo) ? (int?)null : int.Parse(ucProduct.ProductSysNo);
            RequestMsg.VendorSysNo = !string.IsNullOrEmpty(txtVendorSysNo.Text) ? Convert.ToInt32(txtVendorSysNo.Text) : (int?)null;
            RequestMsg.CreateDateFrom = drCreateDate.SelectedDateStart;
            RequestMsg.CreateDateTo = drCreateDate.SelectedDateEnd;
            RequestMsg.StockSysNo = ucStock.SelectedStockSysNo;//!string.IsNullOrEmpty(ucStock.SelectedStockSysNo) ? Convert.ToInt32(ucStock.SelectedStockSysNo) : (int?)null;
            RequestMsg.Category1SysNo = ucCategory.ChooseCategory1SysNo.HasValue ? ucCategory.ChooseCategory1SysNo.Value : (int?)null;
            RequestMsg.Category2SysNo = ucCategory.ChooseCategory2SysNo.HasValue ? ucCategory.ChooseCategory2SysNo.Value : (int?)null;
            RequestMsg.Category3SysNo = ucCategory.ChooseCategory3SysNo.HasValue ? ucCategory.ChooseCategory3SysNo.Value : (int?)null;
            RequestMsg.BrandSysNo = !string.IsNullOrEmpty(ucBrand.SelectedBrandSysNo) ? Convert.ToInt32(ucBrand.SelectedBrandSysNo) : (int?)null;
            RequestMsg.PMSysNo = null != ucPM.SelectedPMSysNo && !string.IsNullOrEmpty(ucPM.SelectedPMSysNo.ToString()) ? Convert.ToInt32(ucPM.SelectedPMSysNo) : (int?)null;
            RequestMsg.IsConsign = 4;
        }

        //检查记录的最大条数:
        private bool CheckMaxinumItemsCount()
        {
            int currentNumber = this.OldItemVM == null ? 0 : this.OldItemVM.Count;
            if (null != OldItemVM)
            {
                foreach (var item in OldItemVM)
                {
                    if (item.SettleSysNo == -1)
                    {
                        currentNumber--;
                    }
                }
            }

            foreach (var newItem in listProductsVM)
            {
                if (OldItemVM.SingleOrDefault(i => i.ConsignToAccLogInfo.LogSysNo == newItem.ConsignToAccLogInfo.LogSysNo) == null)
                {
                    currentNumber++;
                }
            }

            if (currentNumber > 200)
            {
                CurrentWindow.Alert(string.Format(ResSettledProductsRuleQuery.InfoMsg_MaxConsignToAccCount, 200));
                return false;
            }
            else
            {
                return true;
            }
        }


    }

}
