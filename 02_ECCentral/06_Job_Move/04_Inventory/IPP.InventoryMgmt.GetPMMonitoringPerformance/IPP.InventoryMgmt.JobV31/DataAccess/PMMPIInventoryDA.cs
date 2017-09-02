using System.Collections.Generic;
using IPP.InventoryMgmt.JobV31.BusinessEntities;
using Newegg.Oversea.Framework.DataAccess;
using System.Data;
namespace IPP.InventoryMgmt.JobV31.DataAccess
{
    public static class PMMPIInventoryDA
    {

        public static int InsertMPI(string PMSysNos)
        {
            DataCommand command = DataCommandManager.GetDataCommand("QueryPMMPIEntityInfoByCategoryV2");
            command.ReplaceParameterValue("#PMSysNos#", PMSysNos);
            return  command.ExecuteNonQuery();
 
        }

        public static int  Insert(string PMSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryPMMPIEntityInfoByAllPM");
            command.AddInputParameter("@Category1SysNo", DbType.Int32);
            command.AddInputParameter("@Category2SysNo", DbType.Int32);
            command.SetParameterValue("@Category1SysNo", 0);
            command.SetParameterValue("@Category2SysNo", 0);     
            command.CommandText = command.CommandText.Replace("#PMSysNoList#", PMSysNo);
            command.CommandTimeout = 60000;
            return command.ExecuteNonQuery();
           
        }
        public static  int InsertByCategory(string c1)
        {
                CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryPMMPIEntityInfoByCategory");
                dataCommand.AddInputParameter("@Category1SysNo", DbType.Int32);
                dataCommand.AddInputParameter("@Category2SysNo", DbType.Int32);           
                dataCommand.SetParameterValue("@Category1SysNo", c1);
                dataCommand.SetParameterValue("@Category2SysNo", 0);
                dataCommand.CommandText = dataCommand.CommandText.Replace("#TempCondition#", " AND Category1SysNo=@Category1SysNo ");
                dataCommand.CommandTimeout = 60000;
                return dataCommand.ExecuteNonQuery();
       
        }
        public static List<PMMPISysNoEntity> Querty(string  CompanyCode)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryPMMPSysNo");
            command.AddInputParameter("@CompanyCode", DbType.Int32);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.CommandTimeout = 60000;
            return command.ExecuteEntityList<PMMPISysNoEntity>();
             
        }
        public static List<CategorySysNoEntity> QueryCategory1SysNo(string CompanyCode)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCategory1SysNo");
            command.AddInputParameter("@CompanyCode", DbType.Int32);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.CommandTimeout = 60000;
            return command.ExecuteEntityList<CategorySysNoEntity>();    
        }
        
        public static void Trucate()
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("TrucateInventory_PMMonitoringPerformance");
            command.ExecuteNonQuery();
        }

        public static int InitiStatus()
        {
            DataCommand command = DataCommandManager.GetDataCommand("InitiStatus");
            return command.ExecuteNonQuery();
        }


        public static List<PMMPIInventoryEntity> QueryShortageProductsNow()
        {
            DataCommand command = DataCommandManager.GetDataCommand("QueryShortageProductsNow");
            return command.ExecuteEntityList<PMMPIInventoryEntity>();
        }

        public static int UpdateProduct(PMMPIInventoryEntity pm)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateProduct");
            command.SetParameterValue("@PMUserSysNo", pm.PMUserSysNo);
            command.SetParameterValue("@PMName", pm.DisplayName);
            command.SetParameterValue("@Losing", pm.Losing);
            command.SetParameterValue("@ProductSysNo", pm.ProductSysNo);
            command.SetParameterValue("@StockSysNo", pm.StockSysNo);
            command.SetParameterValue("@IsOutOfStock", pm.IsOutOfStock);

            return command.ExecuteNonQuery();
        }

        public static int ImportNewProduct(string PMSysNos)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ImportNewPMMPIProductNew");
            command.ReplaceParameterValue("#PMSysNos#", PMSysNos);

            return command.ExecuteNonQuery();
 
        }


        public static int DeleteProduct()
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteProduct");
            return command.ExecuteNonQuery();
        }

        public static int UpdateProductNew()
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateProductNew");
            return command.ExecuteNonQuery();
        }

    }
}
