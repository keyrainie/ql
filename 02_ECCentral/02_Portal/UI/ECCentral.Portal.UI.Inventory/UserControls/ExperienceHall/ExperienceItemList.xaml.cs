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
    public partial class ExperienceItemList : UserControl
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

        public ExperienceItemList()
        {
            InitializeComponent();
        }

        #endregion

        #region 内部事件处理


        #endregion

        private void hlbtnDelete_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            ExperienceItemVM vm = btn.DataContext as ExperienceItemVM;
            ExperienceVM RequestVM = this.DataContext as ExperienceVM;
            RequestVM.ExperienceItemInfoList.Remove(vm);
            this.dgProductList.ItemsSource = RequestVM.ExperienceItemInfoList;
        }

        private void ShowEditModeDialog(object sender, RoutedEventArgs e,string title,bool isNotReturn)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            List<ExperienceItemVM> itemSource = this.dgProductList.ItemsSource as List<ExperienceItemVM>;
            ExperienceItemVM selected = this.dgProductList.SelectedItem as ExperienceItemVM;

            ExperienceVM RequestVM = this.DataContext as ExperienceVM;
            ExperienceItemVM seleced = RequestVM.ExperienceItemInfoList.Where(p => p.ProductSysNo == selected.ProductSysNo).FirstOrDefault();

            UCProductBatch batch = new UCProductBatch(seleced.ProductSysNo.Value.ToString(), seleced.ProductID, false, null);
             

            //batch.StockSysNo = RequestVM.StockSysNo;
            //if (seleced.ReturnDateETA.HasValue)
            //batch.ReturnDate = seleced.ReturnDateETA;
            //batch.OperationQuantity =  seleced.LendQuantity.HasValue ? seleced.LendQuantity.Value : 0;
            //batch.PType = Models.Request.PageType.Lend;
            //batch.IsCreateMode = false;
            //batch.IsNotLend_Return = isNotReturn;

            IDialog dialog = CurrentWindow.ShowDialog("添加明细", batch, (obj, args) =>
            {
                ProductVMAndBillInfo productList = args.Data as ProductVMAndBillInfo;
                if (productList != null)
                {
                    productList.ProductVM.ForEach(p =>
                    {
                        ExperienceItemVM vm = null;

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
                                        vm = new ExperienceItemVM
                                        {
                                            //ProductSysNo = p.SysNo,
                                            //LendQuantity = quantity,
                                            //ProductName = p.ProductName,
                                            //ProductID = p.ProductID,
                                            //PMUserName = p.PMUserName,
                                            //ReturnDateETA = productList.ReturnDate,
                                            //BatchDetailsInfoList = EntityConverter<BatchInfoVM, ProductBatchInfoVM>.Convert(p.ProductBatchLst),
                                            //IsHasBatch = p.IsHasBatch
                                        };

                                        RequestVM.ExperienceItemInfoList.Remove((ExperienceItemVM)this.dgProductList.SelectedItem);
                                        RequestVM.ExperienceItemInfoList.Add(vm);
                                        this.dgProductList.ItemsSource = RequestVM.ExperienceItemInfoList;
                                    }
                                }
                            });
                        }
                        else
                        {
                            vm = new ExperienceItemVM
                            {
                                //ProductSysNo = p.SysNo,
                                //LendQuantity = quantity,
                                //ProductName = p.ProductName,
                                //ProductID = p.ProductID,
                                //PMUserName = p.PMUserName,
                                //ReturnDateETA = productList.ReturnDate,
                                //BatchDetailsInfoList = EntityConverter<BatchInfoVM, ProductBatchInfoVM>.Convert(p.ProductBatchLst),
                                //IsHasBatch = p.IsHasBatch
                            };
                            
                            RequestVM.ExperienceItemInfoList.Remove((ExperienceItemVM)this.dgProductList.SelectedItem);
                            RequestVM.ExperienceItemInfoList.Add(vm);
                            this.dgProductList.ItemsSource = RequestVM.ExperienceItemInfoList;
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

        private void txtToReturnQuantity_MouseLeftUp(object sender, RoutedEventArgs e)
        {
            ExperienceItemVM selected = this.dgProductList.SelectedItem as ExperienceItemVM;

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
