using System;
using System.Collections.Generic;

using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.Restful.RequestMsg;
using ECCentral.Portal.UI.MKT.Models.Promotion;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class ComboFacade
    {
        private readonly RestClient restClient;
        private readonly IPage viewPage;

        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            }
        }

        public ComboFacade(IPage page)
        {
            viewPage = page;
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(ComboQueryReqVM vm, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ComboQueryFilter filter = vm.ConvertVM<ComboQueryReqVM, ComboQueryFilter>();
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            filter.PM = null;
            filter.PagingInfo = new QueryFilter.Common.PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/Combo/Query";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void Load(int sysNo, EventHandler<RestClientEventArgs<ComboVM>> callback)
        {
            string relativeUrl = string.Format("/MKTService/Combo/{0}", sysNo);
            restClient.Query<ComboInfo>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var vm = args.Result.Convert<ComboInfo, ComboVM>();
                vm.Name = args.Result.Name.Content;
                RestClientEventArgs<ComboVM> e = new RestClientEventArgs<ComboVM>(vm, this.viewPage);
                callback(obj, e);
            });
        }

        public void Create(ComboVM model, EventHandler<RestClientEventArgs<int?>> callback)
        {
            string relativeUrl = "/MKTService/Combo/Create";
            ComboInfo msg = model.ConvertVM<ComboVM, ComboInfo>();
            msg.Name.Content = model.Name;
            msg.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Create<int?>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                model.SysNo = args.Result;
                callback(obj, args);
            });
        }

        public void BatchCreate(ComboBatchVM model, EventHandler<RestClientEventArgs<List<ComboInfo>>> callback)
        {
            string relativeUrl = "/MKTService/Combo/BatchCreate";
            ComboBatchReq msg = model.ConvertVM<ComboBatchVM, ComboBatchReq>();
            msg.Name.Content = model.Name;
            msg.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Create<List<ComboInfo>>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void Update(ComboVM model, EventHandler<RestClientEventArgs<ComboVM>> callback)
        {
            string relativeUrl = "/MKTService/Combo/Update";
            ComboInfo msg = model.ConvertVM<ComboVM, ComboInfo>();
            msg.Name.Content = model.Name;
            restClient.Update<ComboInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var vm = args.Result.Convert<ComboInfo, ComboVM>();
                RestClientEventArgs<ComboVM> e = new RestClientEventArgs<ComboVM>(vm, this.viewPage);
                callback(obj, e);
            });
        }

        public void BatchUpdate(ComboBatchVM model, EventHandler<RestClientEventArgs<List<ComboInfo>>> callback)
        {
            string relativeUrl = "/MKTService/Combo/BatchUpdate";
            ComboBatchReq msg = model.ConvertVM<ComboBatchVM, ComboBatchReq>();
            msg.Name.Content = model.Name;
            msg.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Update<List<ComboInfo>>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void CheckComboItemIsPass(ComboVM model, EventHandler<RestClientEventArgs<List<string>>> callback)
        {
            string relativeUrl = "/MKTService/Combo/CheckComboItemIsPass";
            ComboInfo msg = model.ConvertVM<ComboVM, ComboInfo>();
            msg.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Create<List<string>>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void CheckOptionalAccessoriesItemAndDiys(List<int> sysNos, EventHandler<RestClientEventArgs<List<string>>> callback)
        {
            string relativeUrl = "/MKTService/Combo/CheckOptionalAccessoriesItemAndDiys";
            restClient.Create<List<string>>(relativeUrl, sysNos, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void AuditPass(int sysNo, bool isPass, EventHandler<RestClientEventArgs<ComboInfo>> callback)
        {
            string relativeUrl = "/MKTService/Combo/ApproveCombo";
            ComboUpdateStatusReq msg = new ComboUpdateStatusReq
            {
                SysNo = sysNo,
                TargetStatus = isPass ? ComboStatus.Active : ComboStatus.Deactive
            };
            restClient.Update<ComboInfo>(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void BatchDelete(List<int> sysNoList, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/MKTService/Combo/BatchDelete";
            restClient.Update<object>(relativeUrl, sysNoList, (obj, args) =>
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