using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.SO;
using ECCentral.Service.SO.AppService;
using ECCentral.Service.SO.IDataAccess.NoBizQuery;
using ECCentral.Service.SO.Restful.RequestMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice.Invoice;

namespace ECCentral.Service.SO.Restful
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    public partial class SOService
    {
        #region No Biz Query

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="filter">查询订单条件</param>
        /// <returns>查询的订单数据表</returns>
        [WebInvoke(UriTemplate = "/SO/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QuerySO(SORequestQueryFilter filter)
        {
            return QueryList<SORequestQueryFilter>(filter, ObjectFactory<ISOQueryDA>.Instance.Query);
        }

        /// <summary>
        /// 第三方订单查询
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <returns>查询的订单数据表</returns>
        [WebInvoke(UriTemplate = "/SO/QueryThirdPartSOSearch", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryThirdPartSOSearch(SOThirdPartSOSearchFilter filter)
        {
            return QueryList<SOThirdPartSOSearchFilter>(filter, ObjectFactory<ISOQueryDA>.Instance.ThirdPartSOSearchQuery);
        }

        /// <summary>
        /// 查询发票修改日志
        /// </summary>
        /// <param name="filter">过滤条件集合</param>
        /// <returns>查询结果</returns>
        [WebInvoke(UriTemplate = "/SO/QueryInvoiceChangeLog", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QuerySOInvoiceChangeLog(SOInvoiceChangeLogQueryFilter filter)
        {
            return QueryList<SOInvoiceChangeLogQueryFilter>(filter, ObjectFactory<ISOQueryDA>.Instance.InvoiceChangeLogQuery);
        }

        /// <summary>
        /// 查询改单列表
        /// </summary>
        /// <param name="filter">过滤条件集合</param>
        /// <returns>查询结果</returns>
        [WebInvoke(UriTemplate = "/SO/QueryPending", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryPending(SOPendingQueryFilter filter)
        {
            return QueryList<SOPendingQueryFilter>(filter, ObjectFactory<ISOQueryDA>.Instance.PendingListQuery);
        }

        /// <summary>
        /// 查询OPC
        /// </summary>
        /// <param name="filter">过滤条件集合</param>
        /// <returns>查询结果</returns>
        [WebInvoke(UriTemplate = "/SO/QueryOPC", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryOPC(OPCQueryFilter filter)
        {
            return QueryList<OPCQueryFilter>(filter, ObjectFactory<ISOQueryDA>.Instance.OPCOfflineMasterQuery);
        }

        /// <summary>
        /// 查询订单跟进日志
        /// </summary>
        /// <param name="filter">过滤条件集合</param>
        /// <returns>查询结果</returns>
        [WebInvoke(UriTemplate = "/SO/QueryInternalMemo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryInternalMemo(SOInternalMemoQueryFilter filter)
        {
            QueryResult result = new QueryResult();
            int dataCount = 0;
            result.Data = ObjectFactory<ISOQueryDA>.Instance.InternalMemoQuery(filter, out dataCount);
            var changer = ObjectFactory<SOInternalMemoAppService>.Instance;
            foreach (DataRow row in result.Data.Rows)
            {
                object reasonCodeSysNo = row["ReasonCodeSysNo"];
                if (reasonCodeSysNo is DBNull) continue;
                row["ReasonCodePath"] = changer.GetReasonCodePath(Convert.ToInt32(reasonCodeSysNo), filter.CompanyCode);
            }
            result.TotalCount = dataCount;
            return result;
        }

        /// <summary>
        /// 查询系统日志
        /// </summary>
        /// <param name="filter">过滤条件集合</param>
        /// <returns>查询结果</returns>
        [WebInvoke(UriTemplate = "/SO/QuerySystemLog", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QuerySOSystemLog(SOLogQueryFilter filter)
        {
            return QueryList<SOLogQueryFilter>(filter, ObjectFactory<ISOQueryDA>.Instance.LogQuery);
        }

        [WebInvoke(UriTemplate = "/SO/WHSOOutStockQuery", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryWHSOOutStock(WHSOOutStockQueryFilter filter)
        {
            return QueryList<WHSOOutStockQueryFilter>(filter, ObjectFactory<ISOQueryDA>.Instance.WHSOOutStockQuery);
        }

        /// <summary>
        /// 查询配送历史
        /// </summary>
        /// <param name="filter">查询配送历史过滤条件</param>
        /// <returns>配送历史查询结果</returns>
        [WebInvoke(UriTemplate = "/SO/QueryDeliveryHistory", Method = "POST")]
        public QueryResult GetDeliveryHistoryList(SODeliveryHistoryQueryFilter filter)
        {
            return QueryList<SODeliveryHistoryQueryFilter>(filter, ObjectFactory<ISOQueryDA>.Instance.GetDeliveryHistoryList);
        }

        /// <summary>
        /// 查询配送任务
        /// </summary>
        /// <param name="filter">查询配送任务过滤条件</param>
        /// <returns>配送任务查询结果</returns>
        [WebInvoke(UriTemplate = "/SO/QuerySODeliveryAssignTask", Method = "POST")]
        public QueryResult QuerySODeliveryAssignTask(SODeliveryAssignTaskQueryFilter filter)
        {
            return QueryList<SODeliveryAssignTaskQueryFilter>(filter, ObjectFactory<ISOQueryDA>.Instance.GetDeliveryAssignTask);
        }

        /// <summary>
        /// 配送服务评级
        /// </summary>
        /// <param name="filter">配送服务评级查询条件</param>
        /// <returns>配送服务评级查询结果</returns>
        [WebInvoke(UriTemplate = "/SO/QuerySODeliveryScore", Method = "POST")]
        public QueryResult QuerySODeliveryScore(SODeliveryScoreQueryFilter filter)
        {
            return QueryList<SODeliveryScoreQueryFilter>(filter, ObjectFactory<ISOQueryDA>.Instance.GetDeliveryScoreList);
        }

        /// <summary>
        /// 查询特殊订单
        /// </summary>
        /// <param name="filter">查询特殊订单过滤条件</param>
        /// <returns>特殊订单查询结果</returns>
        [WebInvoke(UriTemplate = "/SO/QuerySpecialSO", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QuerySpecialSO(SpecialSOSearchQueryFilter filter)
        {
            return QueryList<SpecialSOSearchQueryFilter>(filter, ObjectFactory<ISOQueryDA>.Instance.SpecialSOQuery);
        }

        /// <summary>
        /// 查询BackOrder订单商品
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns>BackOrder订单商品查询结果</returns>
        [WebInvoke(UriTemplate = "/SO/QuerySOBackOrderItem/{soSysNo}", Method = "GET")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QuerySOBackOrderItem(string soSysNo)
        {
            int sysNo = int.TryParse(soSysNo, out sysNo) ? sysNo : 0;
            QueryResult result = new QueryResult();
            DataTable data = ObjectFactory<ISOQueryDA>.Instance.QueryBackOrderItem(sysNo);
            result.Data = data;
            result.TotalCount = data.Rows.Count;
            return result;
        }

        #region 中蛋定制

        /// <summary>
        /// 查询手动更改仓库信息订单
        /// </summary>
        /// <param name="filter">过滤条件集合</param>
        /// <returns>查询结果</returns>
        [WebInvoke(UriTemplate = "/SO/QueryWHUpdate", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryWHUpdate(SOWHUpdateQueryFilter filter)
        {
            return QueryList<SOWHUpdateQueryFilter>(filter, ObjectFactory<ISOQueryDA>.Instance.WHUpdateQuery);
        }

        /// <summary>
        /// 查询订单拦截信息
        /// </summary>
        /// <param name="filter">订单拦截配置信息查询过滤条件</param>
        /// <returns>订单拦截信息</returns>
        [WebInvoke(UriTemplate = "/SO/QuerySOIntercept", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QuerySOIntercept(SOInterceptQueryFilter filter)
        {
            QueryResult result = new QueryResult();
            int dataCount = 0;
            result.Data = ObjectFactory<ISOQueryDA>.Instance.SOInterceptQuery(filter, out dataCount);
            result.TotalCount = dataCount;
            return result;
        }

        /// <summary>
        /// 生成出库单查询
        /// </summary>
        /// <param name="filter">生成出库单查询条件</param>
        /// <returns>生成出库单查询结果</returns>
        [WebInvoke(UriTemplate = "/SO/QueryOutStock4Finance", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryOutStock4Finance(SOOutStock4FinanceQueryFilter filter)
        {
            return QueryList<SOOutStock4FinanceQueryFilter>(filter, ObjectFactory<ISOQueryDA>.Instance.OutStock4FinanceQuery);
        }

        #region 中蛋定制

        /// <summary>
        /// 待审核OZZO订单查询
        /// </summary>
        /// <param name="filter">待审核OZZO订单查询条件</param>
        /// <returns>待审核OZZO订单查询结果</returns>
        [WebInvoke(UriTemplate = "/SO/OZZOOriginNoteQuery", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryOZZOOriginNoteQuery(DefaultQueryFilter filter)
        {
            return QueryList<DefaultQueryFilter>(filter, ObjectFactory<ISOQueryDA>.Instance.OZZOOriginNoteQuery);
        }

        #endregion

        #endregion

        #endregion

        #region 订单操作：创建 更新 审核，作废 拆分...

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="entity">订单信息</param>
        /// <returns>创建成功后的订单信息</returns>
        [WebInvoke(UriTemplate = "/SO/Create", Method = "POST")]
        public SOInfo CreateSO(SOInfo entity)
        {
            return ObjectFactory<SOAppService>.Instance.CreateSO(entity);
        }

        /// <summary>
        /// 创建赠品订单
        /// </summary>
        /// <param name="SOCreateGiftReq">创建礼品卡请求信息</param>
        /// <returns>创建成功后的订单信息</returns>
        [WebInvoke(UriTemplate = "/SO/Create/Gift", Method = "POST")]
        public virtual SOInfo CreateGiftSO(SOCreateGiftReq request)
        {
            return ObjectFactory<SOAppService>.Instance.CreateGiftSO(request.SOInfo, request.MasterSOSysNo);
        }

        /// <summary>
        /// 拆分生成新订单
        /// </summary>
        [WebInvoke(UriTemplate = "/SO/CloneSO", Method = "POST")]
        public SOInfo CloneSO(SOInfo entity)
        {
            return ObjectFactory<SOAppService>.Instance.CloneSO(entity);
        }

        /// <summary>
        /// 更新订单
        /// </summary>
        /// <param name="entity">订单信息</param>
        /// <returns>修改后的订单信息</returns>
        [WebInvoke(UriTemplate = "/SO/Update", Method = "PUT")]
        public SOInfo UpdateSO(SOInfo entity)
        {
            return ObjectFactory<SOAppService>.Instance.UpdateSO(entity);
        }

        /// <summary>
        /// 审核订单
        /// </summary>
        /// <param name="request"></param>
        /// <returns>审核后后的订单信息，如果是多个审核订单，则返回的是第一个订单审核后的信息</returns>
        [WebInvoke(UriTemplate = "/SO/Audit", Method = "PUT")]
        public SOInfo AuditSO(SOAuditReq request)
        {
            List<SOInfo> soInfoList = null;
            ObjectFactory<SOAppService>.Instance.AuditSO(request.SOSysNoList, request.IsForce, request.IsManagerAudit, request.IsAuditNetPay, out soInfoList);
            return soInfoList != null && soInfoList.Count > 0 ? soInfoList[0] : null;
        }

        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns>取消审核后的订单信息</returns>
        [WebInvoke(UriTemplate = "/SO/CAudit", Method = "PUT")]
        public SOInfo CancelAuditSO(string soSysNo)
        {
            int sysNo = int.TryParse(soSysNo, out sysNo) ? sysNo : 0;
            return ObjectFactory<SOAppService>.Instance.CancelAuditSO(sysNo);
        }

        /// <summary>
        /// 作废订单
        /// </summary>
        /// <param name="request">作废订单请求信息</param>
        /// <returns>作废后的订单信息</returns>
        [WebInvoke(UriTemplate = "/SO/Abandon", Method = "PUT")]
        public SOInfo AbandonSO(SOAbandonReq request)
        {
            SOInfo soInfo = null;
            SOAppService service = ObjectFactory<SOAppService>.Instance;
            if (request.IsCreateAO)
            {
                service.AbandonSO(request.SOSysNoList[0], request.ImmediatelyReturnInventory, request.IsCreateAO, request.RefundInfo, out soInfo);
            }
            else
            {
                List<SOInfo> soInfoList = null;
                service.AbandonSO(request.SOSysNoList, request.ImmediatelyReturnInventory, out soInfoList);
                soInfo = soInfoList != null && soInfoList.Count > 0 ? soInfoList[0] : null;
            }
            return soInfo;
        }
        /// <summary>
        /// 申报失败，后台作废订单
        /// </summary>
        /// <param name="SOSysNo">订单号</param>
        [WebInvoke(UriTemplate = "/SO/DeclareFailAbandon/{SOSysNo}", Method = "GET")]
        public void DeclareFailAbandon(string SOSysNo)
        {
            SOInfo soInfo = null;
            SOAppService service = ObjectFactory<SOAppService>.Instance;

            SOAbandonReq request = new SOAbandonReq();
            request.SOSysNoList = new List<int>();
            request.SOSysNoList.Add(int.Parse(SOSysNo));
            request.ImmediatelyReturnInventory = true;
            request.IsCreateAO = true;
            request.RefundInfo = new BizEntity.Invoice.SOIncomeRefundInfo();
            var refundInfo = service.GetValidSOIncomeInfo(int.Parse(SOSysNo));
            request.RefundInfo.BankName = "";
            request.RefundInfo.BranchBankName = "";
            request.RefundInfo.CardNumber = "";
            request.RefundInfo.CardOwnerName = "";
            request.RefundInfo.PostAddress = "";
            request.RefundInfo.PostCode = "";
            request.RefundInfo.ReceiverName = "";
            request.RefundInfo.Note = "";
            request.RefundInfo.RefundPayType = BizEntity.Invoice.RefundPayType.BankRefund;            
            request.RefundInfo.RefundCashAmt = refundInfo.OrderAmt;
            request.SOSysNoList = request.SOSysNoList;

            if (request.IsCreateAO)
            {
                service.AbandonSO(request.SOSysNoList[0], request.ImmediatelyReturnInventory, request.IsCreateAO, request.RefundInfo, out soInfo);
            }
            else
            {
                List<SOInfo> soInfoList = null;
                service.AbandonSO(request.SOSysNoList, request.ImmediatelyReturnInventory, out soInfoList);
            }
        }

        /// <summary>
        /// 锁定订单
        /// </summary>
        /// <param name="request">锁定订单请求信息</param>
        /// <returns>锁定后的订单信息</returns>
        [WebInvoke(UriTemplate = "/SO/Hold", Method = "PUT")]
        public SOInfo HoldSO(SOHoldReq request)
        {
            return ObjectFactory<SOAppService>.Instance.HoldSO(request.SOSysNo, request.Note);
        }

        /// <summary>
        /// 解锁订单
        /// </summary>
        /// <param name="request">解锁订单请求信息</param>
        /// <returns>解锁后的订单信息</returns>
        [WebInvoke(UriTemplate = "/SO/Unhold", Method = "PUT")]
        public SOInfo UnholdSO(SOHoldReq request)
        {
            return ObjectFactory<SOAppService>.Instance.UnholdSO(request.SOSysNo, request.Note);
        }

        /// <summary>
        /// 拆分订单
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns>拆分后的订单信息列表</returns>
        [WebInvoke(UriTemplate = "/SO/Split", Method = "PUT")]
        public List<SOInfo> SplitSO(int soSysNo)
        {
            List<SOInfo> subSOList = ObjectFactory<SOAppService>.Instance.SplitSO(soSysNo);
            return subSOList;
        }

        /// <summary>
        /// 取消拆分订单
        /// </summary>
        /// <param name="request">订单编号</param>
        /// <returns>取消拆分后的订单信息列表</returns>
        [WebInvoke(UriTemplate = "/SO/CSplit", Method = "PUT")]
        public SOInfo CancelSplitSO(int soSysNo)
        {
            return ObjectFactory<SOAppService>.Instance.CancelSplitSO(soSysNo);
        }

        /// <summary>
        ///  订单出库后 普票改增票
        /// </summary>
        /// <param name="entity">订单信息</param> 
        [WebInvoke(UriTemplate = "/SO/SetSOVATInvoiveWhenSOOutStock", Method = "PUT")]
        public void SetSOVATInvoiveWhenSOOutStock(SOInfo entity)
        {
            ObjectFactory<SOAppService>.Instance.SetSOVATInvoiveWhenSOOutStock(entity);
        }

        /// <summary>
        /// 设置订单增值税发票为已开具
        /// </summary>
        /// <param name="soSysNoList">订单编号列表</param>
        [WebInvoke(UriTemplate = "/SO/VATPrinted", Method = "PUT")]
        public void SOVATPrinted(List<int> soSysNoList)
        {
            ObjectFactory<SOAppService>.Instance.SOVATPrinted(soSysNoList);
        }

        /// <summary>
        /// 拆分订单发票
        /// </summary>
        /// <param name="request">拆分发票请求信息</param>
        [WebInvoke(UriTemplate = "/SO/SplitInvoice", Method = "PUT")]
        public virtual void SplitSOInvoice(SOSpliteInvoiceReq request)
        {
            ObjectFactory<SOAppService>.Instance.SplitSOInvoice(request.SOSysNo, request.InvoiceItems);
        }

        /// <summary>
        /// 取消拆分订单发票
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        [WebInvoke(UriTemplate = "/SO/CSplitInvoice", Method = "PUT")]
        public virtual void CancelSplitSOInvoice(int soSysNo)
        {
            //  int sysNo = int.TryParse(soSysNo, out sysNo) ? sysNo : 0;
            ObjectFactory<SOAppService>.Instance.CancelSplitSOInvoice(soSysNo);
        }

        /// <summary>
        /// 手动更改订单仓库
        /// </summary>
        /// <param name="info">手动更改订单仓库信息</param>
        /// <returns>是否成功</returns>
        [WebInvoke(UriTemplate = "/SO/WHUpdateStock", Method = "PUT")]
        public bool WHUpdateStock(SOWHUpdateInfo info)
        {
            return ObjectFactory<SOAppService>.Instance.WHUpdateStock(info);
        }

        #endregion

        #region 获取OPC信息

        [WebInvoke(UriTemplate = "/SO/QueryOPCTransactionByMasterID/{masterID}", Method = "GET")]
        public List<OPCOfflineTransactionInfo> QueryOPCTransactionByMasterID(string masterID)
        {
            int sysNo = int.TryParse(masterID, out sysNo) ? sysNo : 0;
            return sysNo == 0 ? null : ObjectFactory<SOAppService>.Instance.GetTransactionsByMasterID(sysNo);
        }

        #endregion

        #region 取得订单信息相关方法

        /// <summary>
        /// 根据订单编号取得订单信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns>订单信息</returns>
        [WebInvoke(UriTemplate = "/SOInfo/Query/{soSysNo}", Method = "GET")]
        public SOInfo GetSO(string soSysNo)
        {
            int sysNo = int.TryParse(soSysNo, out sysNo) ? sysNo : 0;
            return sysNo == 0 ? null : ObjectFactory<SOAppService>.Instance.GetSOBySOSysNo(sysNo);

        }

        /// <summary>
        /// 根据订单编号取得订单基本信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns>订单主信息</returns>
        [WebInvoke(UriTemplate = "/SO/BaseInfo/{soSysNo}", Method = "GET")]
        public SOBaseInfo GetSOBaseInfo(string soSysNo)
        {
            int sysNo = int.TryParse(soSysNo, out sysNo) ? sysNo : 0;
            return sysNo == 0 ? null : ObjectFactory<SOAppService>.Instance.GetSOBaseInfoBySOSysNo(sysNo);
        }

        /// <summary>
        /// 根据订单编写列表取得多个订单
        /// </summary>
        /// <param name="soSysNoList">订单编号列表</param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SO/BaseInfoList", Method = "POST")]
        public List<SOBaseInfo> GetSOBaseInfoBySOSysNoList(List<int> soSysNoList)
        {
            return ObjectFactory<SOAppService>.Instance.GetSOBaseInfoBySOSysNoList(soSysNoList);
        }

        /// <summary>
        /// 计算(价格、费用)
        /// </summary>
        /// <param name="entity">订单信息</param>
        /// <returns>订单信息</returns>
        [WebInvoke(UriTemplate = "/SO/Calculate", Method = "POST")]
        public SOInfo Calculate(SOInfo entity)
        {
            return ObjectFactory<SOAppService>.Instance.Calculate(entity);
        }

        /// <summary>
        /// 根据客户编号获取客户对应的礼品卡信息
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <returns>礼品卡信息</returns>
        [WebInvoke(UriTemplate = "/GiftCardListInfo/Query/{customerSysNo}", Method = "GET")]
        public List<GiftCardInfo> QueryGiftCardListInfo(string customerSysNo)
        {
            return ObjectFactory<SOAppService>.Instance.QueryGiftCardListInfo(int.Parse(customerSysNo));
        }

        /// <summary>
        /// 根据 礼品卡编号 和密码 获取 对应的礼品卡信息
        /// </summary>
        /// <param name="code">礼品卡 卡号</param>
        /// <param name="password">礼品卡 密码</param>
        [WebInvoke(UriTemplate = "/GiftCardInfo/Query/{code}/{password}", Method = "GET")]
        public GiftCardInfo QueryGiftCardByCodeAndPassword(string code, string password)
        {
            return ObjectFactory<SOAppService>.Instance.QueryGiftCardByCodeAndPassword(code, password);
        }

        /// <summary>
        /// 根据客户编号获取订单对应的增值税发票
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <returns>增值税发票信息</returns>
        [WebInvoke(UriTemplate = "/SOVATInvoiceInfo/Query/{customerSysNo}", Method = "GET")]
        public List<SOVATInvoiceInfo> QuerySOVATInvoiceInfo(string customerSysNo)
        {
            return ObjectFactory<SOAppService>.Instance.QuerySOVATInvoiceInfo(int.Parse(customerSysNo));
        }

        /// <summary>
        /// 根据支付方式判断是否为货到付款
        /// </summary>
        /// <param name="payTypeSysNo">支付方式编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns>是否货到付款</returns>
        [WebInvoke(UriTemplate = "/IsPayWhenReceived/{payTypeSysNo}/{companyCode}", Method = "GET")]
        public bool IsPayWhenReceived(string payTypeSysNo, string companyCode)
        {
            return ObjectFactory<SOAppService>.Instance.IsPayWhenReceived(int.Parse(payTypeSysNo));
        }

        /// <summary>
        /// 根据商品编号获取商品时间范围内的调价信息
        /// </summary>
        /// <param name="productSysNoList">商品编号List</param>
        /// <param name="startTime">下单时间</param>
        /// <param name="endTime">收货时间</param>
        /// <returns>价格变化Log信息</returns>
        [WebInvoke(UriTemplate = "/GetProductPriceChangeLogsInfo/{startTime}/{endTime}", Method = "POST")]
        public List<PriceChangeLogInfo> GetPriceChangeLogsInfoByProductSysNoList(List<int> productSysNoList, string startTime, string endTime)
        {
            return ObjectFactory<SOAppService>.Instance.GetPriceChangeLogsInfoByProductSysNoList(productSysNoList, Convert.ToDateTime(startTime), Convert.ToDateTime(endTime));
        }

        /// <summary>
        /// 根据可不系统编号获取客户类型
        /// </summary>
        /// <param name="CustomerSysNo">客户系统编号</param>
        /// <returns>客户类型实体</returns>
        [WebInvoke(UriTemplate = "/GetKnownFraudCustomerInfo/{CustomerSysNo}", Method = "GET")]
        public KnownFraudCustomer GetKnownFraudCustomerInfoByCustomerSysNo(string CustomerSysNo)
        {
            return ObjectFactory<SOAppService>.Instance.GetKnownFraudCustomerInfoByCustomerSysNo(int.Parse(CustomerSysNo));
        }

        /// <summary>
        /// 根据子订单判断该订单是否可取消拆分
        /// </summary>
        /// <param name="subSOSysNo">子订单编号</param>
        /// <returns>可直接取消拆分返回true ,否则返回false</returns>
        [WebInvoke(UriTemplate = "/IsAllSubSONotOutStockList/{subSOSysNo}", Method = "GET")]
        public bool IsAllSubSONotOutStockList(string subSOSysNo)
        {
            return ObjectFactory<SOAppService>.Instance.IsAllSubSONotOutStockList(int.Parse(subSOSysNo));
        }

        #endregion

        #region Email

        /// <summary>
        /// 发送跟进日志邮件
        /// </summary>
        /// <param name="request">跟进日志邮件信息</param>
        /// <returns>发送地址列表</returns>
        [WebInvoke(UriTemplate = "/SO/SendEmail", Method = "POST")]
        public List<string> SendInternalEmail(SendEmailReq request)
        {
            return ObjectFactory<SOAppService>.Instance.SendInternalEmail(request.EmailList, request.Title, request.Content, request.Language);
        }

        #endregion

        [WebInvoke(UriTemplate = "/SO/BatchDealItemInFile", Method = "POST")]
        public List<SOItemInfo> BatchDealItemFromFile(byte[] fileContent)
        {
            return ObjectFactory<SOAppService>.Instance.BatchDealItemFromFile(fileContent);
        }

        /// <summary>
        /// 计算(价格、费用)
        /// </summary>
        /// <param name="entity">订单信息</param>
        /// <returns>订单信息</returns>
        [WebInvoke(UriTemplate = "/SO/ProductQtyChange", Method = "POST")]
        public SOInfo ProductQtyChange(SOInfo entity)
        {
            ISODA soDA = ObjectFactory<ISODA>.Instance;
            foreach (var item in entity.Items)
            {
                if (item.ProductSysNo.HasValue && item.Quantity.HasValue)
                {
                    StockInfo stock = soDA.GetProductStockByProductSysNoAndQty(item.ProductSysNo.Value, item.Quantity.Value);
                    if (stock != null)
                    {
                        item.StockSysNo = stock.SysNo;
                        item.StockName = stock.StockName;
                    }
                }
            }
            return ObjectFactory<SOAppService>.Instance.Calculate(entity);
        }

        /// <summary>
        /// 批量设置申报通过
        /// </summary>
        /// <param name="soSysNoList">订单编号列表</param>
        [WebInvoke(UriTemplate = "/SO/BatchReportedSo", Method = "PUT")]
        public bool BatchReportedSo(List<int> soSysNoList)
        {
            ObjectFactory<SOAppService>.Instance.BatchOperationUpdateSOStatusToReported(soSysNoList);
            return true;
        }

        /// <summary>
        /// 网关订单查询
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        [WebInvoke(UriTemplate = "/SO/QueryBill/{soSysNo}", Method = "GET")]
        public TransactionQueryBill QueryBill(string soSysNo)
        {
            return ObjectFactory<SOAppService>.Instance.QueryBill(soSysNo);
        }

        /// <summary>
        /// 修改订单状态为 已申报待通关
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SO/UpdateSOStatusToReported", Method = "PUT")]
        public bool UpdateSOStatusToReported(int soSysNo)
        {
            ObjectFactory<SOAppService>.Instance.UpdateSOStatusToReported(soSysNo);
            return true;
        }

        /// <summary>
        /// 修改订单状态为 申报失败订单作废
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SO/UpdateSOStatusToReject", Method = "PUT")]
        public bool UpdateSOStatusToReject(int soSysNo)
        {
            ObjectFactory<SOAppService>.Instance.UpdateSOStatusToReject(soSysNo);
            return true;
        }

        /// <summary>
        /// 修改订单状态为 已通关发往顾客
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SO/UpdateSOStatusToCustomsPass", Method = "PUT")]
        public bool UpdateSOStatusToCustomsPass(int soSysNo)
        {
            ObjectFactory<SOAppService>.Instance.UpdateSOStatusToCustomsPass(soSysNo);
            return true;
        }


        /// <summary>
        /// 修改订单状态为 通关失败订单作废
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SO/UpdateSOStatusToCustomsReject", Method = "PUT")]
        public bool UpdateSOStatusToCustomsReject(int soSysNo)
        {
            ObjectFactory<SOAppService>.Instance.UpdateSOStatusToCustomsReject(soSysNo);
            return true;
        }


        /// <summary>
        /// 批量设置 申报失败订单作废
        /// </summary>
        /// <param name="soSysNoList">订单编号列表</param>
        [WebInvoke(UriTemplate = "/SO/BatchOperationUpdateSOStatusToReject", Method = "PUT")]
        public bool BatchOperationUpdateSOStatusToReject(List<int> soSysNoList)
        {
            ObjectFactory<SOAppService>.Instance.BatchOperationUpdateSOStatusToReject(soSysNoList);
            return true;
        }

        /// <summary>
        /// 批量设置 已通关发往顾客
        /// </summary>
        /// <param name="soSysNoList">订单编号列表</param>
        [WebInvoke(UriTemplate = "/SO/BatchOperationUpdateSOStatusToCustomsPass", Method = "PUT")]
        public bool BatchOperationUpdateSOStatusToCustomsPass(List<int> soSysNoList)
        {
            ObjectFactory<SOAppService>.Instance.BatchOperationUpdateSOStatusToCustomsPass(soSysNoList);
            return true;
        }

        /// <summary>
        /// 批量设置 通关失败订单作废
        /// </summary>
        /// <param name="soSysNoList">订单编号列表</param>
        [WebInvoke(UriTemplate = "/SO/BatchOperationUpdateSOStatusToCustomsReject", Method = "PUT")]
        public bool BatchOperationUpdateSOStatusToCustomsReject(List<int> soSysNoList)
        {
            ObjectFactory<SOAppService>.Instance.BatchOperationUpdateSOStatusToCustomsReject(soSysNoList);
            return true;
        }

        /// <summary>
        /// 订单编辑页面 保存 更新备注
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/SO/SOMaintainUpdateNote", Method = "PUT")]
        public void SOMaintainUpdateNote(SOInfo info)
        {
            ObjectFactory<SOAppService>.Instance.SOMaintainUpdateNote(info);
        }
    }
}
