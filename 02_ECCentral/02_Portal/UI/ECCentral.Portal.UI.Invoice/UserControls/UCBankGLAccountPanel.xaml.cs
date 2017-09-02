using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Resources;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    /// <summary>
    /// 现金支付需提供银行科目
    /// </summary>
    public partial class UCBankGLAccountPanel : PopWindow
    {
        List<ValidationEntity> validList;

        public UCBankGLAccountPanel()
        {
            InitializeComponent();
            validList = new List<ValidationEntity>();
            validList.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResUCBankGLAccountPanel.Message_BankGLAccountRequired));
            validList.Add(new ValidationEntity(ValidationEnum.RegexCheck, @"^\d{0,10}$", ResUCBankGLAccountPanel.Message_InvalidBankGLAccount));
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            CloseDialog(this.tbBankGLAccount.Text, Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK);
        }

        private bool ValidateInput()
        {
            return ValidationHelper.Validation(this.tbBankGLAccount, validList);
        }
    }
}