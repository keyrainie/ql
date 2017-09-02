using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Customer.IDataAccess
{
    public interface ICustomerEmailDA
    {
        string GetEmailContent(int sysNo, string dbName);
    }
}
