using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.BizEntity;
using System.Transactions;

namespace ECCentral.Service.Common.BizProcessor
{
    [VersionExport(typeof(ShipTypePayTypeProcessor))]
    public class ShipTypePayTypeProcessor
    {
        public virtual ShipTypePayTypeInfo Create(ShipTypePayTypeInfo entity)
        {
            if (ObjectFactory<IShipTypePayTypeDA>.Instance.IsExistShipPayType(entity))
                throw new BizException(string.Format("配送方式:{0} - 支付方式:{1} 的数据已存在！", entity.ShipTypeName, entity.PayTypeName));

            return ObjectFactory<IShipTypePayTypeDA>.Instance.Create(entity);
        }

        public virtual void DeleteBatch(List<int> sysNos)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (int tNum in sysNos)
                    ObjectFactory<IShipTypePayTypeDA>.Instance.Delete(tNum);

                scope.Complete();
            }
        }
    }
}
