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
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;


namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class BrowrseVideo : UserControl
    {
        
        public IDialog Dialog { get; set; }
        public string URL { get; set; }
        public BrowrseVideo()
        {
            InitializeComponent();
            this.Loaded += BrowrseVideo_Loaded;
        }

        void BrowrseVideo_Loaded(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(URL))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("播放地址不能为空");
                CloseDialog(DialogResultType.Cancel);
                return;
            }
            //media.Source = new Uri(URL, UriKind.Absolute);
        }

        private void StopMedia(object sender, RoutedEventArgs e)
        {
            media.Pause();
        }

        private void PauseMedia(object sender, RoutedEventArgs e)
        {
            media.Pause();
        }

        private void PlayMedia(object sender, RoutedEventArgs e)
        {
            media.Play();
        }

        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }
    }
}
