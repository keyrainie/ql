using System;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.Invoice.Resources;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCReferenceIDSetter : PopWindow
    {
        public UCReferenceIDSetter()
        {
            InitializeComponent();
            validList = new List<ValidationEntity>();
            validList.Add(new ValidationEntity(ValidationEnum.MaxLengthLimit, 20, ResCommon.Message_MaxLength20));
        }

        List<ValidationEntity> validList;

        private void btnOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var flag = ValidationHelper.Validation(tbReferenceID, validList);
            if (!flag)
                return;

            this.CloseDialog(tbReferenceID.Text, DialogResultType.OK);
        }
    }
}