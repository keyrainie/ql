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

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class EmailQuery : PageBase
    {
        public EmailQuery()
        {
            InitializeComponent();
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {

        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}
