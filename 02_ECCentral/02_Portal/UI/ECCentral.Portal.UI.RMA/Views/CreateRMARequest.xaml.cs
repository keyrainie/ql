using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Portal.UI.RMA.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.RMA.Views
{
    [View]
    public partial class CreateRMARequest : PageBase
    {       
        public CreateRMARequest()
        {
            InitializeComponent();            
        }
       
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            this.ucSOInfo.Page = this;

            CodeNamePairHelper.GetList(ConstValue.DomainName_RMA, "ShipType", CodeNamePairAppendItemType.Select, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var vm = new RequestVM();
                vm.ShipTypes = args.Result;
                this.DataContext = vm;               
            });
            btnSave.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Request_CanAdd);
        }       

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.ucSOInfo))
            {               
                RequestVM vm = this.DataContext as RequestVM;
                //[BugFix] 寄回方式、快递名称、邮包编号,不再是必填项
                //当寄回方式不为上门取件物流时，邮包编号必须填写
                //string shipType = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("RMA", "ShipTypePush");

                //if (vm.ShipType != shipType && string.IsNullOrEmpty(vm.TrackingNumber))
                //{
                //    Window.Alert(ResCreateRequest.Validation_TrackingNumberRequired);
                //    return;
                //}
                if (vm.Registers.Count(p => p.IsChecked) == 0)
                {
                    Window.Alert(ResCreateRequest.Validation_ChooseSOItem);
                    return;
                }
                bool flag = true;
                foreach (var item in vm.Registers)
                {
                    if (item.IsChecked && string.IsNullOrEmpty(item.BasicInfo.CustomerDesc))
                    {
                        flag = false;
                        //var members = new List<string> { "CustomerDesc" };
                        //item.BasicInfo.ValidationErrors.Add(new ValidationResult(ResCreateRequest.Validation_DescriptionRequired,members));                                                                        
                    }
                }
                if (!flag)
                {
                    Window.Alert(ResCreateRequest.Validation_DescriptionRequired);
                    return;
                }

                RequestVM clonedVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<RequestVM>(vm);

                var list = clonedVM.Registers.Where(p => p.IsChecked).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].BasicInfo.SOItemType == BizEntity.SO.SOProductType.ExtendWarranty && list[i].RequestType != BizEntity.RMA.RMARequestType.Return)
                    {
                        Window.Alert(ResCreateRequest.Msg_ExtendInfo);
                        return;
                    }
                }
                clonedVM.Registers.Clear();
                list.ForEach(p =>
                {
                    clonedVM.Registers.Add(p);
                });
                new RequestFacade(this).Create(clonedVM, (obj, args) =>
                {
                    vm.SysNo = args.Result.SysNo;
                    string url = string.Format(ConstValue.RMA_RequestMaintainUrl, vm.SysNo);
                    Window.Navigate(url);
                });
            }
        }

        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as RequestVM;
            CheckBox chk = sender as CheckBox;
            if (vm.Registers.Count > 0)
            {
                foreach (var item in vm.Registers)
                {
                    item.IsChecked = chk.IsChecked.Value;
                };
            }
        }
    }
}
