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
        /// 查询转换单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConvertRequest/QueryConvertRequest", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryConvertRequest(ConvertRequestQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IConvertRequestQueryDA>.Instance.QueryConvertRequest(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        #endregion 查询

        #region 维护

        /// <summary>
        /// 根据系统编号加载转换单详细信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConvertRequest/Load/{requestSysNo}", Method = "GET")]
        public ConvertRequestInfo LoadConvertRequestInfo(string requestSysNo)
        {
            int sysNo = int.Parse(requestSysNo);
            ConvertRequestInfo result = ObjectFactory<ConvertRequestAppService>.Instance.GetConvertRequestInfoBySysNo(sysNo);
            return result;
        }

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConvertRequest/GetProductLine/{productSysNo}", Method = "GET")]
        public ConvertRequestInfo GetProductLine(string productSysNo)
        {
            int sysNo = int.Parse(productSysNo);
            ConvertRequestInfo result = ObjectFactory<ConvertRequestAppService>.Instance.GetProductLineInfo(sysNo);
            return result;
        }

        /// <summary>
        /// 创建转换单
        /// </summary>
        /// <param name="entityToCreate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConvertRequest/CreateRequest", Method = "POST")]
        public virtual ConvertRequestInfo CreateConvertRequest(ConvertRequestInfo entityToCreate)
        {
            return ObjectFactory<ConvertRequestAppService>.Instance.CreateRequest(entityToCreate);
        }

        /// <summary>
        /// 更新转换单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConvertRequest/UpdateRequest", Method = "PUT")]
        public virtual ConvertRequestInfo UpdateConvertRequest(ConvertRequestInfo entityToUpdate)
        {
            return ObjectFactory<ConvertRequestAppService>.Instance.UpdateRequest(entityToUpdate);
        }

        /// <summary>
        /// 作废转换单
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConvertRequest/AbandonRequest", Method = "PUT")]
        public virtual ConvertRequestInfo AbandonConvertRequest(ConvertRequestInfo entityToUpdate)
        {
            return ObjectFactory<ConvertRequestAppService>.Instance.AbandonRequest(entityToUpdate);
        }

        /// <summary>
        /// 取消作废转换单
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConvertRequest/CancelAbandonRequest", Method = "PUT")]
        public virtual ConvertRequestInfo CancelAbandonConvertRequest(ConvertRequestInfo entityToUpdate)
        {
            return ObjectFactory<ConvertRequestAppService>.Instance.CancelAbandonRequest(entityToUpdate);
        }

        /// <summary>
        /// 审核转换单
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConvertRequest/VerifyRequest", Method = "PUT")]
        public virtual ConvertRequestInfo VerifyConvertRequest(ConvertRequestInfo entityToUpdate)
        {
            return ObjectFactory<ConvertRequestAppService>.Instance.VerifyRequest(entityToUpdate);
        }

        /// <summary>
        /// 取消审核转换单
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConvertRequest/CancelVerifyRequest", Method = "PUT")]
        public virtual ConvertRequestInfo CancelVerifyConvertRequest(ConvertRequestInfo entityToUpdate)
        {
            return ObjectFactory<ConvertRequestAppService>.Instance.CancelVerifyRequest(entityToUpdate);
        }

        /// <summary>
        /// 转换单出库
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ConvertRequest/OutStockRequest", Method = "PUT")]
        public virtual ConvertRequestInfo OutStockConvertRequest(ConvertRequestInfo entityToUpdate)
        {
            return ObjectFactory<ConvertRequestAppService>.Instance.OutStockRequest(entityToUpdate);
        }

        #endregion 维护
    }
}
