using System;
using System.Collections.Generic;

using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Portal.UI.RMA.Resources;
using ECCentral.QueryFilter.RMA;
using ECCentral.Service.RMA.Restful.ResponseMsg;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.RMA.Facades
{
    public class RegisterFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_RMA, "ServiceBaseUrl");
            }
        }

        public RegisterFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public RegisterFacade(IPage page)
        {
            viewPage = page;
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(RegisterQueryVM vm, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            RegisterQueryFilter filter = new RegisterQueryFilter();
            if (vm.IsQuickSearch)
            {
                //filter = new RegisterQueryVM().ConvertVM<RegisterQueryVM, RegisterQueryFilter>();
                if (vm.QuickSearchCondition.RecvTimeFromDiffDays.HasValue)
                {
                    filter.RecvTimeFrom = DateTime.Now.AddDays((int)vm.QuickSearchCondition.RecvTimeFromDiffDays);
                    filter.RecvTimeFrom = DateTime.Parse(filter.RecvTimeFrom.Value.ToShortDateString());
                }
                if (vm.QuickSearchCondition.RecvTimeToDiffDays.HasValue)
                {
                    filter.RecvTimeTo = DateTime.Now.AddDays((int)vm.QuickSearchCondition.RecvTimeToDiffDays);
                    filter.RecvTimeTo = DateTime.Parse(filter.RecvTimeTo.Value.ToShortDateString() + " 23:59:59");
                }
                filter.RequestStatus = vm.QuickSearchCondition.RequestStatus;
                filter.IsWithin7Days = vm.QuickSearchCondition.IsWithin7Days;
            }
            else
            {
                filter = vm.ConvertVM<RegisterQueryVM, RegisterQueryFilter>();
            }
            filter.CompanyCode = CPApplication.Current.CompanyCode;

            filter.PagingInfo = new QueryFilter.Common.PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/RMAService/Register/Query";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void LoadBySysNo(int sysNo, EventHandler<RestClientEventArgs<RegisterVM>> callback)
        {
            string relativeUrl = string.Format("/RMAService/Register/Load/{0}", sysNo);
            restClient.Query<RegisterDetailInfoRsp>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                //Convert response msg to VM
                RegisterVM vm = args.Result.Register.Convert<RMARegisterInfo, RegisterVM>((s, t) =>
                {                   
                    t.BasicInfo.SOSysNo = args.Result.SOSysNo;                    
                    t.BasicInfo.ProcessType = args.Result.ProcessType;
                    t.BasicInfo.BusinessModel = args.Result.BusinessModel;
                    t.BasicInfo.RequestSysNo = args.Result.RequestSysNo;
                    t.BasicInfo.CustomerName = args.Result.CustomerName;
                    t.BasicInfo.CustomerRank = args.Result.CustomerRank;
                    t.BasicInfo.ReceiveTime = args.Result.ReceiveTime;
                    t.BasicInfo.InvoiceType = args.Result.InvoiceType;
                    t.BasicInfo.RequestType = s.RequestType;
                    t.ProductInventoryInfo = args.Result.ProductInventoryInfo;
                    t.BasicInfo.RefundSysNo = args.Result.RefundSysNo;
                    t.BasicInfo.InventoryType = args.Result.InventoryType;
                    t.BasicInfo.ERPStatus = args.Result.Register.BasicInfo.ERPStatus;
                });
                
                RestClientEventArgs<RegisterVM> arg = new RestClientEventArgs<RegisterVM>(vm, viewPage);

                callback(obj, arg);
            });
        }

        public void LoadSecondHandProducts(string productID, EventHandler<RestClientEventArgs<List<RegisterSecondHandRspVM>>> callback)
        {
            string relativeUrl = string.Format("/RMAService/Register/LoadSecondHandProducts/{0}", productID);
            restClient.Query<List<RegisterSecondHandRsp>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<RegisterSecondHandRspVM> argsVM = new List<RegisterSecondHandRspVM>();
                //Convert response msg to VM
                foreach (RegisterSecondHandRsp info in args.Result)
                {
                    RegisterSecondHandRspVM vm = info.Convert<RegisterSecondHandRsp, RegisterSecondHandRspVM>((s, t) =>
                        {
                            t.ProductID = s.ProductID;
                            t.SysNo = s.SysNo;
                        });
                    argsVM.Add(vm);
                }
                RestClientEventArgs<List<RegisterSecondHandRspVM>> arg = new RestClientEventArgs<List<RegisterSecondHandRspVM>>(argsVM, viewPage);

                callback(obj, arg);
            });
        }

        public void UpdateBasicInfo(RegisterVM vm, EventHandler<RestClientEventArgs<RMARegisterInfo>> callback)
        {
            string relativeUrl = "/RMAService/Register/UpdateBasicInfo";
            RMARegisterInfo msg = vm.ConvertVM<RegisterVM, RMARegisterInfo>();
            msg.SysNo = vm.BasicInfo.SysNo;            
            restClient.Update<RMARegisterInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void UpdateCheckInfo(RegisterVM vm, EventHandler<RestClientEventArgs<RMARegisterInfo>> callback)
        {
            string relativeUrl = "/RMAService/Register/UpdateCheckInfo";
            RMARegisterInfo msg = vm.ConvertVM<RegisterVM, RMARegisterInfo>();            
            msg.SysNo = vm.CheckInfo.SysNo;
            restClient.Update<RMARegisterInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void UpdateResponseInfo(RegisterVM vm, EventHandler<RestClientEventArgs<RMARegisterInfo>> callback)
        {
            string relativeUrl = "/RMAService/Register/UpdateResponseInfo";            
            RMARegisterInfo msg = vm.ConvertVM<RegisterVM, RMARegisterInfo>();            
            msg.SysNo = vm.ResponseInfo.SysNo;
            restClient.Update<RMARegisterInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void SetWaitingReturn(int sysNo, EventHandler<RestClientEventArgs<RMARegisterInfo>> callback)
        {
            string relativeUrl = "/RMAService/Register/SetWaitingReturn";
            restClient.Update<RMARegisterInfo>(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }                
                callback(obj, args);
            });
        }

        public void CancelWaitingReturn(int sysNo, EventHandler<RestClientEventArgs<RMARegisterInfo>> callback)
        {
            string relativeUrl = "/RMAService/Register/CancelWaitingReturn";
            restClient.Update<RMARegisterInfo>(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void SetWaitingOutbound(int sysNo, EventHandler<RestClientEventArgs<RMARegisterInfo>> callback)
        {
            string relativeUrl = "/RMAService/Register/SetWaitingOutbound";
            restClient.Update<RMARegisterInfo>(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void CancelWaitingOutbound(int sysNo, EventHandler<RestClientEventArgs<RMARegisterInfo>> callback)
        {
            string relativeUrl = "/RMAService/Register/CancelWaitingOutbound";
            restClient.Update<RMARegisterInfo>(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void SetWaitingRefund(int sysNo, EventHandler<RestClientEventArgs<RMARegisterInfo>> callback)
        {
            string relativeUrl = "/RMAService/Register/SetWaitingRefund";
            restClient.Update<RMARegisterInfo>(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void CancelWaitingRefund(int sysNo, EventHandler<RestClientEventArgs<RMARegisterInfo>> callback)
        {
            string relativeUrl = "/RMAService/Register/CancelWaitingRefund";
            restClient.Update<RMARegisterInfo>(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void SetWaitingRevert(RegisterVM vm, EventHandler<RestClientEventArgs<RMARegisterInfo>> callback)
        {
            string relativeUrl = "/RMAService/Register/SetWaitingRevert";
            var msg = vm.ConvertVM<RegisterVM, RMARegisterInfo>();           
            restClient.Update<RMARegisterInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void CancelWaitingRevert(RegisterVM vm, EventHandler<RestClientEventArgs<RMARegisterInfo>> callback)
        {
            string relativeUrl = "/RMAService/Register/CancelWaitingRevert";
            var msg = vm.ConvertVM<RegisterVM, RMARegisterInfo>();            
            restClient.Update<RMARegisterInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void ApproveRevertAudit(RegisterVM vm, EventHandler<RestClientEventArgs<RMARegisterInfo>> callback)
        {
            string relativeUrl = "/RMAService/Register/ApproveRevertAudit";
            RMARegisterInfo msg = vm.ConvertVM<RegisterVM, RMARegisterInfo>();            
            restClient.Update<RMARegisterInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void RejectRevertAudit(RegisterVM vm, EventHandler<RestClientEventArgs<RMARegisterInfo>> callback)
        {
            string relativeUrl = "/RMAService/Register/RejectRevertAudit";
            RMARegisterInfo msg = vm.ConvertVM<RegisterVM, RMARegisterInfo>();            
            restClient.Update<RMARegisterInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void Close(int sysNo, EventHandler<RestClientEventArgs<RMARegisterInfo>> callback)
        {
            string relativeUrl = "/RMAService/Register/Close";
            restClient.Update<RMARegisterInfo>(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        /// <summary>
        /// 商家关闭单件
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void CloseCase(int sysNo, EventHandler<RestClientEventArgs<RMARegisterInfo>> callback)
        {
            string relativeUrl = "/RMAService/Register/CloseCase";
            restClient.Update<RMARegisterInfo>(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }        
        
        public void ReOpen(int sysNo, EventHandler<RestClientEventArgs<RMARegisterInfo>> callback)
        {
            string relativeUrl = "/RMAService/Register/ReOpen";
            restClient.Update<RMARegisterInfo>(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void Abandon(int sysNo, EventHandler<RestClientEventArgs<RMARegisterInfo>> callback)
        {
            string relativeUrl = "/RMAService/Register/Abandon";
            restClient.Update<RMARegisterInfo>(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void LoadRegisterDunLog(int registerSysNo, EventHandler<RestClientEventArgs<List<RegisterDunLog>>> callback)
        {
            string relativeUrl = string.Format("/RMAService/Register/LoadRegisterDunLog/{0}", registerSysNo);
            restClient.Query<List<RegisterDunLog>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }


        public void ExportRegisterExcelFile(RegisterQueryVM vm, ColumnSet[] columns)
        {
            RegisterQueryFilter queryFilter = new RegisterQueryFilter();
            if (vm.IsQuickSearch)
            {
                if (vm.QuickSearchCondition.RecvTimeFromDiffDays.HasValue)
                {
                    queryFilter.RecvTimeFrom = DateTime.Now.AddDays((int)vm.QuickSearchCondition.RecvTimeFromDiffDays);
                }
                if (vm.QuickSearchCondition.RecvTimeToDiffDays.HasValue)
                {
                    queryFilter.RecvTimeTo = DateTime.Now.AddDays((int)vm.QuickSearchCondition.RecvTimeToDiffDays);
                }
                queryFilter.RequestStatus = vm.QuickSearchCondition.RequestStatus;
                queryFilter.IsWithin7Days = vm.QuickSearchCondition.IsWithin7Days;
            }
            else
            {
                queryFilter = vm.ConvertVM<RegisterQueryVM, RegisterQueryFilter>();
            }
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;

            queryFilter.PagingInfo = new QueryFilter.Common.PagingInfo
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            string relativeUrl = "/RMAService/Register/Query";

            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }


        public void LoadRegisterMemo(int registerSysNo, EventHandler<RestClientEventArgs<RegisterMemoRsp>> callback)
        {
            string relativeUrl = string.Format("/RMAService/Register/LoadRegisterMemo/{0}", registerSysNo);
            restClient.Query<RegisterMemoRsp>(relativeUrl, callback);
        }

        public void SyncERP(int sysNo, EventHandler<RestClientEventArgs<RMARegisterInfo>> callback)
        {
            string relativeUrl = "/RMAService/Register/SyncERP";
            restClient.Update<RMARegisterInfo>(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

    }
}
