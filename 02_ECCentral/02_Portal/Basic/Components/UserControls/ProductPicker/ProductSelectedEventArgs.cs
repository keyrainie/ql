using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.Portal.Basic.Components.UserControls.ProductPicker
{
    public class ProductSelectedEventArgs : EventArgs
    {
        public ProductSelectedEventArgs(ProductVM selectedProduct)
        {
            this.SelectedProduct = selectedProduct;
        }

        public ProductVM SelectedProduct { get; private set; }
    }
}
