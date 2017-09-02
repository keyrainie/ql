using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.AppService;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using ECCentral.Service.Invoice.Restful.ResponseMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        /// <summary>
        /// 创建网上支付信息
        /// </summary>
        [WebInvoke(UriTemplate = "/NetPay/Create", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public NetPayInfo CreateNetPay(CheckNetPayReq request)
        {
            NetPayInfo netPayInfo = null;
            SOIncomeRefundInfo refundInfo = null;

            request.Convert(out netPayInfo, out refundInfo);
            return ObjectFactory<NetPayAppService>.Instance.Create(netPayInfo, refundInfo, request.IsForceCheck.Value);
        }

        /// <summary>
        /// 批量审核网上支付记录
        /// </summary>
        /// <param name="netpaySysNoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/NetPay/BatchAudit", Method = "PUT")]
        public string BatchAuditNetPay(List<int> netpaySysNoList)
        {
            return ObjectFactory<NetPayAppService>.Instance.BatchAudit(netpaySysNoList);
        }

        /// <summary>
        /// 作废网上支付
        /// </summary>
        [WebInvoke(UriTemplate = "/NetPay/Abandon", Method = "PUT")]
        public void AbandonNetPay(int netpaySysNo)
        {
            ObjectFactory<NetPayAppService>.Instance.Abandon(netpaySysNo);
        }

        /// <summary>
        /// 核对网上支付时加载网上支付信息
        /// </summary>
        /// <param name="netpaySysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/NetPay/Load/{netpaySysNo}", Method = "GET")]
        public NetPayResp LoadNetPayForAudit(string netpaySysNo)
        {
            NetPayInfo netpayInfo;
            SOIncomeRefundInfo refundInfo;
            SOBaseInfo soBaseInfo;

            netpayInfo = ObjectFactory<NetPayAppService>.Instance.LoadForAudit(int.Parse(netpaySysNo), out refundInfo, out soBaseInfo);
            NetPayResp resp = new NetPayResp();
            resp.NetPay = netpayInfo;
            resp.Refund = refundInfo;
            resp.SOBaseInfo = soBaseInfo;

            return resp;
        }

        #region NoBizQuery

        [WebInvoke(UriTemplate = "/NetPay/Query", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryNetPay(NetPayQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<INetPayQueryDA>.Instance.Query(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        #endregion NoBizQuery
    }
}