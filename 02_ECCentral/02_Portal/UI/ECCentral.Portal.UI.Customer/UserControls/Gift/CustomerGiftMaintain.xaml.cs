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
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.UI.Customer.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Customer.Resources;
using Newegg.Oversea.Silverlight.Controls.Containers;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Components.UserControls.CustomerPicker;

namespace ECCentral.Portal.UI.Customer.UserControls.Gift
{
    public partial class CustomerGiftMaintain : UserControl
    {
        private CustomerGiftFacade _facade;
        private CustomerGiftCreateVM viewModel;
        public IDialog Dialog;

        public CustomerGiftMaintain()
        {
            InitializeComponent();
            _facade = new CustomerGiftFacade(CPApplication.Current.CurrentPage);
            InitNewViewModel();
        }

        private void InitNewViewModel()
        {
            this.LayoutRoot.DataContext = viewModel = new CustomerGiftCreateVM();
            this.ButtonSave.IsEnabled = true;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.LayoutRoot))
                return;
            if (!string.IsNullOrEmpty(textboxCustomerIDList.Text))
            {
                string[] list =viewModel.CusIDListString.Trim().Replace("\r", ",").Split(new char[] { ',' });
                List<string> Dislist = list.Distinct().ToList();
                viewModel.CusIDList = Dislist;
                _facade.Create(viewModel, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerGiftMaintain.Msg_AddSuccess, MessageType.Information);
                    this.ButtonSave.IsEnabled = false;
                });
            }
            else 
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerGiftMaintain.Msg_SelectUser);

            #region 原选择顾客功能
            //if (dataGrid1.ItemsSource != null)
            //{
            //    viewModel.CustomerIDList.Clear();
            //    (dataGrid1.ItemsSource as List<ECCentral.Portal.Basic.Components.UserControls.CustomerPicker.CustomerVM>).ForEach(item =>
            //    {
            //        viewModel.CustomerIDList.Add(item.SysNo.Value);
            //    });
            //    _facade.Create(viewModel, (s, args) =>
            //    {
            //        if (args.FaultsHandle())
            //            return;
            //        CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerGiftMaintain.Msg_AddSuccess,MessageType.Information);
            //        this.ButtonSave.IsEnabled = false;
            //    });
            //}
            //else
            //    CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerGiftMaintain.Msg_SelectUser);
            #endregion
        }
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Dialog.Close();
        }

        #region 原选择顾客功能
        //private void ImageCustomerPicker_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    UCCustomerSearch ucCustomerSearch = new UCCustomerSearch();
        //    ucCustomerSearch.SelectionMode = SelectionMode.Multiple;
        //    ucCustomerSearch.DialogHandle = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResCustomerPicker.Dialog_Title, ucCustomerSearch, (obj, args) =>
        //    {
        //        if (args.DialogResult == DialogResultType.OK)
        //        {
        //            var selectedCustomer = args.Data as List<ECCentral.Portal.Basic.Components.UserControls.CustomerPicker.CustomerVM>;
        //            if (selectedCustomer != null)
        //            {
        //                dataGrid1.ItemsSource = selectedCustomer;
        //            }
        //        }
        //    });
        //}
        #endregion
    }
}
