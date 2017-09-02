using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using System.Collections.Generic;
using ECCentral.Portal.UI.MKT.UserControls;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class OptionalAccessoriesMaintain : PageBase
    {
        private int sysNo;

        public OptionalAccessoriesVM VM
        {
            get
            {
                return this.DataContext as OptionalAccessoriesVM;
            }
            private set
            {
                this.DataContext = value;
            }
        }

        public OptionalAccessoriesMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            string no = Request.Param;
            if (!string.IsNullOrEmpty(no))
            {
                if (int.TryParse(no, out sysNo))
                {
                    new OptionalAccessoriesFacade(this).Load(sysNo, (obj, args) =>
                    {
                        this.VM = args.Result;
                        if (args.Result.Status == ComboStatus.WaitingAudit)
                        {
                            var desc = args.Result.DisplayApproveMsg.Join("\r\n");
                            if (!string.IsNullOrEmpty(desc))
                            {
                                Window.MessageBox.Show(desc
                                   , Newegg.Oversea.Silverlight.Controls.Components.MessageBoxType.Warning);
                            }

                        }
                        SetButtonStatus();
                    });
                }
            }
            else
            {
                this.VM = new OptionalAccessoriesVM();
                SetButtonStatus();
            }
            this.gridAddProduct.DataContext = new OptionalAccessoriesItemVM();


        }

        private void SetButtonStatus()
        {
            if (this.VM.Status == ComboStatus.WaitingAudit)
            {
                btnAuditPass.Visibility = Visibility.Visible;
                btnAuditRefuse.Visibility = Visibility.Visible;
                rbActive.IsEnabled = false;
                rbDeactive.IsEnabled = false;
                rbWaitingAudit.IsEnabled = false;
                btnAdd.Visibility = Visibility.Collapsed;
                btnSave.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnAuditPass.Visibility = Visibility.Collapsed;
                btnAuditRefuse.Visibility = Visibility.Collapsed;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.dgItems);
            List<int> productSysNos = null;
            OptionalAccessoriesVM _vm = this.dgItems.DataContext as OptionalAccessoriesVM;
            if (_vm.Items == null || _vm.Items.Count() < 1)
            {
                this.Window.Alert("商品列表不能为空");
            }
            foreach (OptionalAccessoriesItemVM rowVM in _vm.Items)
            {
                if (rowVM.HasValidationErrors) return;
            }

            if (ValidationManager.Validate(this.gridBasicInfo))
            {
                if (this.VM.Items.Count < 2)
                {
                    Window.Alert(ResComboSaleMaintain.Warning_AtLeast2Items);
                    return;
                }
                if (this.VM.SysNo.HasValue && this.VM.SysNo > 0)
                {
                    if (this.VM.IsDeactive)
                    {
                        this.VM.TargetStatus = ComboStatus.Deactive;
                    }
                    else if (this.VM.IsActive)
                    {
                        this.VM.TargetStatus = ComboStatus.Active;
                    }
                    else
                    {
                        this.VM.TargetStatus = ComboStatus.WaitingAudit;
                    }

                    productSysNos = this.VM.Items.Select(o => o.ProductSysNo.Value).ToList<int>();
                    new OptionalAccessoriesFacade(this).CheckSaleRuleItemAndDiys(productSysNos, (obj, args) =>
                    {
                        if (args.Result.Count > 0)
                        {
                            UCMessageConfirm ucMessageConfirm = new UCMessageConfirm(args.Result.Join("\r\n\r\n"));
                            ucMessageConfirm.CurrentDialog = Window.ShowDialog(ResComboSaleMaintain.Tip_Confirm, ucMessageConfirm, (obj1, args1) =>
                            {
                                if (args1.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                                {
                                    new OptionalAccessoriesFacade(this).Update(this.VM, (obj2, args2) =>
                                    {
                                        this.VM.Status = this.VM.TargetStatus;

                                        SetButtonStatus();

                                        Window.Alert(ResComboSaleMaintain.Info_Successfully);
                                        Window.Refresh();
                                    });
                                }
                            });
                        }
                        else
                        {
                            new OptionalAccessoriesFacade(this).Update(this.VM, (obj2, args2) =>
                            {
                                this.VM.Status = this.VM.TargetStatus;

                                SetButtonStatus();

                                Window.Alert(ResComboSaleMaintain.Info_Successfully);
                                Window.Refresh();
                            });
                        }
                    });
                }
                else
                {
                    productSysNos = this.VM.Items.Select(o => o.ProductSysNo.Value).ToList<int>();
                    new OptionalAccessoriesFacade(this).CheckSaleRuleItemAndDiys(productSysNos, (obj, args) =>
                    {
                        if (args.Result.Count > 0)
                        {
                            UCMessageConfirm ucMessageConfirm = new UCMessageConfirm(args.Result.Join("\r\n\r\n"));
                            ucMessageConfirm.CurrentDialog = Window.ShowDialog(ResComboSaleMaintain.Tip_Confirm, ucMessageConfirm, (obj1, args1) =>
                            {
                                if (args1.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                                {
                                    new OptionalAccessoriesFacade(this).Create(this.VM, (obj2, args2) =>
                                    {
                                        this.VM.Status = ComboStatus.Deactive;
                                        Window.Alert(ResComboSaleMaintain.Info_Successfully);
                                        Window.Close();
                                    });
                                }
                            });
                        }
                        else
                        {
                            new OptionalAccessoriesFacade(this).Create(this.VM, (obj2, args2) =>
                            {
                                this.VM.Status = ComboStatus.Deactive;
                                Window.Alert(ResComboSaleMaintain.Info_Successfully);
                                Window.Close();
                            });
                        }
                    });
                }
                //Window.Refresh();
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.gridAddProduct) || !ValidationManager.Validate(this.gridBasicInfo))
            {
                return;
            }

            OptionalAccessoriesItemVM itemVM = (sender as Button).DataContext as OptionalAccessoriesItemVM;
            if (!itemVM.ProductSysNo.HasValue)
            {
                Window.Alert(ResComboSaleMaintain.Warning_NoItems, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
                return;
            }

            if (ValidationManager.Validate(this.gridAddProduct))
            {
                if (this.VM.Items.FirstOrDefault(p => p.ProductSysNo == itemVM.ProductSysNo) != null)
                {
                    Window.Alert(ResComboSaleMaintain.Warning_SaleItemExists, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
                    return;
                }

                var productInfo = this.ucProductPicker.SelectedProductInfo;
                string productID = this.ucProductPicker.ProductID;
                itemVM.ProductID = productID;
                #region 折扣比例
                //var itemDPTmp = 0m;
                //decimal.TryParse(TxtItemDiscountPercent.Text, out itemDPTmp);
                //itemVM.DiscountPercent = (itemDPTmp / 100).ToString("0.00");
                #endregion
                if (productInfo != null)
                {
                    itemVM.ProductName = productInfo.ProductName;
                    itemVM.ProductUnitCost = productInfo.UnitCost;
                    itemVM.ProductCurrentPrice = productInfo.CurrentPrice;
                    itemVM.MerchantName = productInfo.MerchantName;
                }

                var t = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.BinarySerialize(this.VM);
                OptionalAccessoriesVM clonedVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.BinaryDeserialize<OptionalAccessoriesVM>(t);
                //DeepClone方法要报错
                //ComboVM clonedVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ComboVM>(this.VM);
                clonedVM.Items.Add(itemVM);


                //调用Service Check,不能用VM，而要用他的深拷贝对象
                new OptionalAccessoriesFacade(this).CheckOptionalAccessoriesItemIsPass(clonedVM, (obj, args) =>
                {
                    if (args.Result != null && (args.Result.Count > 0))
                    {
                        string msg = args.Result.Join("\r\n");
                        Window.Alert(msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
                        return;
                    }

                    //To Fix bug，如果不深拷贝一次，则添加到列表中的Item不会显示ProductID
                    var str = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.BinarySerialize(itemVM);
                    var clonedItemVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.BinaryDeserialize<OptionalAccessoriesItemVM>(str);

                    this.VM.Items.Add(clonedItemVM);

                    this.gridAddProduct.DataContext = new OptionalAccessoriesItemVM();

                    this.CheckBox_Click(this.cbIsMaster, e);
                });
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //UI上隐藏了此按钮            
        }

        private void btnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResComboSaleMaintain.Confirm_Delete, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    var itemVM = (sender as HyperlinkButton).DataContext as OptionalAccessoriesItemVM;
                    this.VM.Items.Remove(itemVM);
                }
            });
        }

        private void btnAuditRefuse_Click(object sender, RoutedEventArgs e)
        {
            new OptionalAccessoriesFacade(this).ApproveOptionalAccessories(this.VM.SysNo.Value, false, (obj, args) =>
            {
                this.VM.Status = ComboStatus.Deactive;

                SetButtonStatus();

                Window.Alert(ResComboSaleMaintain.Info_Successfully);
                Window.Close();
            });
        }

        private void btnAuditPass_Click(object sender, RoutedEventArgs e)
        {
            new OptionalAccessoriesFacade(this).ApproveOptionalAccessories(this.VM.SysNo.Value, true, (obj, args) =>
            {
                this.VM.Status = ComboStatus.Active;

                SetButtonStatus();

                Window.Alert(ResComboSaleMaintain.Info_Successfully);
                Window.Close();
            });
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbIsMaster = sender as CheckBox;
            if (cbIsMaster.IsChecked ?? false)
            {
                TxtItemPriority.Text = "0";
                TxtItemDiscount.Text = "0";
                TxtItemDiscountPercent.Text = "100";
                TxtItemPriority.IsEnabled = false;
                TxtItemDiscount.IsEnabled = false;
                TxtItemDiscountPercent.IsEnabled = false;
            }
            else
            {
                TxtItemPriority.Text = "9999";
                TxtItemPriority.IsEnabled = true;
                TxtItemDiscount.IsEnabled = true;
                TxtItemDiscountPercent.IsEnabled = true;
            }
        }

        private void dgItems_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            OptionalAccessoriesItemVM currentItem = e.Row.DataContext as OptionalAccessoriesItemVM;

            if (currentItem.IsMasterItemB && e.Column.ClipboardContentBinding.Path.Path != "Quantity")
            {
                e.Cancel = true;
            }
        }
    }
}
