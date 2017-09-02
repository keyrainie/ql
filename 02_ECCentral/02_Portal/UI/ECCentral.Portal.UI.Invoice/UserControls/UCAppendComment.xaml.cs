using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ECCentral.Portal.UI.Invoice.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCAppendComment : PopWindow
    {
        public UCAppendComment()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.tbFinAppendNote.Text == "")
            {
                tbFinAppendNote.Validation(ResUCAppendComment.Message_RequiredNote);
                tbFinAppendNote.Focus();
                return;
            }
            if (this.tbFinAppendNote.Text.Length > 500)
            {
                tbFinAppendNote.Validation(ResUCAppendComment.Message_NoteLengthNotMoreThan500);
                tbFinAppendNote.Focus();
                return;
            }
            tbFinAppendNote.ClearValidationError();
            CloseDialog(this.tbFinAppendNote.Text, DialogResultType.OK);
        }
    }
}