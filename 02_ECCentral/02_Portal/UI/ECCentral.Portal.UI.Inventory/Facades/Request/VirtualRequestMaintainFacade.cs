using System;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.Basic;
using System.Collections.Generic;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.Restful.RequestMsg;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class VirtualRequestMaintainFacade
    {
        private readonly RestClient restClient;

        /// <summary>
        /// InventoryService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Inventory, ConstValue.Key_ServiceBaseUrl);
            }
        }

        public VirtualRequestMaintainFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 虚库单 - 审核同意
        /// </summary>
        /// <param name="viewVM"></param>
        /// <param name="callback"></param>
        public void ApproveVirtualRequest(VirtualRequestVM viewVM, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/InventoryService/VirtualRequest/ApproveVirtualRequest";
            restClient.Update(relativeUrl, VirtualRequestVMToInfo(viewVM), callback);
        }

        /// <summary>
        /// 虚库单 - 审核拒绝
        /// </summary>
        /// <param name="viewVM"></param>
        /// <param name="callback"></param>
        public void RejectVirtualRequest(VirtualRequestVM viewVM, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/InventoryService/VirtualRequest/RejectVirtualRequest";
            restClient.Update(relativeUrl, VirtualRequestVMToInfo(viewVM), callback);
        }

        /// <summary>
        /// 虚库单 - ViewModel转Entity
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        private VirtualRequestInfo VirtualRequestVMToInfo(VirtualRequestVM vm)
        {
            return EntityConverter<VirtualRequestVM, VirtualRequestInfo>.Convert(vm, (s, t) =>
            {
                t.Stock = new StockInfo() { SysNo = s.StockSysNo, StockName = s.StockName };
                t.VirtualProduct = new BizEntity.IM.ProductInfo() { SysNo = s.ProductSysNo.Value, ProductID = s.ProductID };
                t.CompanyCode = CPApplication.Current.CompanyCode;
            });
        }
        private VirtualRequestVM VirtualRequestInfoToVM(VirtualRequestInfo requestInfo)
        {
            return requestInfo.Convert<VirtualRequestInfo, VirtualRequestVM>((info, vm) =>
            {
                if (info.Stock != null)
                {
                    vm.StockName = info.Stock.StockName;
                    vm.StockSysNo = info.Stock.SysNo;
                }
                if (info.VirtualProduct != null)
                {
                    vm.ProductID = info.VirtualProduct.ProductID;
                    vm.ProductName = info.VirtualProduct.ProductName;
                    vm.ProductSysNo = info.VirtualProduct.SysNo;
                }
            });
        }

        public void ApplyRequest(List<VirtualRequestVM> vmList, Action<List<VirtualRequestVM>> callback)
        {
            string relativeUrl = "/InventoryService/VirtualRequest/Apply";
            VirtualRequestInfoReq request = new VirtualRequestInfoReq();
            request.CanOperateItemOfLessThanPrice = AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_VirtualRequestApply_CanOperateItemOfLessThanPrice);
            request.CanOperateItemOfSecondHand = AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_VirtualRequestApply_CanOperateItemOfSecondHand);
            vmList.ForEach(vm =>
            {
                request.RequestList.Add(VirtualRequestVMToInfo(vm));
            });

            restClient.Create<List<VirtualRequestInfo>>(relativeUrl, request, (obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        vmList = new List<VirtualRequestVM>();
                        if (args.Result != null)
                        {
                            args.Result.ForEach(info =>
                            {
                                vmList.Add(VirtualRequestInfoToVM(info));
                            });
                        }
                        if (callback != null)
                        {
                            callback(vmList);
                        }
                    }
                });
        }
    }
}
