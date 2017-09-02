using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    public interface IAreaDeliveryDA
    {
        AreaDeliveryInfo Create(AreaDeliveryInfo entity);

        AreaDeliveryInfo Update(AreaDeliveryInfo entity);

        void Delete(int transactionNumber);

        AreaDeliveryInfo GetAreaDeliveryInfoByID(int transactionNumber);

        List<AreaDeliveryInfo> GetAreaDeliveryList();
    }
}
