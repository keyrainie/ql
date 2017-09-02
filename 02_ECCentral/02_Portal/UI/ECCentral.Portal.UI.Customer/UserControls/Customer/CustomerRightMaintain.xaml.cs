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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;


namespace ECCentral.Portal.UI.Customer.UserControls.Customer
{
    public partial class CustomerRightMaintain : UserControl
    {
        private CustomerQueryFacade facade;
        public CustomerRightMaintainView viewModel;
        public IDialog Dialog
        {
            get;
            set;
        }
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            facade = new CustomerQueryFacade(CPApplication.Current.CurrentPage);
            this.DataContext = viewModel;
            CustomerRights.Bind();
        }
        public CustomerRightMaintain()
        {
            viewModel = new CustomerRightMaintainView();
            InitializeComponent();
        }

        private void btnUpdateCustomerRight_Click(object sender, RoutedEventArgs e)
        {
            facade.UpdateCustomerRights(viewModel, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                CurrentWindow.Alert(ResCustomerRightMaintain.MsgTitle_UpdateCustomerRightOk, ResCustomerRightMaintain.Msg_UpdateCustomerRightOk, MessageType.Information, (obj2, args2) =>
                {
                    CustomerRights.Bind();
                });
            });
        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked.Value)//全选
            {
                viewModel.RightList.ForEach(a => a.ItemChecked = true);
            }
            else//取消
            {
                viewModel.RightList.ForEach(a => a.ItemChecked = false);
            }
        }

        private void CustomerRights_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.LoadCustomerRight(viewModel.CustomerSysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<CustomerRight> userRight = args.Result;
                List<CodeNamePair> customerRightType = new List<CodeNamePair>();
                CodeNamePairHelper.GetList("Customer", "CustomerRightType", (obj2, args2) =>
                {
                    customerRightType = args2.Result;  //在顾客端拿到的类型为: List<CodeNamePair> 
                    viewModel.RightList = new List<CustomerRightVM>();
                    foreach (CodeNamePair pair in customerRightType)
                    {
                        CustomerRightVM rightItem = new CustomerRightVM();
                        rightItem.RightDescription = pair.Name.Trim();
                        rightItem.Right = int.Parse(pair.Code);
                        rightItem.ItemChecked = false;
                        foreach (CustomerRight right in userRight)
                        {
                            if (right.Right.ToString() == pair.Code)
                            {
                                rightItem.ItemChecked = true;
                            }
                        }
                        viewModel.RightList.Add(rightItem);
                    }
                    CustomerRights.ItemsSource = viewModel.RightList;
                });
            });
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Dialog.Close();
        }


    }
}
