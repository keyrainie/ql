
using System.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.Portal.UI.Customer.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic.Components.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Customer.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class ShipTypeSMSMaintain : UserControl
    {
        public ShipTypeSMSMaintainVM viewModel;
        public IDialog Dialog
        {
            get;
            set;
        }

        public ShipTypeSMSMaintain()
        {
            viewModel = new ShipTypeSMSMaintainVM();
            this.DataContext = viewModel;
            InitializeComponent();
            InitVM();
        }


        private void InitVM()
        {
            CodeNamePairHelper.GetList("Customer", "SMSType", CodeNamePairAppendItemType.Select, (s, arg) =>
            {
                if (arg.FaultsHandle())
                    return;
                foreach (var item in arg.Result)
                {
                    viewModel.SMSTypeList.Add(item);
                }
            });
            new CommonDataFacade(CPApplication.Current.CurrentPage).GetShippingTypeList(true, (s, arg) =>
            {
                if (arg.FaultsHandle())
                    return;
                foreach (var item in arg.Result)
                {
                    viewModel.ShippingTypeList.Add(item);
                }
            });
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.LayoutRoot);
            if (!viewModel.HasValidationErrors)
            {
                ShipTypeSMS request = viewModel.EntityVM.ConvertVM<ShipTypeSMSVM, ShipTypeSMS>();
                request.SMSContent = new BizEntity.LanguageContent() { Content = viewModel.EntityVM.SMSContent };
                request.WebChannel = new BizEntity.Common.WebChannel() { ChannelID = viewModel.EntityVM.ChannelID };
                if (request.SysNo.HasValue)
                    new ShipTypeSMSQueryFacade(CPApplication.Current.CurrentPage).Update(request, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                            return;
                        Dialog.ResultArgs.Data = viewModel;
                        Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                        Dialog.Close();
                    });
                else
                    new ShipTypeSMSQueryFacade(CPApplication.Current.CurrentPage).Create(request, (obj, args) =>
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

        private void SelectTemplate_Click(object sender, RoutedEventArgs e)
        {
            SMSTemplateQuery dialogWindow = new SMSTemplateQuery();
            dialogWindow.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResShipTypeSMSMaintain.windowTitle_SelectTemplate, dialogWindow, (a, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                    viewModel.EntityVM.SMSContent = args.Data.ToString();
            }, new Size(600, 500));
        }
    }
}
