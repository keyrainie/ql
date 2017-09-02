using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Invoice;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Invoice
{
    public class NetPayDA
    {

        public static NetPayInfo GetValidBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetValidNetPayBySOSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteEntity<NetPayInfo>();
        }
    }
}
