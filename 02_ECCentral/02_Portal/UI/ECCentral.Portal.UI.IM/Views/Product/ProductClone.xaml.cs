using System;
using System.Linq;
using System.Text;
using System.Windows;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Models.Product;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductClone : PageBase
    {
        public ProductCloneVM VM
        {
            get { return DataContext as ProductCloneVM; }
        }

        public ProductClone()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            DataContext = new ProductCloneVM();
            cmbProductTypeList.SelectedIndex = 0;
        }

        private void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(VM.ProductIDList))
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("请输入至少一个ProductID！", MessageBoxType.Warning);
                return;
            }

            var productIDList = VM.ProductIDList.Split('\r').Distinct().ToList();

            new ProductFacade().ProductClone(productIDList, VM.ProductCloneType, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }

                var result = new StringBuilder();

                arg.Result.SuccessProductList.ForEach(sp => result.AppendLine(sp.SourceProductID + "克隆成功，新商品为" + sp.TargetProductID));

                arg.Result.ErrorProductList.ForEach(ep => result.AppendLine(ep.ProductID + "克隆失败，" + ep.ErrorMsg));

                CPApplication.Current.CurrentPage.Context.Window.Alert(result.ToString(),MessageType.Information);
            });
        }
    }

}
