using System;
using System.Linq;
using System.Windows.Controls;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Interface;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using ECCentral.Portal.UI.IM.Views;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Windows;
using ECCentral.Portal.Basic.Components.UserControls.Language;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic;


namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainProperty : UserControl, ISave, IBatchSave
    {
        public ProductMaintainPropertyVM VM
        {
            get { return DataContext as ProductMaintainPropertyVM; }
        }

        public ProductMaintainProperty()
        {
            InitializeComponent();
        }

        public IWindow MyWindow { get; set; }

        public void Save()
        {
            var mainPage = CPApplication.Current.CurrentPage;

            if (mainPage != null && mainPage is ProductMaintain)

                new ProductFacade().UpdateProductPropertyInfo(
                    (mainPage as ProductMaintain).VM,
                    (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        mainPage.Context.Window.MessageBox.Show("商品属性信息更新成功", MessageBoxType.Success);

                        var page = mainPage as ProductMaintain;
                        if (page != null)
                        {
                            page.VM.ProductMaintainCommonInfo.GroupProductList.First(
                                    p => p.PrductIDLinkColor == "Red").ProductGroupProperties = page.VM.ProductMaintainProperty.ProductPropertyValueList.Where(
                                    property => property.PropertyType == PropertyType.Grouping).Select(
                                        property =>
                                        String.IsNullOrEmpty(property.PersonalizedValue)
                                            ? property.ProductPropertyValue.ValueDescription
                                            : property.PersonalizedValue).Join("  ");
                            page.VM.ProductMaintainBasicInfo.ProductMaintainBasicInfoDescriptionInfo.Performance =
                                args.Result;
                        }
                    });
        }

        private void hyperlinkPropertyValueMultiLanguage_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemMultiLanguage))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("你无此操作权限");
                return;
            }

            dynamic selectItem = this.dgProductPropertyInfo.SelectedItem as dynamic;

            if (selectItem.SysNo == null)
            {
                MessageBox.Show("还没有输入该属性的值，请输入值保存之后再刷新页面！");
                return;
            }

            UCMultiLanguageMaintain item = new UCMultiLanguageMaintain(selectItem.SysNo, "ProductHasProperty");

            item.Dialog = MyWindow.ShowDialog("设置属性多语言", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    //this.dgProductPropertyInfo.Bind();
                }
            }, new Size(750, 600));

        }

        private void BtnCopyPropertyClick(object sender, RoutedEventArgs e)
        {
            var mainPage = CPApplication.Current.CurrentPage;

            if (mainPage != null && mainPage is ProductMaintain)
            {


                var mainUC = new ProductMaintainPropertyCopyProperty
                {
                    DataContext = new ProductCopyPropertyVM
                                      {
                                          SourceProductID = (mainPage as ProductMaintain).VM.ProductMaintainCommonInfo.ProductID,
                                          SourceProductSysNo = (mainPage as ProductMaintain).VM.ProductSysNo
                                      }
                };
                var window = CPApplication.Current.CurrentPage.Context.Window;

                mainUC.Dialog = window.ShowDialog(IM.Resources.ResProductMaintain.Dialog_CopyProperty, mainUC, (s, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {

                    }
                });
            }
        }

        public void BatchSave()
        {
            var mainPage = CPApplication.Current.CurrentPage;

            if (mainPage != null && mainPage is ProductMaintain)

                new ProductFacade().BatchUpdateProductPropertyInfo(
                    (mainPage as ProductMaintain).VM,
                    (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        mainPage.Context.Window.MessageBox.Show("商品属性信息批量更新成功", MessageBoxType.Success);
                    });
        }
    }
}
