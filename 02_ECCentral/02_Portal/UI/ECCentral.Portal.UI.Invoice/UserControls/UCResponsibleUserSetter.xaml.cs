using System;
using System.Collections.Generic;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCResponsibleUserSetter : PopWindow
    {
        private List<CodeNamePair> responsibleUserList;
        private List<ValidationEntity> validList;

        public UCResponsibleUserSetter()
        {
            InitializeComponent();
            validList = new List<ValidationEntity>();
            validList.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, this.cmbResponsibleUser.SelectedValue,
                ResUCResponsibleUserSetter.Msg_PleaseChooseResponsibleUser));
            Loaded += new System.Windows.RoutedEventHandler(UCResponsibleUserSetter_Loaded);
        }

        public UCResponsibleUserSetter(List<CodeNamePair> responsibleUserList)
            : this()
        {
            this.responsibleUserList = responsibleUserList;
        }

        private void UCResponsibleUserSetter_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.cmbResponsibleUser.ItemsSource = this.responsibleUserList;
            ValidationHelper.ClearValidation(this.cmbResponsibleUser);
        }

        private void btnOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var getSelected = this.cmbResponsibleUser.SelectedItem as CodeNamePair;
            var flag = ValidationHelper.Validation(cmbResponsibleUser, validList);
            if (!flag)
                return;

            ValidationHelper.ClearValidation(this.cmbResponsibleUser);
            this.CloseDialog(getSelected.Code, DialogResultType.OK);
        }
    }
}