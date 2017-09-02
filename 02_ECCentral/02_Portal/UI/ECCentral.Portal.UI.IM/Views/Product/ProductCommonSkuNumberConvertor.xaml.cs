using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.IM.UserControls;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductCommonSkuNumberConvertor : PageBase
    {
        public ProductCommonSkuNumberConvertor()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProductCommonSkuNumberConvertorFacade facade = new ProductCommonSkuNumberConvertorFacade();
            List<string> list = new List<string>();
            string[] arr = this.txtProductIDs.Text.Split('\r');
            list=arr.ToList();
            if (list.Count > 0 && !string.IsNullOrEmpty(list[0]))
            {
                string txtCommonSkuNumberSource = "";
                facade.GetCommonSkuNumbersByProductIDs(list, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    dynamic d = arg.Result.Rows as dynamic;
                    foreach (var item in d)
                    {
                        txtCommonSkuNumberSource = txtCommonSkuNumberSource + item.CommonSkuNumber + "\r";
                    }
                    this.txtKuNumberIDs.Text = txtCommonSkuNumberSource;
                    Window.MessageBox.Show("查询成功", Newegg.Oversea.Silverlight.Controls.Components.MessageBoxType.Success);
                });
            }
            else
            {
                Window.MessageBox.Show("至少指定一个ProductID", Newegg.Oversea.Silverlight.Controls.Components.MessageBoxType.Error);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ProductCommonSkuNumberConvertorFacade facade = new ProductCommonSkuNumberConvertorFacade();
            List<string> list = new List<string>();
            string[] arr = this.txtKuNumberIDs.Text.Split('\r');
            list = arr.ToList();
            if (list.Count > 0 && !string.IsNullOrEmpty(list[0]))
            {
                string txtProductIDsSource = "";
                facade.GetProductIDsByCommonSkuNumbers(list, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    dynamic d = arg.Result.Rows as dynamic;
                    foreach (var item in d)
                    {
                        txtProductIDsSource = txtProductIDsSource + item.ProductID + "\r";
                    }
                    this.txtProductIDs.Text = txtProductIDsSource;
                    Window.MessageBox.Show("查询成功", Newegg.Oversea.Silverlight.Controls.Components.MessageBoxType.Success);
                });
            }
            else
            {
                Window.MessageBox.Show("至少指定一个CommonSkuNumber", Newegg.Oversea.Silverlight.Controls.Components.MessageBoxType.Error);
            }
        }



        
    }
}
