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

namespace ECCentral.Portal.Basic.Components.UserControls.StockPicker
{
    public class StockSelectedEventArgs : EventArgs
    {
        public StockSelectedEventArgs(StockVM selectedStock)
        {
            this.SelectedStock = selectedStock;
        }

        public StockVM SelectedStock { get; private set; }
    }
}
