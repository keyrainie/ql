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
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class CategoryRelatedMaintain : UserControl
    {
        public IDialog Dialog { get; set; }
        private CategoryRelatedFacade facade;
        public CategoryRelatedMaintain()
        {
            InitializeComponent();
            facade = new CategoryRelatedFacade();
            this.Loaded += new RoutedEventHandler(CategoryRelatedMaintain_Loaded);
        }

        void CategoryRelatedMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new CategoryRelatedVM();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            CategoryRelatedVM model = this.DataContext as CategoryRelatedVM;
            facade.CreateCategoryRelated(model, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CloseDialog(DialogResultType.OK);
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResPropertyMaintain.Info_SaveSuccessfully);
            });
           
        }
        
       private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

       private void btnClose_Click(object sender, RoutedEventArgs e)
       {
           CloseDialog(DialogResultType.Cancel);
       }
    }
}
