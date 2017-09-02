using System;
using System.Windows;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class PMMaintain : PageBase
    {
        PMVM model;
        private PMFacade facade;
        private string pmSysNo;

        public PMMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            facade = new PMFacade(this);
            btnSave.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.IM_PM_PMMaintain);
            //model = new PMVM();
            //this.DataContext = model;

            pmSysNo = this.Request.Param;

            if (!string.IsNullOrEmpty(pmSysNo))
            {
                facade.GetPMBySysNo(int.Parse(pmSysNo), (obj, args) =>
                {
                    PMVM vm = new PMVM();

                    vm.PMID = args.Result.UserInfo.UserID;
                    vm.PMUserName = args.Result.UserInfo.UserName;
                    //vm.Status = Convert.ToInt32(args.Result.Status).ToString();                    
                    this.DataContext = vm;
                    if (args.Result.Status == PMStatus.Active)
                    {
                        cbPMStatus.SelectedIndex = 0;
                    }
                    else
                    {
                        cbPMStatus.SelectedIndex = 1;
                    }

                });
            }
            else
            {
                this.DataContext = new PMVM();
                cbPMStatus.SelectedIndex = 0;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            PMVM vm = this.DataContext as PMVM;
            vm.SysNo = Convert.ToInt32(pmSysNo);
            if (Convert.ToInt32(vm.SysNo) > 0)
            {
                facade.UpdatePM(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    vm.PMUserName = args.Result.UserInfo.UserName;
                    Window.Alert(ResPMMaintain.Info_SaveSuccessfully);
                });
            }
            else
            {
                facade.CreatePM(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    vm.PMUserName = args.Result.UserInfo.UserName;
                    //vm.SysNo = args.Result.SysNo;
                    //vm.BasicInfo.RegisterTime = args.Result.BasicInfo.RegisterTime;
                    //vm.BasicInfo.CustomerSysNo = args.Result.SysNo;
                    Window.Alert(ResPMMaintain.Info_SaveSuccessfully);
                });
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Window.Close();
        }
    }

}
