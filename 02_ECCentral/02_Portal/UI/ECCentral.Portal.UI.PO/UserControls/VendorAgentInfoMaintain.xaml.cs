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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Inventory;
using System.Windows.Data;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Components.UserControls.StockPicker;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class VendorAgentInfoMaintain : UserControl
    {
        public IDialog Dialog { get; set; }
        public bool EditFlag;
        public bool IsDeliveryControlHide;
        private VendorAgentInfoVM newAgentInfo;

        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }
        public VendorFacade vendorFacade;

        List<ValidationEntity> validationPeriodList;

        public VendorAgentInfoMaintain(VendorBasicInfoVM vendorBasicVM)
        {
            EditFlag = false;
            newAgentInfo = new VendorAgentInfoVM();
            validationPeriodList = new List<ValidationEntity>();
            InitializeComponent();
            BindComboBoxData();
            if (vendorBasicVM.ConsignFlag.HasValue && vendorBasicVM.ConsignFlag.Value == VendorConsignFlag.Consign)
            {
                //如果为"代销" ,则显示代销结算模式Row:
                SetSettleTypeVisible(Visibility.Visible);
            }
            else
            {
                //如果为非代销，则隐藏代销结算模式Row:
                SetSettleTypeVisible(Visibility.Collapsed);
            }
            if (null != vendorBasicVM.ExtendedInfo)
            {
                //if (vendorBasicVM.ExtendedInfo.ShippingType == VendorShippingType.MET || vendorBasicVM.ExtendedInfo.InvoiceType == VendorInvoiceType.MET || vendorBasicVM.ExtendedInfo.StockType == VendorStockType.MET)
                //{
                //    SetDeliveryAndOrderTimeControl(Visibility.Collapsed);
                //}
                //else
                //{
                //    SetDeliveryAndOrderTimeControl(Visibility.Visible);

                //}
                //开票方式=商家开票&仓储方式=商家仓储,显示 “该模式下前台顾客运费均免收，请确保佣金收取足以承受运费的支出”
                if (vendorBasicVM.ExtendedInfo.InvoiceType == VendorInvoiceType.MET && vendorBasicVM.ExtendedInfo.StockType == VendorStockType.MET)
                {
                    this.lblCommissionFeeAlert.Text = ResVendorMaintain.Label_Commission_Fee_Alert;
                    this.lblCommissionFeeAlert.Visibility = Visibility.Visible;
                }
                else
                {
                    this.lblCommissionFeeAlert.Visibility = Visibility.Collapsed;
                }
            }
            this.Loaded += new RoutedEventHandler(VendorAgentInfoMaintain_Loaded);
            SetAccessControl();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="editVendorAgentInfo"></param>
        /// <param name="vendorBasicVM"></param>
        /// <param name="consignFlag"></param>
        public VendorAgentInfoMaintain(VendorAgentInfoVM editVendorAgentInfo, VendorBasicInfoVM vendorBasicVM, bool isView)
        {
            EditFlag = true;
            newAgentInfo = new VendorAgentInfoVM();
            validationPeriodList = new List<ValidationEntity>();
            newAgentInfo = editVendorAgentInfo;

            InitializeComponent();
            BindComboBoxData();
            if (isView)
            {
                this.btnAddAgentInfo.Visibility = Visibility.Collapsed;
            }
            this.ucSaleStageSettings.VendorStageSaleSettingsList = editVendorAgentInfo.VendorCommissionInfo.SaleRuleEntity.StagedSaleRuleItems;

            if (vendorBasicVM.ConsignFlag.HasValue && vendorBasicVM.ConsignFlag.Value == VendorConsignFlag.Consign)
            {
                //如果为"代销" ,则显示代销结算模式Row:
                SetSettleTypeVisible(Visibility.Visible);
            }
            else
            {
                //如果为非代销，则隐藏代销结算模式Row:
                SetSettleTypeVisible(Visibility.Collapsed);
            }
            if (null != vendorBasicVM.ExtendedInfo)
            {
                //if (vendorBasicVM.ExtendedInfo.ShippingType == VendorShippingType.MET || vendorBasicVM.ExtendedInfo.InvoiceType == VendorInvoiceType.MET || vendorBasicVM.ExtendedInfo.StockType == VendorStockType.MET)
                //{
                //    SetDeliveryAndOrderTimeControl(Visibility.Collapsed);
                //}
                //else
                //{
                //    SetDeliveryAndOrderTimeControl(Visibility.Visible);
                //}
                //开票方式=商家开票&仓储方式=商家仓储,显示 “该模式下前台顾客运费均免收，请确保佣金收取足以承受运费的支出”
                if (vendorBasicVM.ExtendedInfo.InvoiceType == VendorInvoiceType.MET && vendorBasicVM.ExtendedInfo.StockType == VendorStockType.MET)
                {
                    this.lblCommissionFeeAlert.Text = ResVendorMaintain.Label_Commission_Fee_Alert;
                    this.lblCommissionFeeAlert.Visibility = Visibility.Visible;
                }
                else
                {
                    this.lblCommissionFeeAlert.Visibility = Visibility.Collapsed;
                }
            }

            if (editVendorAgentInfo.SettleType == SettleType.P)
            {
                this.rdoSettleType_O.IsChecked = false;
                this.rdoSettleType_P.IsChecked = true;

                this.lblSettlePercentage.Visibility = Visibility.Visible;
                this.txtSettlePercentage.Visibility = Visibility.Visible;
                this.txtSettlePercentage.Text = editVendorAgentInfo.SettlePercentage;
            }
            else
            {
                this.rdoSettleType_O.IsChecked = true;
                this.rdoSettleType_P.IsChecked = false;
            }

            this.btnAddAgentInfo.Content = ResVendorMaintain.Button_Agent_Modify;

            //绑定下单日期 - CheckBoxList:
            BindVendorBuyWeekDayCheckBoxList(string.IsNullOrEmpty(editVendorAgentInfo.RequestBuyWeekDay) ? editVendorAgentInfo.BuyWeekDay : editVendorAgentInfo.RequestBuyWeekDay);
            //待审核状态：显示"以下为修改待审核状态信息:"
            if (editVendorAgentInfo.RequestType == VendorModifyRequestStatus.Apply)
            {
                lblAuditText.Text = ResVendorMaintain.Msg_AuditAgentAlertText;
                lblAuditText.Visibility = Visibility.Visible;
            }
            this.Loaded += new RoutedEventHandler(VendorAgentInfoMaintain_Loaded);
            SetAccessControl();
        }

        private void BindComboBoxData()
        {
            CodeNamePairHelper.GetList("PO", "VendorAgentLevelNew", (obj, args) =>
             {
                 this.cmbAgentLevel.ItemsSource = args.Result;
             });

        }


        private void SetAccessControl()
        {
            //修改供应商佣金信息:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Vendor_EditVendorCommission))
            {
                this.groupBoxVendorCommissionInfo.IsEnabled = false;
            }
        }

        void VendorAgentInfoMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            vendorFacade = new VendorFacade(CPApplication.Current.CurrentPage);
            SwitchControlVisible();
            if (!EditFlag)
            {
                this.cmbAgentLevel.SelectedIndex = 0;
                this.DataContext = newAgentInfo;
                //绑定送货周期:
                //BuildStockShippingDeliveryTimeControl(null);
            }
            else
            {
                //绑定送货周期:
                //BuildStockShippingDeliveryTimeControl(() =>
                //{
                //    BindSendPeriodTextBox(string.IsNullOrEmpty(newAgentInfo.RequestSendPeriod) ? newAgentInfo.SendPeriod : newAgentInfo.RequestSendPeriod);
                //});
                this.cmbCategory.LoadCategoryCompleted += (s, args) =>
                {
                    this.DataContext = newAgentInfo;
                };
            }
        }

        private void SwitchControlVisible()
        {
            if (EditFlag)
            {
                this.cmbAgentLevel.IsEnabled = false;
                this.cmbCategory.IsAllowCategorySelect = false;
                this.cmbCategory.EnableThirdCategory = false;
                this.ucManufactuerAndVendor.IsAllowManufacturerAndBrandSelected = false;
            }
            else
            {
                this.cmbAgentLevel.IsEnabled = true;
            }
        }

        private void SetSettleTypeVisible(Visibility visibility)
        {
            this.txtSettleType.Visibility = visibility;
            this.spSettleType.Visibility = visibility;
        }

        //private void SetDeliveryAndOrderTimeControl(Visibility visibility)
        //{
        //    this.gbShippingDeliveryTime.Visibility = visibility;
        //    this.lblOrderTime.Visibility = visibility;
        //    this.spOrderTime.Visibility = visibility;
        //    IsDeliveryControlHide = visibility == Visibility.Visible ? false : true;
        //}

        private void btnAddAgentInfo_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            if (string.IsNullOrEmpty(newAgentInfo.ManufacturerInfo.ManufacturerNameDisplay))
            {
                CurrentWindow.Alert(ResVendorMaintain.Msg_ManufacturerRequired);
                return;
            }
            //foreach (var item in this.grid_ShippingDeliveryTime.Children)
            //{
            //    TextBox boxItem = item as TextBox;
            //    if (null != boxItem)
            //    {
            //        if (!ValidationHelper.Validation(boxItem, validationPeriodList))
            //        {
            //            return;
            //        }
            //    }
            //}
            //验证必填项 :
            RefreshVendorSaleRuleList();
            newAgentInfo.BuyWeekDay = BuildSelectVendorBuyWeekDayString();
            newAgentInfo.SendPeriod = BuildSendPeriodString();
            newAgentInfo.VendorCommissionInfo.SaleRuleEntity.StagedSaleRuleItems = this.ucSaleStageSettings.VendorStageSaleSettingsList;
            if (newAgentInfo.VendorCommissionInfo.SaleRuleEntity.StagedSaleRuleItems != null && newAgentInfo.VendorCommissionInfo.SaleRuleEntity.StagedSaleRuleItems.Count > 0)
            {
                newAgentInfo.VendorCommissionInfo.SaleRuleEntity.StagedSaleRuleItems[newAgentInfo.VendorCommissionInfo.SaleRuleEntity.StagedSaleRuleItems.Count - 1].EndAmt = 99999999;
            }
            newAgentInfo.VendorCommissionInfo.SaleRuleEntity.MinCommissionAmt = string.IsNullOrEmpty(newAgentInfo.VendorCommissionInfo.GuaranteedAmt) ? (decimal?)null : decimal.Parse(newAgentInfo.VendorCommissionInfo.GuaranteedAmt);
            if (spSettleType.Visibility == Visibility.Collapsed)
            {
                newAgentInfo.SettleType = null;
            }
            else
            {
                newAgentInfo.SettleType = (this.rdoSettleType_O.IsChecked.HasValue && this.rdoSettleType_O.IsChecked.Value) ? SettleType.O : SettleType.P;
            }

            if (!EditFlag)
            {
                newAgentInfo.C2Name = this.cmbCategory.Category2Name;
                newAgentInfo.C3Name = this.cmbCategory.Category3Name;

                newAgentInfo.RowState = VendorRowState.Added;
            }
            else
            {
                newAgentInfo.RowState = newAgentInfo.AgentSysNo.HasValue && newAgentInfo.AgentSysNo.Value > 0 ? VendorRowState.Modified : VendorRowState.Added;
            }
            this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
            this.Dialog.ResultArgs.Data = newAgentInfo;
            this.Dialog.Close(true);
        }

        private void rdoSettleType_O_Click(object sender, RoutedEventArgs e)
        {
            if (this.rdoSettleType_O.IsChecked.HasValue && this.rdoSettleType_O.IsChecked.Value)
            {
                newAgentInfo.SettleType = SettleType.O;
                this.txtSettlePercentage.Text = string.Empty;
                this.txtSettlePercentage.Visibility = Visibility.Collapsed;
                this.lblSettlePercentage.Visibility = Visibility.Collapsed;
            }
            else
            {
                newAgentInfo.SettleType = SettleType.P;
                this.txtSettlePercentage.Visibility = Visibility.Visible;
                this.lblSettlePercentage.Visibility = Visibility.Visible;

            }
        }
        private void rdoSettleType_P_Click(object sender, RoutedEventArgs e)
        {
            if (this.rdoSettleType_O.IsChecked.HasValue && this.rdoSettleType_O.IsChecked.Value)
            {
                newAgentInfo.SettleType = SettleType.O;
                this.txtSettlePercentage.Text = string.Empty;
                this.txtSettlePercentage.Visibility = Visibility.Collapsed;
                this.lblSettlePercentage.Visibility = Visibility.Collapsed;

            }
            else
            {
                newAgentInfo.SettleType = SettleType.P;
                this.txtSettlePercentage.Visibility = Visibility.Visible;
                this.lblSettlePercentage.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 重新构建SaleRuleItems
        /// </summary>
        private void RefreshVendorSaleRuleList()
        {
            if (null != ucSaleStageSettings.VendorStageSaleSettingsList)
            {
                this.ucSaleStageSettings.VendorStageSaleSettingsList.Clear();
            }
            else
            {
                this.ucSaleStageSettings.VendorStageSaleSettingsList = new List<VendorStagedSaleRuleInfoVM>();
            }
            int index = 1;
            foreach (var ucItem in this.ucSaleStageSettings.spSaleStageSettings.Children)
            {
                if (ucItem is VendorSaleStageSettingsItem)
                {
                    VendorSaleStageSettingsItem uc = (VendorSaleStageSettingsItem)ucItem;
                    this.ucSaleStageSettings.VendorStageSaleSettingsList.Add(new VendorStagedSaleRuleInfoVM()
                    {
                        Order = index,
                        StartAmt = uc.StageAmtBeginVal,
                        EndAmt = uc.StageAmtEndVal,
                        Percentage = uc.StagePercentage
                    });
                }
                index++;
            }

            if (!string.IsNullOrEmpty(this.ucSaleStageSettings.txtStagedPercentage_Last.Text))
            {
                this.ucSaleStageSettings.VendorStageSaleSettingsList.Add(new VendorStagedSaleRuleInfoVM()
                {
                    Order = index,
                    StartAmt = string.IsNullOrEmpty(this.ucSaleStageSettings.lblStagedAmtBegin_Last.Text) ? 0 : decimal.Parse(this.ucSaleStageSettings.lblStagedAmtBegin_Last.Text),
                    EndAmt = 0,
                    Percentage = string.IsNullOrEmpty(this.ucSaleStageSettings.txtStagedPercentage_Last.Text) ? 0 : decimal.Parse(this.ucSaleStageSettings.txtStagedPercentage_Last.Text),
                });
            }
        }

        #region [销售提成 - 阶梯设置]

        //private void btnAddSaleStageSetting_Click(object sender, RoutedEventArgs e)
        //{
        //    int getCurrentStageCount = this.spSaleStageSettings.Children.Count;
        //    decimal? getLastStageAmtEnd = null;
        //    decimal? getLastStagePercentage = null;

        //    if (0 < getCurrentStageCount)
        //    {
        //        getLastStageAmtEnd = ((VendorSaleStageSettings)this.spSaleStageSettings.Children.Last()).StageAmtEndVal;
        //        getLastStagePercentage = ((VendorSaleStageSettings)this.spSaleStageSettings.Children.Last()).StagePercentage;
        //    }
        //    VendorSaleStageSettings newStageAddSetting = new VendorSaleStageSettings() { Name = string.Format("ucSaleStageSetting_{0}", (getCurrentStageCount + 1)), StageAmtBeginVal = getLastStageAmtEnd };
        //    newStageAddSetting.OnBeginStageAmtChanged += (obj, args) =>
        //    {
        //        SetSaleStageChangedEvent(args);
        //    };
        //    this.spSaleStageSettings.Children.Add(newStageAddSetting);
        //}

        //private void ucSaleStageSetting_1_OnBeginStageAmtChanged(object sender, SaleStageValEventArgs e)
        //{
        //    SetSaleStageChangedEvent(e);
        //}

        //private void SetSaleStageChangedEvent(SaleStageValEventArgs args)
        //{
        //    foreach (var ucItem in this.spSaleStageSettings.Children)
        //    {
        //        if (ucItem is VendorSaleStageSettings)
        //        {
        //            if (((VendorSaleStageSettings)ucItem).Name.EndsWith("_" + args.ControlIndex))
        //            {
        //                ((VendorSaleStageSettings)ucItem).StageAmtBeginVal = args.NewStageEndVal;
        //                break;
        //            }
        //        }
        //    }
        //}

        #endregion

        /// <summary>
        /// 构建下单日期 - CheckBoxList
        /// </summary>
        /// <returns></returns>
        private string BuildSelectVendorBuyWeekDayString()
        {
            string returnStr = string.Empty;
            //foreach (var chkItem in this.spOrderTime.Children)
            //{
            //    CheckBox chkBox = chkItem as CheckBox;
            //    if (null != chkBox && chkBox.IsChecked.HasValue && chkBox.IsChecked.Value)
            //    {
            //        returnStr += string.Format("{0};", chkBox.Tag.ToString());
            //    }
            //}
            return (!string.IsNullOrEmpty(returnStr) ? returnStr.TrimEnd(';') : returnStr);

        }

        private void BindVendorBuyWeekDayCheckBoxList(string buyWeekDayString)
        {
            //if (!string.IsNullOrEmpty(buyWeekDayString))
            //{
            //    string[] strs = buyWeekDayString.Split(';');
            //    foreach (var strWeek in strs)
            //    {
            //        if (!string.IsNullOrEmpty(strWeek))
            //        {
            //            var chkItem = this.spOrderTime.Children.SingleOrDefault(i => ((CheckBox)i).Tag.ToString() == strWeek.Trim()) as CheckBox;
            //            if (null != chkItem)
            //            {
            //                ((CheckBox)chkItem).IsChecked = true;
            //            }
            //        }
            //    }
            //}
        }

        private string BuildSendPeriodString()
        {
            string returnStr = string.Empty;
            //foreach (var item in this.grid_ShippingDeliveryTime.Children)
            //{
            //    TextBox boxItem = item as TextBox;
            //    if (null != boxItem)
            //    {
            //        string getStockNumber = boxItem.Name.Split('_')[1];
            //        string getDeliveryTime = boxItem.Text;
            //        if (!string.IsNullOrEmpty(getDeliveryTime))
            //        {
            //            returnStr += string.Format("{0}:{1};", getStockNumber, getDeliveryTime);
            //        }
            //    }
            //}
            return (!string.IsNullOrEmpty(returnStr) ? returnStr.TrimEnd(';') : returnStr);
        }

        private void BindSendPeriodTextBox(string sendPeriodString)
        {
            //if (!string.IsNullOrEmpty(sendPeriodString))
            //{
            //    string[] strs = sendPeriodString.Split(';');
            //    foreach (var strTime in strs)
            //    {
            //        if (!string.IsNullOrEmpty(strTime))
            //        {
            //            string stockNumber = strTime.Split(':')[0];
            //            string stockDeliveryTime = strTime.Split(':')[1];

            //            var txtItem = this.grid_ShippingDeliveryTime.Children.SingleOrDefault(item => item is TextBox && ((TextBox)item).Name == "txtStockNumber_" + stockNumber);
            //            if (null != txtItem)
            //            {
            //                ((TextBox)txtItem).Text = stockDeliveryTime;
            //            }
            //        }
            //    }
            //}
        }


        //private void BuildStockShippingDeliveryTimeControl(Action action)
        //{
        //    if (!IsDeliveryControlHide)
        //    {
        //        vendorFacade.GetWarehouseInfo((obj, args) =>
        //         {
        //             if (args.FaultsHandle())
        //             {
        //                 return;
        //             }
        //             List<WarehouseInfo> getStockList = args.Result;
        //             if (null != getStockList)
        //             {
        //                 getStockList.RemoveAll(x => x.SysNo == 90 || x.SysNo == 99 || x.SysNo == 59);
        //                 getStockList = getStockList.Where(x => x.WarehouseStatus == BizEntity.Inventory.ValidStatus.Valid).ToList();
        //                 getStockList = getStockList.OrderBy(x => x.SysNo).ToList();
        //             }

        //             int getRowDefCount = getStockList.Count <= 3 ? 1 : (getStockList.Count % 2 == 0 ? getStockList.Count / 3 : getStockList.Count / 3 + 1);
        //             for (int i = 0; i <= getRowDefCount; i++)
        //             {
        //                 grid_ShippingDeliveryTime.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
        //             }

        //             for (int index = 0; index < getStockList.Count; index++)
        //             {
        //                 int getLabelColumnIndex = index % 3 + 2 * (index % 3);
        //                 int getTextBoxColumnIndex = getLabelColumnIndex + 1;
        //                 int getDescColumnIndex = getTextBoxColumnIndex + 1;
        //                 int getRowIndex = index / 3;


        //                 Label newStockLabel = new Label();
        //                 newStockLabel.Margin = new System.Windows.Thickness(15, 0, 0, 0);
        //                 newStockLabel.Content = string.Format("{0}:", getStockList[index].WarehouseName);
        //                 newStockLabel.SetValue(Grid.RowProperty, getRowIndex);
        //                 newStockLabel.SetValue(Grid.ColumnProperty, getLabelColumnIndex);
        //                 grid_ShippingDeliveryTime.Children.Add(newStockLabel);

        //                 TextBox newTextBox = new TextBox();
        //                 newTextBox.SetValue(Grid.RowProperty, getRowIndex);
        //                 newTextBox.SetValue(Grid.ColumnProperty, getTextBoxColumnIndex);
        //                 newTextBox.Width = 140;
        //                 newTextBox.Name = string.Format("txtStockNumber_{0}", getStockList[index].SysNo.ToString());
        //                 grid_ShippingDeliveryTime.Children.Add(newTextBox);
        //                 //添加验证:
        //                 validationPeriodList.Add(new ValidationEntity(ValidationEnum.IsInteger, newTextBox.Text, ResVendorMaintain.Msg_IntegerRequired));

        //                 Label newTextLabel = new Label();
        //                 newTextLabel.Content = ResVendorMaintain.Label_Agent_Stock_Day;
        //                 newTextLabel.SetValue(Grid.RowProperty, getRowIndex);
        //                 newTextLabel.SetValue(Grid.ColumnProperty, getDescColumnIndex);
        //                 grid_ShippingDeliveryTime.Children.Add(newTextLabel);
        //             }
        //             if (null != action)
        //             {
        //                 action();
        //             }
        //         });
        //    }
        //}

    }
}
