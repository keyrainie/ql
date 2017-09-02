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

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class UCViewAccountLog : UserControl
    {
        public IDialog Dialog { get; set; }

        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public IPage CurrentPage
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        public UCViewAccountLog()
        {
            InitializeComponent();
        }

        public UCViewAccountLog(dynamic data)
        {
            InitializeComponent();
            this.DataContext = data;
        }

        //关闭
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
                Dialog.Close();
        }
    }
}
