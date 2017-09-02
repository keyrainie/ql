using System;
using System.Collections.Generic;
using System.Linq;
using IPP.InventoryMgmt.JobV31.BusinessEntities;
using Newegg.Oversea.Framework.Entity;
using Newegg.Oversea.Framework.DataAccess;
using IPP.InventoryMgmt.JobV31.DataAccess;
using IPP.InventoryMgmt.JobV31.Common;

namespace IPP.InventoryMgmt.JobV31.DataAccess
{
    public static class ThirdPartInventoryDA
    {
        public static List<ThirdPartInventoryEntity> Query(CommonConst Common)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryThirdPartInventory");

            //CommonConst commonConst = new CommonConst();
            string sql = command.CommandText;

            sql = sql.Replace("#WarehouseNumber#", Util.Contract(",", Common.WareHourseNumbers));
            sql = sql.Replace("#PartnerType#", "'" + Common.PartnerType + "'");
            sql = sql.Replace("#CompanyCode#", "'" + Common.CompanyCode + "'");

            command.CommandText = sql;

            List<ThirdPartInventoryEntity> list = command.ExecuteEntityList<ThirdPartInventoryEntity>();

            return list;
        }

        /// <summary>
        /// 修改本地第三方库存
        /// </summary>
        /// <param name="InventoryQty">分仓的库存增量</param>
        /// <param name="SynInventoryQty">总仓的增量</param>
        /// <param name="ProductMappingSysno"></param>
        public static void Modify(int InventoryQty, int SynInventoryQty, int ProductMappingSysno, CommonConst Common)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ModifyThirdPartInventory");
            //CommonConst commonConst = new CommonConst();
            command.SetParameterValue("@InventoryQty", InventoryQty);
            command.SetParameterValue("@SynInventoryQty", SynInventoryQty);
            command.SetParameterValue("@ProductMappingSysno", ProductMappingSysno);
            command.SetParameterValue("@EditUser", Common.UserLoginName);
            command.SetParameterValue("@CompanyCode", Common.CompanyCode);
            command.ExecuteNonQuery();
        }

        public static void Insert(int InventoryQty, int SynInventoryQty, int ProductMappingSysno, CommonConst Common)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertThirdPartInventory");
            //CommonConst commonConst = new CommonConst();
            command.SetParameterValue("@ProductMappingSysno", ProductMappingSysno);
            command.SetParameterValue("@WarehouseNumber", Common.ThirdPartWareHourseNumber);
            command.SetParameterValue("@WarehouseName", Common.ThirdPartWareHourseName);
            command.SetParameterValue("@InventoryQty", InventoryQty);
            command.SetParameterValue("@ProductDescription", "");
            command.SetParameterValue("@SynInventoryQty", SynInventoryQty);
            command.SetParameterValue("@PurchasePrice", 0);
            command.SetParameterValue("@CompanyCode", Common.CompanyCode);
            command.SetParameterValue("@StoreCompanyCode", Common.StoreCompanyCode);
            command.SetParameterValue("@InUser", Common.UserLoginName);
            command.SetParameterValue("@InventoryAlarmQty", Common.InventoryAlarmQty);
            command.ExecuteNonQuery();
        }
    }
}
