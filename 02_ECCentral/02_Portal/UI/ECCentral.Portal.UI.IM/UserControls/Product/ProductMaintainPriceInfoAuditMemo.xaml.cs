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
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainPriceInfoAuditMemo : UserControl
    {
        public ProductMaintainPriceInfoAuditMemoVM VM
        {
            get { return DataContext as ProductMaintainPriceInfoAuditMemoVM; }
        }

        public ProductMaintainPriceInfoAuditMemo()
        {
            InitializeComponent();
        }
    }
}
