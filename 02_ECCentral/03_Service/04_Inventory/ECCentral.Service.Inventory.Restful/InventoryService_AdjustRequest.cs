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
        /// 查询损益单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AdjustRequest/QueryAdjustRequest", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryAdjustRequest(AdjustRequestQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IAdjustRequestQueryDA>.Instance.QueryAdjustRequest(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        #endregion 查询

        #region 维护

        /// <summary>
        /// 根据系统编号加载损益单详细信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AdjustRequest/Load/{requestSysNo}", Method = "GET")]
        public AdjustRequestInfo LoadAdjustRequestInfo(string requestSysNo)
        {
            int sysNo = int.Parse(requestSysNo);
            AdjustRequestInfo result = ObjectFactory<AdjustRequestAppService>.Instance.GetAdjustRequestInfoBySysNo(sysNo);
            return result;
        }

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AdjustRequest/GetProductLine/{productSysNo}", Method = "GET")]
        public AdjustRequestInfo GetAdjustProductLine(string productSysNo)
        {
            int sysNo = int.Parse(productSysNo);
            AdjustRequestInfo result = ObjectFactory<AdjustRequestAppService>.Instance.GetProductLineInfo(sysNo);
            return result;
        }
        /// <summary>
        /// 创建损益单
        /// </summary>
        /// <param name="entityToCreate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AdjustRequest/CreateRequest", Method = "POST")]
        public List<AdjustRequestInfo> CreateAdjustRequest(AdjustRequestInfo entityToCreate)
        {
            List<AdjustRequestInfo> result = ObjectFactory<AdjustRequestAppService>.Instance.CreateRequest(entityToCreate);

            return result;
        }

        /// <summary>
        /// 更新损益单
        /// </summary>
        /// <param name="entityToSave"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AdjustRequest/UpdateRequest", Method = "PUT")]
        public AdjustRequestInfo UpdateAdjustRequest(AdjustRequestInfo entityToUpdate)
        {
            return ObjectFactory<AdjustRequestAppService>.Instance.UpdateRequest(entityToUpdate);
        }

        /// <summary>
        /// 作废损益单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AdjustRequest/AbandonRequest", Method = "PUT")]
        public AdjustRequestInfo AbandonAdjustRequest(AdjustRequestInfo entityToUpdate)
        {
            return ObjectFactory<AdjustRequestAppService>.Instance.AbandonRequest(entityToUpdate);
        }

        /// <summary>
        /// 取消作废损益单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AdjustRequest/CancelAbandonRequest", Method = "PUT")]
        public AdjustRequestInfo CancelAbandonAdjustRequest(AdjustRequestInfo entityToUpdate)
        {   
            return ObjectFactory<AdjustRequestAppService>.Instance.CancelAbandonRequest(entityToUpdate);
        }

        /// <summary>
        /// 审核损益单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AdjustRequest/VerifyRequest", Method = "PUT")]
        public AdjustRequestInfo VerifyAdjustRequest(AdjustRequestInfo entityToUpdate)
        {
            return ObjectFactory<AdjustRequestAppService>.Instance.VerifyRequest(entityToUpdate);
        }

        /// <summary>
        /// 取消审核损益单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AdjustRequest/CancelVerifyRequest", Method = "PUT")]
        public AdjustRequestInfo CancelVerifyAdjustRequest(AdjustRequestInfo entityToUpdate)
        {   
            return ObjectFactory<AdjustRequestAppService>.Instance.CancelVerifyRequest(entityToUpdate);
        }

        /// <summary>
        /// 损益单出库
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AdjustRequest/OutStockRequest", Method = "PUT")]
        public AdjustRequestInfo OutStockAdjustRequest(AdjustRequestInfo entityToUpdate)
        {   
            return ObjectFactory<AdjustRequestAppService>.Instance.OutStockRequest(entityToUpdate);
        }

        /// <summary>
        /// 损益单出库
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/AdjustRequest/MaintainAdjustInvoiceInfo", Method = "PUT")]
        public AdjustRequestInfo MaintainAdjustInvoiceInfo(AdjustRequestInfo entityToUpdate)
        {
            return ObjectFactory<AdjustRequestAppService>.Instance.MaintainAdjustInvoiceInfo(entityToUpdate);
        }
  
        #endregion 维护
    }
}
