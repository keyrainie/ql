using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.InventoryMgmt.JobV31.BusinessEntities.InventoryAging;
using Newegg.Oversea.Framework.DataAccess;

namespace IPP.InventoryMgmt.JobV31.Dac.InventoryAging
{
    public class InventoryAgeDA
    {

        public static DateTime? GetInventoryLastDate()
        {
            object obj = DataCommandManager.GetDataCommand("GetInventoryLastDate").ExecuteScalar();
            if (Convert.IsDBNull(obj))
            {
                return null;
            }
            else
            {
                return Convert.ToDateTime(obj);
            }
        }


        public static DateTime? GetInventoryAgeDate(DateTime? inventoryLastDate)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetInventoryAgeDate");
            command.SetParameterValue("@LastDate", inventoryLastDate);
            object obj = command.ExecuteScalar();

            if (Convert.IsDBNull(obj))
            {
                return null;
            }
            else
            {
                return Convert.ToDateTime(obj);
            }
        }


        public static int totalAgeCount(DateTime? ageDate)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetInventoryAgeCount");
            command.SetParameterValue("@AgeDate", ageDate);
            return Convert.ToInt32(command.ExecuteScalar());
        }


        /// <summary>
        /// 清空库龄信息
        /// </summary>
        /// <returns></returns>
        public static int ClearInventoryAge()
        {
            DataCommand command = DataCommandManager.GetDataCommand("ClearInventoryAgeTable");
            return command.ExecuteNonQuery();     
        }

        /// <summary>
        /// 获取库龄信息
        /// </summary>
        /// <returns></returns>
        public static List<InventoryAgeEntity> GetInventoryAgeData(int startNumber, int batch, DateTime? biDate)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetInventoryAgeData");
            command.SetParameterValue("@BIDate", biDate);
            command.SetParameterValue("@StartNumber",startNumber);
            command.SetParameterValue("@Batch",batch);
            return command.ExecuteEntityList<InventoryAgeEntity>();
        }

        /// <summary>
        /// 插入库龄信息
        /// </summary>
        /// <param name="inventoryAge"></param>
        /// <returns></returns>
        public static int InsertInventoryAge(InventoryAgeEntity inventoryAge)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertInventoryAge");
            command.SetParameterValue("@InventoryDate",inventoryAge.InventoryDate);
            command.SetParameterValue("@Item_Key",inventoryAge.Item_Key);
            command.SetParameterValue("@VendorNumber",inventoryAge.VendorNumber);
            command.SetParameterValue("@Quantity",inventoryAge.Quantity);
            command.SetParameterValue("@DateRange",inventoryAge.DateRange);
            command.SetParameterValue("@InStockDays",inventoryAge.InStockDays);
            command.SetParameterValue("@UnitCost",inventoryAge.UnitCost);
            return command.ExecuteNonQuery();
        }

    }
}
