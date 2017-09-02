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
    public partial class UCAddShipPayType : UserControl
    {
        public IDialog Dialog { get; set; }
        public IPage Page { get; set; }
        public int? _sysNo;

        ShipTypePayTypeInfoVM VM;

        public UCAddShipPayType(int? sysNo)
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCAddShipPayType_Loaded);
        }

        void UCAddShipPayType_Loaded(object sender, RoutedEventArgs e)
        {
            VM = new ShipTypePayTypeInfoVM();
            LayoutRoot.DataContext = VM;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                if (!_sysNo.HasValue)
                {
                    new ShipTypePayTypeFacade(CPApplication.Current.CurrentPage).CreateShipTypePayType(VM, (s, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        Message(ResShipPayType.Info_SaveSuccessfully);
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

        private void Combox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}
