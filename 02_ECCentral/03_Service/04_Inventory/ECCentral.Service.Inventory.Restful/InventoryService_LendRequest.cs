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

namespace ECCentral.Service.Inventory.Restful
{
    public partial class InventoryService
    {        
        #region 查询
        /// <summary>
        /// 查询借货单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/LendRequest/QueryLendRequest", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryLendRequest(LendRequestQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<ILendRequestQueryDA>.Instance.QueryLendRequest(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 统计当前条件下的借货单初始状态和作废状态的成本
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/LendRequest/QueryLendCostbyStatus", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryLendCostbyStatus(LendRequestQueryFilter request)
        {            
            var dataTable = ObjectFactory<ILendRequestQueryDA>.Instance.QueryLendCostbyStatus(request);
            return new QueryResult()
            {
                Data = dataTable               
            };
        }

        /// <summary>
        /// 查询借货单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/LendRequest/ExportAllByPM", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult ExportAllByPM(LendRequestQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<ILendRequestQueryDA>.Instance.ExportAllByPM(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        #endregion 查询

        #region 维护

        /// <summary>
        /// 根据系统编号加载借货单详细信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/LendRequest/Load/{requestSysNo}", Method = "GET")]        
        public LendRequestInfo LoadLendRequestInfo(string requestSysNo)
        {
            int sysNo = int.Parse(requestSysNo);
            LendRequestInfo result = ObjectFactory<LendRequestAppService>.Instance.GetLendRequestInfoBySysNo(sysNo);
            return result;
        }

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/LendRequest/GetProductLine/{productSysNo}", Method = "GET")]
        public LendRequestInfo GetLendProductLine(string productSysNo)
        {
            int sysNo = int.Parse(productSysNo);
            LendRequestInfo result = ObjectFactory<LendRequestAppService>.Instance.GetProductLineInfo(sysNo);
            return result;
        }

        /// <summary>
        /// 创建借货单
        /// </summary>
        /// <param name="entityToCreate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/LendRequest/CreateRequest", Method = "POST")]
        public virtual LendRequestInfo CreateLendRequest(LendRequestInfo entityToCreate)
        {
            return ObjectFactory<LendRequestAppService>.Instance.CreateRequest(entityToCreate);
        }

        /// <summary>
        /// 更新借货单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/LendRequest/UpdateRequest", Method = "PUT")]
        public virtual LendRequestInfo UpdateLendRequest(LendRequestInfo entityToUpdate)
        {
            return ObjectFactory<LendRequestAppService>.Instance.UpdateRequest(entityToUpdate);
        }


        /// <summary>
        /// 作废借货单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/LendRequest/AbandonRequest", Method = "PUT")]
        public virtual LendRequestInfo AbandonLendRequest(LendRequestInfo entityToUpdate)
        {
            return ObjectFactory<LendRequestAppService>.Instance.AbandonRequest(entityToUpdate);
        }

        /// <summary>
        /// 取消作废借货单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/LendRequest/CancelAbandonRequest", Method = "PUT")]
        public virtual LendRequestInfo CancelAbandonLendRequest(LendRequestInfo entityToUpdate)
        {
            return ObjectFactory<LendRequestAppService>.Instance.CancelAbandonRequest(entityToUpdate);
        }

        /// <summary>
        /// 审核借货单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/LendRequest/VerifyRequest", Method = "PUT")]
        public virtual LendRequestInfo VerifyLendRequest(LendRequestInfo entityToUpdate)
        {
            return ObjectFactory<LendRequestAppService>.Instance.VerifyRequest(entityToUpdate);
        }

        /// <summary>
        /// 取消审核借货单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/LendRequest/CancelVerifyRequest", Method = "PUT")]
        public virtual LendRequestInfo CancelVerifyLendRequest(LendRequestInfo entityToUpdate)
        {
            return ObjectFactory<LendRequestAppService>.Instance.CancelVerifyRequest(entityToUpdate);
        }

        /// <summary>
        /// 借货出库
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/LendRequest/OutStockRequest", Method = "PUT")]
        public virtual LendRequestInfo OutStockLendRequest(LendRequestInfo entityToUpdate)
        {
            return ObjectFactory<LendRequestAppService>.Instance.OutStockRequest(entityToUpdate);
        }

        /// <summary>
        /// 借货归还
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/LendRequest/ReturnLendItem", Method = "PUT")]
        public virtual LendRequestInfo ReturnLendRequest(LendRequestInfo entityToUpdate)
        {            
            return ObjectFactory<LendRequestAppService>.Instance.ReturnRequest(entityToUpdate);
        }
    
        #endregion 维护
    }
}
