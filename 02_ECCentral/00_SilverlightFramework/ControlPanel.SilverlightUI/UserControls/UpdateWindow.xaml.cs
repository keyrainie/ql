using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Utilities;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls
{
    public partial class UpdateWindow : ChildWindow
    {
        public static bool IsOpen;

        public XapVersionInfo Current { get; set; }
        
        public UpdateWindow()
        {
            InitializeComponent();

            this.Style = Application.Current.Resources["UpdateBoxStyle"] as Style;
            this.ButtonUpdateNow.Click += new RoutedEventHandler(ButtonUpdateNow_Click);
            this.ButtonUpdateLater.Click += new RoutedEventHandler(ButtonUpdateLater_Click);
            this.ButtonClose.Click += new RoutedEventHandler(ButtonClose_Click);
            this.KeyDown += new KeyEventHandler(UpdateBox_KeyDown);
        }

        void UpdateBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                ButtonClose_Click(null, null);
            }
        }

        void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonUpdateLater.Visibility == System.Windows.Visibility.Collapsed)
            {
                this.CloseWindow();
                UtilityHelper.RestartApplication();
            }
            else
            {
                this.CloseWindow();
            }
        }

        void ButtonUpdateLater_Click(object sender, RoutedEventArgs e)
        {
            this.CloseWindow();
        }

        void ButtonUpdateNow_Click(object sender, RoutedEventArgs e)
        {
            this.CloseWindow();
            UtilityHelper.RestartApplication();
        }


        public UpdateWindow(XapVersionInfo xapVersion)
            : this(new List<XapVersionInfo>() { xapVersion })
        {

        }

        public UpdateWindow(List<XapVersionInfo> xapVersionList)
            : this()
        {
            var e = xapVersionList.FirstOrDefault(item => item.UpdateLevel.Equals("U", StringComparison.OrdinalIgnoreCase));

            if (e != null)
            {
                ButtonUpdateLater.Visibility = System.Windows.Visibility.Collapsed;
                this.Current = e;
            }
            else
            {
                this.Current = xapVersionList.First();
            }

            this.GridContainer.DataContext = this.Current;
        }

        public void CloseWindow()
        {
            UpdateWindow.IsOpen = false;
            this.Close();
        }

        public void ShowWindow()
        {
            if (!UpdateWindow.IsOpen)
            {
                UpdateWindow.IsOpen = true;
                this.Show();
            }
        }
    }
}

