using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Common.AppService
{
    [VersionExport(typeof(FreeShippingChargeRuleAppService))]
    public class FreeShippingChargeRuleAppService
    {
        /// <summary>
        /// 加载一条免运费规则
        /// </summary>
        /// <param name="sysno">免运费规则编号</param>
        /// <returns></returns>
        public virtual FreeShippingChargeRuleInfo Load(int sysno)
        {
            return ObjectFactory<FreeShippingChargeRuleProcessor>.Instance.Load(sysno);
        }

        /// <summary>
        /// 创建一条免运费规则
        /// </summary>
        /// <param name="entity">免运费规则DTO</param>
        /// <returns>免运费规则</returns>
        public virtual FreeShippingChargeRuleInfo Create(FreeShippingChargeRuleInfo entity)
        {
            return ObjectFactory<FreeShippingChargeRuleProcessor>.Instance.Create(entity);
        }

        /// <summary>
        /// 更新一条免运费规则
        /// </summary>
        /// <param name="entity">免运费规则DTO</param>
        public virtual void Update(FreeShippingChargeRuleInfo entity)
        {
            ObjectFactory<FreeShippingChargeRuleProcessor>.Instance.Update(entity);
        }

        /// <summary>
        /// 设置一条免运费规则为有效状态
        /// </summary>
        /// <param name="sysno">免运费规则编号</param>
        public virtual void Valid(int sysno)
        {
            ObjectFactory<FreeShippingChargeRuleProcessor>.Instance.Valid(sysno);
        }

        /// <summary>
        /// 设置一条免运费规则为无效状态
        /// </summary>
        /// <param name="sysno">免运费规则编号</param>
        public virtual void Invalid(int sysno)
        {
            ObjectFactory<FreeShippingChargeRuleProcessor>.Instance.Invalid(sysno);
        }

        /// <summary>
        /// 删除一条免运费规则
        /// </summary>
        /// <param name="sysno">免运费规则编号</param>
        public virtual void Delete(int sysno)
        {
            ObjectFactory<FreeShippingChargeRuleProcessor>.Instance.Delete(sysno);
        }
    }
}
