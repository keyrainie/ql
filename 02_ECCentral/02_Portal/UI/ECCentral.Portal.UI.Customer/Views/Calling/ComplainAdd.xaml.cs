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
using System.Windows.Navigation;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.BizEntity.SO;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Customer.Views.Calling
{
    [View]
    public partial class ComplainAdd : PageBase
    {
        public IDialog Dialog { get; set; }
        public CustomerCalingToComplainVM viewModel { get; set; }
        public ComplainAdd()
        {
            InitializeComponent();
            viewModel = new CustomerCalingToComplainVM();
            this.DataContext = viewModel;
            BindComboBoxData();
        }

        private void BindComboBoxData()
        {
            //投诉类别
            CodeNamePairHelper.GetList(ConstValue.DomainName_SO, ConstValue.Key_ComplainType, (o, p) =>
            {
                this.cmbComplainType.ItemsSource = p.Result;
                this.cmbComplainType.SelectedIndex = 0;
            });

            //投诉来源
            CodeNamePairHelper.GetList(ConstValue.DomainName_SO, ConstValue.Key_SOComplainSourceType, (o, p) =>
            {
                this.cmbComplainSourceType.ItemsSource = p.Result;
                this.cmbComplainSourceType.SelectedIndex = 2;
            });
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.LayoutRoot);
            if (viewModel.HasValidationErrors)
                return;
            if (Dialog != null)
            {
                var newComplain = viewModel.ConvertVM<CustomerCalingToComplainVM, SOComplaintCotentInfo>();
                newComplain.SysNo = viewModel.CallsEventSysNo;
                (new CustomerCallingFacade()).CallingToComplain(newComplain, (cobj, cargs) =>
                {
                    if (cargs.FaultsHandle())
                    {
                        //创建失败
                        return;
                    }
                    Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    Dialog.Close();
                });
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.Close();
            }
        }
    }
}