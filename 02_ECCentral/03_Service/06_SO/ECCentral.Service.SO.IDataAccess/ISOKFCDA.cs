using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.SO.IDataAccess
{
    public interface ISOKFCDA
    {
        /// <summary>
        /// 持续化KFC数据
        /// </summary>
        /// <param name="entity">请求实体</param>
        /// <returns>影响的行数</returns>
        int InsertKnowFrandCustomer(KnownFraudCustomer entity);

        /// <summary>
        /// 获取用户的KFC
        /// </summary>
        /// <param name="customSysNo">用户编号</param>
        /// <returns>用户的KFC记录</returns>
        KnownFraudCustomer GetKFCByCustomerSysNo(int customSysNo);

        /// <summary>
        /// 根据IP和Tel获取KFC列表
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="mobilePhone">手机</param>
        /// <param name="telephone">电话</param>
        /// <param name="companyCode">公司编码</param>
        /// <returns>KFC列表</returns>
        List<KnownFraudCustomer> GetKFCByIPAndTel(string ipAddress, string mobilePhone, string telephone, string companyCode);

        /// <summary>
        /// 修改用户的KFC状态
        /// </summary>
        /// <param name="item">申请实体</param>
        void UpdateKnowFrandCustomerStatus(KnownFraudCustomer item);
    }
}
