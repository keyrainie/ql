using System;
using System.Linq;
using System.Collections.Generic;

using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.QueryFilter.RMA;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.Restful.ResponseMsg;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.RMA.Facades
{
    public class RequestFacade
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

        public RequestFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public RequestFacade(IPage page)
        {
            viewPage = page;
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(RequestQueryReqVM vm, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {            
            RMARequestQueryFilter filter = vm.ConvertVM<RequestQueryReqVM, RMARequestQueryFilter>();
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            filter.PagingInfo = new QueryFilter.Common.PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/RMAService/Request/Query";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void GetAllReceiveUsers(EventHandler<RestClientEventArgs<List<UserInfo>>> callback)
        {
            string relativeUrl = "/RMAService/Request/GetAllReceiveUsers";
            restClient.Query<List<UserInfo>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args); 
            });
        }

        public void GetAllConfirmUsers(EventHandler<RestClientEventArgs<List<UserInfo>>> callback)
        {
            string relativeUrl = "/RMAService/Request/GetAllConfirmUsers";
            restClient.Query<List<UserInfo>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void LoadBySysNo(int sysNo, EventHandler<RestClientEventArgs<RequestVM>> callback)
        {
            string relativeUrl = string.Format("/RMAService/Request/Load/{0}", sysNo);
            restClient.Query<RequestDetailInfoRsp>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                var vm = args.Result.RequestInfo.Convert<RMARequestInfo, RequestVM>((s, t) =>
                {
                    t.CustomerID = args.Result.CustomerID;
                    t.CustomerName = args.Result.CustomerName;
                    t.SOID = args.Result.SOID;                                        
                    //t.ReceiveUserName = s.ReceiveUserInfo.UserName;
                    t.BusinessModel = args.Result.BusinessModel;
                    if (string.IsNullOrEmpty(s.ReceiveWarehouse))
                    {
                        //默认显示上海仓
                        t.ReceiveWarehouse = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("RMA", "DefaultStockID"); //"51";
                    }                   
                });

                RestClientEventArgs<RequestVM> arg = new RestClientEventArgs<RequestVM>(vm, viewPage); 

                callback(obj, arg);
            });
        }

        public void Create(RequestVM vm, EventHandler<RestClientEventArgs<RMARequestInfo>> callback)
        {
            string relativeUrl = "/RMAService/Request/Create";
            var msg = vm.ConvertVM<RequestVM, RMARequestInfo>();
            msg.CompanyCode = CPApplication.Current.CompanyCode;
            msg.NeedVerifyDuplicate = !vm.IsCancelVerifyDuplicate;   
            
            restClient.Create<RMARequestInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void Update(RequestVM vm, EventHandler<RestClientEventArgs<RMARequestInfo>> callback)
        {
            string relativeUrl = "/RMAService/Request/Update";
            var msg = vm.ConvertVM<RequestVM, RMARequestInfo>();            
            restClient.Update<RMARequestInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void Receive(RequestVM vm, EventHandler<RestClientEventArgs<RMARequestInfo>> callback)
        {
            string relativeUrl = "/RMAService/Request/Receive";
            var msg = vm.ConvertVM<RequestVM, RMARequestInfo>();            
            restClient.Update<RMARequestInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void CancelReceive(RequestVM vm, EventHandler<RestClientEventArgs<RMARequestInfo>> callback)
        {
            string relativeUrl = "/RMAService/Request/CancelReceive";
            var msg = vm.ConvertVM<RequestVM, RMARequestInfo>();            
            restClient.Update<RMARequestInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void Close(RequestVM vm, EventHandler<RestClientEventArgs<RMARequestInfo>> callback)
        {
            string relativeUrl = "/RMAService/Request/Close";
            var msg = vm.ConvertVM<RequestVM, RMARequestInfo>();            
            restClient.Update<RMARequestInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void Abandon(RequestVM vm, EventHandler<RestClientEventArgs<RMARequestInfo>> callback)
        {
            string relativeUrl = "/RMAService/Request/Abandon";
            var msg = vm.ConvertVM<RequestVM, RMARequestInfo>();            
            restClient.Update<RMARequestInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void PrintLabels(RequestVM vm, EventHandler<RestClientEventArgs<RMARequestInfo>> callback)
        {
            string relativeUrl = "/RMAService/Request/PrintLabels";
            var msg = vm.ConvertVM<RequestVM, RMARequestInfo>();            
            restClient.Update<RMARequestInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void AuditPassed(RequestVM vm, EventHandler<RestClientEventArgs<RMARequestInfo>> callback)
        {
            string relativeUrl = "/RMAService/Request/Audit";
            var msg = vm.ConvertVM<RequestVM, RMARequestInfo>();
            restClient.Update<RMARequestInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }


        public void AuditRefused(RequestVM vm, EventHandler<RestClientEventArgs<RMARequestInfo>> callback)
        {
            string relativeUrl = "/RMAService/Request/Refused";
            var msg = vm.ConvertVM<RequestVM, RMARequestInfo>();
            restClient.Update<RMARequestInfo>(relativeUrl, msg, (obj, args) =>
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
