using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(CustomerEmailProcessor))]
    public class CustomerEmailProcessor
    {
        public virtual string GetEmailContent(int sysNo, string dbName)
        {
            return ObjectFactory<ICustomerEmailDA>.Instance.GetEmailContent(sysNo, dbName);
        }
    }
}
