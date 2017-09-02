using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.PO;
using ECCentral.Service.Utility;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.AppService;
using ECCentral.Service.PO.Restful.RequestMsg;
using ECCentral.Service.PO.Restful.ResponseMsg;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.BizEntity.PO.Commission;

namespace ECCentral.Service.PO.Restful
{
    public partial class POService
    {
        /// <summary>
        /// 代收代付结算单查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CollectionPayment/QueryCollectionPaymentList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryCollectionPaymentList(CollectionPaymentFilter request)
        {
            int totalCount = 0;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<ICollectionPaymentQueryDA>.Instance.QueryCollectionPayment(request, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }

        /// <summary>
        /// 加载结算单详细
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CollectionPayment/Load/{sysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public CollectionPaymentInfo Load(string sysNo)
        {
            int getSysNoParam = 0;
            int.TryParse(sysNo, out getSysNoParam);
            CollectionPaymentInfo result = ObjectFactory<CollectionPaymentAppService>.Instance.Load(getSysNoParam);
            return result;
        }

        /// <summary>
        /// 创建代收代付结算单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CollectionPayment/Create", Method = "POST")]
        public CollectionPaymentInfo Create(CollectionPaymentInfo info)
        {
            return ObjectFactory<CollectionPaymentAppService>.Instance.Create(info);
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="consignInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CollectionPayment/Update", Method = "PUT")]
        public void Update(CollectionPaymentInfo consignInfo)
        {
            ObjectFactory<CollectionPaymentAppService>.Instance.Update(consignInfo);
        }

        /// <summary>
        /// 作废代收代付结算单
        /// </summary>
        /// <param name="consignInfo"></param>
        [WebInvoke(UriTemplate = "/CollectionPayment/Abandon", Method = "PUT")]
        public void Abandon(CollectionPaymentInfo consignInfo)
        {
            ObjectFactory<CollectionPaymentAppService>.Instance.Abandon(consignInfo);
        }
        /// <summary>
        /// 作废代收代付结算单
        /// </summary>
        /// <param name="consignInfo"></param>
        [WebInvoke(UriTemplate = "/CollectionPayment/CancelAbandon", Method = "PUT")]
        public void CancelAbandon(CollectionPaymentInfo consignInfo)
        {
            ObjectFactory<CollectionPaymentAppService>.Instance.CancelAbandon(consignInfo);
        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="consignInfo"></param>
        [WebInvoke(UriTemplate = "/CollectionPayment/Audit", Method = "PUT")]
        public void Audit(CollectionPaymentInfo consignInfo)
        {
            ObjectFactory<CollectionPaymentAppService>.Instance.Audit(consignInfo);
        }
        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="consignInfo"></param>
        [WebInvoke(UriTemplate = "/CollectionPayment/CancelAudited", Method = "PUT")]
        public void CancelAudited(CollectionPaymentInfo consignInfo)
        {
            ObjectFactory<CollectionPaymentAppService>.Instance.CancelAudited(consignInfo);
        }
        /// <summary>
        /// 结算
        /// </summary>
        /// <param name="consignInfo"></param>
        [WebInvoke(UriTemplate = "/CollectionPayment/Settle", Method = "PUT")]
        public void Settle(CollectionPaymentInfo consignInfo)
        {
            ObjectFactory<CollectionPaymentAppService>.Instance.Settle(consignInfo);
        }
        /// <summary>
        /// 取消结算
        /// </summary>
        /// <param name="consignInfo"></param>
        [WebInvoke(UriTemplate = "/CollectionPayment/CancelSettled", Method = "PUT")]
        public void CancelSettled(CollectionPaymentInfo consignInfo)
        {
            ObjectFactory<CollectionPaymentAppService>.Instance.CancelSettled(consignInfo);
        }

        /*
        /// <summary>
        /// 根据不同权限获取PMList:
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConsignSettlement/GetPMSysNoListByType", Method = "POST")]
        public List<int> GetPMSysNoListByType(ConsignSettlementBizInfo info)
        {
            return ObjectFactory<ConsignSettlementAppService>.Instance.GetPMSysNoListByType(info);
        }

        */

        [WebInvoke(UriTemplate = "/Batch/POBatchInstock", Method = "POST")]
        public void POBatchInstock(POBatchInfo info)
        {
            ObjectFactory<CollectionPaymentAppService>.Instance.POBatchInstock(info);
        }
    }
}
