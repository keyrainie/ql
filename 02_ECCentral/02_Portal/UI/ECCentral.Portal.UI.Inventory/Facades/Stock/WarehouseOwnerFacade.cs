using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class WarehouseOwnerFacade
    {
        private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Inventory, ConstValue.Key_ServiceBaseUrl);
        IPage Page;
        public WarehouseOwnerFacade(IPage page)
        {
            Page = page;
            restClient = new RestClient(serviceBaseUrl, page);
        }

        public void GetWarehouseOwnerInfo(int warehouseOwnerSysNo, Action<WarehouseOwnerInfoVM> action)
        {
            restClient.Query<WarehouseOwnerInfo>(String.Format("/InventoryService/Stock/WarehouseOwner/Get/{0}", warehouseOwnerSysNo.ToString()), (obj, args) =>
            {
                if (!args.FaultsHandle() && action != null)
                {
                    WarehouseOwnerInfoVM vm = ConvertOwnerInfoToVM(args.Result);//.Convert<WarehouseOwnerInfo, WarehouseOwnerInfoVM>();
                    action(vm);
                }
            });
        }

        public void CreateWarehouseOwnerInfo(WarehouseOwnerInfoVM vm, Action callback)
        {
            vm.CompanyCode = CPApplication.Current.CompanyCode;
            vm.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            WarehouseOwnerInfo info = ConvertOwnerVMToInfo(vm);
            restClient.Query<WarehouseOwnerInfo>("/InventoryService/Stock/WarehouseOwner/Create", info, (obj, args) =>
            {
                if (!args.FaultsHandle() && args.Result != null)
                {
                    vm.SysNo = args.Result.SysNo;
                    this.Page.Context.Window.Alert("仓库所有者信息创建成功", MessageType.Information);
                }
                if (callback != null)
                {
                    callback();
                }
            });
        }

        public void UpdateWarehouseOwnerInfo(WarehouseOwnerInfoVM vm, Action callback)
        {
            vm.CompanyCode = CPApplication.Current.CompanyCode;
            vm.EditUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            WarehouseOwnerInfo info = ConvertOwnerVMToInfo(vm);
            restClient.Query<WarehouseInfo>("/InventoryService/Stock/WarehouseOwner/Update", info, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    Page.Context.Window.Alert("仓库所有者信息修改成功", MessageType.Information);
                }
                if (callback != null)
                {
                    callback();
                }
            });
        }

        #region Convert
        private WarehouseOwnerInfoVM ConvertOwnerInfoToVM(WarehouseOwnerInfo info)
        {
            WarehouseOwnerInfoVM vm = info.Convert<WarehouseOwnerInfo, WarehouseOwnerInfoVM>((i, v) =>
            {   
                v.CreateUserSysNo = i.CreateUser == null ? null : i.CreateUser.SysNo;
                v.EditUserSysNo = i.EditUser == null ? null : i.EditUser.SysNo;             
            });          
            return vm;
        }

        private WarehouseOwnerInfo ConvertOwnerVMToInfo(WarehouseOwnerInfoVM vm)
        {
            WarehouseOwnerInfo info = vm.ConvertVM<WarehouseOwnerInfoVM, WarehouseOwnerInfo>((v, i) =>
            {   
                i.CreateUser = new BizEntity.Common.UserInfo { SysNo = v.CreateUserSysNo };
                i.EditUser = new BizEntity.Common.UserInfo { SysNo = v.EditUserSysNo };             
            });            
            return info;
        }
        #endregion Convert
    }
}
