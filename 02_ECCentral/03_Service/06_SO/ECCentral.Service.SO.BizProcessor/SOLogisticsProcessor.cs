using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.SO.BizProcessor
{
    [VersionExport(typeof(SOLogisticsProcessor))]
    public class SOLogisticsProcessor
    {

        ISOLogisticDA LogisticDA = ObjectFactory<ISOLogisticDA>.Instance;


        public void MarkDeliveryExp(DeliveryExpMarkEntity info)
        {
            foreach (int? orderSysNo in info.OrderSysNos)
            {
                int opUser=ServiceContext.Current.UserSysNo;
                LogisticDA.MarkDeliveryExp(orderSysNo.Value, info.OrderType.Value, info.CompanyCode, opUser);
            }
        }




    }
}
