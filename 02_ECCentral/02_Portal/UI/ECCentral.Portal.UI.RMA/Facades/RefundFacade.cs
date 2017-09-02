using System;
using System.Collections.Generic;

using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.QueryFilter.RMA;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.Restful.ResponseMsg;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.QueryFilter.Common;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.RMA.Facades
{
    public class RefundFacade
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

        public RefundFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public RefundFacade(IPage page)
        {
            viewPage = page;
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(RefundQueryReqVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            RefundQueryFilter filter = model.ConvertVM<RefundQueryReqVM, RefundQueryFilter>();
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            filter.PagingInfo = new QueryFilter.Common.PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            if (filter.CreateTimeTo != null)
            {
                filter.CreateTimeTo = filter.CreateTimeTo.Value.Date.AddDays(1);
            }
            if (filter.RefundTimeTo != null)
            {
                filter.RefundTimeTo = filter.RefundTimeTo.Value.Date.AddDays(1);
            }
            string relativeUrl = "/RMAService/Refund/Query";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void GetWaitingSOForRefund(EventHandler<RestClientEventArgs<List<int?>>> callback)
        {
            string relativeUrl = "/RMAService/Refund/GetWaitingSOForRefund";
            restClient.Query<List<int?>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void GetWaitingRegisters(int? soSysNo,EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/RMAService/Refund/GetWaitingRegisters";
            restClient.QueryDynamicData(relativeUrl, soSysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void Create(RefundVM model, EventHandler<RestClientEventArgs<RefundInfo>> callback)
        {
            string relativeUrl = "/RMAService/Refund/Create";
            RefundInfo msg = ConvertVM(model);
            msg.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Create<RefundInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void Update(RefundVM model, EventHandler<RestClientEventArgs<RefundInfo>> callback)
        {
            string relativeUrl = "/RMAService/Refund/Update";
            RefundInfo msg = ConvertVM(model);            
            restClient.Update<RefundInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void Calculate(RefundVM model, EventHandler<RestClientEventArgs<RefundInfo>> callback)
        {
            string relativeUrl = "/RMAService/Refund/Calculate";
            RefundInfo msg = ConvertVM(model);
            msg.CompanyCode = CPApplication.Current.CompanyCode;       
            restClient.Update<RefundInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void UpdateFinanceNote(RefundVM model, EventHandler<RestClientEventArgs<RefundInfo>> callback)
        {
            string relativeUrl = "/RMAService/Refund/UpdateFinanceNote";
            RefundInfo msg = ConvertVM(model);            
            restClient.Update<RefundInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void SubmitAudit(RefundVM model, EventHandler<RestClientEventArgs<RefundInfo>> callback)
        {
            string relativeUrl = "/RMAService/Refund/SubmitAudit";
            RefundInfo msg = ConvertVM(model);            
            restClient.Update<RefundInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void CancelSubmitAudit(RefundVM model, EventHandler<RestClientEventArgs<RefundInfo>> callback)
        {
            string relativeUrl = "/RMAService/Refund/CancelSubmitAudit";
            RefundInfo msg = ConvertVM(model);            
            restClient.Update<RefundInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void Refund(int sysNo, EventHandler<RestClientEventArgs<RefundInfo>> callback)
        {
            string relativeUrl = "/RMAService/Refund/Refund";            
            restClient.Update<RefundInfo>(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }        

        public void Abandon(int sysNo, EventHandler<RestClientEventArgs<RefundInfo>> callback)
        {
            string relativeUrl = "/RMAService/Refund/Abandon";          
            restClient.Update<RefundInfo>(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void GetRefundReaons(EventHandler<RestClientEventArgs<List<CodeNamePair>>> callback)
        {
            string relativeUrl = "/RMAService/Refund/GetRefundReaons";            
            restClient.Query<List<CodeNamePair>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void Load(int sysNo, EventHandler<RestClientEventArgs<RefundVM>> callback)
        {
            string relativeUrl = string.Format("/RMAService/Refund/Load/{0}", sysNo);

            restClient.Query<RefundDetailInfoRsp>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                
                var vm = args.Result.RefundInfo.Convert<RefundInfo, RefundVM>((s, t) =>
                {
                    t.CustomerName = args.Result.CustomerName;
                    t.IsRelateCash = t.CashFlag == CashFlagStatus.Yes ? true : false;
                    if (s.IncomeBankInfo != null)
                    {
                        t.BankName = s.IncomeBankInfo.BankName;
                        t.BranchBankName = s.IncomeBankInfo.BranchBankName;
                        t.CardNumber = s.IncomeBankInfo.CardNumber;
                        t.CardOwnerName = s.IncomeBankInfo.CardOwnerName;
                        t.PostAddress = s.IncomeBankInfo.PostAddress;
                        t.PostCode = s.IncomeBankInfo.PostCode;
                        t.ReceiverName = s.IncomeBankInfo.ReceiverName;
                        t.IncomeNote = s.IncomeBankInfo.Note;
                    }
                    var contactInfo = args.Result.CustomerContact;
                    if (contactInfo != null)
                    {
                        t.BankName = contactInfo.BranchBankName;
                        t.BranchBankName = contactInfo.BranchBankName;
                        t.CardNumber = contactInfo.CardNumber;
                        t.CardOwnerName = contactInfo.CardOwnerName;
                    }                    
                });                

                RestClientEventArgs<RefundVM> arg = new RestClientEventArgs<RefundVM>(vm, viewPage);

                callback(obj, arg);
            });
        }

        public void GetShipFee(RefundVM vm, EventHandler<RestClientEventArgs<ShippingFeeRsp>> callback)
        {
            string relativeUrl = "/RMAService/Refund/GetShipFee";
            var msg = ConvertVM(vm);
            restClient.Query<ShippingFeeRsp>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                RestClientEventArgs<ShippingFeeRsp> arg = new RestClientEventArgs<ShippingFeeRsp>(args.Result, viewPage);

                callback(obj, arg);
            });
        }

        public void ExportExcelFile(RefundQueryReqVM vm, ColumnSet[] columns)
        {
            RefundQueryFilter filter = vm.ConvertVM<RefundQueryReqVM, RefundQueryFilter>();

            filter.PagingInfo = new PagingInfo
            {
                PageSize = ConstValue.MaxRowCountLimit, 
                PageIndex = 0,
                SortBy = string.Empty
            };
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/RMAService/Refund/Query";

            restClient.ExportFile(relativeUrl, filter, columns);
        }

        private RefundInfo ConvertVM(RefundVM vm)
        {
            RefundInfo refund = vm.ConvertVM<RefundVM, RefundInfo>((s, t) =>
            {
                t.IncomeBankInfo.BankName = s.BankName;
                t.IncomeBankInfo.BranchBankName = s.BranchBankName;
                t.IncomeBankInfo.CardNumber = s.CardNumber;
                t.IncomeBankInfo.CardOwnerName = s.CardOwnerName;
                t.IncomeBankInfo.PostAddress = s.PostAddress;
                t.IncomeBankInfo.PostCode = s.PostCode;
                t.IncomeBankInfo.ReceiverName = s.ReceiverName;
                t.IncomeBankInfo.Note = s.IncomeNote;
                t.IncomeBankInfo.RefundReason = s.RefundReason;
            });
            return refund;
        }        
    }
}
