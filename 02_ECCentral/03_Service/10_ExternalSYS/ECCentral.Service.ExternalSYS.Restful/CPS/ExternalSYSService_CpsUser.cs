using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Service.ExternalSYS.AppService;

namespace ECCentral.Service.ExternalSYS.Restful
{
    public partial class ExternalSYSService
    {

        /// <summary>
        /// 根据query得到CPSUser信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CpsUser/GetCpsUser", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetCpsUser(CpsUserQueryFilter query)
        {

            int totalCount;
            var dataTable = ObjectFactory<ICpsUserDA>.Instance.GetCpsUser(query, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 获取UserSource
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CpsUser/GetUserSource", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetUserSource(CpsUserSourceQueryFilter query)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICpsUserDA>.Instance.GetUserSource(query, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
       
        /// <summary>
        /// 获取网站类型
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CpsUser/GetWebSiteType", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetWebSiteType()
        {
            var dataTable = ObjectFactory<ICpsUserDA>.Instance.GetWebSiteType();
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = 0
            };
        }

        /// <summary>
        /// 获取银行类型
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CpsUser/GetBankType", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetBankType()
        {
            var dataTable = ObjectFactory<ICpsUserDA>.Instance.GetBankType();
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = 0
            };
        }
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/CpsUser/UpdateUserStatus", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void UpdateUserStatus(CpsUserInfo info)
        {

            ObjectFactory<CpsUserAppService>.Instance.UpdateUserStatus(info);

        }

        /// <summary>
        /// 更新Source
        /// </summary>
        /// <param name="Info"></param>
        [WebInvoke(UriTemplate = "/CpsUser/UpdateUserSource", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void UpdateUserSource(CpsUserInfo info)
        {
            ObjectFactory<CpsUserAppService>.Instance.UpdateUserSource(info);


        }
        /// <summary>
        /// 更新基本信息
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/CpsUser/UpdateBasicUser", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void UpdateBasicUser(CpsUserInfo info)
        {
            ObjectFactory<CpsUserAppService>.Instance.UpdateBasicUser(info);
        }

        /// <summary>
        /// 更新收款账户信息
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/CpsUser/UpdateCpsReceivablesAccount", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void UpdateCpsReceivablesAccount(CpsUserInfo info)
        {
            ObjectFactory<CpsUserAppService>.Instance.UpdateCpsReceivablesAccount(info);
        }
        /// <summary>
        /// 创建source
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/CpsUser/CreateUserSource", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public void CreateUserSource(CpsUserInfo info)
        {
            ObjectFactory<CpsUserAppService>.Instance.CreateUserSource(info);
        }
        /// <summary>
        /// 获取审核记录
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CpsUser/GetAuditHistory", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult GetAuditHistory(int SysNo)
        {
            var dataTable = ObjectFactory<ICpsUserDA>.Instance.GetAuditHistory(SysNo);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = 0
            };
        }
         /// <summary>
        /// 审核
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/CpsUser/AuditUser", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void AuditUser(CpsUserInfo info)
        {
            ObjectFactory<CpsUserAppService>.Instance.AuditUser(info);
        }
       
    }
}
