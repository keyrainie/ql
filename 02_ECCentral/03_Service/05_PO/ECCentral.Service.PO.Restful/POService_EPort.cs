using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO;
using ECCentral.Service.PO.AppService;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace ECCentral.Service.PO.Restful
{
    public partial class POService
    {
        /// <summary>
        /// 创建一个全新的供应商信息。
        /// </summary>
        /// <param name="newVendor"></param>
        [WebInvoke(UriTemplate = "/EPort/CreatEPort", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public EPortEntity CreateEPort(EPortEntity newEport)
        {
            return ObjectFactory<EPortService>.Instance.CreateEPort(newEport);
        }

        /// <summary>
        /// 保存电子口岸
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/EPort/SaveEport", Method = "PUT")]
        public EPortEntity SaveEport(EPortEntity entity)
        {
            return ObjectFactory<EPortService>.Instance.SaveEport(entity);
        }
        /// <summary>
        /// 删除电子口岸
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/EPort/DeleteEport", Method = "PUT")]
        public int DeleteEport(int sysNO)
        {
            return ObjectFactory<EPortService>.Instance.DeleteEport(sysNO);
        }
        /// <summary>
        /// 获取电子口岸
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/EPort/GetEport/{sysNo}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        public EPortEntity GetEport(string sysNO)
        {
            return ObjectFactory<EPortService>.Instance.GetEport(sysNO);
        }

        /// <summary>
        /// 获取电子口岸列表。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/EPort/QueryEport", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryEport(EPortFilter request)
        {
            int totalCount = 0;
            QueryResult returnResult = new QueryResult()
            {
                Data = ObjectFactory<IEPortDA>.Instance.QueryEPort(request, out totalCount)
            };
            returnResult.TotalCount = totalCount;
            return returnResult;
        }

        /// <summary>
        /// 获取电子口岸列表。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/EPort/GetAllEPort", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public List<EPortEntity> GetAllEPort()
        {
            return ObjectFactory<EPortService>.Instance.GetAllEPort();
        }
    }
}
