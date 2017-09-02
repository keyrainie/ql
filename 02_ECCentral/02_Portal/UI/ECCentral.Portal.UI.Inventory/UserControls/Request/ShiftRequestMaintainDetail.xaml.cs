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

namespace ECCentral.Portal.UI.Inventory.UserControls.Request
{
    public partial class ShiftRequestMaintainDetail : UserControl
    {

        public IDialog Dialog { get; set; }
        //AdjustRequestVM model;

        public ShiftRequestMaintainDetail()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //model = new AdjustRequestVM();
            //this.DataContext = model;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
