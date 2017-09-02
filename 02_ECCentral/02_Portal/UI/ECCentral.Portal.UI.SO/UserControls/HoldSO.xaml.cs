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
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.UI.SO.Facades;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.SO.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class HoldSO : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }
        public IPage Page
        {
            get;
            set;
        }
        SOFacade SOFacade;
        private SOVM _currentSOVM;
        public SOVM CurrentSOVM
        {
            get { return _currentSOVM; }
            set
            {
                _currentSOVM = value;
                this.DataContext = _currentSOVM.BaseInfoVM;
                if (_currentSOVM != null)
                {
                    if (_currentSOVM.BaseInfoVM.HoldStatus.Value == BizEntity.SO.SOHoldStatus.BackHold || _currentSOVM.BaseInfoVM.HoldStatus.Value == BizEntity.SO.SOHoldStatus.WebHold)
                    {
                        btnHold.Visibility = System.Windows.Visibility.Collapsed;
                        btnUnHold.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        btnHold.Visibility = System.Windows.Visibility.Visible;
                        btnUnHold.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
            }
        }
        private IWindow Window
        {
            get
            {
                return Page == null ? Page.Context.Window : CPApplication.Current.CurrentPage.Context.Window;
            }
        }
        public HoldSO()
        {
            InitializeComponent();
            //订单锁定:原因:{0} 操作人:{1} 操作时间:{2}
            //订单解锁:原因:{0} 操作人:{1} 操作时间:{2}
            Loaded += new RoutedEventHandler(HoldSO_Loaded);
        }

        void HoldSO_Loaded(object sender, RoutedEventArgs e)
        {
            SOFacade = new SOFacade(Page);
        }

        private void btnHold_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string reason = null;
            switch (btn.CommandParameter.ToString())
            {
                case "Hold":
                    reason = string.Format(ResSO.Content_SO_Hold, txtHoldReason.Text, CPApplication.Current.LoginUser.LoginName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    break;
                case "UnHold":
                    reason = string.Format(ResSO.Content_SO_UnHold, txtHoldReason.Text, CPApplication.Current.LoginUser.LoginName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    break;
            }

            if (CurrentSOVM.BaseInfoVM.HoldStatus.Value == BizEntity.SO.SOHoldStatus.BackHold || CurrentSOVM.BaseInfoVM.HoldStatus.Value == BizEntity.SO.SOHoldStatus.WebHold)
            {
                SOFacade.UnholdSO(CurrentSOVM.SysNo.Value, reason, (vm) =>
                {
                    CurrentSOVM = vm;
                    Window.Alert(ResSO.Info_SO_Processer_ChangePrice_UnHoldSO, MessageType.Information);
                    Dialog.Close();
                });
            }
            else
            {
                SOFacade.HoldSO(CurrentSOVM.SysNo.Value, reason, (vm) =>
                {
                    CurrentSOVM = vm;
                    Window.Alert(ResSO.Info_SO_Processer_ChangePrice_HoldSO, MessageType.Information);
                    Dialog.Close();
                });
            }
        }
    }
}
