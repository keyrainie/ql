using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    public interface IFreeShippingChargeRuleDA
    {
        /// <summary>
        /// 加载一条免运费规则信息
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        FreeShippingChargeRuleInfo Load(int sysno);

        /// <summary>
        /// 取得指定状态的所有免运费规则 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        List<FreeShippingChargeRuleInfo> GetAllByStatus(FreeShippingAmountSettingStatus status);

        /// <summary>
        /// 创建一条免运费规则
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        FreeShippingChargeRuleInfo Create(FreeShippingChargeRuleInfo entity);

        /// <summary>
        /// 更新一条免运费规则
        /// </summary>
        /// <param name="entity"></param>
        void UpdateInfo(FreeShippingChargeRuleInfo entity);

        /// <summary>
        /// 更新一条免运费规则状态
        /// </summary>
        /// <param name="entity"></param>
        void UpdateStatus(FreeShippingChargeRuleInfo entity);

        /// <summary>
        /// 删除一条免运费规则
        /// </summary>
        /// <param name="sysno"></param>
        void Delete(int sysno);
    }
}
