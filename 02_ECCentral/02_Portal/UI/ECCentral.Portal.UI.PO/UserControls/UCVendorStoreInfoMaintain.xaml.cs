using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models;
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

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class UCVendorStoreInfoMaintain : UserControl
    {
        public VendorStoreInfoVM VM
        {
            get
            {
                return this.DataContext as VendorStoreInfoVM;
            }
            set
            {
                this.DataContext = value;
            }
        }

        public IPage Page
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        public IDialog Dialog { get; set; }

        public UCVendorStoreInfoMaintain()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this))
            {
                if (this.VM.OpeningHoursFrom > this.VM.OpeningHoursTo)
                {
                    Page.Context.Window.Alert("营业结束时间不能小于开始时间！", MessageType.Warning);
                    return;
                }
                this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.OK, Data = this.VM };
                this.Dialog.Close();

                //var facade = new VendorFacade(this.Page);
                //if (this.VM.SysNo != null)
                //{
                //    facade.UpdateVendorStoreInfo(this.VM, (s, a) =>
                //    {
                //        if (a.FaultsHandle())
                //        {
                //            return;
                //        }
                //        Page.Context.Window.Alert("提示", "保存成功", MessageType.Information, (se, ar) =>
                //        {
                //            this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.OK, Data = this.VM };
                //            this.Dialog.Close();
                //        });
                //    });
                //}
                //else
                //{                    
                //    facade.CreateVendorStoreInfo(this.VM, (s, a) =>
                //    {
                //        if (a.FaultsHandle())
                //        {
                //            return;
                //        }
                //        this.VM.SysNo = a.Result.SysNo;
                //        Page.Context.Window.Alert("提示", "保存成功", MessageType.Information, (se, ar) =>
                //        {
                //            this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.OK, Data = this.VM };
                //            this.Dialog.Close();
                //        });
                //    });
                //}
            }            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.Cancel };
            this.Dialog.Close();
        }
    }
}
