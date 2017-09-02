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
using System.Windows.Navigation;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class PMGroupMaintain : PageBase
    {

        PMGroupVM model;
        private PMGroupFacade facade;
        private string pmGroupSysNo;

        public PMGroupMaintain()
        {
            InitializeComponent();
           
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            
            facade = new PMGroupFacade(this);

            pmGroupSysNo = this.Request.Param;

            if (!string.IsNullOrEmpty(pmGroupSysNo))
            {
                facade.GetPMGroupBySysNo(int.Parse(pmGroupSysNo), (obj, args) =>
                {
                    PMGroupVM vm = new PMGroupVM();

                    vm.PMGroupName = args.Result.PMGroupName.Content;
                    vm.PMUserSysNo = args.Result.UserInfo.SysNo.Value.ToString();
                    vm.Status = Convert.ToInt32(args.Result.Status).ToString();
                    this.DataContext = vm;
                    checkBoxListPM.ItemsSource = args.Result.ProductManagerInfoList;
                    if (args.Result.Status == PMGroupStatus.Active)
                    {
                        cmbPMGroupStatus.SelectedIndex = 0;
                    }
                    else
                    {
                        cmbPMGroupStatus.SelectedIndex = 1;
                    }
                    txtPMGroupName.IsReadOnly = true;
                });
            }
            else
            {
                //查询PM列表
                facade.GetPMListByPMGroupSysNo(-1, (obj, args) =>
                {
                    this.DataContext = new PMGroupVM();
                    checkBoxListPM.ItemsSource = args.Result;
                });

            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            PMGroupVM vm = this.DataContext as PMGroupVM;
            vm.SysNo = Convert.ToInt32(pmGroupSysNo);

            //获取选中的项目
            IEnumerable<ProductManagerInfo> list = (IEnumerable<ProductManagerInfo>)checkBoxListPM.ItemsSource;
            vm.PMList = list.Where(p => p.IsExistGroup == 1).ToList();            

            if (vm.SysNo > 0)
            {
                facade.UpdatePMGroup(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    Window.Alert(ResPMGroupMaintain.Info_SaveSuccessfully);
                });
            }
            else
            {
                facade.CreatePMGroup(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    Window.Alert(ResPMGroupMaintain.Info_SaveSuccessfully);
                });
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Window.Close();
        }
    }

}
