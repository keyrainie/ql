using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Customer.AppService
{
    [VersionExport(typeof(CustomerEmailAppService))]
    public class CustomerEmailAppService
    {
        /// <summary>
        /// 获得发送邮件的内容
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public virtual string GetEmailContent(int sysNo, string dbName)
        {
            return ObjectFactory<CustomerEmailProcessor>.Instance.GetEmailContent(sysNo, dbName);
        }
    }
}
