using System;
using System.Collections.Generic;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCLossTypeSetter : PopWindow
    {
        private List<CodeNamePair> lossTypeList;
        private List<ValidationEntity> validList;

        public UCLossTypeSetter()
        {
            InitializeComponent();
            validList = new List<ValidationEntity>();
            validList.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, this.cmbLossType.SelectedValue, ResUCLossTypeSetter.Msg_PleaseChooseLossType));
            Loaded += new System.Windows.RoutedEventHandler(UCLossTypeSetter_Loaded);
        }

        public UCLossTypeSetter(List<CodeNamePair> lossTypeList)
            : this()
        {
            this.lossTypeList = lossTypeList;
        }

        private void UCLossTypeSetter_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.cmbLossType.ItemsSource = this.lossTypeList;
            ValidationHelper.ClearValidation(this.cmbLossType);
        }

        private void btnOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var getSelected = this.cmbLossType.SelectedItem as CodeNamePair;
            var flag = ValidationHelper.Validation(cmbLossType, validList);
            if (!flag)
                return;

            ValidationHelper.ClearValidation(this.cmbLossType);
            this.CloseDialog(getSelected.Code, DialogResultType.OK);
        }
    }
}