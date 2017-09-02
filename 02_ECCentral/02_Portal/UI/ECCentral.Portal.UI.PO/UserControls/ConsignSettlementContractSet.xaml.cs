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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class ConsignSettlementContractSet : UserControl
    {
        public IDialog Dialog { get; set; }
        public ConsignSettlementItemInfoVM vm;
        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public IPage CurrentPage
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        List<ValidationEntity> validationList;

        public ConsignSettlementContractSet(ConsignSettlementItemInfoVM viewVM)
        {
            InitializeComponent();
            validationList = new List<ValidationEntity>();
            vm = new ConsignSettlementItemInfoVM();
            vm = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ConsignSettlementItemInfoVM>(viewVM);
            this.Loaded += new RoutedEventHandler(ConsignSettlementContractSet_Loaded);
        }

        #region [Events]

        void ConsignSettlementContractSet_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ConsignSettlementContractSet_Loaded;

            this.cmbContractReturnPointType.ItemsSource = EnumConverter.GetKeyValuePairs<ConsignSettleReturnPointCalcType>();
            if (vm.AcquireReturnPointType.HasValue)
            {
                if (vm.AcquireReturnPointType == 0)
                {
                    this.cmbContractReturnPointType.SelectedIndex = 0;
                }
                else
                {
                    this.cmbContractReturnPointType.SelectedIndex = 1;
                }
            }
            else
            {
                this.cmbContractReturnPointType.SelectedIndex = 0;
            }
            this.txtAcquirePoint.Text = this.vm.AcquireReturnPoint.HasValue ? vm.AcquireReturnPoint.Value.ToString("f2") : string.Empty;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationHelper.Validation(this.txtAcquirePoint, validationList))
            {
                return;
            }
            //保存:
            vm.AcquireReturnPointType = this.cmbContractReturnPointType.SelectedIndex;
            vm.AcquireReturnPoint = Convert.ToDecimal(this.txtAcquirePoint.Text.Trim());
            this.Dialog.ResultArgs.Data = new List<object>() { "Save", vm };
            Dialog.Close(true);
        }

        private void btnSaveSameProducts_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationHelper.Validation(this.txtAcquirePoint, validationList))
            {
                return;
            }
            //保存至相同商品
            vm.AcquireReturnPointType = this.cmbContractReturnPointType.SelectedIndex;
            vm.AcquireReturnPoint = Convert.ToDecimal(this.txtAcquirePoint.Text.Trim());
            this.Dialog.ResultArgs.Data = new List<object>() { "SaveSame", vm };
            Dialog.Close(true);
        }

        private void btnSaveAllProducts_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationHelper.Validation(this.txtAcquirePoint, validationList))
            {
                return;
            }
            //保存至所有商品
            vm.AcquireReturnPointType = this.cmbContractReturnPointType.SelectedIndex;
            vm.AcquireReturnPoint = Convert.ToDecimal(this.txtAcquirePoint.Text.Trim());
            this.Dialog.ResultArgs.Data = new List<object>() { "SaveAll", vm };
            Dialog.Close(true);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //取消
            Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            Dialog.ResultArgs.Data = null;
            Dialog.Close(true);
        }

        private void cmbContractReturnPointType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            validationList.Clear();
            if (this.cmbContractReturnPointType.SelectedIndex == 0)
            {
                this.lblAmt.Visibility = Visibility.Visible;
                this.lblAmtPercent.Visibility = Visibility.Collapsed;
                this.lblPercent.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.lblAmt.Visibility = Visibility.Collapsed;
                this.lblAmtPercent.Visibility = Visibility.Visible;
                this.lblPercent.Visibility = Visibility.Visible;
            }
            validationList.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, this.txtAcquirePoint.Text, "返点金额不能为空!"));
            validationList.Add(new ValidationEntity(ValidationEnum.IsDecimal, this.txtAcquirePoint.Text, "请输入小数!"));
        }


        #endregion

    }
}
