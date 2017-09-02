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
using ECCentral.Portal.UI.Customer.Models;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class SMSTemplateEdit : UserControl
    {
        public ShipTypeSMSTemplateVM viewModel;
        public IDialog Dialog { get; set; }
        public SMSTemplateEdit()
        {
            viewModel = new ShipTypeSMSTemplateVM();
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.LayoutRoot.DataContext = viewModel;
        }
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.LayoutRoot);
            if (!viewModel.HasValidationErrors)
            {
                SMSTemplate request = viewModel.ConvertVM<ShipTypeSMSTemplateVM, SMSTemplate>();
                if (request.SysNo.HasValue)
                    new ShipTypeSMSTemplateFacade(CPApplication.Current.CurrentPage).Update(request, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                            return;
                        Dialog.ResultArgs.Data = viewModel;
                        Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                        Dialog.Close();
                    });
                else
                    new ShipTypeSMSTemplateFacade(CPApplication.Current.CurrentPage).Create(request, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                            return;
                        Dialog.ResultArgs.Data = viewModel;
                        Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                        Dialog.Close();
                    });
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            Dialog.Close();
        }


    }
}
