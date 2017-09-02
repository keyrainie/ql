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
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Resources;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Facades;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainPriceInfoVolumePrice : UserControl
    {
        public ProductMaintainPriceInfoVolumePriceVM VM
        {
            get { return DataContext as ProductMaintainPriceInfoVolumePriceVM; }   
        }

        public ProductMaintainPriceInfoVolumePrice()
        {
            InitializeComponent();
        }
    }
}
