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
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.RMA.UserControls
{
    public partial class UCShipFeeDetail : UserControl
    {
        public IDialog Dialog { get; set; }

        public UCShipFeeDetail()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.Close();
        }
    }
}
