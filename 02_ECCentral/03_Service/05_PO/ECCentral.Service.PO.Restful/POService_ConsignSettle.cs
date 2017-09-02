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
using ECCentral.BizEntity.PO.Settlement;

namespace ECCentral.Service.PO.Restful
{
    public partial class POService
    {
        /// <summary>
        /// 代销结算单查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConsignSettlement/QueryConsignSettlementList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryConsignSettlementList(ConsignSettleQueryFilter request)
        {
            int totalCount = 0;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<IConsignSettleQueryDA>.Instance.QueryConsignSettlement(request, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }

        /// <summary>
        /// TODO:查询返点列表(调用 EIMS接口)
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConsignSettlement/QueryConsignSettlmentEIMSList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public CosignSettlementEIMSQueryrRsp QueryConsignSettlmentEIMSList(ConsignSettlementEIMSQueryRsq request)
        {
            CosignSettlementEIMSQueryrRsp rep = new CosignSettlementEIMSQueryrRsp();
            int totalCount = 0;
            rep.ResultList = ObjectFactory<ConsignSettlementAppService>.Instance.LoadConsignEIMSList(request.queryCondition, request.PageInfo.PageIndex, request.PageInfo.PageSize, request.PageInfo.SortBy, out totalCount);
            rep.TotalCount = totalCount;
            return rep;
        }

        /// <summary>
        /// 加载代销结算单详细
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConsignSettlement/LoadConsignSettlement/{sysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public ConsignSettlementInfo LoadConsignSettlement(string sysNo)
        {
            int getSysNoParam = 0;
            int.TryParse(sysNo, out getSysNoParam);
            return ObjectFactory<ConsignSettlementAppService>.Instance.LoadConsignSettlementInfoBySysNo(getSysNoParam);
        }

        /// <summary>
        /// 更新代销结算单
        /// </summary>
        /// <param name="consignInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConsignSettlement/UpdateConsignSettlement", Method = "PUT")]
        public void UpdateConsignSettlementInfo(ConsignSettlementInfo consignInfo)
        {
            ObjectFactory<ConsignSettlementAppService>.Instance.UpdateConsignSettlementInfo(consignInfo);
        }

        /// <summary>
        /// 作废代销结算单
        /// </summary>
        /// <param name="consignInfo"></param>
        [WebInvoke(UriTemplate = "/ConsignSettlement/AbandonConsignSettlement", Method = "PUT")]
        public void AbandonConsignSettmentInfo(ConsignSettlementInfo consignInfo)
        {
            ObjectFactory<ConsignSettlementAppService>.Instance.AbandonConsignSettlementInfo(consignInfo);
        }

        /// <summary>
        /// 取消作废代销结算单
        /// </summary>
        /// <param name="consignInfo"></param>
        [WebInvoke(UriTemplate = "/ConsignSettlement/CancelAbandonConsignSettlement", Method = "PUT")]
        public void CancelAbandonConsignSettlement(ConsignSettlementInfo consignInfo)
        {
            ObjectFactory<ConsignSettlementAppService>.Instance.CancelAbandonSettlementInfo(consignInfo);
        }

        /// <summary>
        /// 代销结算单 - 操作
        /// </summary>
        /// <param name="consignInfo"></param>
        [WebInvoke(UriTemplate = "/ConsignSettlement/SettleConsignSettlement", Method = "PUT")]
        public void SettleConsignSettlement(ConsignSettlementInfo consignInfo)
        {
            ObjectFactory<ConsignSettlementAppService>.Instance.SettleConsignSettlementInfo(consignInfo);
        }

        /// <summary>
        /// 代销结算单 - 取消结算操作
        /// </summary>
        /// <param name="consignInfo"></param>
        [WebInvoke(UriTemplate = "/ConsignSettlement/CancelSettleConsignSettlement", Method = "PUT")]
        public void CancelSettleConsignSettlement(ConsignSettlementInfo consignInfo)
        {
            ObjectFactory<ConsignSettlementAppService>.Instance.CancelSettleConsignSettlement(consignInfo);
        }

        /// <summary>
        /// 审核代销结算单
        /// </summary>
        /// <param name="consignInfo"></param>
        [WebInvoke(UriTemplate = "/ConsignSettlement/AuditConsignSettlement", Method = "PUT")]
        public void AuditConsignSettlement(ConsignSettlementInfo consignInfo)
        {
            ObjectFactory<ConsignSettlementAppService>.Instance.AuditConsignSettlement(consignInfo);
        }

        /// <summary>
        /// 取消审核 - 代销结算单
        /// </summary>
        /// <param name="consignInfo"></param>
        [WebInvoke(UriTemplate = "/ConsignSettlement/CancelAuditConsignSettlement", Method = "PUT")]
        public void CancelAuditConsignSettlement(ConsignSettlementInfo consignInfo)
        {
            ObjectFactory<ConsignSettlementAppService>.Instance.CancelAuditConsignSettlement(consignInfo);
        }

        /// <summary>
        /// 查询代销结算单商品(新建)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConsignSettlement/GetConsignSettlmentProductList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetConsignSettlmentProductList(ConsignSettlementProductsQueryFilter filter)
        {
            QueryResult rsp = new QueryResult();
            int totalCount = 0;
            rsp.Data = ObjectFactory<IConsignSettleQueryDA>.Instance.GetConsignSettlmentProductList(filter, out totalCount);
            rsp.TotalCount = totalCount;
            return rsp;
        }

        /// <summary>
        /// 创建代销结算单
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConsignSettlement/CreateConsignSettlement", Method = "POST")]
        public ConsignSettlementInfo CreateConsignSettlement(ConsignSettlementInfo info)
        {
            return ObjectFactory<ConsignSettlementAppService>.Instance.CreateConsignSettlementInfo(info);
        }

        [WebInvoke(UriTemplate = "/ConsignSettlement/CreateConsignSettlementBySystem", Method = "POST")]
        public ConsignSettlementInfo CreateConsignSettlementBySystem(ConsignSettlementInfo info)
        {
            return ObjectFactory<ConsignSettlementAppService>.Instance.CreateConsignSettlementInfoBySystem(info);
        }

        /// <summary>
        /// 自动创建代销结算单(Job)
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConsignSettlement/SystemCreateConsignSettlement", Method = "PUT")]
        public ConsignSettlementInfo SystemCreateConsignSettlement(ConsignSettlementInfo info)
        {
            return ObjectFactory<ConsignSettlementAppService>.Instance.SystemCreateConsignSettlementInfo(info);
        }

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

        #region 经销商品结算单

        /// <summary>
        /// 查询未结算的 进货单、返厂单、进价变价单
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConsignSettlement/QuerySettleAccountWithOrigin", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QuerySettleAccountWithOrigin(SettleOrderCreateQueryFilter filter)
        {
            QueryResult rsp = new QueryResult();
            int totalCount = 0;
            rsp.Data = ObjectFactory<IConsignSettleQueryDA>.Instance.QuerySettleAccountWithOrigin(filter, out totalCount);
            rsp.TotalCount = totalCount;
            return rsp;
        }

        /// <summary>
        /// 创建经销商品结算单
        /// </summary>
        /// <param name="SettleInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConsignSettlement/CreateSettleAccountBil", Method = "POST")]
        public SettleInfo CreateSettleAccount(SettleInfo SettleInfo)
        {
            return ObjectFactory<ConsignSettlementAppService>.Instance.CreateSettleAccount(SettleInfo);
        }

        /// <summary>
        /// 查询经销商品详细信息(基本信息和个子项税率信息)
        /// </summary>
        /// <param name="SettleInfo"></param>
        /// <returns></returns>

        [WebInvoke(UriTemplate = "/ConsignSettlement/GetSettleAccountBil", Method = "POST")]
        public SettleInfo GetSettleAccount(SettleInfo SettleInfo)
        {
            return ObjectFactory<ConsignSettlementAppService>.Instance.GetSettleAccount(SettleInfo);
        }

        /// <summary>
        /// 审核经销商品结算单
        /// </summary>
        /// <param name="SettleInfo"></param>
        [WebInvoke(UriTemplate = "/ConsignSettlement/AuditSettleAccount", Method = "POST")]
        public void AuditSettleAccount(SettleInfo SettleInfo)
        {
            ObjectFactory<ConsignSettlementAppService>.Instance.AuditSettleAccount(SettleInfo);
        }
        
        #endregion

    }
}
