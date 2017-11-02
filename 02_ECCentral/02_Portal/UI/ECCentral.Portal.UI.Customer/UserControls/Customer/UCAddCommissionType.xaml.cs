using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.UI.Customer.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
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

namespace ECCentral.Portal.UI.Customer.UserControls.Customer
{
    public partial class UCAddCommissionType : UserControl
    {
        public IDialog Dialog { get; set; }
        public IPage Page { get; set; }
        public int? _sysNo;

        private CommissionTypeVM VM;
        private CommissionTypeQueryFacade facade;

        public UCAddCommissionType(int? sysNo)
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
            facade = new CommissionTypeQueryFacade(CPApplication.Current.CurrentPage);
            if (_sysNo.HasValue)
            {
                facade.LoadCommissionType(_sysNo, (obj, args) =>
                {
                    VM = args.Result;
                    VM.IsEdit = true;
                    LayoutRoot.DataContext = VM;
                    if (VM.CommissionStatus == Convert.ToBoolean(SYNStatus.Yes))
                        VM.IsNetPay = true;
                });
            }
            else
            {
                VM = new CommissionTypeVM() { IsEdit = false, CommissionStatus = Convert.ToBoolean(SYNStatus.Yes) };
                LayoutRoot.DataContext = VM;
                VM.CommissionStatus = false;
                comCommissionStatus.SelectedIndex = 0;
            }
            comCommissionStatus.SelectionChanged += new SelectionChangedEventHandler(comCommissionStatus_SelectionChanged);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                if (!_sysNo.HasValue)
                {
                    facade.CreateCommissionType(VM, (s, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        Message(ResCommissionTypeQuery.Info_SaveSuccessfully);
                        CloseDialog(new ResultEventArgs() { DialogResult = DialogResultType.OK });
                    });
                }
                else
                {
                    facade.UpdateCommissionType(VM, (s, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        Message(ResCommissionTypeQuery.Info_SaveSuccessfully);
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

        private void comCommissionStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VM.CommissionStatus == Convert.ToBoolean(SYNStatus.Yes))
            {
                VM.CommissionStatus = true;
               // comCommissionStatus.SelectedIndex = 0;
            }
            else
            {
                VM.CommissionStatus = false;
               // comCommissionStatus.SelectedIndex = 1;
            }
        }
    }
}
