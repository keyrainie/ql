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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.Common.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Common.Facades;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.Common.Resources;

namespace ECCentral.Portal.UI.Common.UserControls
{
    public partial class UCAddPayType : UserControl
    {
        public IDialog Dialog { get; set; }
        public IPage Page { get; set; }
        public int? _sysNo;

        private PayTypeVM VM;
        private PayTypeFacade facade;

        public UCAddPayType(int? sysNo)
        {
            InitializeComponent();
            if (sysNo.HasValue && sysNo.Value > 0)
            {
                _sysNo = sysNo;
            }
            Loaded += new RoutedEventHandler(UCAddPayType_Loaded);
        }

        void UCAddPayType_Loaded(object sender, RoutedEventArgs e)
        {
            facade = new PayTypeFacade(CPApplication.Current.CurrentPage);
            if (_sysNo.HasValue)
            {
                facade.LoadPayType(_sysNo, (obj, args) =>
                {
                    VM = args.Result;
                    VM.IsEdit = true;
                    LayoutRoot.DataContext = VM;
                    if (VM.IsNetEnum == SYNStatus.Yes)
                        VM.IsNetPay = true;
                    comIsNet.SelectionChanged+=new SelectionChangedEventHandler(comIsNet_SelectionChanged);
                    
                });
            }
            else
            {
                VM = new PayTypeVM() { IsEdit = false, IsNetEnum = SYNStatus.No };
                LayoutRoot.DataContext = VM;
                comIsPayWhenRecv.SelectedIndex = 0;
                comIsOnlineShow.SelectedIndex = 0;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                if (!_sysNo.HasValue)
                {
                    facade.CreatePayType(VM, (s, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        Message(ResQueryPayType.Info_SaveSuccessfully);
                        CloseDialog(new ResultEventArgs() { DialogResult = DialogResultType.OK });
                    });
                }
                else
                {
                    facade.UpdatePayType(VM, (s, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        Message(ResQueryPayType.Info_SaveSuccessfully);
                        CloseDialog(new ResultEventArgs() { DialogResult = DialogResultType.OK });
                    });
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.Data = null;
                Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
                Dialog.Close();
            }
        }

        #region 辅助方法
        private void Message(string msg)
        {
            Page.Context.Window.Alert(msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
        }

        private void CloseDialog(ResultEventArgs args)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs = args;
                Dialog.Close();
            }
        }
        #endregion

        private void comIsNet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VM.IsNetEnum == SYNStatus.Yes)
            {
                VM.IsNetPay = true;
                comNetPayType.SelectedIndex = 0;
            }
            else
            {
                VM.IsNetPay = false;
                VM.NetPayType = null;
            }
        }

    }
}
