using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    public interface IShipTypePayTypeDA
    {
        ShipTypePayTypeInfo Create(ShipTypePayTypeInfo entity);

        void Delete(int sysNo);

        bool IsExistShipPayType(ShipTypePayTypeInfo entity);
    }
}
