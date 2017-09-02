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
    public partial class ComboSaleMaintain : PageBase
    {
        private int sysNo;
        /// <summary>
        /// 是否有效,有效状态下不能删除商品
        /// </summary>
        private bool IsActive;
        public ComboVM VM
        {
            get
            {
                return this.DataContext as ComboVM;
            }
            private set
            {
                this.DataContext = value;
            }
        }

        public ComboSaleMaintain()
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
                    new ComboFacade(this).Load(sysNo, (obj, args) =>
                    {
                        this.VM = args.Result;
                        IsActive = VM.IsActive;
                        SetButtonStatus();
                        setRadioButtonIsEnabled();
                    });
                }
            }
            else
            {
                this.VM = new ComboVM();
            }
            this.gridAddProduct.DataContext = new ComboItemVM();


        }

        private void SetButtonStatus()
        {
            if (this.VM.Status == ComboStatus.WaitingAudit)
            {
                btnAuditPass.Visibility = Visibility.Visible;
                btnAuditRefuse.Visibility = Visibility.Visible;
            }
            else
            {
                btnAuditPass.Visibility = Visibility.Collapsed;
                btnAuditRefuse.Visibility = Visibility.Collapsed;
            }
            if (VM.RequestSysNo > 0)
            {
                btnAdd.Visibility = Visibility.Collapsed;
                btnSave.Visibility = Visibility.Collapsed;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.gridBasicInfo))
            {
                List<int> productSysNos = null;
                if (this.VM.SysNo.HasValue && this.VM.SysNo > 0)
                {
                    if (this.VM.IsDeactive)
                    {
                        IsActive = false;
                        this.VM.TargetStatus = ComboStatus.Deactive;
                    }
                    else if (this.VM.IsActive)
                    {
                        IsActive = true;
                        this.VM.TargetStatus = ComboStatus.Active;
                    }
                    else
                    {
                        IsActive = false;
                        this.VM.TargetStatus = ComboStatus.WaitingAudit;
                    }
                    if (this.VM.Items == null || this.VM.Items.Count == 0)
                    {
                        this.VM.TargetStatus = ComboStatus.Deactive;
                    }

                    //Check Items 是否在随心配与DIY中
                    productSysNos = this.VM.Items.Select(o => o.ProductSysNo.Value).ToList<int>();
                    new ComboFacade(this).CheckOptionalAccessoriesItemAndDiys(productSysNos, (obj, args) =>
                    {
                        if (args.Result.Count > 0)
                        {
                            UCMessageConfirm ucMessageConfirm = new UCMessageConfirm(args.Result.Join("\r\n\r\n"));
                            ucMessageConfirm.CurrentDialog = Window.ShowDialog(ResComboSaleMaintain.Tip_Confirm, ucMessageConfirm, (obj1, args1) =>
                            {
                                if (args1.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                                {
                                    new ComboFacade(this).Update(this.VM, (obj2, args2) =>
                                    {
                                        this.VM.Status = this.VM.TargetStatus;
                                        SetButtonStatus();
                                        Window.Alert(ResComboSaleMaintain.Info_Successfully);
                                    });
                                }
                            });
                        }
                        else
                        {
                            new ComboFacade(this).Update(this.VM, (obj2, args2) =>
                            {
                                this.VM.Status = this.VM.TargetStatus;
                                SetButtonStatus();
                                Window.Alert(ResComboSaleMaintain.Info_Successfully);
                            });
                        }
                    });
                }
                else
                {
                    //Check Items 是否在随心配与DIY中
                    productSysNos = this.VM.Items.Select(o => o.ProductSysNo.Value).ToList<int>();
                    new ComboFacade(this).CheckOptionalAccessoriesItemAndDiys(productSysNos, (obj, args) =>
                    {
                        if (args.Result.Count > 0)
                        {
                            UCMessageConfirm ucMessageConfirm = new UCMessageConfirm(args.Result.Join("\r\n\r\n"));
                            ucMessageConfirm.CurrentDialog = Window.ShowDialog(ResComboSaleMaintain.Tip_Confirm, ucMessageConfirm, (obj1, args1) =>
                            {
                                if (args1.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                                {
                                    new ComboFacade(this).Create(this.VM, (obj2, args2) =>
                                    {
                                        this.VM.Status = ComboStatus.Deactive;
                                        Window.Alert(ResComboSaleMaintain.Info_Successfully);
                                    });
                                }
                            });
                        }
                        else
                        {
                            new ComboFacade(this).Create(this.VM, (obj2, args2) =>
                            {
                                this.VM.Status = ComboStatus.Deactive;
                                Window.Alert(ResComboSaleMaintain.Info_Successfully);
                            });
                        }
                    });
                }
                //Window.Refresh();
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ComboItemVM itemVM = (sender as Button).DataContext as ComboItemVM;
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
                if (productInfo != null)
                {
                    itemVM.ProductName = productInfo.ProductName;
                    itemVM.ProductUnitCost = productInfo.UnitCost;
                    itemVM.ProductCurrentPrice = productInfo.CurrentPrice;
                    itemVM.MerchantName = productInfo.MerchantName;
                }

                var t = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.BinarySerialize(this.VM);
                ComboVM clonedVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.BinaryDeserialize<ComboVM>(t);
                //DeepClone方法要报错
                //ComboVM clonedVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ComboVM>(this.VM);
                clonedVM.Items.Add(itemVM);


                //调用Service Check,不能用VM，而要用他的深拷贝对象
                new ComboFacade(this).CheckComboItemIsPass(clonedVM, (obj, args) =>
                {
                    if (args.Result != null && (args.Result.Count > 0))
                    {
                        string msg = args.Result.Join("\r\n");
                        Window.Alert(msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
                        return;
                    }

                    //To Fix bug，如果不深拷贝一次，则添加到列表中的Item不会显示ProductID
                    var str = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.BinarySerialize(itemVM);
                    var clonedItemVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.BinaryDeserialize<ComboItemVM>(str);

                    this.VM.Items.Add(clonedItemVM);
                    this.gridAddProduct.DataContext = new ComboItemVM();
                    setRadioButtonIsEnabled();
                });
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //UI上隐藏了此按钮            
        }

        private void btnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (IsActive)
            {
                //Window.Alert("当销售规则为有效时,不能删除商品!");
                Window.Alert(ResComboSaleMaintain.Warning_CanntDelProductActiveSaleRule);
                return;
            }
            Window.Confirm(ResComboSaleMaintain.Confirm_Delete, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    var itemVM = (sender as HyperlinkButton).DataContext as ComboItemVM;
                    this.VM.Items.Remove(itemVM);
                    setRadioButtonIsEnabled();
                }
            });
        }

        private void btnAuditRefuse_Click(object sender, RoutedEventArgs e)
        {
            new ComboFacade(this).AuditPass(this.VM.SysNo.Value, false, (obj, args) =>
            {
                this.VM.Status = ComboStatus.Deactive;

                SetButtonStatus();

                Window.Alert(ResComboSaleMaintain.Info_Successfully);
            });
        }

        private void btnAuditPass_Click(object sender, RoutedEventArgs e)
        {
            new ComboFacade(this).AuditPass(this.VM.SysNo.Value, true, (obj, args) =>
            {
                this.VM.Status = ComboStatus.Active;

                SetButtonStatus();

                Window.Alert(ResComboSaleMaintain.Info_Successfully);
            });
        }
        /// <summary>
        /// 设置RadioIsEnabled 解决Bug92508 没有商品时 不能选择有效和待审核状态
        /// </summary>
        private void setRadioButtonIsEnabled()
        {
            if (this.VM.Items.Count > 0)
            {
                this.radActive.IsEnabled = true;
                this.radDeactive.IsEnabled = true;
                this.radWaiting.IsEnabled = true;
            }
            else
            {
                this.radActive.IsEnabled = false;
                this.radDeactive.IsEnabled = true;
                this.radWaiting.IsEnabled = false;
            }
        }
    }
}
