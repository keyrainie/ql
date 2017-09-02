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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.Customer.UserControls.Refund
{
    public partial class Refuse : UserControl
    {
        RequestRefuseVM viewModel;
        public IDialog Dialog;
        public Refuse()
        {
            viewModel = new RequestRefuseVM();
            this.DataContext = viewModel;
            InitializeComponent();
        }

        private void btnRefuse_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.LayoutRoot);
            if (viewModel.HasValidationErrors)
                return;
            Dialog.ResultArgs.Data = viewModel.Memo;
            Dialog.ResultArgs.DialogResult = DialogResultType.OK;
            Dialog.Close();
        }
    }
    public class RequestRefuseVM : ModelBase
    {
        private string _Memo;
        [Validate(ValidateType.Required)]
        public string Memo
        {
            get { return _Memo; }
            set { SetValue("Memo", ref _Memo, value); }
        }
    }

}
