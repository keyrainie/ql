using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.IDataAccess
{
    public interface ISMSTemplateDA
    {
        /// <summary>
        /// 创建短信模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        SMSTemplate Create(SMSTemplate entity);

        /// <summary>
        /// 更新短信模板
        /// </summary>
        /// <param name="entity"></param>
        void Update(SMSTemplate entity);

        /// <summary>
        /// 加载短信模板
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        SMSTemplate Load(int sysNo);
    }
}
