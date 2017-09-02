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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.PO.Views
{
    [View]
    public partial class PurchaseOrderPrintView : PageBase
    {
        public PurchaseOrderPrintView()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
        }

        #region [Events]

        #endregion
    }

}
