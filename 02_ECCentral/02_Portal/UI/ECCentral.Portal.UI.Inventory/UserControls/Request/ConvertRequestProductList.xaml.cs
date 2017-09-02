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
using System.Text.RegularExpressions;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.QueryFilter.Inventory;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Inventory.UserControls.Inventory;
using ECCentral.Portal.UI.Inventory.Models.Inventory;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Inventory.UserControls
{
    public partial class ConvertRequestProductList : UserControl
    {
        private List<ConvertRequestItemVM> itemList;
        public List<ConvertRequestItemVM> ItemList
        {
            get
            {
                return itemList;
            }
            set
            {

                itemList = value;

                dgProductList.ItemsSource = itemList;
                SetTotalCost();
                if (itemList != null)
                {
                    itemList.ForEach(item =>
                    {
                        item.QuantityOrCostChanged = SetTotalCost;
                    });
                }
            }
        }
        OtherDomainDataFacade OtherDomainDataFacade;
        InventoryQueryFacade InventoryQueryFacade;
        private List<ConvertRequestItemVM> RequestItemList
        {
            get
            {
                return RequestVM.ConvertItemInfoList;
            }
        }

        private ConvertRequestVM requestVM;
        public ConvertRequestVM RequestVM
        {
            get
            {
                return requestVM;
            }
            set
            {
                requestVM = value;
                if (requestVM != null)
                {
                    btnAdd.IsEnabled = requestVM.RequestStatus == ConvertRequestStatus.Origin;
                }
            }
        }

        public ConvertProductType ConvertType
        {
            get;
            set;
        }
        public decimal TotalCost
        {
            get
            {
                if (ItemList != null && ItemList.Count > 0)
                {
                    return ItemList.Sum(item => item.ConvertQuantity * item.ConvertUnitCost);
                }
                return 0;
            }
        }

        /// <summary>
        /// 当前打开的Tab页面
        /// </summary>
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public IPage Page
        { get; set; }

        public ConvertRequestProductList()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(ConvertRequestProductList_Loaded);
        }

        void ConvertRequestProductList_Loaded(object sender, RoutedEventArgs e)
        {
            OtherDomainDataFacade = new Facades.OtherDomainDataFacade(Page);
            InventoryQueryFacade = new InventoryQueryFacade(Page);
            ItemList = new List<ConvertRequestItemVM>();
            Loaded -= new RoutedEventHandler(ConvertRequestProductList_Loaded);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!RequestVM.StockSysNo.HasValue)
            {
                CurrentWindow.Alert("请选择转换商品所在仓库。");
                return;
            }

            UCProductBatch batch = new UCProductBatch();
            batch.IsCreateMode = true;
            batch.PType = Models.Request.PageType.Convert;
            batch.ConvertType = this.ConvertType;
            if(RequestVM.StockSysNo.HasValue)
            batch.StockSysNo = RequestVM.StockSysNo.Value;
            batch.IsNotLend_Return = true;

            IDialog dialog = CurrentWindow.ShowDialog("批次信息", batch, (obj, args) =>
            {
                ProductVMAndBillInfo productList = args.Data as ProductVMAndBillInfo;

                if (null != productList && productList.ProductVM != null)
                {
                    productList.ProductVM.ForEach(p =>
                    {
                        ConvertRequestItemVM vm = RequestItemList.FirstOrDefault(item =>
                        {
                            return item.ProductSysNo == p.SysNo;
                        });
                        if (vm == null)
                        {
                            string errorMessage = "对不起，您没有权限访问{0}商品!";
                            InventoryQueryFilter queryFilter = new InventoryQueryFilter();
                            queryFilter.ProductSysNo = p.SysNo;
                            queryFilter.UserName = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.LoginName;
                            queryFilter.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                            queryFilter.UserSysNo = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.UserSysNo;
                            //判断改商品是否属于当前PM

                            int quantity = 1;
                            if (p.IsHasBatch == 1)
                            {
                                quantity = (from s in p.ProductBatchLst select s.Quantity).Sum();
                            }
                            else if (p.IsHasBatch == 0)
                            {
                                quantity = productList.Quantity;
                            }

                            if (!AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_SeniorPM_Query))
                            {
                                InventoryQueryFacade.CheckOperateRightForCurrentUser(queryFilter, (Innerogj, innerArgs) =>
                                {
                                    if (!innerArgs.FaultsHandle())
                                    {
                                        if (!innerArgs.Result)
                                        {
                                            Page.Context.Window.Alert(string.Format(errorMessage, p.ProductID));
                                            return;
                                        }
                                        else
                                        {
                                            vm = new ConvertRequestItemVM
                                            {
                                                ProductSysNo = p.SysNo,
                                                ConvertQuantity = quantity,
                                                ConvertType = ConvertType,
                                                ConvertUnitCost = p.UnitCost,
                                                ConvertUnitCostWithoutTax = p.UnitCostWithoutTax,
                                                ProductName = p.ProductName,
                                                ProductID = p.ProductID,
                                                BatchDetailsInfoList = EntityConverter<BatchInfoVM, ProductBatchInfoVM>.Convert(p.ProductBatchLst),
                                                IsHasBatch = p.IsHasBatch,
                                                RequestStatus = ConvertRequestStatus.Origin
                                            };
                                            vm.QuantityOrCostChanged = SetTotalCost;
                                            ItemList.Add(vm);
                                            RequestItemList.Add(vm);
                                            dgProductList.ItemsSource = ItemList;
                                            SetTotalCost();
                                        }
                                    }
                                });
                            }
                            else
                            {
                                vm = new ConvertRequestItemVM
                                {
                                    ProductSysNo = p.SysNo,
                                    ConvertQuantity = quantity,
                                    ConvertType = ConvertType,
                                    ConvertUnitCost = p.UnitCost,
                                    ConvertUnitCostWithoutTax = p.UnitCostWithoutTax,
                                    ProductName = p.ProductName,
                                    ProductID = p.ProductID,
                                    BatchDetailsInfoList = EntityConverter<BatchInfoVM, ProductBatchInfoVM>.Convert(p.ProductBatchLst),
                                    IsHasBatch = p.IsHasBatch,
                                    RequestStatus = ConvertRequestStatus.Origin
                                };
                                vm.QuantityOrCostChanged = SetTotalCost;
                                ItemList.Add(vm);
                                RequestItemList.Add(vm);
                                dgProductList.ItemsSource = ItemList;
                                SetTotalCost();
                            }
                        }
                        else
                        {
                            Page.Context.Window.Alert(String.Format(vm.ConvertType == ConvertProductType.Source ? "商品:{0},已经存在于源商品中" : "商品:{0},已经存在于目标商品中", vm.ProductID));
                        }
                    });
                }
            });

            batch.DialogHandler = dialog;
        }

        private void hlbtnDelete_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            ConvertRequestItemVM vm = btn.DataContext as ConvertRequestItemVM;
            ItemList.Remove(vm);
            RequestItemList.Remove(vm);
            dgProductList.ItemsSource = ItemList;
            SetTotalCost();
        }

        private void hlbtnEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            ConvertRequestItemVM selected = this.dgProductList.SelectedItem as ConvertRequestItemVM;
            ConvertRequestItemVM seleced = RequestItemList.Where(p => p.ProductSysNo == selected.ProductSysNo).FirstOrDefault();
            #region Dialog

            UCProductBatch batch = new UCProductBatch(seleced.ProductSysNo.Value.ToString(), seleced.ProductID, seleced.HasBatchInfo, seleced.BatchDetailsInfoList);
            batch.IsCreateMode = false;
            batch.PType = Models.Request.PageType.Convert;
            batch.IsNotLend_Return = true;
            batch.StockSysNo = RequestVM.StockSysNo;
            batch.ProductSysNo = selected.ProductSysNo.Value.ToString();
            batch.ProductID = selected.ProductID;
            batch.OperationQuantity = seleced.ConvertQuantity;
            batch.ConverterCost = seleced.ConvertUnitCost;
            batch.ConvertType = this.ConvertType;

            IDialog dialog = CurrentWindow.ShowDialog("批次信息", batch, (obj, args) =>
            {
                ProductVMAndBillInfo productList = args.Data as ProductVMAndBillInfo;

                if (null != productList && productList.ProductVM != null)
                {
                    productList.ProductVM.ForEach(p =>
                    {
                        ConvertRequestItemVM vm = RequestItemList.FirstOrDefault(item =>
                        {
                            return item.ProductSysNo == p.SysNo;
                        });
                        if (vm == null)
                        {
                            string errorMessage = "对不起，您没有权限访问{0}商品!";
                            InventoryQueryFilter queryFilter = new InventoryQueryFilter();
                            queryFilter.ProductSysNo = p.SysNo;
                            queryFilter.UserName = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.LoginName;
                            queryFilter.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                            queryFilter.UserSysNo = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.UserSysNo;
                            //判断改商品是否属于当前PM

                            int quantity = 1;
                            if (p.IsHasBatch == 1)
                            {
                                quantity = (from s in p.ProductBatchLst select s.Quantity).Sum();
                            }
                            else if (p.IsHasBatch == 0)
                            {
                                quantity = productList.Quantity;
                            }

                            if (!AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_SeniorPM_Query))
                            {
                                InventoryQueryFacade.CheckOperateRightForCurrentUser(queryFilter, (Innerogj, innerArgs) =>
                                {
                                    if (!innerArgs.FaultsHandle())
                                    {
                                        if (!innerArgs.Result)
                                        {
                                            Page.Context.Window.Alert(string.Format(errorMessage, p.ProductID));
                                            return;
                                        }
                                        else
                                        {
                                            vm = new ConvertRequestItemVM
                                            {
                                                ProductSysNo = p.SysNo,
                                                ConvertQuantity = quantity,
                                                ConvertType = ConvertType,
                                                ConvertUnitCost = p.UnitCost,
                                                ConvertUnitCostWithoutTax = p.UnitCostWithoutTax,
                                                ProductName = p.ProductName,
                                                ProductID = p.ProductID,
                                                BatchDetailsInfoList = EntityConverter<BatchInfoVM, ProductBatchInfoVM>.Convert(p.ProductBatchLst),
                                                IsHasBatch = p.IsHasBatch,
                                                RequestStatus = ConvertRequestStatus.Origin
                                            };
                                            vm.QuantityOrCostChanged = SetTotalCost;
                                            ItemList.Add(vm);
                                            RequestItemList.Add(vm);
                                            dgProductList.ItemsSource = ItemList;
                                            SetTotalCost();
                                        }
                                    }
                                });
                            }
                            else
                            {
                                vm = new ConvertRequestItemVM
                                {
                                    ProductSysNo = p.SysNo,
                                    ConvertQuantity = quantity,
                                    ConvertType = ConvertType,
                                    ConvertUnitCost = p.UnitCost,
                                    ConvertUnitCostWithoutTax = p.UnitCostWithoutTax,
                                    ProductName = p.ProductName,
                                    ProductID = p.ProductID,
                                    BatchDetailsInfoList = EntityConverter<BatchInfoVM, ProductBatchInfoVM>.Convert(p.ProductBatchLst),
                                    IsHasBatch = p.IsHasBatch,
                                    RequestStatus = ConvertRequestStatus.Origin
                                };
                                vm.QuantityOrCostChanged = SetTotalCost;
                                ItemList.Add(vm);
                                RequestItemList.Add(vm);
                                dgProductList.ItemsSource = ItemList;
                                SetTotalCost();
                            }
                        }
                        else
                        {
                            Page.Context.Window.Alert(String.Format(vm.ConvertType == ConvertProductType.Source ? "商品:{0},已经存在于源商品中" : "商品:{0},已经存在于目标商品中", vm.ProductID));
                        }
                    });
                }
            });

            batch.DialogHandler = dialog;

            #endregion

            SetTotalCost();
        }

        private void SetTotalCost()
        {
            txtProductCostTotal.Text = TotalCost.ToString(".00");
        }

        private void txtQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            txt.Text = txt.Text.Trim();
            //ConvertRequestItemVM vm = txt.DataContext as ConvertRequestItemVM;
            if (!Regex.IsMatch(txt.Text, @"^[1-9](\d{0,5})$"))
            {
                if (Regex.IsMatch(txt.Text, @"^\d+$"))
                {
                    txt.Text = txt.Text.Length > 6 ? txt.Text.Substring(0, 6) : txt.Text;
                }
                else
                {
                    txt.Text = "0";
                }
            }
        }

        private void txtUnitCost_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            txt.Text = txt.Text.Trim();
            ConvertRequestItemVM vm = txt.DataContext as ConvertRequestItemVM;
            if (!Regex.IsMatch(txt.Text, @"^\d\d{0,5}?(\.|(\.\d{0,2}0*))?$"))
            {
                if (Regex.IsMatch(txt.Text, @"^\d*(\.\d*)?$"))
                {
                    int i = txt.Text.IndexOf(".");
                    string d1 = txt.Text;
                    string d2 = "";
                    if (i >= 0 && txt.Text.Length > i + 1)
                    {
                        d1 = txt.Text.Substring(0, i + 1);
                        d1 = d1.Length > 7 ? d1.Substring(d1.Length - 7, 7) : d1;
                        d2 = txt.Text.Length > i + 2 ? txt.Text.Substring(i + 1, 2) : txt.Text.Substring(i + 1, txt.Text.Length - i - 1);
                    }
                    else
                    {
                        d1 = d1.Length > 6 ? d1.Substring(d1.Length - 6, 6) : d1;
                    }
                    txt.Text = d1 + d2;
                }
                else
                {
                    txt.Text = "0";
                }
            }
        }
    }
}
