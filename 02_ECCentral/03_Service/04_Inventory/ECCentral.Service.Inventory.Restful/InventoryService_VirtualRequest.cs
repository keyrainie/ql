using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Service.Inventory.AppService;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.Inventory.Restful.RequestMsg;

namespace ECCentral.Service.Inventory.Restful
{
    public partial class InventoryService
    {
        #region Query
        [WebInvoke(UriTemplate = "VirtualRequest/QueryProducts", Method = "POST")]
        public QueryResult QueryProducts(VirtualRequestQueryProductsFilter filter)
        {
            int totalCount;
            DataTable dt = ObjectFactory<IVirtualRequestQueryDA>.Instance.QueryProducts(filter, out totalCount);
            return new QueryResult
            {
                Data = dt,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 查询虚库列表:
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/VirtualRequest/QueryVirtualRequestList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryVirtualRequestList(VirtualRequestQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            int totalCount = 0;
            result.Data = ObjectFactory<IVirtualRequestQueryDA>.Instance.QueryVirtualRequest(queryFilter, out totalCount);
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 查询虚库日志:
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/VirtualRequest/QueryVirtualRequestMemoList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryVirtualRequestMemoList(VirtualRequestQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            int totalCount = 0;
            result.Data = ObjectFactory<IVirtualRequestQueryDA>.Instance.QueryVirtualRequestMemo(queryFilter, out totalCount);
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 查询虚库单关闭日志List:
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/VirtualRequest/QueryVirtualRequestCloseLog", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryVirtualRequestCloseLog(VirtualRequestQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            int totalCount = 0;
            result.Data = ObjectFactory<IVirtualRequestQueryDA>.Instance.QueryVirtualRequestCloseLog(queryFilter, out totalCount);
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 查询虚库单库存信息
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/VirtualRequest/QueryVirtualInventoryInfoByStock", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryVirtualInventoryInfoByStock(VirtualRequestQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            result.Data = ObjectFactory<IVirtualRequestQueryDA>.Instance.QueryVirtualInventoryInfoByStock(queryFilter);
            result.TotalCount = result.Data.Rows.Count;
            return result;
        }


        /// <summary>
        /// 查询虚库单 - 查询最后虚库变更：
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/VirtualRequest/QueryVirtualInventoryLastVerifiedRequest", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryVirtualInventoryLastVerifiedRequest(VirtualRequestQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            result.Data = ObjectFactory<IVirtualRequestQueryDA>.Instance.QueryVirtualInventoryLastVerifiedRequest(queryFilter);
            result.TotalCount = result.Data.Rows.Count;
            return result;
        }

        /// <summary>
        /// 查询虚库单 - 查询虚库变更List：
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/VirtualRequest/QueryModifiedVirtualRequest", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryModifiedVirtualRequest(VirtualRequestQueryFilter queryFilter)
        {
            int totalCount = 0;
            QueryResult result = new QueryResult();
            result.Data = ObjectFactory<IVirtualRequestQueryDA>.Instance.QueryModifiedVirtualRequest(queryFilter, out totalCount);
            result.TotalCount = totalCount;
            return result;
        }

        /// <summary>
        /// 查询虚库日志创建者列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/VirtualRequest/QueryVirtualRequestMemoCreateUserList", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryVirtualMemoCreateUserList(string companyCode)
        {
            int totalCount;
            var dataTable = ObjectFactory<IVirtualRequestQueryDA>.Instance.QueryVirtualRequestMemoCreateUserList(companyCode, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 查询虚库申请单创建者列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/VirtualRequest/QueryVirtualRequestCreateUserList", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryVirtualRequestCreateUserList(string companyCode)
        {
            int totalCount;
            var dataTable = ObjectFactory<IVirtualRequestQueryDA>.Instance.QueryVirtualRequestCreateUserList(companyCode, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 查询此商品对应需要关闭的虚库申请单数量
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/VirtualRequest/QueryNeedCloseRequestCount", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public int QueryNeedCloseRequestCount(VirtualRequestQueryFilter queryFilter)
        {
            return ObjectFactory<IVirtualRequestDA>.Instance.ExistNeedCloseVirtualQuantity(queryFilter.StockSysNo.Value, queryFilter.ProductSysNo.Value,queryFilter.SysNo.Value,queryFilter.CompanyCode);                      
        }

        #endregion Query

        #region Action

        [WebInvoke(UriTemplate = "/VirtualRequest/Apply", Method = "POST")]
        public List<VirtualRequestInfo> ApplyRequest(VirtualRequestInfoReq request)
        {
            ObjectFactory<VirtualRequestAppService>.Instance.ApplyRequestBatch(request.CanOperateItemOfLessThanPrice, request.CanOperateItemOfSecondHand, request.RequestList);
            return request.RequestList;
        }

        /// <summary>
        /// 虚库单审核 - 同意
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/VirtualRequest/ApproveVirtualRequest", Method = "PUT")]
        public void ApproveVirtualRequest(VirtualRequestInfo info)
        {
            ObjectFactory<VirtualRequestAppService>.Instance.ApproveVirtualRequest(info);
        }

        /// <summary>
        /// 虚库单审核 - 批量同意
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/VirtualRequest/BatchApproveVirtualRequest", Method = "PUT")]
        public string BatchApproveVirtualRequest(List<VirtualRequestInfo> infoList)
        {
           return  ObjectFactory<VirtualRequestAppService>.Instance.BatchApproveVirtualRequest(infoList);
        }

        /// <summary>
        /// 虚库单审核 - 拒绝
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/VirtualRequest/RejectVirtualRequest", Method = "PUT")]
        public void RejectVirtualRequest(VirtualRequestInfo info)
        {
            ObjectFactory<VirtualRequestAppService>.Instance.RejectVirtualRequest(info);
        }
        #endregion
    }
}
