using ECCentral.Portal.UI.PO.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class UCVendorStoreInfoList : UserControl
    {       
        private VendorInfoVM VM
        {
            get
            {
                return this.DataContext as VendorInfoVM;
            }
        }

        public IPage Page
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        public UCVendorStoreInfoList()
        {
            InitializeComponent();            
        }        

        private void btnNewVendorStore_Click(object sender, RoutedEventArgs e)
        {
            UCVendorStoreInfoMaintain uc = new UCVendorStoreInfoMaintain();
            uc.VM = new Models.VendorStoreInfoVM { VendorSysNo = this.VM.SysNo };
            IDialog dialog = this.Page.Context.Window.ShowDialog("添加门店信息", uc, (s, a) =>
            {
                if (a.DialogResult == DialogResultType.OK)
                {
                    this.VM.VendorStoreInfoList.Add(a.Data as VendorStoreInfoVM);
                }
            });
            uc.Dialog = dialog;
        }

        private void hpl_AgentInfoEdit_Click(object sender, RoutedEventArgs e)
        {
            UCVendorStoreInfoMaintain uc = new UCVendorStoreInfoMaintain();
            var btn = sender as HyperlinkButton;
            var vm = btn.DataContext as Models.VendorStoreInfoVM;
            vm.VendorSysNo = this.VM.SysNo;
            uc.VM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone(vm);

            IDialog dialog = this.Page.Context.Window.ShowDialog("修改门店信息", uc, (s, a) =>
            {
                if (a.DialogResult == DialogResultType.OK)
                {
                    var index = this.VM.VendorStoreInfoList.IndexOf(vm);
                    this.VM.VendorStoreInfoList.Remove(vm);
                    this.VM.VendorStoreInfoList.Insert(index, a.Data as VendorStoreInfoVM);
                }
            });
            uc.Dialog = dialog;
        }        
    }
}
