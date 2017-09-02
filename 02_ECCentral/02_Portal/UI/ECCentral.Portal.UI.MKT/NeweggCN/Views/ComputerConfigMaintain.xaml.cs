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

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.BizEntity.Enum;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Service.Utility;
using ECCentral.Portal.UI.MKT.UserControls;


namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ComputerConfigMaintain : PageBase
    {

        public ComputerConfigMaintain()
        {
            InitializeComponent();

        }
        private bool _isEditing;
        private ComputerConfigMasterVM _currentVM;
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            //页面加载后固定跨列的文本框的宽度
            this.txtConfigName.Width = this.txtConfigName.ActualWidth;
            this.txtDescription.Width = this.txtDescription.ActualWidth;

            this.lstChannel.ItemsSource = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            if (string.IsNullOrEmpty(this.Request.Param))
            {
                _isEditing = false;
                this.Title = ResComputerConfig.Info_AddTitle;
                new ComputerConfigFacade(this).GetAllParts((itemVMList) =>
                {
                    _currentVM = new ComputerConfigMasterVM();
                    _currentVM.Status = ComputerConfigStatus.Origin;
                    _currentVM.ConfigItemList = itemVMList;
                    this.DataContext = _currentVM;
                    this.lstChannel.SelectedIndex = 0;
                    this.lstConfigTypes.SelectedIndex = 0;
                    this.btnSave.Visibility = Visibility.Visible;
                    this.btnSubmit.Visibility = System.Windows.Visibility.Visible;
                });
            }
            else
            {
                if (this.Request.UserState != null
                    && (ComputerConfigMaintainMode)this.Request.UserState == ComputerConfigMaintainMode.Copy)
                {
                    //复制新建
                    _isEditing = false;
                    this.Title = ResComputerConfig.Info_AddTitle;
                }
                else
                {
                    _isEditing = true;
                    this.Title = ResComputerConfig.Info_EditTitle;
                }
                new ComputerConfigFacade(this).Load(int.Parse(this.Request.Param), (masterVM) =>
                {
                    _currentVM = masterVM;
                    if (!_isEditing)
                    {
                        //复制新建时修改拷贝过来的VM状态
                        _currentVM.Status = ComputerConfigStatus.Origin;
                        _currentVM.SysNo = null;
                    }
                    if (_currentVM.Status == ComputerConfigStatus.Pending)
                    {
                        //this.btnSave.Visibility = System.Windows.Visibility.Visible;
                        this.btnApprovePass.Visibility = System.Windows.Visibility.Visible;
                        this.btnApproveDecline.Visibility = System.Windows.Visibility.Visible;
                    }
                    else if (_currentVM.Status == ComputerConfigStatus.Origin)
                    {
                        this.btnSave.Visibility = Visibility.Visible;
                        this.btnSubmit.Visibility = System.Windows.Visibility.Visible;
                    }
                    if (_currentVM.Status == ComputerConfigStatus.Pending
                        || _currentVM.Status == ComputerConfigStatus.Running
                        || _currentVM.Status == ComputerConfigStatus.Void)
                    {
                        //禁止输入
                        SetAllReadOnly(_currentVM);
                    }
                    this.DataContext = _currentVM;
                });
            }

        }

        void lstChannel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lstChannel.SelectedValue == null) return;
            //如果已经加载了，直接返回
            if (this.lstConfigTypes.Tag == this.lstChannel.SelectedValue) return;
            ComputerConfigFacade service = new ComputerConfigFacade(this);
            service.GetAllConfigTypes((s, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var configTypeList = args.Result;
                this.lstConfigTypes.ItemsSource = configTypeList;
                this.lstConfigTypes.Tag = this.lstChannel.SelectedValue;
            });
        }

        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            _isEditing = false;
            this.Title = ResComputerConfig.Info_AddTitle;
            this.btnSave.IsEnabled = true;
            this.lstChannel.SelectedIndex = 0;
            new ComputerConfigFacade(this).GetAllParts((itemVMList) =>
                {
                    _currentVM = new ComputerConfigMasterVM();
                    _currentVM.Status = ComputerConfigStatus.Origin;
                    _currentVM.ConfigItemList = itemVMList;
                    this.DataContext = _currentVM;
                    this.btnSave.Visibility = Visibility.Visible;
                    this.btnSubmit.Visibility = System.Windows.Visibility.Visible;
                });
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
                return;
            _currentVM.Status = ComputerConfigStatus.Origin;

            //Check Items 是否存在于随心配与销售规则中
            List<int> productSysNos = _currentVM.ConfigItemList.Where(o => !String.IsNullOrWhiteSpace(o.ProductID) && o.ProductSysNo != null)
                                    .Select(o => o.ProductSysNo.Value).ToList<int>();
            new ComputerConfigFacade(this).CheckOptionalAccessoriesItemAndCombos(productSysNos, (obj, args) =>
            {
                if (args.Result.Count > 0)
                {
                    UCMessageConfirm ucMessageConfirm = new UCMessageConfirm(args.Result.Join("\r\n\r\n"));
                    ucMessageConfirm.CurrentDialog = Window.ShowDialog(ResComboSaleMaintain.Tip_Confirm, ucMessageConfirm, (obj1, args1) =>
                    {
                        if (args1.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                        {
                            SaveAndUpdate();
                        }
                    });
                }
                else
                {
                    SaveAndUpdate();
                }
            });
        }

        private void ucProductPicker_ProductSelected(object sender, ProductSelectedEventArgs e)
        {
            var uc = sender as UCProductPicker;
            var configItemVM = uc.DataContext as ComputerConfigItemVM;
            new ComputerConfigFacade(this).BuildConfigItem(configItemVM, (result) =>
                {
                    if (result == null)
                    {
                        uc.ProductSysNo = null;
                        uc.ProductID = null;
                        return;
                    }
                    configItemVM.ProductName = result.ProductName;
                    configItemVM.ProductQty = result.ProductQty ?? 0;
                    configItemVM.OnlineQty = result.OnlineQty;
                    configItemVM.UnitCost = result.UnitCost;
                    configItemVM.CurrentPrice = result.CurrentPrice ?? 0m;
                    configItemVM.Discount = result.Discount ?? 0m;
                    _currentVM.CalcTotal();
                    Window.MessageBox.Show(ResComputerConfig.Info_ProductLoadSuccess
                                    , Newegg.Oversea.Silverlight.Controls.Components.MessageBoxType.Success);
                });
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            _currentVM.Status = ComputerConfigStatus.Pending;

            //Check Items 是否存在于随心配与销售规则中
            List<int> productSysNos = _currentVM.ConfigItemList.Where(o => !String.IsNullOrWhiteSpace(o.ProductID) && o.ProductSysNo != null)
                                    .Select(o => o.ProductSysNo.Value).ToList<int>();
            new ComputerConfigFacade(this).CheckOptionalAccessoriesItemAndCombos(productSysNos, (obj, args) =>
            {
                if (args.Result.Count > 0)
                {
                    UCMessageConfirm ucMessageConfirm = new UCMessageConfirm(args.Result.Join("\r\n\r\n"));
                    ucMessageConfirm.CurrentDialog = Window.ShowDialog(ResComboSaleMaintain.Tip_Confirm, ucMessageConfirm, (obj1, args1) =>
                    {
                        if (args1.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                        {

                            SaveAndUpdate();
                        }
                    });
                }
                else
                {
                    SaveAndUpdate();
                }
            });
        }

        private void btnApprovePass_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResComputerConfig.Confirm_AuditPass, (cs, cr) =>
            {
                if (cr.DialogResult == DialogResultType.OK)
                {
                    List<int> sysNoList = new List<int>();
                    sysNoList.Add(_currentVM.SysNo.Value);
                    new ComputerConfigFacade(this).ApprovePass(sysNoList, () =>
                    {
                        Window.Alert(ResComputerConfig.Info_AuditPassSuccess);
                        Window.Close();
                    });
                }
            });
        }

        private void btnApproveDecline_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResComputerConfig.Confirm_AuditDecline, (cs, cr) =>
            {
                if (cr.DialogResult == DialogResultType.OK)
                {
                    List<int> sysNoList = new List<int>();
                    sysNoList.Add(_currentVM.SysNo.Value);
                    new ComputerConfigFacade(this).ApproveDecline(sysNoList, () =>
                    {
                        Window.Alert(ResComputerConfig.Info_AuditDeclineSuccess);
                        Window.Close();
                    });
                }
            });
        }

        private void txt_LostFocus(object sender, RoutedEventArgs e)
        {
            var binding = (sender as TextBox).GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();
            _currentVM.CalcTotal();
        }

        private void SetAllReadOnly(ComputerConfigMasterVM _vm)
        {
            foreach (UIElement item in this.gridMain.Children)
            {
                if (item is TextBox)
                {
                    ((TextBox)item).IsReadOnly = true;
                }
                if (item is Combox)
                {
                    ((Combox)item).IsEnabled = false;
                }
            }
            _vm.ConfigItemList.ForEach(item => item.IsControlReadOnly = true);
        }


        private void UCProductPicker_KeyDown(object sender, KeyEventArgs e)
        {
            var ucProduct = sender as UCProductPicker;
            var ucProductSysNo = ucProduct.FindName("txtProductSysNo") as TextBox;
            var ucProductID = ucProduct.FindName("txtProductID") as TextBox;
            if (e.Key == Key.Enter && (string.IsNullOrEmpty(ucProductSysNo.Text) || string.IsNullOrEmpty(ucProductID.Text)))
            {
                var configItemVM = ucProduct.DataContext as ComputerConfigItemVM;
                configItemVM.ProductName = string.Empty;
                configItemVM.ProductQty = 0;
                configItemVM.OnlineQty = 0;
                configItemVM.UnitCost = 0;
                configItemVM.CurrentPrice = 0;
                configItemVM.Discount = 0;
            }
        }

        private void SaveAndUpdate()
        {
            if (_isEditing)
            {
                new ComputerConfigFacade(this).Update(_currentVM, (s, args2) =>
                {
                    if (args2.FaultsHandle())
                        return;
                    Window.Alert(ResComputerConfig.Info_EditSuccess);
                    Window.Close();
                });
            }
            else
            {
                new ComputerConfigFacade(this).Create(_currentVM, (s, args2) =>
                {
                    if (args2.FaultsHandle())
                        return;
                    Window.Alert(ResComputerConfig.Info_AddSuccess);
                    this.btnSave.IsEnabled = false;
                    Window.Close();
                });
            }
        }

        //private void DataGridParts_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    ComputerConfigMasterVM _vm = e.NewValue as ComputerConfigMasterVM;
        //    DataGrid _gridParts = sender as DataGrid;
        //    FrameworkElement _ele = null;
        //    DataGridCell cell = null;
        //    if (_vm != null)
        //    {
        //        foreach (var rowItem in _gridParts.ItemsSource)
        //        {
        //            foreach (var c in _gridParts.Columns)
        //            {
        //                _ele = c.GetCellContent(rowItem);
        //                if (_ele != null)
        //                {
        //                    cell = _ele.Parent as DataGridCell;
        //                    if (cell != null && cell.Content is TextBox)
        //                    {
        //                        ((TextBox)cell.Content).IsReadOnly = lstConfigTypes.IsEnabled;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }

}
