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
using ECCentral.Portal.UI.Customer.Resources;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class CustomerPointsAddRequestItemsView : UserControl
    {
        public IDialog Dialog { get; set; }
        public List<CustomerPointsAddItemVM> ItemList;
        public int ItemSOSysNo;
        public string SysAccount;
        public CustomerPointsAddRequestItemsView()
        {
            InitializeComponent();
        }

        public CustomerPointsAddRequestItemsView(string SOSysNo, string sysAccount)
            : this()
        {
            this.ItemSOSysNo = Convert.ToInt32(SOSysNo);
            this.SysAccount = sysAccount;
            ItemList = new List<CustomerPointsAddItemVM>();
            LoadSOItemsList(SOSysNo);
        }

        private void LoadSOItemsList(string SOSysNo)
        {
            this.SOItemsResultGrid.Bind();
        }

        private void DataGrid_SOItemsResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            //TODO:调用SO Service接口，加载符合要求的SOItem信息 ：
            List<CustomerPointsAddItemVM> items = new List<CustomerPointsAddItemVM>();
            new CustomerPointsAddQueryFacade(CPApplication.Current.CurrentPage).QuerySO(ItemSOSysNo,this.SysAccount, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                if (args.Result != null)
                {
                    foreach (var item in args.Result.Items)
                    {
                        items.Add(new CustomerPointsAddItemVM()
                        {
                            SysNo = item.SysNo,
                            ProductSysNo = item.ProductSysNo.Value,
                            SOSysNo = ItemSOSysNo,
                            ProductID = item.ProductID,
                            BriefName = item.ProductName,
                            Quantity = item.Quantity.Value,
                            CurrentPrice = item.Price,
                            Point =string.Empty
                        });
                    }
                    this.SOItemsResultGrid.ItemsSource = items;
                }
                else
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerPointsAddRequest.msg_SoNotFound);
                }
            });

        }
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs.Data = this.SOItemsResultGrid.ItemsSource;
            this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
            this.Dialog.Close(true);

        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.Close(true);
        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            if (SOItemsResultGrid.ItemsSource != null)
            {
                if (((CheckBox)sender).IsChecked.Value)//全选
                {
                    foreach (object ovj in SOItemsResultGrid.ItemsSource)
                    {
                        CheckBox cb1 = SOItemsResultGrid.Columns[0].GetCellContent(ovj).FindName("itemChecked") as CheckBox; //cb为

                        cb1.IsChecked = true;
                    }
                }
                else//取消
                {
                    foreach (object obj in SOItemsResultGrid.ItemsSource)
                    {
                        CheckBox cb2 = SOItemsResultGrid.Columns[0].GetCellContent(obj).FindName("itemChecked") as CheckBox;

                        cb2.IsChecked = false;
                    }
                }
            }
        }

        private void TextBox_ItemsAddPoints_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox targetTextBoxInput = sender as TextBox;
            if (null != targetTextBoxInput)
            {
                bool isInt = false;
                int isInteger = 0;
                if (int.TryParse(((TextBox)sender).Text.Trim(), out isInteger))
                {
                    if (isInteger > 0)
                    {
                        isInt = true;
                    }
                }
                if (!isInt)
                {
                    ((TextBox)sender).Text = string.Empty;
                }
            }
        }
    }
}
