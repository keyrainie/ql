using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.Customer;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.AppService;
using ECCentral.Service.Customer.Restful.RequestMsg;

namespace ECCentral.Service.Customer.Restful
{
    public partial class CustomerService
    {
        /// <summary>
        /// 更新FPCheckMaster
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/FPCheckMaster/BatchUpdate", Method = "PUT")]
        public void UpdateFPCheckMaster(List<FPCheck> msg)
        {
            ObjectFactory<CSToolAppService>.Instance.UpdateFPCheckMaster(msg);
        }
        /// <summary>
        /// 获取所有的FPCheckMaster
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/FPCheckMaster/Query", Method = "POST")]
        public QueryResult GetFPCheckMaster(FPCheckQueryFilter query)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<IFPCheckQueryDA>.Instance.Query(query, out totalCount),
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/CHSet/Query", Method = "POST")]
        public QueryResult GetCHSet(CHQueryFilter query)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<IFPCheckQueryDA>.Instance.QueryCHSet(query, out totalCount),
                TotalCount = totalCount
            };
        }
        /// <summary>
        /// 添加串货订单限制
        /// </summary>
        /// <param name="query"></param>
        [WebInvoke(UriTemplate = "/CHSet/CreateCH", Method = "POST")]
        public void CreateCH(CHSetReq request)
        {
            ObjectFactory<CSToolAppService>.Instance.CreateCH(request.ChannelID, request.Status, request.CategorySysNo, request.ProductID);
        }

        [WebInvoke(UriTemplate = "/CHSet/UpdateCHItemStatus", Method = "PUT")]
        public void UpdateCHItemStatus(string id)
        {
            ObjectFactory<CSToolAppService>.Instance.UpdateCHItemStatus(int.Parse(id));
        }
        /// <summary>
        /// 炒货订单查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ETC/Query", Method = "POST")]
        public QueryResult GetETC(string WebChannelID)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<IFPCheckQueryDA>.Instance.GetETC(WebChannelID, out totalCount),
                TotalCount = totalCount
            };
        }
        /// <summary>
        /// 更新炒货订单信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ETC/UpdateETC", Method = "PUT")]
        public void UpdateETC(List<CCSetReq> request)
        {
            request.ForEach(item =>
            {
                ObjectFactory<CSToolAppService>.Instance.UpdateETC(item.SysNo, item.Params, item.Status);
            });
        }
    }
}
