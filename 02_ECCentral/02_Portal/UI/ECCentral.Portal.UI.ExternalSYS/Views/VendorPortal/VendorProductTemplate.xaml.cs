using System;
using System.Windows;

using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.Portal.UI.ExternalSYS.Resources;
using ECCentral.Portal.UI.ExternalSYS.UserControls.VendorPortal;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.ExternalSYS;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true)]
    public partial class VendorProductTemplate : PageBase
    {
        public VendorProductTemplate()
        {
            InitializeComponent();
        }

        void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Vendor_ProductTemplate_Download))
            {
                Window.Alert(ResVendorInfo.Msg_HasNoRight);
                return;
            }
            if (!ucCategoryPicker.ChooseCategory3SysNo.HasValue)
            {
                Window.Alert(ResVendorInfo.Msg_C3SysNo);
            }
            else
            {
                (new VendorFacade(this)).DownTemplate(ucCategoryPicker.ChooseCategory3SysNo.Value, CPApplication.Current.CompanyCode);
            }
        }
    }
}
