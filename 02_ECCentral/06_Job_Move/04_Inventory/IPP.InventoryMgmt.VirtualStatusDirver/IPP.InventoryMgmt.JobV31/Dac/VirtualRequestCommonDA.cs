using System.Collections.Generic;
using System.Data;
using IPP.InventoryMgmt.JobV31.BusinessEntities;
using Newegg.Oversea.Framework.DataAccess;

namespace IPP.InventoryMgmt.JobV31.Dac
{
    public class VirtualRequestCommonDA
    {
        public static List<string> GetUserMailByVirtualRequest(int virtualRequestSysNo,string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetUserMailByVirtualRequest");
            command.SetParameterValue("@VirtualRequestSysNo", virtualRequestSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);

            IDataReader reader = command.ExecuteDataReader();

            List<string> list = new List<string>();
            while (reader.Read())
            {
                if (!reader.IsDBNull(0))
                {
                    list.Add(reader.GetString(0));
                }
            }
            return list;
            
        }

        public static ProductVirtualInfoEntity GetProductInfoByVirtualRequestSysNo(int requestSysNo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductInfoByVirtualRequestSysNo");
            command.SetParameterValue("@VirtualRequestSysNo", requestSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);
            return command.ExecuteEntity<ProductVirtualInfoEntity>();
        }


    }
}
