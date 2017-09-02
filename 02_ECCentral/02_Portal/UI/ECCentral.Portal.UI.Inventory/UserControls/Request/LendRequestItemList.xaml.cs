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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities;

using ECCentral.Portal.Basic;
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.UI.Inventory.UserControls.Inventory;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Models.Inventory;

namespace ECCentral.Portal.UI.Inventory.UserControls
{
    public partial class LendRequestItemList : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }

        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        private IPage CurrentPage
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        #region 初始化

        public LendRequestItemList()
        {
            InitializeComponent();
        }

        #endregion

        #region 内部事件处理


        #endregion

        private void hlbtnDelete_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            LendRequestItemVM vm = btn.DataContext as LendRequestItemVM;
            LendRequestVM RequestVM = this.DataContext as LendRequestVM;
            RequestVM.LendItemInfoList.Remove(vm);
            this.dgProductList.ItemsSource = RequestVM.LendItemInfoList;
        }

        private void ShowEditModeDialog(object sender, RoutedEventArgs e,string title,bool isNotReturn)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            List<LendRequestItemVM> itemSource = this.dgProductList.ItemsSource as List<LendRequestItemVM>;
            LendRequestItemVM selected = this.dgProductList.SelectedItem as LendRequestItemVM;

            LendRequestVM RequestVM = this.DataContext as LendRequestVM;
            LendRequestItemVM seleced = RequestVM.LendItemInfoList.Where(p => p.ProductSysNo == selected.ProductSysNo).FirstOrDefault();

            UCProductBatch batch = new UCProductBatch(seleced.ProductSysNo.Value.ToString(), seleced.ProductID, selected.HasBatchInfo, seleced.BatchDetailsInfoList);
             

            batch.StockSysNo = RequestVM.StockSysNo;
            if (seleced.ReturnDateETA.HasValue)
            batch.ReturnDate = seleced.ReturnDateETA;
            batch.OperationQuantity =  seleced.LendQuantity.HasValue ? seleced.LendQuantity.Value : 0;
            batch.PType = Models.Request.PageType.Lend;
            batch.IsCreateMode = false;
            batch.IsNotLend_Return = isNotReturn;

            IDialog dialog = CurrentWindow.ShowDialog("添加明细", batch, (obj, args) =>
            {
                ProductVMAndBillInfo productList = args.Data as ProductVMAndBillInfo;
                if (productList != null)
                {
                    productList.ProductVM.ForEach(p =>
                    {
                        LendRequestItemVM vm = null;

                        #region 只允许添加自己权限范围内可以访问商品
                        string errorMessage = "对不起，您没有权限访问{0}商品!";
                        InventoryQueryFilter queryFilter = new InventoryQueryFilter();
                        queryFilter.ProductSysNo = p.SysNo;
                        queryFilter.UserName = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.LoginName;
                        queryFilter.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                        queryFilter.UserSysNo = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.UserSysNo;

                        int? returnQuantity = null;
                        int quantity = 1;
                        if (p.IsHasBatch == 1)
                        {
                            quantity = (from s in p.ProductBatchLst select s.Quantity).Sum();
                        }
                        else if (p.IsHasBatch == 0)
                        {
                            quantity = productList.Quantity;
                        }
                        if (!batch.IsNotLend_Return)
                        {
                            returnQuantity = (from s in p.ProductBatchLst select s.ReturnQuantity).Sum();
                        }


                        if (!AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_SeniorPM_Query))
                        {
                            new InventoryQueryFacade(CurrentPage).CheckOperateRightForCurrentUser(queryFilter, (Innerogj, innerArgs) =>
                            {
                                if (!innerArgs.FaultsHandle())
                                {
                                    if (!innerArgs.Result)
                                    {
                                        CurrentWindow.Alert(string.Format(errorMessage, p.ProductID));
                                        return;
                                    }
                                    else
                                    {
                                        vm = new LendRequestItemVM
                                        {
                                            ProductSysNo = p.SysNo,
                                            LendQuantity = quantity,
                                            ProductName = p.ProductName,
                                            ProductID = p.ProductID,
                                            PMUserName = p.PMUserName,
                                            ReturnDateETA = productList.ReturnDate,
                                            BatchDetailsInfoList = EntityConverter<BatchInfoVM, ProductBatchInfoVM>.Convert(p.ProductBatchLst),
                                            IsHasBatch = p.IsHasBatch
                                        };

                                        RequestVM.LendItemInfoList.Remove((LendRequestItemVM)this.dgProductList.SelectedItem);
                                        RequestVM.LendItemInfoList.Add(vm);
                                        this.dgProductList.ItemsSource = RequestVM.LendItemInfoList;
                                    }
                                }
                            });
                        }
                        else
                        {
                            vm = new LendRequestItemVM
                            {
                                ProductSysNo = p.SysNo,
                                LendQuantity = quantity,
                                ProductName = p.ProductName,
                                ProductID = p.ProductID,
                                PMUserName = p.PMUserName,
                                ReturnDateETA = productList.ReturnDate,
                                BatchDetailsInfoList = EntityConverter<BatchInfoVM, ProductBatchInfoVM>.Convert(p.ProductBatchLst),
                                IsHasBatch = p.IsHasBatch
                            };
                            
                            RequestVM.LendItemInfoList.Remove((LendRequestItemVM)this.dgProductList.SelectedItem);
                            RequestVM.LendItemInfoList.Add(vm);
                            this.dgProductList.ItemsSource = RequestVM.LendItemInfoList;
                        }

                        #endregion
                    });
                }

            });

            batch.DialogHandler = dialog;
        }

        private void hlbtnReset_Click(object sender, RoutedEventArgs e)
        {
            ShowEditModeDialog(sender, e,"更新明细",true);
        }

        private void hlbtnReturn_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            List<LendRequestItemVM> itemSource = this.dgProductList.ItemsSource as List<LendRequestItemVM>;
            LendRequestItemVM selected = this.dgProductList.SelectedItem as LendRequestItemVM;

            LendRequestVM RequestVM = this.DataContext as LendRequestVM;
            LendRequestItemVM seleced = RequestVM.LendItemInfoList.Where(p => p.ProductSysNo == selected.ProductSysNo).FirstOrDefault();

            UCProductBatch batch = new UCProductBatch(seleced.ProductSysNo.Value.ToString(), seleced.ProductID, selected.HasBatchInfo, seleced.BatchDetailsInfoList);


            batch.StockSysNo = RequestVM.StockSysNo;
            if (seleced.ReturnDateETA.HasValue)
                batch.ReturnDate = seleced.ReturnDateETA;
            batch.PType = Models.Request.PageType.Lend;
            batch.IsCreateMode = false;
            batch.IsNotLend_Return = false;

            IDialog dialog = CurrentWindow.ShowDialog("添加明细", batch, (obj, args) =>
            {
                ProductVMAndBillInfo productList = args.Data as ProductVMAndBillInfo;
                if (productList != null)
                {
                    productList.ProductVM.ForEach(p =>
                    {
                        #region 只允许添加自己权限范围内可以访问商品
                        string errorMessage = "对不起，您没有权限访问{0}商品!";
                        InventoryQueryFilter queryFilter = new InventoryQueryFilter();
                        queryFilter.ProductSysNo = p.SysNo;
                        queryFilter.UserName = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.LoginName;
                        queryFilter.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                        queryFilter.UserSysNo = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.UserSysNo;

                        int? returnQuantity = null;
                        Dictionary<string,int> batchReturns = new Dictionary<string,int>();
                        if (p.IsHasBatch == 1)
                        {
                            returnQuantity = (from s in p.ProductBatchLst select s.ReturnQuantity).Sum();
                            foreach(var item in p.ProductBatchLst)
                            {
                                if(item.ReturnQuantity > 0)
                                {
                                    batchReturns.Add(item.BatchNumber,item.ReturnQuantity);
                                }
                            }
                        }
                        else if (p.IsHasBatch == 0)
                        {
                            returnQuantity = productList.ReturnQuantity;
                        }

                        if (!AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_SeniorPM_Query))
                        {
                            new InventoryQueryFacade(CurrentPage).CheckOperateRightForCurrentUser(queryFilter, (Innerogj, innerArgs) =>
                            {
                                if (!innerArgs.FaultsHandle())
                                {
                                    if (!innerArgs.Result)
                                    {
                                        CurrentWindow.Alert(string.Format(errorMessage, p.ProductID));
                                        return;
                                    }
                                    else
                                    {
                                        RequestVM.LendItemInfoList.ForEach(t =>
                                        {
                                            if (t.ProductSysNo == selected.ProductSysNo)
                                            {
                                                if (p.IsHasBatch == 0)
                                                {
                                                    t.ToReturnQuantity = returnQuantity;
                                                }
                                                else if (p.IsHasBatch == 1)
                                                {
                                                    foreach (var item in batchReturns)
                                                    {
                                                        t.BatchDetailsInfoList.ForEach(b =>
                                                        {
                                                            if (b.BatchNumber.Equals(item.Key))
                                                            {
                                                                b.ReturnQuantity = item.Value;
                                                                // UI
                                                                if (t.ToReturnQuantity.HasValue)
                                                                {
                                                                    t.ToReturnQuantity = t.ToReturnQuantity.Value + item.Value;
                                                                }
                                                                else
                                                                {
                                                                    t.ToReturnQuantity = item.Value;
                                                                }
                                                            }
                                                        });
                                                    }
                                                }
                                            }
                                        });

                                        this.dgProductList.ItemsSource = RequestVM.LendItemInfoList;
                                    }
                                }
                            });
                        }
                        else
                        {
                            RequestVM.LendItemInfoList.ForEach(t =>
                            {
                                // clear toReturnQuantity
                                t.ToReturnQuantity = null;

                                if (t.ProductSysNo == selected.ProductSysNo)
                                {
                                    if (p.IsHasBatch == 0)
                                    {
                                        t.ToReturnQuantity = returnQuantity;
                                    }
                                    else if (p.IsHasBatch == 1)
                                    {
                                        foreach (var item in batchReturns)
                                        {
                                            t.BatchDetailsInfoList.ForEach(b => 
                                            {
                                                if (b.BatchNumber.Equals(item.Key))
                                                {
                                                    b.ReturnQuantity = item.Value;
                                                    // UI
                                                    if (t.ToReturnQuantity.HasValue)
                                                    {
                                                        t.ToReturnQuantity = t.ToReturnQuantity.Value + item.Value;
                                                    }
                                                    else
                                                    {
                                                        t.ToReturnQuantity = item.Value;
                                                    }
                                                    
                                                    
                                                }
                                            });
                                        }
                                    }
                                }
                            });

                            this.dgProductList.ItemsSource = RequestVM.LendItemInfoList;
                        }

                        #endregion
                    });
                }

            });

            batch.DialogHandler = dialog;
        }

        private void txtLendQuantity_TextChanged(object sender, TextChangedEventArgs e)
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

        private void txtReturnQuantity_TextChanged(object sender, TextChangedEventArgs e)
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

        private void txtToReturnQuantity_TextChanged(object sender, TextChangedEventArgs e)
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

        private void hlbtnViewReturnLog_Click(object sender, RoutedEventArgs e)
        {
            LendRequestVM requestVM = this.DataContext as LendRequestVM;
            LendRequestItemVM returnItem = this.dgProductList.SelectedItem as LendRequestItemVM;
            int productSysNo = (int)returnItem.ProductSysNo;

            LendRequestItemVM seleced = requestVM.LendItemInfoList.Where(p => p.ProductSysNo == returnItem.ProductSysNo).FirstOrDefault();
            List<ProductBatchInfoVM> bathcInfoList = seleced.BatchDetailsInfoList;

            List<LendRequestReturnItemInfo> returnItemLog = new List<LendRequestReturnItemInfo>();
            requestVM.ReturnItemInfoList.ForEach(i =>
                 {
                     if (i.ReturnProduct.SysNo == productSysNo)
                     {
                         LendRequestReturnItemInfo ri = new LendRequestReturnItemInfo
                         {
                             ReturnDate = i.ReturnDate,
                             ReturnQuantity = i.ReturnQuantity
                         };
                         returnItemLog.Add(ri);
                     }
                 });
            LendRequestReturnItem ucReturnItemLog = new LendRequestReturnItem { ReturnItemList = returnItemLog };
            CurrentWindow.ShowDialog("归还", ucReturnItemLog);
        }

        private void txtToReturnQuantity_MouseLeftUp(object sender, RoutedEventArgs e)
        {
            LendRequestItemVM selected = this.dgProductList.SelectedItem as LendRequestItemVM;

            UCProductBatch batch = new UCProductBatch();
            IDialog dialog = CurrentWindow.ShowDialog("添加明细", batch, (obj, args) =>
            {
                List<ProductVM> productList = args.Data as List<ProductVM>;
                if (productList != null)
                {

                }

            }, new Size(800, 500));
        }
    }
}
