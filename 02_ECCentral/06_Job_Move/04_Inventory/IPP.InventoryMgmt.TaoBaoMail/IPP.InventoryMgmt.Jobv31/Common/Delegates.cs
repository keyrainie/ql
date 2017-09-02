using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.InventoryMgmt.JobV31.BusinessEntities;

namespace IPP.InventoryMgmt.JobV31.Common
{

    public delegate List<ThirdPartInventoryEntity> QueryThirdPartInventoryEntity();

    public delegate List<TaobaoProduct> QueryTaobaoProduct();

    public delegate void SendMailDelegate<T>(T source);

}
