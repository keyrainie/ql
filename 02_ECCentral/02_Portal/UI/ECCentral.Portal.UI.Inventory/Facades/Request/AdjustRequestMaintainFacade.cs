using System;
using System.Collections.Generic;
using System.Linq;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.Basic;
//using Newegg.Oversea.Silverlight.Utilities.Basic;
using ECCentral.Portal.UI.Inventory.Models.Inventory;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class AdjustRequestMaintainFacade
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
        public AdjustRequestMaintainFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        #region  信息处理
        private AdjustRequestVM ConvertRequestInfoToVM(AdjustRequestInfo info)
        {
            AdjustRequestVM vm = info.Convert<AdjustRequestInfo, AdjustRequestVM>((i, v) =>
            {
                v.StockSysNo = i.Stock == null ? null : i.Stock.SysNo;
                v.AuditUserSysNo = i.AuditUser == null ? null : i.AuditUser.SysNo;
                v.CreateUserSysNo = i.CreateUser == null ? null : i.CreateUser.SysNo;
                v.EditUserSysNo = i.EditUser == null ? null : i.EditUser.SysNo;
                v.OutStockUserSysNo = i.OutStockUser == null ? null : i.OutStockUser.SysNo;
                v.ProductLineSysno = i.ProductLineSysno == null ? null : i.ProductLineSysno;
                v.InvoiceInfo = i.InvoiceInfo.Convert<AdjustRequestInvoiceInfo, AdjustRequestInvoiceVM>();
            });
            if (info.AdjustItemInfoList != null)
            {
                info.AdjustItemInfoList.ForEach(p =>
                {
                    AdjustRequestItemVM item = vm.AdjustItemInfoList.FirstOrDefault(i => i.SysNo == p.SysNo);
                    if (p.AdjustProduct != null)
                    {
                        item.ProductSysNo = p.AdjustProduct.SysNo;
                        item.ProductName = p.AdjustProduct.ProductBasicInfo.ProductTitle.Content;
                        item.ProductID = p.AdjustProduct.ProductID;
                    }

                    item.BatchDetailsInfoList = EntityConverter<InventoryBatchDetailsInfo, ProductBatchInfoVM>.Convert(p.BatchDetailsInfoList);

                });
            }
            return vm;
        }
        private AdjustRequestInfo ConvertRequestVMToInfo(AdjustRequestVM vm)
        {
            AdjustRequestInfo info = vm.ConvertVM<AdjustRequestVM, AdjustRequestInfo>((v, i) =>
                {
                    i.Stock = new StockInfo { SysNo = v.StockSysNo};
                    i.CreateUser = new BizEntity.Common.UserInfo { SysNo = v.CreateUserSysNo };
                    i.EditUser = new BizEntity.Common.UserInfo { SysNo = v.EditUserSysNo };
                    i.AuditUser = new BizEntity.Common.UserInfo { SysNo = v.AuditUserSysNo };
                    i.OutStockUser = new BizEntity.Common.UserInfo { SysNo = v.OutStockUserSysNo };
                    i.ProductLineSysno = v.ProductLineSysno;
                    i.InvoiceInfo = v.InvoiceInfo.ConvertVM<AdjustRequestInvoiceVM, AdjustRequestInvoiceInfo>();
                });
            info.AdjustItemInfoList = new List<AdjustRequestItemInfo>();
            vm.AdjustItemInfoList.ForEach(item =>
            {
                AdjustRequestItemInfo itemInfo = item.ConvertVM<AdjustRequestItemVM, AdjustRequestItemInfo>();
                itemInfo.AdjustProduct = new BizEntity.IM.ProductInfo { SysNo = item.ProductSysNo.Value };
                itemInfo.BatchDetailsInfoList = new List<InventoryBatchDetailsInfo>();
                itemInfo.BatchDetailsInfoList = EntityConverter<ProductBatchInfoVM, InventoryBatchDetailsInfo>.Convert(item.BatchDetailsInfoList);
                info.AdjustItemInfoList.Add(itemInfo);
            });
            return info;
        }

        public void GetAdjustRequestInfoBySysNo(int sysNo, Action<AdjustRequestVM> callback)
        {
            string relativeUrl = string.Format("/InventoryService/AdjustRequest/Load/{0}", sysNo);
            restClient.Query<AdjustRequestInfo>(relativeUrl, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    AdjustRequestVM vm = null;
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

        public void CreateRequest(AdjustRequestVM requestVM, Action<List<AdjustRequestVM>> callback)
        {
            string relativeUrl = "/InventoryService/AdjustRequest/CreateRequest";
            requestVM.CompanyCode = CPApplication.Current.CompanyCode;
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Create);
            AdjustRequestInfo msg = ConvertRequestVMToInfo(requestVM);
            restClient.Create<List<AdjustRequestInfo>>(relativeUrl, msg, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    List<AdjustRequestVM> vmList = new List<AdjustRequestVM>();
                    if (args.Result != null)
                    {
                        args.Result.ForEach(x => {
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

        public void UpdateRequest(AdjustRequestVM requestVM, Action<AdjustRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/AdjustRequest/UpdateRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Update);
            AdjustRequestInfo msg = ConvertRequestVMToInfo(requestVM);
            restClient.Update<AdjustRequestInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    AdjustRequestVM vm = null;
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


        private void UpdateRequestStatus(string relativeUrl, AdjustRequestVM requestVM, Action<AdjustRequestVM> callback)
        {
            AdjustRequestInfo msg = ConvertRequestVMToInfo(requestVM);
            restClient.Update<AdjustRequestInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    AdjustRequestVM vm = null;
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

        public void AbandonRequest(AdjustRequestVM requestVM, Action<AdjustRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/AdjustRequest/AbandonRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Abandon);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void CancelAbandonRequest(AdjustRequestVM requestVM, Action<AdjustRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/AdjustRequest/CancelAbandonRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.CancelAbandon);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void VerifyRequest(AdjustRequestVM requestVM, Action<AdjustRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/AdjustRequest/VerifyRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Audit);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void CancelVerifyRequest(AdjustRequestVM requestVM, Action<AdjustRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/AdjustRequest/CancelVerifyRequest";            
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.CancelAudit);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void OutStockRequest(AdjustRequestVM requestVM, Action<AdjustRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/AdjustRequest/OutStockRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.OutStock);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void GetProductLineSysNoBySysNo(int sysNo, Action<AdjustRequestVM> callback)
        {
            string relativeUrl = string.Format("/InventoryService/AdjustRequest/GetProductLine/{0}", sysNo);
            restClient.Query<AdjustRequestInfo>(relativeUrl, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    AdjustRequestVM vm = null;
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

        #endregion  移仓单信息处理

        #region 移仓单发票信息

        public void MaintainRequestInvoiceInfo(AdjustRequestVM requestVM, Action<AdjustRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/AdjustRequest/MaintainAdjustInvoiceInfo";
            AdjustRequestInfo msg = ConvertRequestVMToInfo(requestVM);
            restClient.Update<AdjustRequestInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    AdjustRequestVM vm = null;
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

        #endregion 移仓单发票信息

        #region User Info

        private void SetRequestUserInfo(AdjustRequestVM requestVM, InventoryAdjustSourceAction action)
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
        
        #endregion User Info

    }
}
