using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventConsumer
{
    public static class ConstValue
    {
        public const string DataBaseName_SSB = "NCService";

        /// <summary>
        /// http://soa.newegg.com/SOA/CN/OrderManagement/V10/Warehouse/OverseaIPPWMSOffline
        /// </summary>
        public const string SSB_From_OverseaIPPWMSOffline = "http://soa.newegg.com/SOA/CN/OrderManagement/V10/Warehouse/OverseaIPPWMSOffline";

        /// <summary>
        /// http://soa.newegg.com/SOA/CN/InfrastructureService/V10/NeweggCN/PubSubService
        /// </summary>
        public const string SSB_To_PubSubServic = "http://soa.newegg.com/SOA/CN/InfrastructureService/V10/NeweggCN/PubSubService";

        /// <summary>
        /// http://soa.newegg.com/SOA/CN/InfrastructureService/V10/Warehouse51/FromService
        /// </summary>
        public const string SSB_From_Warehouse51FromService = "http://soa.newegg.com/SOA/CN/InfrastructureService/V10/Warehouse51/FromService";

        /// <summary>
        /// http://soa.newegg.com/SOA/CN/InventoryManagement/V10/Warehouse/WMSShippingProcessDownLoad
        /// </summary>
        public const string SSB_From_WMSShippingProcessDownLoad = "http://soa.newegg.com/SOA/CN/InventoryManagement/V10/Warehouse/WMSShippingProcessDownLoad";


    }
}