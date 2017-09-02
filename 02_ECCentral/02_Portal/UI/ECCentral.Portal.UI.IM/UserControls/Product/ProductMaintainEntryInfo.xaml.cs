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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.IM.Models.Product;
using ECCentral.Portal.Basic.Interface;
using ECCentral.Portal.UI.IM.UserControls.Product;
using ECCentral.Portal.UI.IM.Facades.Product;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.IM.Product;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainEntryInfo : UserControl, ISave
    {
        public IDialog Dialog { get; set; }
        ProductEntryFacade facades;
        ProductEntryInfoVM vm;
        public IPage CurrentPage
        {
            get { return CPApplication.Current.CurrentPage; }
        }
        public ProductMaintainEntryInfo()
        {


            this.Loaded += new RoutedEventHandler(ProductMaintainEntryInfo_Loaded);
            InitializeComponent();
        }
        void ProductMaintainEntryInfo_Loaded(object sender, RoutedEventArgs e)
        {
            vm = new ProductEntryInfoVM();
            facades = new ProductEntryFacade(CPApplication.Current.CurrentPage);
            string tempSysNo = string.Empty;

            if (CurrentPage.Context.Request.QueryString != null)
            {
                tempSysNo = CurrentPage.Context.Request.QueryString["ProductSysNo"];
            }
            if (!string.IsNullOrEmpty(CurrentPage.Context.Request.Param))
            {
                tempSysNo = CurrentPage.Context.Request.Param;
            }
            int productSysNo;
            if (Int32.TryParse(tempSysNo, out productSysNo))
            {
                facades.LoadProductEntryInfo(productSysNo, (obj, args) =>
                {
                    vm = EntityConverter<ProductEntryInfo, ProductEntryInfoVM>.Convert(
                                                     args.Result,
                                                     (s, t) =>
                                                     { });
                    this.DataContext = vm;
                });
            }
            else
            {
                this.DataContext = vm;
            }
        }

        public void Save()
        {
            new ProductEntryFacade(this.CurrentPage).Update(this.vm, (args) =>
            {

                if (args)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("更新成功！");
                }
                else
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("更新失败！");
                }
            });
        }

    }
}
