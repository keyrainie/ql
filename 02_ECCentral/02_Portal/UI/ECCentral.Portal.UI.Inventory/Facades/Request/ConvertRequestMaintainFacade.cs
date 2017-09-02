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
using ECCentral.Portal.UI.Inventory.Models.Inventory;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class ConvertRequestMaintainFacade
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

        public ConvertRequestMaintainFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }
        public ConvertRequestMaintainFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        #region 转换单信息处理

        private ConvertRequestVM ConvertRequestInfoToVM(ConvertRequestInfo info)
        {
            ConvertRequestVM vm = info.Convert<ConvertRequestInfo, ConvertRequestVM>((i, v) =>
            {
                v.StockSysNo = i.Stock == null ? null : i.Stock.SysNo;
                v.AuditUserSysNo = i.AuditUser == null ? null : i.AuditUser.SysNo;
                v.CreateUserSysNo = i.CreateUser == null ? null : i.CreateUser.SysNo;
                v.EditUserSysNo = i.EditUser == null ? null : i.EditUser.SysNo;
                v.OutStockUserSysNo = i.OutStockUser == null ? null : i.OutStockUser.SysNo;
                v.OutStockUserSysNo = i.OutStockUser == null ? null : i.OutStockUser.SysNo;
                v.ProductLineSysno = i.ProductLineSysno == null ? null : i.ProductLineSysno;
            });
            if (info.ConvertItemInfoList != null)
            {
                info.ConvertItemInfoList.ForEach(p =>
                {
                    ConvertRequestItemVM item = vm.ConvertItemInfoList.FirstOrDefault(i => i.SysNo == p.SysNo);
                    if (p.ConvertProduct != null)
                    {
                        item.ProductSysNo = p.ConvertProduct.SysNo;
                        item.ProductName = p.ConvertProduct.ProductBasicInfo.ProductTitle.Content;
                        item.ProductID = p.ConvertProduct.ProductID;
                    }
                    item.BatchDetailsInfoList = new List<ProductBatchInfoVM>();
                    item.BatchDetailsInfoList = EntityConverter<InventoryBatchDetailsInfo, ProductBatchInfoVM>.Convert(p.BatchDetailsInfoList);
                });
            }
            return vm;
        }
        private ConvertRequestInfo ConvertRequestVMToInfo(ConvertRequestVM vm)
        {
            ConvertRequestInfo info = vm.ConvertVM<ConvertRequestVM, ConvertRequestInfo>((v, i) =>
                {
                    i.Stock = new StockInfo { SysNo = v.StockSysNo };
                    i.CreateUser = new BizEntity.Common.UserInfo { SysNo = v.CreateUserSysNo };
                    i.EditUser = new BizEntity.Common.UserInfo { SysNo = v.EditUserSysNo };
                    i.AuditUser = new BizEntity.Common.UserInfo { SysNo = v.AuditUserSysNo };
                    i.OutStockUser = new BizEntity.Common.UserInfo { SysNo = v.OutStockUserSysNo };
                    i.ProductLineSysno = v.ProductLineSysno;
                });
            info.ConvertItemInfoList = new List<ConvertRequestItemInfo>();
            vm.ConvertItemInfoList.ForEach(item =>
            {
                ConvertRequestItemInfo itemInfo = item.ConvertVM<ConvertRequestItemVM, ConvertRequestItemInfo>();
                itemInfo.ConvertProduct = new BizEntity.IM.ProductInfo { SysNo = item.ProductSysNo.Value };
                itemInfo.BatchDetailsInfoList = new List<InventoryBatchDetailsInfo>();
                itemInfo.BatchDetailsInfoList = EntityConverter<ProductBatchInfoVM, InventoryBatchDetailsInfo>.Convert(item.BatchDetailsInfoList);
                info.ConvertItemInfoList.Add(itemInfo);
            });
            return info;
        }

        public void GetConvertRequestInfoBySysNo(int sysNo, Action<ConvertRequestVM> callback)
        {
            string relativeUrl = string.Format("/InventoryService/ConvertRequest/Load/{0}", sysNo);
            restClient.Query<ConvertRequestInfo>(relativeUrl, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    ConvertRequestVM vm = null;
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

        public void CreateRequest(ConvertRequestVM requestVM, Action<ConvertRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/ConvertRequest/CreateRequest";
            requestVM.CompanyCode = CPApplication.Current.CompanyCode;
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Create);
            ConvertRequestInfo msg = ConvertRequestVMToInfo(requestVM);
            restClient.Create<ConvertRequestInfo>(relativeUrl, msg, (obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        requestVM = null;
                        if (args.Result != null)
                        {
                            requestVM = ConvertRequestInfoToVM(args.Result);
                        }
                        callback(requestVM);
                    }
                });
        }
        public void UpdateRequest(ConvertRequestVM requestVM, Action<ConvertRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/ConvertRequest/UpdateRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Update);
            ConvertRequestInfo msg = ConvertRequestVMToInfo(requestVM);
            restClient.Update<ConvertRequestInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    requestVM = null;
                    if (args.Result != null)
                    {
                        requestVM = ConvertRequestInfoToVM(args.Result);
                    }
                    callback(requestVM);
                }
            });
        }

        private void UpdateRequestStatus(string relativeUrl, ConvertRequestVM requestVM, Action<ConvertRequestVM> callback)
        {
            ConvertRequestInfo msg = ConvertRequestVMToInfo(requestVM);
            restClient.Update<ConvertRequestInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    ConvertRequestVM resultRequestVM = null;
                    if (args.Result != null)
                    {
                        resultRequestVM = ConvertRequestInfoToVM(args.Result);
                    }
                    callback(resultRequestVM);
                }
            });
        }

        public void AbandonRequest(ConvertRequestVM requestVM, Action<ConvertRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/ConvertRequest/AbandonRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Abandon);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void CancelAbandonRequest(ConvertRequestVM requestVM, Action<ConvertRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/ConvertRequest/CancelAbandonRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.CancelAbandon);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void AuditRequest(ConvertRequestVM requestVM, Action<ConvertRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/ConvertRequest/VerifyRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.Audit);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void CancelAuditRequest(ConvertRequestVM requestVM, Action<ConvertRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/ConvertRequest/CancelVerifyRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.CancelAudit);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void OutStockRequest(ConvertRequestVM requestVM, Action<ConvertRequestVM> callback)
        {
            string relativeUrl = "/InventoryService/ConvertRequest/OutStockRequest";
            SetRequestUserInfo(requestVM, InventoryAdjustSourceAction.OutStock);
            UpdateRequestStatus(relativeUrl, requestVM, callback);
        }

        public void GetProductLineSysNoBySysNo(int sysNo, Action<ConvertRequestVM> callback)
        {
            string relativeUrl = string.Format("/InventoryService/ConvertRequest/GetProductLine/{0}", sysNo);
            restClient.Query<ConvertRequestInfo>(relativeUrl, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    ConvertRequestVM vm = null;
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
        #endregion  转换单信息处理

        #region User Info
        private void SetRequestUserInfo(ConvertRequestVM requestVM, InventoryAdjustSourceAction action)
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
