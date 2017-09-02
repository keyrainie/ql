using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic.Components.UserControls.AreaPicker;
using ECCentral.Portal.UI.Common.Facades;
using ECCentral.Portal.UI.Common.Models;
using ECCentral.Portal.UI.Common.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;

namespace ECCentral.Portal.UI.Common.UserControls
{
    public partial class UCFreeShippingChargeRule : UserControl
    {
        public IPage CurrentPage
        {
            get { return CPApplication.Current.CurrentPage; }
        }

        private IDialog _currentDialog;
        public IDialog CurrentDialog
        {
            get { return _currentDialog; }
            set
            {
                _currentDialog = value;
                if (value != null)
                {
                    _currentDialog.Closing -= CurrentDialog_Closing;
                    _currentDialog.Closing += CurrentDialog_Closing;
                }
            }
        }

        private int? _sysNo;
        private FreeShippingChargeRuleFacade _facade;
        private AreaQueryFacade _areaQueryFacade;
        private FreeShippingChargeRuleVM _model;
        private bool _dataStateHasUpdated = false;

        public UCFreeShippingChargeRule(int? sysNo)
        {
            InitializeComponent();

            _sysNo = sysNo;

            Loaded += UCFreeShippingChargeRule_Loaded;
        }

        private void UCFreeShippingChargeRule_Loaded(object sender, RoutedEventArgs e)
        {
            _facade = new FreeShippingChargeRuleFacade(CurrentPage);
            _areaQueryFacade = new AreaQueryFacade();

            _areaQueryFacade.QueryProvinceAreaList((_, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<AreaInfo> areaList = args.Result;
                if (areaList == null)
                {
                    areaList = new List<AreaInfo>();
                }
                areaList.Insert(0, new AreaInfo() { ProvinceName = ResCommonEnum.Enum_Select });
                cmbArea.ItemsSource = areaList;
            });

            if (_sysNo.HasValue && _sysNo.Value > 0)
            {
                _facade.Load(_sysNo.Value, (_, args) =>
                {
                    _model = args.Result;
                    LayoutRoot.DataContext = _model;
                    this.SetButtonState();
                });
            }
            else
            {
                _model = new FreeShippingChargeRuleVM() { Status = FreeShippingAmountSettingStatus.DeActive, IsGlobal = false };
                LayoutRoot.DataContext = _model;

                this.SetButtonState();
            }
        }

        private void CurrentDialog_Closing(object sender, ClosingEventArgs e)
        {
            if (_dataStateHasUpdated)
            {
                CurrentDialog.ResultArgs.DialogResult = DialogResultType.OK;
            }
        }

        private void SetButtonState()
        {
            if (AuthMgr.HasFunctionPoint(AuthKeyConst.Common_FreeShippingChargeRule_Edit))
            {
                btnSave.IsEnabled = true;
            }
            else
            {
                btnSave.IsEnabled = false;
            }

            if (AuthMgr.HasFunctionPoint(AuthKeyConst.Common_FreeShippingChargeRule_Invalid) && 
                _model.SysNo.HasValue && _model.Status == FreeShippingAmountSettingStatus.Active)
            {
                btnInvalid.IsEnabled = true;
            }
            else
            {
                btnInvalid.IsEnabled = false;
            }

            if (AuthMgr.HasFunctionPoint(AuthKeyConst.Common_FreeShippingChargeRule_Valid) && 
                _model.SysNo.HasValue && _model.Status == FreeShippingAmountSettingStatus.DeActive)
            {
                btnValid.IsEnabled = true;
            }
            else
            {
                btnValid.IsEnabled = false;
            }

            if (_model.Status == FreeShippingAmountSettingStatus.Active)
            {
                UtilityHelper.ReadOnlyControl(LayoutRoot, LayoutRoot.Children.Count, true);
                btnSave.IsEnabled = false;
            }
            else
            {
                UtilityHelper.ReadOnlyControl(LayoutRoot, LayoutRoot.Children.Count, false);
                btnSave.IsEnabled = true;
            }
        }



        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!_model.StartDate.HasValue || !_model.EndDate.HasValue)
            {
                if (!_model.StartDate.HasValue)
                {
                    Message("开始日期不能为空。");
                    return;
                }
                if (!_model.EndDate.HasValue)
                {
                    Message("结束日期不能为空。");
                    return;
                }
            }
            else
            {
                if (_model.StartDate.Value > _model.EndDate.Value)
                {
                    Message("有效期开始日期不能大于截止日期。");
                    return;
                }
            }
            if (!ValidationManager.Validate(this.LayoutRoot))
            {
                return;
            }

