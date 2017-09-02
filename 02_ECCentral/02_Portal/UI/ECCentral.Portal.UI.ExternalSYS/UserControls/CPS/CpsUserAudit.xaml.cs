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
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.BizEntity.ExternalSYS;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.ExternalSYS.UserControls
{
    public partial class CpsUserAudit : UserControl
    {
        public IDialog Dialog { get; set; }
        public int SysNo { get; set; }
           private CpsUserFacade facade;
     public CpsUserAudit()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => 
            {
                facade = new CpsUserFacade();
            };
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            CpsUserVM vm = new CpsUserVM() { SysNo = SysNo, Status = AuditStatus.AuditNoClearance, AuditNoClearanceInfo = txtInfo.Text };
            facade.AuditUser(vm, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert("审核成功!");
                CloseDialog(DialogResultType.OK);
            });
        }

   

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
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
