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
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductRelatedMaintain : UserControl
    {
        public IDialog Dialog { get; set; }
        private ProductRelatedFacade facade;
        public ProductRelatedMaintain()
        {
            InitializeComponent();
            facade = new ProductRelatedFacade();
            this.Loaded += new RoutedEventHandler(ItemRelatedMaintain_Loaded);
        }

        void ItemRelatedMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new ProductRelatedVM();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }
        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            
            ProductRelatedVM item = this.DataContext as ProductRelatedVM;
            // facade.CreateItemRelated(item, (obj, args) =>
            // {
            //         if (args.FaultsHandle())
            //         {
            //             return;
            //         }

            //         CPApplication.Current.CurrentPage.Context.Window.Alert(ResPropertyMaintain.Info_SaveSuccessfully);
            //});
            facade.CreateItemRelated(item, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                CPApplication.Current.CurrentPage.Context.Window.Alert(ResPropertyMaintain.Info_SaveSuccessfully);
            });
        }
    }
}
