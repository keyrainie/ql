using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IPP.Oversea.CN.InventoryMgmt.JobV31.BusinessEntities;
using Newegg.Oversea.Framework.DataAccess;
using IPP.Oversea.CN.InventoryMgmt.JobV31.Common;
using System.Data;
namespace IPP.Oversea.CN.InventoryMgmt.JobV31.DataAccess
{
    public class CommonDA
    {
        public static void UpdateECommerceStatus(int productSysNo, int channelSysNo, string status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateECommerceStatus");

            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@ChannelSysNo", channelSysNo);
            command.SetParameterValue("@IsClearInventory", status);
            command.SetParameterValue("@CompanyCode", Config.CompanyCode);
            command.SetParameterValue("@OptIp", Util.GetHostIP());
            command.SetParameterValue("@OptUserSysNo", Config.UserSysNo);

            command.ExecuteNonQuery();
        }

        public static List<int> GetAllChannel()
        {
            List<int> result = new List<int>();
            DataCommand command = DataCommandManager.GetDataCommand("GetAllChannel");
            using (IDataReader reader = command.ExecuteDataReader())
            {
                while (reader.Read())
                {
                    result.Add(reader.GetInt32(0));
                }
            }
            return result;
        }
    }
}
