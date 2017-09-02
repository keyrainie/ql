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
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Resources;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Components.UserControls.Language;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductGroupQuery : PageBase
    {

        ProductGroupQueryVM model;
        private List<ProductGroupVM> _vmList;

        public ProductGroupQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new ProductGroupQueryVM();
            this.DataContext = model;
        }

        private void btnProductGroupNew_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(ConstValue.IM_ProductGroupMaintainCreateFormat, null, true);
        }

        private void hyperlinkMultiLanguageEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectItem = this.dgProductGroupQueryResult.SelectedItem as dynamic;

            UCMultiLanguageMaintain item = new UCMultiLanguageMaintain(selectItem.SysNo, "ProductGroup");

            item.Dialog = Window.ShowDialog("设置商品组多语言", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    //this.dgProductPropertyInfo.Bind();
                }
            }, new Size(750, 600));
        }

        private void dgProductGroupQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            ProductGroupFacade facade = new ProductGroupFacade(this);

            facade.QueryProductGroupInfo(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                _vmList = DynamicConverter<ProductGroupVM>.ConvertToVMList<List<ProductGroupVM>>(args.Result.Rows);
                this.dgProductGroupQueryResult.ItemsSource = _vmList;
                this.dgProductGroupQueryResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            dgProductGroupQueryResult.Bind();
        }

        private void hyperlinkOperationEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic group = this.dgProductGroupQueryResult.SelectedItem as dynamic;
            if (group != null)
            {
                this.Window.Navigate(string.Format(ConstValue.IM_ProductGroupMaintainUrlFormat, group.SysNo), null, true);
            }
            else
            {
                Window.Alert(ResProductGroupQuery.Msg_OnSelectProductGroup, MessageType.Error);
            }
        }
    }
}
