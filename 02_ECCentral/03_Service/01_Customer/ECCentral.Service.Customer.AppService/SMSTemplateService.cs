using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.BizProcessor;

namespace ECCentral.Service.Customer.AppService
{
    [VersionExport(typeof(SMSTemplateService))]
    public class SMSTemplateService
    {
        /// <summary>
        /// 创建短信模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual SMSTemplate Create(SMSTemplate entity)
        {
            return ObjectFactory<SMSTemplateProcessor>.Instance.Create(entity); 
        }

        /// <summary>
        /// 更新短信模板
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(SMSTemplate entity)
        {
            ObjectFactory<SMSTemplateProcessor>.Instance.Update(entity);
        }

        /// <summary>
        /// 加载短信模板
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual SMSTemplate Load(int sysNo)
        {
            return ObjectFactory<SMSTemplateProcessor>.Instance.Load(sysNo);
        }
    }
}
