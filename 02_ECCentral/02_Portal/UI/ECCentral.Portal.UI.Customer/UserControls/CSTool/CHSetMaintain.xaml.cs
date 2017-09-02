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
using ECCentral.Portal.UI.Customer.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Customer.Resources;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class CHSetMaintain : UserControl
    {
        public CHMaintainVM viewModel;
        private FPCheckFacade fpCheckFacade;
        public IDialog Dialog
        {
            get;
            set;
        }

        public CHSetMaintain()
        {
            viewModel = new CHMaintainVM();
            this.DataContext = viewModel;
            InitializeComponent();
        }
        public CHSetMaintain(bool isCateogryDefault)
        {
            viewModel = new CHMaintainVM();
            this.DataContext = viewModel;
            InitializeComponent();
            this.grpCategory.IsChecked = isCateogryDefault;
            this.grpProduct.IsChecked = !isCateogryDefault;
        }
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ValidationErrors.Clear();
            ValidationManager.Validate(LayoutRoot);
            if (viewModel.HasValidationErrors)
                return;
            fpCheckFacade = new FPCheckFacade(CPApplication.Current.CurrentPage);
            fpCheckFacade.CreateCH(viewModel, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert("添加成功！");
                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                Dialog.Close();
            });
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            Dialog.Close();
        }

        private void grpCategory_Checked(object sender, RoutedEventArgs e)
        {
            viewModel.ProductID = "";
            viewModel.ProductSysNo = "";
            viewModel.ValidationErrors.Clear();
        }

        private void grpProduct_Checked(object sender, RoutedEventArgs e)
        {
            viewModel.CategorySysNo = null;
            viewModel.ValidationErrors.Clear();
        }
    }
}