            _facade.Save(_model, (_, args) =>
            {
                _model = args.Result;
                _dataStateHasUpdated = true;
                LayoutRoot.DataContext = _model;
                this.SetButtonState();

                this.Message("保存成功！");
            });
        }

        private void btnValid_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage.Context.Window.Confirm("你确定要将该条免运费规则设置为有效吗？", (_, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    _facade.Valid(_model.SysNo.Value, (a, b) =>
                    {
                        if (b.FaultsHandle())
                        {
                            return;
                        }
                        _model.Status = FreeShippingAmountSettingStatus.Active;
                        _dataStateHasUpdated = true;
                        this.SetButtonState();
                        this.Message("设置成功！");
                    });
                }
            });
        }

        private void btnInvalid_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage.Context.Window.Confirm("你确定要将该条免运费规则设置为无效吗？", (_, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    _facade.Invalid(_model.SysNo.Value, (a, b) =>
                    {
                        if (b.FaultsHandle())
                        {
                            return;
                        }
                        _model.Status = FreeShippingAmountSettingStatus.DeActive;
                        _dataStateHasUpdated = true;
                        this.SetButtonState();
                        this.Message("设置成功！");
                    });
                }
            });
        }

        private void btnAddPayType_Click(object sender, RoutedEventArgs e)
        {
            var selPaytype = cmbPayType.SelectedPayTypeItem;
            if (!selPaytype.SysNo.HasValue)
            {
                this.Message("请先选择支付类型！");
                return;
            }

            var selectedItem = _model.PayTypeSettingValue.Where(x => x.ID == selPaytype.SysNo.Value.ToString());
            if (selectedItem != null && selectedItem.Count() > 0)
            {
                this.Message(string.Format("已经存在相同的支付方式【{0}】！", selectedItem.First().Name));
                return;
            }

            SimpleObject simpleObject = new SimpleObject(selPaytype.SysNo, selPaytype.SysNo.Value.ToString(), selPaytype.PayTypeName);
            _model.PayTypeSettingValue.Add(simpleObject);
        }

        private void btnAddShipArea_Click(object sender, RoutedEventArgs e)
        {
            var selArea = (AreaInfo)cmbArea.SelectedItem;
            if (!selArea.SysNo.HasValue)
            {
                this.Message("请先选择配送区域！");
                return;
            }

            var selectedItem = _model.ShipAreaSettingValue.Where(x => x.ID == selArea.SysNo.Value.ToString());
            if (selectedItem != null && selectedItem.Count() > 0)
            {
                this.Message(string.Format("已经存在相同的配送区域【{0}】！", selectedItem.First().Name));
                return;
            }

            SimpleObject simpleObject = new SimpleObject(selArea.SysNo, selArea.SysNo.Value.ToString(), selArea.ProvinceName);
            _model.ShipAreaSettingValue.Add(simpleObject);
        }


        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            UCProductSearch ucProductSearch = new UCProductSearch() { SelectionMode = SelectionMode.Multiple };
            ucProductSearch.DialogHandler = CurrentPage.Context.Window.ShowDialog(ResProductPicker.Dialog_Title, ucProductSearch, OnProductSelected);
        }

        private void OnProductSelected(object sender, ResultEventArgs e)
        {
            if (e.DialogResult == DialogResultType.OK)
            {
                var selectedProductList =  (List<ProductVM>)e.Data;
                foreach (var productItem in selectedProductList)
                {
                    if (!_model.ProductSettingValue.Any(x => x.SysNo == productItem.SysNo))
                    {
                        SimpleObject productObject = new SimpleObject()
                        {
                            SysNo = productItem.SysNo,
                            ID = productItem.ProductID,
                            Name = productItem.ProductName
                        };
                        _model.ProductSettingValue.Add(productObject);
                    }
                }
            }
        }

        private void RemovePayTypeItem_Click(object sender, MouseButtonEventArgs e)
        {
            TextBlock label = sender as TextBlock;
            if (_model.Status != FreeShippingAmountSettingStatus.Active &&
                label != null && label.DataContext != null && label.DataContext is SimpleObject)
            {
                SimpleObject simpleObject = (SimpleObject)label.DataContext;
                _model.PayTypeSettingValue.Remove(simpleObject);
            }
        }

        private void RemoveShipAreaItem_Click(object sender, MouseButtonEventArgs e)
        {
            TextBlock label = sender as TextBlock;
            if (_model.Status != FreeShippingAmountSettingStatus.Active && 
                label != null && label.DataContext != null && label.DataContext is SimpleObject)
            {
                SimpleObject simpleObject = (SimpleObject)label.DataContext;
                _model.ShipAreaSettingValue.Remove(simpleObject);
            }
        }

        private void RemoveProductItem_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hyperlinkButton = sender as HyperlinkButton;
            if (_model.Status != FreeShippingAmountSettingStatus.Active && 
                hyperlinkButton != null && hyperlinkButton.DataContext != null && hyperlinkButton.DataContext is SimpleObject)
            {
                SimpleObject simpleObject = (SimpleObject)hyperlinkButton.DataContext;
                _model.ProductSettingValue.Remove(simpleObject);
            }
        }

        private void ckbIsGlobal_Click(object sender, RoutedEventArgs e)
        {
            if (ckbIsGlobal.IsChecked == true)
            {
                this.Confirm("当前已有设置的数据，改变\"是否全网商品\"的选择后，现有设置将清除！请确认是否要清除？", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        _model.ProductSettingValue.Clear();
                    }
                    else
                    {
                        ckbIsGlobal.IsChecked = false;
                    }
                });
            }
        }

        #region 辅助方法

        private void Message(string msg)
        {
            CurrentPage.Context.Window.Alert(msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
        }

        private void Confirm(string content, ResultHandler callback)
        {
            CurrentPage.Context.Window.Confirm("确认", content, callback);
        }


        #endregion 辅助方法
    }
}