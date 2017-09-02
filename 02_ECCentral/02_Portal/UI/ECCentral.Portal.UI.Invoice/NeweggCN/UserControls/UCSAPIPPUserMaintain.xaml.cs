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
using ECCentral.Portal.UI.Invoice.UserControls;
using ECCentral.Portal.UI.Invoice.NeweggCN.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Invoice.NeweggCN.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.UserControls
{
    public partial class UCSAPIPPUserMaintain : PopWindow
    {
        private SAPIPPUserVM vm;

        public UCSAPIPPUserMaintain()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCSAPIPPUserMaintain_Loaded);
            this.vm = new SAPIPPUserVM();
        }

        public UCSAPIPPUserMaintain(SAPIPPUserVM maintainVM)
            : this()
        {
            // TODO: Complete member initialization
            this.vm = maintainVM;
        }

        private void UCSAPIPPUserMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCSAPIPPUserMaintain_Loaded);
            this.DataContext = vm;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.BaseInfo);
            if (vm.HasValidationErrors)
            {
                return;
            }
            new SAPFacade(CPApplication.Current.CurrentPage).UpdateIPPUser(vm, () =>
                CloseDialog(DialogResultType.OK));
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

    }
}
