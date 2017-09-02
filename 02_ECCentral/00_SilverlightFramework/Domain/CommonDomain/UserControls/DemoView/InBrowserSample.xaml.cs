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

namespace Newegg.Oversea.Silverlight.CommonDomain.UserControls
{
    public partial class InBrowserSample : UserControl
    {

        public InBrowserSample()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(InBrowserSample_Loaded);
        }

        void InBrowserSample_Loaded(object sender, RoutedEventArgs e)
        {
            txtMessageID.Text = "0791bbd3-d42d-4c3d-ae16-901e5e7fe894";
        }

        private void btnCall_Click(object sender, RoutedEventArgs e)
        {
            string url = string.Format("{0}{1}", CPApplication.Current.PortalBaseAddress, "Pages/BatchMail.aspx?MessageID={0}&LanguageCode=en-US");
            url = string.Format(url, txtMessageID.Text);
            wbMail.Navigate(new Uri(url, UriKind.Absolute));
            wbMail.ScriptNotify += (s, ee) =>
                {
                    bool result = bool.Parse(ee.Value);
                    if (result)
                    {
                        txtResult.Text = "Send successfully";
                    }
                    else
                    {
                        txtResult.Text = "Send failed";
                    }
                };
            
        }
    }
}
