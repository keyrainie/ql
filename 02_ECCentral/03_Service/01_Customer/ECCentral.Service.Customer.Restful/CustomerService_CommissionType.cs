using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Customer.Society;
using ECCentral.QueryFilter.Customer;
using ECCentral.Service.Customer.AppService;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;

namespace ECCentral.Service.Customer.Restful
{
    public partial class CustomerService
    {
        [WebInvoke(UriTemplate = "/CommissionType/QueryCommissionType", Method = "POST")]
        public QueryResult QueryCommissionType(CommissionTypeQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICommissionTypeQueryDA>.Instance.QueryCommissionType(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 创建返佣金方式
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommissionType/Create", Method = "POST")]
        public CommissionType CreateCommissionType(CommissionType request)
        {
            return ObjectFactory<CommissionTypeAppService>.Instance.Create(request);
        }

        /// <summary>
        /// 更新返佣金方式
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommissionType/Update", Method = "PUT")]
        public CommissionType UpdateCommissionType(CommissionType request)
        {
            return ObjectFactory<CommissionTypeAppService>.Instance.Update(request);
        }

        /// <summary>
        /// 加载返佣金方式
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/CommissionType/Load/{sysNo}", Method = "GET")]
        public CommissionType LoadCommissionType(string sysNo)
        {
            int _sysNo = int.Parse(sysNo);
            return ObjectFactory<CommissionTypeAppService>.Instance.QueryCommissionType(_sysNo);
        }
        #region 扩展属性
        [WebInvoke(UriTemplate = "/CommissionType/SocietyCommissionQuery", Method = "POST")]
        public QueryResult SocietyCommissionQuery(CommissionTypeQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICommissionTypeQueryDA>.Instance.SocietyCommissionQuery(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        #endregion
    }
}
