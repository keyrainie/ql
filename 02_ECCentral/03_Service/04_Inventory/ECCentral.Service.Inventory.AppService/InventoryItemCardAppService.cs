using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.Inventory.AppService
{
    [VersionExport(typeof(InventoryItemCardAppService))]
    public class InventoryItemCardAppService
    {
        public DateTime? GetRMAInventoryOnlineDateForItemCardQuery(string key, string companyCode)
        {
            string getResult = ObjectFactory<ICommonBizInteract>.Instance.GetSystemConfigurationValue(key, companyCode);
            return string.IsNullOrEmpty(getResult) ? (DateTime?)null : Convert.ToDateTime(getResult);
        }
    }
}
