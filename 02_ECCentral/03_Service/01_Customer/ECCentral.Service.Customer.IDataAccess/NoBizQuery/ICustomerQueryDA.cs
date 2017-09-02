using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.BizEntity.Customer;
using ECCentral.QueryFilter.Customer;

namespace ECCentral.Service.Customer.IDataAccess.NoBizQuery
{
    public interface ICustomerQueryDA
    {
        DataTable SimpleQuery(CustomerSimpleQueryFilter queryCriteria, out int totalCount);

        /// <summary>
        /// 查询顾客
        /// </summary>
        /// <returns></returns>
        DataTable Query(ECCentral.QueryFilter.Customer.CustomerQueryFilter queryCriteria, out int totalCount);

        /// <summary>
        /// 经验值历史
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryCustomerExperience(ECCentral.QueryFilter.Customer.CustomerExperienceLogQueryFilter queryCriteria, out int totalCount);

        /// <summary>
        /// 积分历史
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryCustomerPointLog(ECCentral.QueryFilter.Customer.CustomerPointLogQueryFilter queryCriteria, out int totalCount);

        /// <summary>
        /// 恶意用户操作历史
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryMaliceCustomerLog(int customerSysNo);

        /// <summary>
        /// 获取第三方账户信息
        /// </summary>
        /// <param name="customerSysNoList"></param>
        /// <returns></returns>
        List<ThirdPartyUser> GetThirdPartyUserInfo(List<int?> customerSysNoList);

        /// <summary>
        /// 获取客户安全密保问题
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        DataTable GetSecurityQuestion(int customerSysNo);

    }
}
