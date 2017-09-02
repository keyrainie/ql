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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductBatchChangeCategory : PageBase
    {
        public ProductBatchChangeCategory()
        {
            InitializeComponent();
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();
            string[] arr = this.tb_ProductIDs.Text.Split('\r');
            list = arr.ToList().Distinct().ToList();

            if (!this.dpCategory.ChooseCategory3SysNo.HasValue)
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("请选择商品三级类别.", MessageBoxType.Warning);
                return;
            }

            if (this.tb_ProductIDs.Text.Trim().Equals(string.Empty))
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("请输入至少一个ProductID！.", MessageBoxType.Warning);
                return;
            }

            ProductBatchChangeCategoryFacade facade = new ProductBatchChangeCategoryFacade();

            facade.BatchChangeCategory(list, this.dpCategory.ChooseCategory3SysNo.Value, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
            });
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Window.Close();
        }
    }
}
