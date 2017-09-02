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

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class VendorSaleStageSettingsItem : UserControl
    {
        public delegate void BeginStageAmtChangedHandler(object sender, SaleStageValEventArgs e);
        public event BeginStageAmtChangedHandler OnBeginStageAmtChanged;

        public decimal? StageAmtBeginVal
        {
            get
            {
                return string.IsNullOrEmpty(this.lblStagedAmtBegin.Text) ? (decimal?)null : decimal.Parse(this.lblStagedAmtBegin.Text);
            }
            set
            {

                this.lblStagedAmtBegin.Text = value.HasValue ? value.ToString() : "0";
            }
        }
        public decimal? StageAmtEndVal
        {
            get
            {
                return string.IsNullOrEmpty(this.txtStagedAmtEnd.Text) ? (decimal?)null : decimal.Parse(this.txtStagedAmtEnd.Text);

            }
            set
            {

                this.txtStagedAmtEnd.Text = value.HasValue ? value.ToString() : string.Empty;
            }
        }
        public decimal? StagePercentage
        {
            get
            {
                return string.IsNullOrEmpty(this.txtStagedPercentage.Text) ? (decimal?)null : decimal.Parse(this.txtStagedPercentage.Text);

            }
            set
            {

                this.txtStagedPercentage.Text = value.HasValue ? value.ToString() : string.Empty;
            }
        }
        public StageValidateVM vm;
        public VendorSaleStageSettingsItem()
        {
            InitializeComponent();
            this.DataContext = vm;
            vm = new StageValidateVM();
        }

        public class SaleStageValEventArgs : EventArgs
        {
            public SaleStageValEventArgs() { }

            public decimal? NewStageEndVal { get; set; }
            public string ControlIndex { get; set; }
        }

        private void txtStagedAmtEnd_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal getInputInt = 0;
            if (!decimal.TryParse(txtStagedAmtEnd.Text.Trim(), out getInputInt))
            {
                this.vm.Amt = "0";
            }
            if (null != OnBeginStageAmtChanged)
            {
                SaleStageValEventArgs args = new SaleStageValEventArgs()
                {
                    NewStageEndVal = getInputInt,
                    ControlIndex = (int.Parse(this.Name.Split('_')[1]) + 1).ToString()

                };
                OnBeginStageAmtChanged(sender, args);
            }
        }
    }
}
