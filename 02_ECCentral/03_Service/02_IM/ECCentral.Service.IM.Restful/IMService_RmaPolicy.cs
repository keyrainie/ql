using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        /// <summary>
        /// 查询退换货信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/RmaPolicy/QueryRmaPolicy", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryRmaPolicy(RmaPolicyQueryFilter query)
        {
            
             int totalCount;
            var data = ObjectFactory<IRmaPolicyQueryDA>.Instance.QueryRmaPolicy(query, out totalCount);
            var source = new QueryResult { Data = data, TotalCount = totalCount };
            return source;
        }
        /// <summary>
        /// 创建退换货信息
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/RmaPolicy/CreateRmaPolicy", Method = "POST")]
        public void CreateRmaPolicy(RmaPolicyInfo info)
        {
            ObjectFactory<RmaPolicyAppservice>.Instance.CreateRmaPolicy(info);
        }
        /// <summary>
        /// 更新退换货信息
        /// </summary>
        /// <param name="info"></param>
         [WebInvoke(UriTemplate = "/RmaPolicy/UpdateRmaPolicy", Method = "PUT")]
        public void UpdateRmaPolicy(RmaPolicyInfo info)
        {
            ObjectFactory<RmaPolicyAppservice>.Instance.UpdateRmaPolicy(info);
        }

        /// <summary>
        ///作废
        /// </summary>
        /// <param name="sysNo"></param>
         [WebInvoke(UriTemplate = "/RmaPolicy/DeActiveRmaPolicy", Method = "PUT")]
         public void DeActiveRmaPolicy(List<RmaPolicyInfo> list)
        {
            ObjectFactory<RmaPolicyAppservice>.Instance.DeActiveRmaPolicy(list);
              
        }

        /// <summary>
        /// 激活
        /// </summary>
        /// <param name="sysNo"></param>
         [WebInvoke(UriTemplate = "/RmaPolicy/ActiveRmaPolicy", Method = "PUT")]
         public void ActiveRmaPolicy(List<RmaPolicyInfo> list)
        {
            ObjectFactory<RmaPolicyAppservice>.Instance.ActiveRmaPolicy(list);
        }

        /// <summary>
        /// 根据SysNo得到RmaPolicy
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
         [WebInvoke(UriTemplate = "/RmaPolicy/QueryRmaPolicyBySysNo", Method = "POST")]
         public RmaPolicyInfo QueryRmaPolicyBySysNo(int SysNo)
         {
          return   ObjectFactory<IRmaPolicyQueryDA>.Instance.QueryRmaPolicyBySysNo(SysNo);
         }
         /// <summary>
         /// 得到所有的退换货信息
         /// </summary>
         /// <param name="SysNo"></param>
         /// <returns></returns>
         [WebInvoke(UriTemplate = "/RmaPolicy/GetAllRmaPolicy", Method = "POST")]
         public List<RmaPolicyInfo> GetAllRmaPolicy()
         {
             return ObjectFactory<IRmaPolicyQueryDA>.Instance.GetAllRmaPolicy();
         }
    }
}
