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
using ECCentral.Portal.UI.MKT.Models.Floor;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls.Floor
{
    public partial class UCLinkForSection : UserControl
    {
        public IDialog Dialog { get; set; }
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }
        FloorSectionLinkVM vm = new FloorSectionLinkVM();
        public UCLinkForSection()
        {
            InitializeComponent();
            DataContext = vm;
        }
        public UCLinkForSection(FloorSectionLinkVM inVM)
        {
            InitializeComponent();
            vm = inVM;
            DataContext = vm;
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.Data = vm;
                Dialog.Close(true);
            }
        }
    }
}
