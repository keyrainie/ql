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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Resources;

namespace ECCentral.Portal.UI.RMA.UserControls
{
    public partial class UCNewRMARefund : UserControl
    {
        private bool? saved;

        public IDialog Dialog { get; set; }
        public List<RegisterVM> Registers { get; set; }


        public UCNewRMARefund()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            btnCreate.Visibility = Visibility.Collapsed;
            int? soSysNo = this.Registers[0].BasicInfo.SOSysNo;
            int? warehouseSysNo = this.Registers[0].BasicInfo.WarehouseSysNo;

            string msg = string.Empty;
            foreach (var item in this.Registers)
            {
                if (soSysNo == null || soSysNo != item.BasicInfo.SOSysNo)
                {
                    msg = ResCreateRefund.Warning_RegisterMustSameSO;
                    break;
                }
                if (warehouseSysNo == null || warehouseSysNo != item.BasicInfo.WarehouseSysNo)
                {
                    msg = ResCreateRefund.Warning_RegisterMustSameShippedWarehouse;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(msg))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(msg, MessageType.Warning);
                return;
            }

            RefundVM vm = new RefundVM();
            vm.SOSysNo = soSysNo;
            vm.CheckIncomeStatus = !chkNotCheckIncomeStatus.IsChecked.Value;
            this.Registers.ForEach(p =>
            {               
                vm.RefundItems.Add(new RefundItemVM { RegisterSysNo = p.BasicInfo.SysNo });  
            });
            
            new RefundFacade(CPApplication.Current.CurrentPage).Create(vm, (obj, args) =>
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResCreateRefund.Info_OperateSuccessfully);
                saved = true;
                btnCancel_Click(null,null);

            });                    
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs = new ResultEventArgs { Data = saved };
            this.Dialog.Close();
        }
    }
}
