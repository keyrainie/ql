using ECCentral.Portal.UI.Invoice.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.Portal.UI.Invoice.Resources;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCPriceChangeSetter : UserControl
    {
        private PriceChangeVM vm;

        public IDialog DialogHanlder
        {
            get;
            set;
        }

        public bool? IsAuditModel { get; set; }

        public bool? IsBatchModel { get; set; }

        public UCPriceChangeSetter()
        {
            InitializeComponent();

            this.Loaded += UCPriceChangeSetter_Loaded;
        }

        void UCPriceChangeSetter_Loaded(object sender, RoutedEventArgs e)
        {
            if (null == vm)
            {
                vm = new PriceChangeVM();
                vm.ItemList = new System.Collections.ObjectModel.ObservableCollection<ChangeItemVM>();
            }

            // Single Audit Model
            if (IsAuditModel.HasValue && IsAuditModel.Value)
            {
                vm.TabSelectedIndex = 1;
                vm.IsAuditMemoVisibility = System.Windows.Visibility.Visible;
                vm.IsBatchAuditVisibility = System.Windows.Visibility.Visible;
                vm.IsChangeRateVisibility = System.Windows.Visibility.Collapsed;
            }
            else if (IsBatchModel.HasValue && IsBatchModel.Value)
            {
                vm.TabSelectedIndex = 1;
                vm.IsAuditMemoVisibility = System.Windows.Visibility.Visible;
                vm.IsBatchAuditVisibility = System.Windows.Visibility.Collapsed;
                vm.IsChangeRateVisibility = System.Windows.Visibility.Collapsed;
            }
            else if (!IsAuditModel.HasValue)
            {
                vm.TabSelectedIndex = 0;
                vm.IsAuditMemoVisibility = System.Windows.Visibility.Collapsed;
                vm.IsBatchAuditVisibility = System.Windows.Visibility.Collapsed;
                vm.IsChangeRateVisibility = System.Windows.Visibility.Visible;
            }

            this.LayoutRoot.DataContext = this.vm;
        }

        public UCPriceChangeSetter(PriceChangeVM vm)
            : this()
        {
            this.vm = vm;
        }

        private void btnSaveAuditMemo_Click(object sender, RoutedEventArgs e)
        {
            this.vm.ValidationErrors.Clear();

            if (string.IsNullOrEmpty(this.vm.AudtiMemo))
            {
                vm.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(
                   ResPriceChangeMaintain.Message_AuditRemark, new string[] { "AudtiMemo" }));

                return;
            }

            this.DialogHanlder.ResultArgs.Data = this.vm.AudtiMemo;
            this.DialogHanlder.ResultArgs.DialogResult = DialogResultType.OK;

            this.DialogHanlder.Close();
        }


        private void btnSaveRate_Click(object sender, RoutedEventArgs e)
        {
            this.vm.ValidationErrors.Clear();

            decimal? rate = null;
            if (!string.IsNullOrEmpty(this.ChangeRate.Text))
            {
                rate = Convert.ToDecimal(this.ChangeRate.Text.Trim()) / 100;
            }
            if (string.IsNullOrEmpty(this.ChangeRate.Text))
            {
                vm.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(
                    string.Format(ResPriceChangeMaintain.Message_NotNullChangeRate), new string[] { "ChangeRate" }));

                this.Dispatcher.BeginInvoke(() => this.ChangeRate.Focus());
                return;
            }

            Regex regex = new Regex(@"^[0-9]{1,5}|([0-9]\d+\.\d+)$", RegexOptions.IgnoreCase);
            if (!regex.IsMatch(this.vm.ChangeRate))
            {
                vm.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(
                    string.Format(ResPriceChangeMaintain.Message_ErrorChangeRate), new string[] { "ChangeRate" }));

                this.Dispatcher.BeginInvoke(() => this.ChangeRate.Focus());
                return;
            }

            this.DialogHanlder.ResultArgs.Data = rate;
            this.DialogHanlder.ResultArgs.DialogResult = DialogResultType.OK;

            this.DialogHanlder.Close();
        }
    }
}
