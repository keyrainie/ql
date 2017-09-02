using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(SMSTemplateProcessor))]
    public class SMSTemplateProcessor
    {
        /// <summary>
        /// 创建短信模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual SMSTemplate Create(SMSTemplate entity)
        {
            return ObjectFactory<ISMSTemplateDA>.Instance.Create(entity);
        }

        /// <summary>
        /// 更新短信模板
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(SMSTemplate entity)
        {
            ObjectFactory<ISMSTemplateDA>.Instance.Update(entity);
        }

        /// <summary>
        /// 加载短信模板
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual SMSTemplate Load(int sysNo)
        {
            SMSTemplate entity = new SMSTemplate();

            return entity;
        }
    }
}
