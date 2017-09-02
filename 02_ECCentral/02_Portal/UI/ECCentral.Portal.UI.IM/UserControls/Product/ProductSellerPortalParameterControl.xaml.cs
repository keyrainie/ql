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
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductSellerPortalParameterControl : UserControl
    {
        public ProductSellerPortalParameterControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void SetControlBackground(Color color)
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = color;

            foreach (object item in this.controlDetail.Children)
            {
                if (item.GetType() != typeof(TextBlock))
                {
                    ((Control)item).Background = brush;
                }

            }
        }

        private void hyperlinkView_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as SellerProductRequestVM;

            if (vm == null)
            {
                return;
            }

            HtmlViewHelper.ViewHtmlInBrowser("IM", "<div align=\"left\" style=\"overflow:auto;height:585px;width:790px\">" + vm.ProductDescLong ?? string.Empty + "</div>", null, new Size(800, 600), false, false);

        }
    }
}
