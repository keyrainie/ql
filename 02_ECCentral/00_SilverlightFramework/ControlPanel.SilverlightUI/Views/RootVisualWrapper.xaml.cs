using System.Windows;
using System.Windows.Controls;
using ControlPanel.SilverlightUI;
using Newegg.Oversea.Silverlight.Core.Components;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Views
{
    public partial class RootVisualWrapper : UserControl
    {
        public RootVisualWrapper()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(RootVisualWrapper_Loaded);
        }

        void RootVisualWrapper_Loaded(object sender, RoutedEventArgs e)
        {
            ComponentFactory.GetComponent<ILogin>().AutoLogin((result) =>
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    if (result)
                    {
                        ((App)App.Current).InitApp();
                    }
                    else
                    {
                        this.LoginArea.Visibility = System.Windows.Visibility.Visible;
                        this.BorderLoadingLayer.Visibility = System.Windows.Visibility.Collapsed;
                    }
                });

            });

        }
    }
}
