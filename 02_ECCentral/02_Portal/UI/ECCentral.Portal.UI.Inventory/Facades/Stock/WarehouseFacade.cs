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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM.Product;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class WarehouseFacade
    {
        private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Inventory, ConstValue.Key_ServiceBaseUrl);
        IPage Page;
        public WarehouseFacade(IPage page)
        {
            Page = page;
            restClient = new RestClient(serviceBaseUrl, page);
        }

        public void GetWarehouseInfo(int warehouseSysNo, Action<WarehouseInfoVM> action)
        {
            restClient.Query<WarehouseInfo>(String.Format("/InventoryService/Stock/WH/Get/{0}", warehouseSysNo), (obj, args) =>
            {
                if (!args.FaultsHandle() && action != null)
                {
                    WarehouseInfoVM vm = ConvertWarehouseInfoToVM(args.Result);
                    action(vm);
                }
            });
        }

        public void CreateWarehouseInfo(WarehouseInfoVM vm, Action callback)
        {
            WarehouseInfo info = ConvertWarehouseVMToInfo(vm);
            restClient.Query<WarehouseInfo>("/InventoryService/Stock/WH/Create", info, (obj, args) =>
            {
                if (!args.FaultsHandle() && args.Result != null)
                {
                    vm.SysNo = args.Result.SysNo;
                    this.Page.Context.Window.Alert("仓库信息创建成功", MessageType.Information);
                }
                if (callback != null)
                {
                    callback();
                }
            });
        }

        public void UpdateWarehouseInfo(WarehouseInfoVM vm, Action callback)
        {
            WarehouseInfo info = ConvertWarehouseVMToInfo(vm);
            restClient.Query<WarehouseInfo>("/InventoryService/Stock/WH/Update", info, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    Page.Context.Window.Alert("仓库信息修改成功", MessageType.Information);
                }
                if (callback != null)
                {
                    callback();
                }
            });
        }

        public void GetProductCountryList(EventHandler<RestClientEventArgs<List<ProductCountry>>> callback)
        {
            restClient.Query("/IMService/Product/GetProductCountryList", callback);
        }

        #region Convert
        private WarehouseInfoVM ConvertWarehouseInfoToVM(WarehouseInfo info)
        {
            WarehouseInfoVM vm = info.Convert<WarehouseInfo, WarehouseInfoVM>((i, v) =>
            {
                v.CreateUserSysNo = i.CreateUser == null ? null : i.CreateUser.SysNo;
                v.EditUserSysNo = i.EditUser == null ? null : i.EditUser.SysNo;
                //v.OwnerSysNo = i.OwnerInfo == null ? null : i.OwnerInfo.SysNo.ToString();//(中蛋定制化 不需要此信息)
                v.TransferRate = decimal.Round(i.TransferRate, 2); 
            });
            return vm;
        }

        private WarehouseInfo ConvertWarehouseVMToInfo(WarehouseInfoVM vm)
        {
            WarehouseInfo info = vm.ConvertVM<WarehouseInfoVM, WarehouseInfo>((v, i) =>
            {
                i.CreateUser = new UserInfo { SysNo = v.CreateUserSysNo };
                i.EditUser = new UserInfo { SysNo = v.EditUserSysNo };
                //i.OwnerInfo = new WarehouseOwnerInfo { SysNo = int.Parse(v.OwnerSysNo) };//(中蛋定制化 不需要此信息)
                i.TransferRate = decimal.Round(vm.TransferRate, 2); 
            });
            return info;
        }
        #endregion Convert
    }
}
