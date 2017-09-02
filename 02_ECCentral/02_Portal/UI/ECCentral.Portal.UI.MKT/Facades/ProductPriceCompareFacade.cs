using System;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using ECCentral.Service.MKT.Restful.RequestMsg;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class ProductPriceCompareFacade
    {
        private readonly RestClient restClient;
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        public ProductPriceCompareFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(ProductPriceCompareQueryVM vm, PagingInfo pagingInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = vm.ConvertVM<ProductPriceCompareQueryVM, ProductPriceCompareQueryFilter>();
            data.PageInfo = pagingInfo;
            data.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/MKTService/ProductPriceCompare/Query";
            restClient.QueryDynamicData(relativeUrl, data, callback);
        }

        //价格举报有效
        public void AuditPass(int sysNo, Action cb)
        {
            string relativeUrl = "/MKTService/ProductPriceCompare/AuditPass/" + sysNo.ToString();
            restClient.Update(relativeUrl, null, (s, args) =>
            {
                if (args.FaultsHandle() || cb == null) return;
                cb();
            });

        }

        //价格举报无效
        public void AuditDecline(int sysNo, string commaSeperatedReasonIDs, Action cb)
        {
            ProductPriceCompareInvalidReq req = new ProductPriceCompareInvalidReq();
            req.SysNo = sysNo;
            req.CommaSeperatedReasonCodes = commaSeperatedReasonIDs;

            string relativeUrl = "/MKTService/ProductPriceCompare/AuditDecline";
            restClient.Update(relativeUrl, req, (s, args) =>
            {
                if (args.FaultsHandle() || cb == null) return;
                cb();
            });

        }

        //价格举报恢复
        public void Recover(int sysNo, Action cb)
        {
            string relativeUrl = "/MKTService/ProductPriceCompare/UpdateResetLinkShow/" + sysNo.ToString();
            restClient.Update(relativeUrl, null, (s, args) =>
            {
                if (args.FaultsHandle() || cb == null) return;
                cb();
            });

        }

        public void Load(int sysNo, Action<ProductPriceCompareVM> cb)
        {
            string relativeUrl = "/MKTService/ProductPriceCompare/" + sysNo.ToString();
            restClient.Query<ProductPriceCompareEntity>(relativeUrl, (s, args) =>
            {
                if (args.FaultsHandle() || cb == null) return;
                var result = args.Result.Convert<ProductPriceCompareEntity, ProductPriceCompareVM>();
                cb(result);
            });
        }

        public void GetInvalidReasons(Action<List<InvalidReasonVM>> cb)
        {
            string relativeUrl = "/MKTService/ProductPriceCompare/GetInvalidReasons";
            restClient.Query<List<CodeNamePair>>(relativeUrl, (s, args) =>
            {
                if (args.FaultsHandle() || cb == null) return;
                List<InvalidReasonVM> result = new List<InvalidReasonVM>();
                foreach (var item in args.Result)
                {
                    InvalidReasonVM reason = new InvalidReasonVM();
                    reason.ReasonID = item.Code;
                    reason.ReasonDesc = item.Name;
                    result.Add(reason);
                }
                cb(result);
            });
        }
    }
}
