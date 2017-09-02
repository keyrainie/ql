using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using System.Data;

namespace ECCentral.Service.Customer.IDataAccess
{
    public interface IGiftDA
    {
        CustomerGift Insert(CustomerGift entity);

        int UpdateStatus(CustomerGift entity);

        CustomerGift Load(int sysNo);

        CustomerGift Load(int customerSysNo, int productSysNo, CustomerGiftStatus status);

        void Update(CustomerGift entity);

        void ReturnGiftForSO(int soSysNo);
    }
}
