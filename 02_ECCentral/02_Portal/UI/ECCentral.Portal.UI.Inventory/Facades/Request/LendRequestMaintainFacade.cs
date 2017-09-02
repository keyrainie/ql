using System;
using System.Linq;
using System.Collections;
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
    public class LendRequestMaintainFacade
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

        public LendRequestMaintainFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public LendRequestMaintainFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        #region 借货单信息处理
 
        public void GetLendRequestInfoBySysNo(int sysNo, Action<LendRequestVM> callback)
        {
            string relativeUrl = string.Format("/InventoryService/LendRequest/Load/{0}", sysNo);
            restClient.Query<LendRequestInfo>(relativeUrl, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    LendRequestVM vm = null;
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


        public void CreateRequest(LendRequestVM requestVM, Action<LendRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/LendRequest/CreateRequest";
            requestVM.CompanyCode = CPApplication.Current.CompanyCode;
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Create);

            LendRequestInfo msg = ConvertRequestVMToInfo(requestVM);
            restClient.Create<LendRequestInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    LendRequestVM vm = null;
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

        public void UpdateRequest(LendRequestVM requestVM, Action<LendRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/LendRequest/UpdateRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Update);
            LendRequestInfo msg = ConvertRequestVMToInfo(requestVM);
            restClient.Update<LendRequestInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    LendRequestVM vm = null;
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


        private void UpdateRequestStatus(string relativeUrl, LendRequestVM requestVM, Action<LendRequestVM> callback)
        {            
            LendRequestInfo msg = ConvertRequestVMToInfo(requestVM);
            restClient.Update<LendRequestInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    LendRequestVM vm = null;
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

        public void AbandonRequest(LendRequestVM requestVM, Action<LendRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/LendRequest/AbandonRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Abandon);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void CancelAbandonRequest(LendRequestVM requestVM, Action<LendRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/LendRequest/CancelAbandonRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.CancelAbandon);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void VerifyRequest(LendRequestVM requestVM, Action<LendRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/LendRequest/VerifyRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Audit);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void CancelVerifyRequest(LendRequestVM requestVM, Action<LendRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/LendRequest/CancelVerifyRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.CancelAudit);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void OutStockRequest(LendRequestVM requestVM, Action<LendRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/LendRequest/OutStockRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.OutStock);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void ReturnRequest(LendRequestVM requestVM, Action<LendRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/LendRequest/ReturnLendItem";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Return);
            LendRequestInfo msg = ConvertRequestVMToInfo(requestVM);
            restClient.Update<LendRequestInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    LendRequestVM vm = null;
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

        public void GetProductLineSysNoBySysNo(int sysNo, Action<LendRequestVM> callback)
        {
            string relativeUrl = string.Format("/InventoryService/LendRequest/GetProductLine/{0}", sysNo);
            restClient.Query<LendRequestInfo>(relativeUrl, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    LendRequestVM vm = null;
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

        #endregion  借货单信息处理

        private LendRequestVM ConvertRequestInfoToVM(LendRequestInfo info)
        {
            LendRequestVM vm = info.Convert<LendRequestInfo, LendRequestVM>((i, v) =>
            {
                v.StockSysNo = i.Stock == null ? null : i.Stock.SysNo;                              
                v.AuditUserSysNo = i.AuditUser == null ? null : i.AuditUser.SysNo;
                v.CreateUserSysNo = i.CreateUser == null ? null : i.CreateUser.SysNo;
                v.EditUserSysNo = i.EditUser == null ? null : i.EditUser.SysNo;
                v.OutStockUserSysNo = i.OutStockUser == null ? null : i.OutStockUser.SysNo;
                v.LendUserSysNo = i.LendUser == null ? null : i.LendUser.SysNo;
                v.ProductLineSysno = i.ProductLineSysno == null ? null : i.ProductLineSysno;
            });
            if (info.LendItemInfoList != null)
            {
                info.LendItemInfoList.ForEach(p =>
                {
                    LendRequestItemVM item = vm.LendItemInfoList.FirstOrDefault(i => i.SysNo == p.SysNo);
                    if (p.LendProduct != null)
                    {
                        item.ProductSysNo = p.LendProduct.SysNo;
                        item.ProductName = p.LendProduct.ProductBasicInfo.ProductTitle.Content;
                        item.ProductID = p.LendProduct.ProductID;
                        item.PMUserSysNo = p.LendProduct.ProductBasicInfo.ProductManager.UserInfo.SysNo;
                        item.PMUserName = p.LendProduct.ProductBasicInfo.ProductManager.UserInfo.UserDisplayName;
                        item.ReturnDateETA = p.ExpectReturnDate;
                        item.ReturnQuantity = p.ReturnQuantity;
                    }
                });
            }
            return vm;
        }

        private LendRequestInfo ConvertRequestVMToInfo(LendRequestVM vm)
        {
            LendRequestInfo info = vm.ConvertVM<LendRequestVM, LendRequestInfo>((v, i) =>
            {
                i.Stock = new StockInfo { SysNo = v.StockSysNo };
                i.CreateUser = new BizEntity.Common.UserInfo { SysNo = v.CreateUserSysNo };
                i.EditUser = new BizEntity.Common.UserInfo { SysNo = v.EditUserSysNo };
                i.AuditUser = new BizEntity.Common.UserInfo { SysNo = v.AuditUserSysNo };
                i.OutStockUser = new BizEntity.Common.UserInfo { SysNo = v.OutStockUserSysNo };
                i.LendUser = new BizEntity.Common.UserInfo { SysNo = v.LendUserSysNo };
                i.ProductLineSysno = v.ProductLineSysno;
            });
            info.LendItemInfoList = new List<LendRequestItemInfo>();
            vm.LendItemInfoList.ForEach(item =>
            {
                LendRequestItemInfo itemInfo = item.ConvertVM<LendRequestItemVM, LendRequestItemInfo>();
                if (itemInfo.BatchDetailsInfoList != null)
                {
                    itemInfo.BatchDetailsInfoList.ForEach(p =>
                    {
                        var re = item.BatchDetailsInfoList.FirstOrDefault(k => k.ProductSysNo == p.ProductSysNo&&k.BatchNumber.Equals(p.BatchNumber));
                        if(re!=null)
                        {
                            p.ReturnQty = re.ReturnQuantity ?? 0;
                        }
                    });
                }
                itemInfo.LendProduct = new BizEntity.IM.ProductInfo { SysNo = item.ProductSysNo.Value };
                if (item.ReturnDateETA.HasValue)
                {
                    itemInfo.ExpectReturnDate = (DateTime)item.ReturnDateETA;
                }
                info.LendItemInfoList.Add(itemInfo);
            });
            return info;
        }

        private void SetRequestUserInfo(LendRequestVM requestVM, InventoryAdjustSourceAction action)
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
            else
            {
                requestVM.EditUserSysNo = currentUserSysNo;
            }

        }
      
    }
}
