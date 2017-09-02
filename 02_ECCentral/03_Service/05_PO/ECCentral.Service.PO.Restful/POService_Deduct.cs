using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.PO.PurchaseOrder;
using ECCentral.QueryFilter.PO;
using ECCentral.Service.PO.AppService;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.PO.Restful
{
    public partial class POService
    {
        /// <summary>
        /// 获取扣款项维护信息列表。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Deduct/QueryDeductList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryDeductList(DeductQueryFilter request)
        {
            int totalCount = 0;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<IDeductQueryDA>.Instance.LoadDeductInfo(request, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }

        /// <summary>
        /// 作废单个扣款项维护信息
        /// </summary>
        /// <param name="fileIdentity"></param>
        [WebInvoke(UriTemplate = "/Deduct/DeleteDeductBySysNo", Method = "DELETE")]
        public int DeleteDeductBySysNo(int sysNo)
        {
            return ObjectFactory<IDeductDA>.Instance.DeleteDeductBySysNo(sysNo);
        }

        /// <summary>
        /// 查询单条扣款项信息
        /// </summary>
        /// <param name="fileIdentity"></param>
        [WebInvoke(UriTemplate = "/Deduct/GetSingleDeductBySysNo/{sysNo}", Method = "GET",ResponseFormat=WebMessageFormat.Json)]
        public Deduct GetSingleDeductBySysNo(string sysNo)
        {
            return ObjectFactory<IDeductDA>.Instance.GetSingleDeductBySysNo(sysNo);
        }

        /// <summary>
        /// 编辑扣款项
        /// </summary>
        /// <param name="fileIdentity"></param>
        [WebInvoke(UriTemplate = "/Deduct/UpdateDeduct", Method = "PUT")]
        public Deduct UpdateDeduct(Deduct deduct)
        {
            return ObjectFactory<DeductAppService>.Instance.MaintainDeduct(deduct);
        }

        /// <summary>
        /// 创建扣款项
        /// </summary>
        /// <param name="deduct"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Deduct/CreateDeduct", Method = "POST",ResponseFormat=WebMessageFormat.Json)]
        public Deduct CreateDeduct(Deduct deduct)
        {
            return ObjectFactory<DeductAppService>.Instance.CreateDeduct(deduct);
        }
    }
}
