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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Utilities;
using System.Text;

namespace ECCentral.Portal.UI.MKT.UserControls.SaleGift
{
    public partial class UCSaleGiftRule : UserControl
    {
        bool isLoaded = false;
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public UCSaleGiftRule()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCSaleGiftRule_Loaded);
        }

        void UCSaleGiftRule_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                return;
            }

            chkGiftComboType.IsEnabled = true;
            tbItemGiftCount.IsEnabled = true;

            SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
            if (vm.Type == SaleGiftType.Vendor)
            {
                chkGiftComboType.IsEnabled = false;
                chkGiftComboType.IsChecked = true;
                tbItemGiftCount.IsEnabled = false;
            }
            //if (vm.Type == SaleGiftType.Additional)
            //{
            //    chkGiftComboType.IsEnabled = false;
            //    chkGiftComboType.IsChecked = false;
            //}

            this.dgGiftProduct.ItemsSource = vm.GiftItemList;
            this.dgGiftProduct.Bind();

            if (vm.GiftComboType.Value == SaleGiftGiftItemType.GiftPool)
            {
                dgGiftProduct.Columns[dgGiftProduct.Columns.Count - 2].Visibility = System.Windows.Visibility.Collapsed;

                tbItemGiftCount.SetReadOnly(false);
            }
            else
            {
                dgGiftProduct.Columns[dgGiftProduct.Columns.Count - 2].Visibility = System.Windows.Visibility.Visible;
                
                tbItemGiftCount.SetReadOnly(true);
                tbItemGiftCount.Text = "";
            }

            //如果是满额加购，需要设置加购价格
            //if (vm.Type == SaleGiftType.Additional)
            //{
            //    tbItemGiftCount.Text = "1";
            //    gridGiftType.RowDefinitions[0].Height = new GridLength(0);
            //    dgGiftProduct.Columns[dgGiftProduct.Columns.Count - 3].Visibility = System.Windows.Visibility.Visible;
            //}
            //else
            //{
            //    dgGiftProduct.Columns[dgGiftProduct.Columns.Count - 3].Visibility = System.Windows.Visibility.Collapsed;
            //}

            if (vm.IsOnlyViewMode)
            {
                OperationControlStatusHelper.SetControlsStatus(this.gridLayout, true);
            }

            isLoaded = true;
        }

        private void hybtnViewUDLog_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.dgGiftProduct.SelectedItem as dynamic;
            if (item == null) return;
            var sysNo = Convert.ToInt32(item.ProductSysNo);

            //创建ID编辑
            UCSaleGiftLog detail = new UCSaleGiftLog();
            detail.ProductSysNo = sysNo;
            detail.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("查看log", detail, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {

                }
            }, new Size(650, 450));
        }

        private void btnRemoveGift_Click(object sender, RoutedEventArgs e)
        {
            SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
            List<SaleGift_GiftItemViewModel> needRemoveList = new List<SaleGift_GiftItemViewModel>();
            if (vm.GiftItemList.Count > 0)
            {
                foreach (SaleGift_GiftItemViewModel setting in vm.GiftItemList)
                {
                    if (setting.IsChecked.Value)
                    {
                        needRemoveList.Add(setting);
                    }
                }
                needRemoveList.ForEach(f => vm.GiftItemList.Remove(f));
                dgGiftProduct.ItemsSource = vm.GiftItemList;
                dgGiftProduct.Bind();
            }
        }

        private void btnAddGift_Click(object sender, RoutedEventArgs e)
        {
            UCProductSearch ucPicker = new UCProductSearch();
            ucPicker.SelectionMode = SelectionMode.Multiple;
            ucPicker.DialogHandler = CurrentWindow.ShowDialog("选择商品", ucPicker, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
                    List<ProductVM> selectedList = args.Data as List<ProductVM>;

                    StringBuilder message = new StringBuilder();

                    foreach (ProductVM product in selectedList)
                    {
                        //赠品必须是非展示状态
                        if (product.Status != BizEntity.IM.ProductStatus.InActive_Show)
                        {
                            message.AppendLine(string.Format("赠品{0}必须是非展示状态！", product.ProductID));
                            continue;
                        }


                        SaleGift_GiftItemViewModel giftVM = new SaleGift_GiftItemViewModel();
                        giftVM.IsChecked = false;

                        giftVM.ProductSysNo = product.SysNo;
                        giftVM.ProductID = product.ProductID;
                        giftVM.ProductName = product.ProductName;
                        giftVM.AvailableQty = product.AvailableQty;
                        giftVM.ConsignQty = product.ConsignQty;
                        giftVM.VirtualQty = product.OnlineQty - (product.AvailableQty + product.ConsignQty);
                        giftVM.UnitCost = product.UnitCost;
                        giftVM.CurrentPrice = product.CurrentPrice;

                        if (vm.GiftItemList.FirstOrDefault(f => f.ProductSysNo == giftVM.ProductSysNo) != null)
                        {
                            message.AppendLine(string.Format("商品{0}已经存在!", product.ProductID));
                            continue;
                        }

                        //获取商品的毛利
                        new CouponsFacade(CPApplication.Current.CurrentPage).GetCouponGrossMarginRate(product.SysNo.Value, (s, a) =>
                        {
                            giftVM.GrossMarginRate = a.Result;
                        });

                        giftVM.Priority = (vm.GiftItemList.Count + 1).ToString();
                        if (vm.GiftComboType.Value == SaleGiftGiftItemType.AssignGift)
                        {
                            giftVM.Count = "1";
                        }
                        else
                        {
                            giftVM.Count = "0";
                            giftVM.PlusPrice = giftVM.CurrentPrice.GetValueOrDefault().ToString("f2");
                        }

                        vm.GiftItemList.Add(giftVM);
                    }

                    if (message.Length > 0)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert(message.ToString(), MessageType.Warning);
                    }

                    dgGiftProduct.ItemsSource = vm.GiftItemList;
                    dgGiftProduct.Bind();
                }
            });
        }

        private void chkGiftProductCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
            CheckBox chk = (CheckBox)sender;
            vm.GiftItemList.ForEach(f => f.IsChecked = chk.IsChecked.Value);
        }

        private void btnSaveGiftSetting_Click(object sender, RoutedEventArgs e)
        {
            SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
            if (vm.GiftComboType == SaleGiftGiftItemType.GiftPool)
            {
                foreach (SaleGift_GiftItemViewModel giftitem in vm.GiftItemList)
                {
                    giftitem.Count = "0";
                    if (string.IsNullOrEmpty(giftitem.PlusPrice))
                    {
                        giftitem.PlusPrice = giftitem.CurrentPrice.GetValueOrDefault().ToString("f2");
                    }
                }
                int count = 0;
                int.TryParse(vm.ItemGiftCount, out count);
                if (string.IsNullOrEmpty(vm.ItemGiftCount) || count < 1)
                {
                    tbItemGiftCount.Validation("赠品池任选总数量不能为空并且必须为大于0的整数！", vm, this.gridGiftType);
                    return;
                }
                else
                {
                    tbItemGiftCount.ClearValidationError();
                }
            }
            else
            {
                tbItemGiftCount.ClearValidationError();
            }

            List<SaleGift_GiftItemViewModel> list = dgGiftProduct.ItemsSource as List<SaleGift_GiftItemViewModel>;
            if (list == null || list.Count == 0)
            {
                CurrentWindow.Alert("请至少设置1个赠品！");
                return;
            }

            if (vm.GiftComboType.Value == SaleGiftGiftItemType.AssignGift)
            {
                foreach (SaleGift_GiftItemViewModel item in list)
                {
                    if (string.IsNullOrEmpty(item.Count) || item.Count.Equals("0"))
                    {
                        CurrentWindow.Alert("赠品数量必须大于0！");
                        return;
                    }
                }
            }

            ValidationManager.Validate(this.dgGiftProduct);
            foreach (SaleGift_GiftItemViewModel rowVM in list)
            {
                if (rowVM.HasValidationErrors) return;
            }


            SaleGiftFacade facade = new SaleGiftFacade(CPApplication.Current.CurrentPage);
            if (vm.IsAccess) //有高级权限
            {
                facade.CheckGiftStockResult(vm, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    if (arg.Result.Length > 0)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Confirm(arg.Result + ";是否继续保存?", (objs, args) =>
                         {

                             if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                             {
                                 facade.SetSaleGiftGiftItemRules(vm, (o, a) =>
                                 {
                                     if (a.FaultsHandle())
                                     {
                                         return;
                                     }
                                     CurrentWindow.Alert("赠品规则保存成功！");
                                 });
                             }
                         });
                    }
                    else
                    {
                        facade.SetSaleGiftGiftItemRules(vm, (o, a) =>
                        {
                            if (a.FaultsHandle())
                            {
                                return;
                            }
                            CurrentWindow.Alert("赠品规则保存成功！");
                        });
                    }
                });
            }
            else
            {
                facade.SetSaleGiftGiftItemRules(vm, (o, a) =>
                {
                    if (a.FaultsHandle())
                    {
                        return;
                    }
                    CurrentWindow.Alert("赠品规则保存成功！");
                });
            }
           
        }

        private void chkGiftComboType_Click(object sender, RoutedEventArgs e)
        {
            if (!chkGiftComboType.IsChecked.Value)
            {
                dgGiftProduct.Columns[dgGiftProduct.Columns.Count - 2].Visibility = System.Windows.Visibility.Collapsed;
                tbItemGiftCount.SetReadOnly(false);
            }
            else
            {
                dgGiftProduct.Columns[dgGiftProduct.Columns.Count - 2].Visibility = System.Windows.Visibility.Visible;
                tbItemGiftCount.SetReadOnly(true);
                tbItemGiftCount.Text = "";
            }

        }

        private void tbItemGiftCount_LostFocus(object sender, RoutedEventArgs e)
        {
            //SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
            //ValidationManager.Validate(this.gridGiftType);
            //if (!vm.HasValidationErrors)
            //{
            //    tbItemGiftCount.ClearValidationError();
            //}
        }
    }
}
