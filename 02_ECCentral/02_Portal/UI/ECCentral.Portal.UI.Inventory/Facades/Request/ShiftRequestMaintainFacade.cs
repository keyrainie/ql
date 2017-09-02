using System;
using System.Linq;
using System.Collections.Generic;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class ShiftRequestMaintainFacade
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

        public ShiftRequestMaintainFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        #region 移仓单信息处理
        private List<ShiftRequestVM> ConvertRequestInfoToVM(List<ShiftRequestInfo> infoList)
        {
            if (infoList == null || infoList.Count <= 0)
            {
                return null;
            }

            List<ShiftRequestVM> vmList = new List<ShiftRequestVM>();
            infoList.ForEach(info => {
                vmList.Add(ConvertRequestInfoToVM(info));
            });

            return vmList;
        }

        private List<ShiftRequestInfo> ConvertRequestVMToInfo(List<ShiftRequestVM> vmList)
        {
            if (vmList == null || vmList.Count <= 0)
            {
                return null;
            }

            List<ShiftRequestInfo> infoList = new List<ShiftRequestInfo>();
            vmList.ForEach(vm => {
                infoList.Add(ConvertRequestVMToInfo(vm));
            });

            return infoList;
        }

        private ShiftRequestVM ConvertRequestInfoToVM(ShiftRequestInfo info)
        {
            ShiftRequestVM vm = info.Convert<ShiftRequestInfo, ShiftRequestVM>((i, v) =>
            {
                v.AuditUserSysNo = i.AuditUser == null ? null : i.AuditUser.SysNo;
                v.CreateUserSysNo = i.CreateUser == null ? null : i.CreateUser.SysNo;
                v.EditUserSysNo = i.EditUser == null ? null : i.EditUser.SysNo;
                v.OutStockUserSysNo = i.OutStockUser == null ? null : i.OutStockUser.SysNo;
                v.SpecialShiftSetUserSysNo = i.SpecialShiftSetUser == null ? null : i.SpecialShiftSetUser.SysNo;
                v.InStockUserSysNo = i.InStockUser == null ? null : i.InStockUser.SysNo;
                v.TargetStockSysNo = i.TargetStock == null ? null : i.TargetStock.SysNo;
                v.SourceStockSysNo = i.SourceStock == null ? null : i.SourceStock.SysNo;
                v.ShiftShippingType = i.ShiftShippingType;
                v.ProductLineSysno = i.ProductLineSysno == null ? null : i.ProductLineSysno;
            });
            if (info.ShiftItemInfoList != null)
            {
                info.ShiftItemInfoList.ForEach(p =>
                {
                    ShiftRequestItemVM item = vm.ShiftItemInfoList.FirstOrDefault(i => i.SysNo == p.SysNo);
                    if (p.ShiftProduct != null)
                    {
                        item.ProductSysNo = p.ShiftProduct.SysNo;
                        item.ProductName = p.ShiftProduct.ProductBasicInfo.ProductTitle.Content;
                        item.ProductID = p.ShiftProduct.ProductID;
                        item.TotalWeight=p.TotalWeight;
                    }
                });
            }
            return vm;
        }

        private ShiftRequestInfo ConvertRequestVMToInfo(ShiftRequestVM vm)
        {
            ShiftRequestInfo info = vm.ConvertVM<ShiftRequestVM, ShiftRequestInfo>((v, i) =>
            {
                i.CreateUser = new BizEntity.Common.UserInfo { SysNo = v.CreateUserSysNo };
                i.EditUser = new BizEntity.Common.UserInfo { SysNo = v.EditUserSysNo };
                i.AuditUser = new BizEntity.Common.UserInfo { SysNo = v.AuditUserSysNo };
                i.OutStockUser = new BizEntity.Common.UserInfo { SysNo = v.OutStockUserSysNo };
                i.SpecialShiftSetUser = new BizEntity.Common.UserInfo { SysNo = v.SpecialShiftSetUserSysNo };
                i.InStockUser = new BizEntity.Common.UserInfo { SysNo = v.InStockUserSysNo };
                i.SourceStock = new StockInfo { SysNo = v.SourceStockSysNo };
                i.TargetStock = new StockInfo { SysNo = v.TargetStockSysNo };
                i.ProductLineSysno = v.ProductLineSysno;

            });
            info.ShiftItemInfoList = new List<ShiftRequestItemInfo>();
            vm.ShiftItemInfoList.ForEach(item =>
            {
                ShiftRequestItemInfo itemInfo = item.ConvertVM<ShiftRequestItemVM, ShiftRequestItemInfo>();
                itemInfo.ShiftProduct = new BizEntity.IM.ProductInfo { SysNo = item.ProductSysNo.Value };
                info.ShiftItemInfoList.Add(itemInfo);
            });
            return info;
        }

        public void GetShiftRequestInfoBySysNo(int sysNo, Action<ShiftRequestVM> callback)
        {
            string relativeUrl = string.Format("/InventoryService/ShiftRequest/Load/{0}", sysNo);
            restClient.Query<ShiftRequestInfo>(relativeUrl, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    ShiftRequestVM vm = null;
                    if (args.Result != null)
                    {
                        vm = ConvertRequestInfoToVM(args.Result);
                    }
                    if (callback != null)
                    {
                        callback(vm);
                    }
                }
            });
        }

        public void GetProductLineSysNoBySysNo(int sysNo, Action<ShiftRequestVM> callback)
        {
            string relativeUrl = string.Format("/InventoryService/ShiftRequest/GetProductLine/{0}", sysNo);
            restClient.Query<ShiftRequestInfo>(relativeUrl, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    ShiftRequestVM vm = null;
                    if (args.Result != null)
                    {
                        vm = ConvertRequestInfoToVM(args.Result);
                    }
                    if (callback != null)
                    {
                        callback(vm);
                    }
                }
            });
        }

        public void CreateRequest(ShiftRequestVM requestVM, Action<List<ShiftRequestVM>> callback)
        {
            string relativeUrl = "/InventoryService/ShiftRequest/CreateRequest";
            requestVM.CompanyCode = CPApplication.Current.CompanyCode;
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Create);
            ShiftRequestInfo msg = ConvertRequestVMToInfo(requestVM);
            restClient.Create<List<ShiftRequestInfo>>(relativeUrl, msg, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    List<ShiftRequestVM> vmList = new List<ShiftRequestVM>();
                    if (args.Result != null)
                    {
                        args.Result.ForEach(x =>
                        {
                            vmList.Add(ConvertRequestInfoToVM(x));
                        });
                    }                    
                    if (callback != null)
                    {
                        callback(vmList);
                    }
                }
            });
        }

        public void UpdateRequest(ShiftRequestVM requestVM, Action<ShiftRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/ShiftRequest/UpdateRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Update);
            ShiftRequestInfo msg = ConvertRequestVMToInfo(requestVM);
            restClient.Update<ShiftRequestInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    ShiftRequestVM vm = null;
                    if (args.Result != null)
                    {
                        vm = ConvertRequestInfoToVM(args.Result);
                    }
                    if (callback != null)
                    {
                        callback(vm);
                    }
                }
            });
        }

        private void UpdateRequestStatus(string relativeUrl, ShiftRequestVM requestVM, Action<ShiftRequestVM> callback)
        {
            ShiftRequestInfo msg = ConvertRequestVMToInfo(requestVM);
            restClient.Update<ShiftRequestInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    ShiftRequestVM vm = null;
                    if (args.Result != null)
                    {
                        vm = ConvertRequestInfoToVM(args.Result);
                    }
                    if (callback != null)
                    {
                        callback(vm);
                    }
                }
            });
        }

        public void AbandonRequest(ShiftRequestVM requestVM, Action<ShiftRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/ShiftRequest/AbandonRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Abandon);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void CancelAbandonRequest(ShiftRequestVM requestVM, Action<ShiftRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/ShiftRequest/CancelAbandonRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.CancelAbandon);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void VerifyRequest(ShiftRequestVM requestVM, Action<ShiftRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/ShiftRequest/VerifyRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Audit);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void CancelVerifyRequest(ShiftRequestVM requestVM, Action<ShiftRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/ShiftRequest/CancelVerifyRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.CancelAudit);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void OutStockRequest(ShiftRequestVM requestVM, Action<ShiftRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/ShiftRequest/OutStockRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.OutStock);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void InStockRequest(ShiftRequestVM requestVM, Action<ShiftRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/ShiftRequest/InStockRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.InStock);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        private void UpdateSpecialShiftType(List<ShiftRequestVM> requestVMList, Action<List<ShiftRequestVM>> callback)
        {
            string relativeUrl = "/InventoryService/ShiftRequest/UpdateSpecialShiftTypeBatch";
            List<ShiftRequestInfo> infoList = ConvertRequestVMToInfo(requestVMList);
            restClient.Update<List<ShiftRequestInfo>>(relativeUrl, infoList, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    List<ShiftRequestVM> vmList = null;
                    if (args.Result != null)
                    {
                        vmList = ConvertRequestInfoToVM(args.Result);
                    }
                    if (callback != null)
                    {
                        callback(vmList);
                    }
                }
            });
        }

        public void SetSpecialShiftType(List<int> requestSysNoList, Action<List<ShiftRequestVM>> callback)
        {
            List<ShiftRequestVM> requestVMList = new List<ShiftRequestVM>();
            requestSysNoList.ForEach(sysNo => {
                requestVMList.Add(new ShiftRequestVM() { 
                    SysNo = sysNo,
                    SpecialShiftSetUserSysNo = CPApplication.Current.LoginUser.UserSysNo,
                    SpecialShiftType = SpecialShiftRequestType.HandWork
                });
            });
           
            UpdateSpecialShiftType(requestVMList, callback);
        }

        public void CancelSpecialShiftType(List<int> requestSysNoList, Action<List<ShiftRequestVM>> callback)
        {
            List<ShiftRequestVM> requestVMList = new List<ShiftRequestVM>();
            requestSysNoList.ForEach(sysNo =>
            {
                requestVMList.Add(new ShiftRequestVM()
                {
                    SysNo = sysNo,
                    SpecialShiftSetUserSysNo = CPApplication.Current.LoginUser.UserSysNo,
                    SpecialShiftType = SpecialShiftRequestType.Default
                });
            });

            UpdateSpecialShiftType(requestVMList, callback);
        }

        #endregion  移仓单信息处理

        #region 订单日志处理

        private ShiftRequestMemoVM ConvertMemoInfoToVM(ShiftRequestMemoInfo info)
        {
            ShiftRequestMemoVM vm = info.Convert<ShiftRequestMemoInfo, ShiftRequestMemoVM>((i, v) =>
            {
                if (i.CreateUser != null)
                {
                    v.CreateUserSysNo = i.CreateUser.SysNo;
                    v.CreateUserName = i.CreateUser.UserDisplayName;
                }
                if (i.EditUser != null)
                {
                    v.EditUserSysNo = i.EditUser.SysNo;
                    v.EditUserName = i.EditUser.UserDisplayName;
                }
            });
            return vm;
        }

        private ShiftRequestMemoInfo ConvertMemoVMToInfo(ShiftRequestMemoVM vm)
        {
            ShiftRequestMemoInfo info = vm.ConvertVM<ShiftRequestMemoVM, ShiftRequestMemoInfo>((v, i) =>
            {
                i.CreateUser = new BizEntity.Common.UserInfo { SysNo = v.CreateUserSysNo };
                i.EditUser = new BizEntity.Common.UserInfo { SysNo = v.EditUserSysNo }; ;
            });
            return info;
        }

        public void AddShiftRequestMemo(ShiftRequestMemoVM vm, Action<ShiftRequestMemoVM> callback)
        {
            string relativeUrl = "/InventoryService/ShiftRequest/CreateShiftRequestMemo";

            restClient.Create<ShiftRequestMemoInfo>(relativeUrl, ConvertMemoVMToInfo(vm),
                (obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        vm = null;
                        if (args.Result != null && callback != null)
                        {
                            vm = ConvertMemoInfoToVM(args.Result);
                        }
                        if (callback != null)
                        {
                            callback(vm);
                        }
                    }
                });
        }

        public void AddShiftRequestMemo(List<ShiftRequestMemoVM> vmList, Action<List<ShiftRequestMemoVM>> callback)
        {
            string relativeUrl = "/InventoryService/ShiftRequest/CreateShiftRequestMemo";
            List<ShiftRequestMemoInfo> infoList = new List<ShiftRequestMemoInfo>();
            if (vmList != null && vmList.Count > 0)
            {
                vmList.ForEach(vm =>
                {
                    infoList.Add(ConvertMemoVMToInfo(vm));
                });

                restClient.Create<List<ShiftRequestMemoInfo>>(relativeUrl, infoList,
                    (obj, args) =>
                    {
                        if (!args.FaultsHandle())
                        {
                            vmList = null;
                            if (args.Result != null && callback != null)
                            {
                                vmList = new List<ShiftRequestMemoVM>();
                                args.Result.ForEach(item =>
                                {
                                    vmList.Add(ConvertMemoInfoToVM(item));
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

        public void UpdateShiftRequestMemo(List<ShiftRequestMemoVM> vmList, Action<List<ShiftRequestMemoVM>> callback)
        {
            string relativeUrl = "/InventoryService/ShiftRequest/UpdateShiftRequestMemoList";
            List<ShiftRequestMemoInfo> infoList = new List<ShiftRequestMemoInfo>();
            if (vmList != null && vmList.Count > 0)
            {
                vmList.ForEach(vm =>
                {
                    infoList.Add(ConvertMemoVMToInfo(vm));
                });

                restClient.Update<List<ShiftRequestMemoInfo>>(relativeUrl, infoList,
                    (obj, args) =>
                    {
                        if (!args.FaultsHandle())
                        {
                            vmList = null;
                            if (args.Result != null && callback != null)
                            {
                                vmList = new List<ShiftRequestMemoVM>();
                                args.Result.ForEach(item =>
                                {
                                    vmList.Add(ConvertMemoInfoToVM(item));
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

        public void GetShiftRequestMemoByRequestSysNo(int requestSysNo, Action<List<ShiftRequestMemoVM>> callback)
        {
            string relativeUrl = String.Format("/InventoryService/ShiftRequest/GetShiftRequestMemoListByRequestSysNo/{0}", requestSysNo);

            restClient.Query<List<ShiftRequestMemoInfo>>(relativeUrl,
                (obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        List<ShiftRequestMemoVM> vmList = null;
                        if (args.Result != null && callback != null)
                        {
                            vmList = new List<ShiftRequestMemoVM>();
                            args.Result.ForEach(item =>
                                {
                                    vmList.Add(ConvertMemoInfoToVM(item));
                                });
                        }
                        if (callback != null)
                        {
                            callback(vmList);
                        }
                    }
                });
        }

        public void GetShiftRequestMemoBySysNo(int memoSysNo, Action<ShiftRequestMemoVM> callback)
        {
            string relativeUrl = String.Format("/InventoryService/ShiftRequest/GetShiftRequestMemoInfoBySysNo/{0}", memoSysNo);

            restClient.Query<ShiftRequestMemoInfo>(relativeUrl,
                (obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        ShiftRequestMemoVM
                        vm = null;
                        if (args.Result != null && callback != null)
                        {
                            vm = ConvertMemoInfoToVM(args.Result);
                        }
                        if (callback != null)
                        {
                            callback(vm);
                        }
                    }
                });
        }


        #endregion

        #region User Info

        private void SetRequestUserInfo(ShiftRequestVM requestVM, InventoryAdjustSourceAction action)
        {
            int? currentUserSysNo = CPApplication.Current.LoginUser.UserSysNo;

            if (action == InventoryAdjustSourceAction.Create)
            {
                requestVM.CreateUserSysNo = currentUserSysNo;                
            }
            else if (action == InventoryAdjustSourceAction.Audit || action == InventoryAdjustSourceAction.CancelAudit)
            {
                requestVM.AuditUserSysNo = currentUserSysNo;
            }
            else if (action == InventoryAdjustSourceAction.OutStock)
            {
                requestVM.OutStockUserSysNo = currentUserSysNo;                
            }
            else if (action == InventoryAdjustSourceAction.InStock)
            {
                requestVM.InStockUserSysNo = currentUserSysNo;
            }
            else
            {
                requestVM.EditUserSysNo = currentUserSysNo;
            }

        }

        #endregion User Info


        public void GetProductLineSysNoBySysNo(ShiftRequestVM RequestVM, Action<ShiftRequestVM> action)
        {
            throw new NotImplementedException();
        }
    }
}
