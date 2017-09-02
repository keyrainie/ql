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
using ECCentral.Portal.UI.PO.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.PO.Resources;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class VendorSaleStageSettings : UserControl
    {

        public List<VendorStagedSaleRuleInfoVM> VendorStageSaleSettingsList
        {
            get;
            set;
        }
        public StageValidateVM validateVM;
        public VendorSaleStageSettings()
        {
            InitializeComponent();
            VendorStageSaleSettingsList = new List<VendorStagedSaleRuleInfoVM>();
            validateVM = new StageValidateVM();
            this.Loaded += new RoutedEventHandler(VendorSaleStageSettings_Loaded);
        }
        public VendorSaleStageSettings(List<VendorStagedSaleRuleInfoVM> vendorSaleRuleList)
        {
            InitializeComponent();
            VendorStageSaleSettingsList = new List<VendorStagedSaleRuleInfoVM>();
            VendorStageSaleSettingsList = vendorSaleRuleList;
            validateVM = new StageValidateVM();
            this.Loaded += new RoutedEventHandler(VendorSaleStageSettings_Loaded);
        }

        void VendorSaleStageSettings_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = validateVM;
            if (null != VendorStageSaleSettingsList && 0 < VendorStageSaleSettingsList.Count)
            {
                BindVendorSaleStageList();
            }
        }

        public void BindVendorSaleStageList()
        {
            for (int index = 0; index < VendorStageSaleSettingsList.Count - 1; ++index)
            {
                int getCurrentStageCount = this.spSaleStageSettings.Children.Count;
                VendorSaleStageSettingsItem newStageAddSettingItem = new VendorSaleStageSettingsItem() { Name = string.Format("ucStageSettingItem_{0}", (getCurrentStageCount + 1)), StageAmtBeginVal = VendorStageSaleSettingsList[index].StartAmt, StageAmtEndVal = VendorStageSaleSettingsList[index].EndAmt, StagePercentage = VendorStageSaleSettingsList[index].Percentage };
                newStageAddSettingItem.OnBeginStageAmtChanged += (obj, args) =>
                {
                    SetSaleStageChangedEvent(args);
                };
                this.spSaleStageSettings.Children.Add(newStageAddSettingItem);
            }
            if (VendorStageSaleSettingsList.Count >= 1)
            {
                this.lblStagedAmtBegin_Last.Text = VendorStageSaleSettingsList[VendorStageSaleSettingsList.Count - 1].StartAmt.HasValue ? VendorStageSaleSettingsList[VendorStageSaleSettingsList.Count - 1].StartAmt.Value.ToString() : "0";
                this.lblStagedAmtEnd_Last.Text = "MAX";
                this.txtStagedPercentage_Last.Text = VendorStageSaleSettingsList[VendorStageSaleSettingsList.Count - 1].Percentage.HasValue ? VendorStageSaleSettingsList[VendorStageSaleSettingsList.Count - 1].Percentage.Value.ToString() : "0";
            }
        }

        private void btnAddSaleStageSetting_Click(object sender, RoutedEventArgs e)
        {
            int getCurrentStageCount = this.spSaleStageSettings.Children.Count;
            decimal? getLastStageAmtEnd = null;
            decimal? getLastStagePercentage = null;

            if (0 < getCurrentStageCount)
            {
                getLastStageAmtEnd = ((VendorSaleStageSettingsItem)this.spSaleStageSettings.Children.Last()).StageAmtEndVal;
                getLastStagePercentage = ((VendorSaleStageSettingsItem)this.spSaleStageSettings.Children.Last()).StagePercentage;
            }
            if (getCurrentStageCount < 3)
            {
                if (!string.IsNullOrEmpty(this.lblStagedAmtBegin_Last.Text))
                {
                    this.lblStagedAmtBegin_Last.Text = string.Empty;
                }
                //如果没有超过最大添加数量(3个阶梯),则添加新的UserControl:
                VendorSaleStageSettingsItem newStageAddSettingItem = new VendorSaleStageSettingsItem() { Name = string.Format("ucStageSettingItem_{0}", (getCurrentStageCount + 1)), StageAmtBeginVal = getLastStageAmtEnd };
                if (getCurrentStageCount > 0)
                {
                    newStageAddSettingItem.lblStagedAmtBegin.Text = !getLastStageAmtEnd.HasValue ? string.Empty : getLastStageAmtEnd.Value.ToString();
                }
                newStageAddSettingItem.OnBeginStageAmtChanged += (obj, args) =>
                {
                    SetSaleStageChangedEvent(args);
                };
                this.spSaleStageSettings.Children.Add(newStageAddSettingItem);
            }
            //如果超过最大添加数量(3个阶梯),更新BUTTON状态:
            UpdateOperationButtionsState();
        }

        private void UpdateOperationButtionsState()
        {
            int getCurrentStageCount = this.spSaleStageSettings.Children.Count;
            if (getCurrentStageCount >= 3)
            {
                this.btnAddSaleStageSetting.IsEnabled = false;
            }
            else
            {
                this.btnAddSaleStageSetting.IsEnabled = true;
            }

        }

        private void SetSaleStageChangedEvent(ECCentral.Portal.UI.PO.UserControls.VendorSaleStageSettingsItem.SaleStageValEventArgs args)
        {
            foreach (var ucItem in this.spSaleStageSettings.Children)
            {
                if (ucItem is VendorSaleStageSettingsItem)
                {
                    if (((VendorSaleStageSettingsItem)ucItem).Name.EndsWith("_" + args.ControlIndex))
                    {
                        ((VendorSaleStageSettingsItem)ucItem).StageAmtBeginVal = args.NewStageEndVal;
                        break;
                    }
                }
            }
            //更新最后一个阶梯的StageBeginAmt:
            if (0 < this.spSaleStageSettings.Children.Count)
            {
                this.lblStagedAmtBegin_Last.Text = ((VendorSaleStageSettingsItem)spSaleStageSettings.Children.Last()).StageAmtEndVal.HasValue ? ((VendorSaleStageSettingsItem)spSaleStageSettings.Children.Last()).StageAmtEndVal.Value.ToString() : string.Empty;
            }
        }

        private void btnResetSaleStageSetting_Click(object sender, RoutedEventArgs e)
        {
            int getTotalControlCount = this.spSaleStageSettings.Children.Count;
            for (int index = getTotalControlCount - 1; index >= 0; index--)
            {
                this.spSaleStageSettings.Children.RemoveAt(index);
            }
            this.lblStagedAmtBegin_Last.Text = "0";
            this.txtStagedPercentage_Last.Text = string.Empty;
            if (!this.btnAddSaleStageSetting.IsEnabled)
            {
                this.btnAddSaleStageSetting.IsEnabled = true;
            }
        }

    }


    public class StageValidateVM : ModelBase
    {
        private string amt;
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,6}$", ErrorMessageResourceName = "Decimal_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public string Amt
        {
            get { return amt; }
            set { base.SetValue("Amt", ref amt, value); }
        }

        private string percent;
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,6}$", ErrorMessageResourceName = "Decimal_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public string Percent
        {
            get { return percent; }
            set { base.SetValue("Percent", ref percent, value); }
        }
    }

}
