using System.Collections.Generic;
using IPP.InventoryMgmt.JobV31.BusinessEntities;
using Newegg.Oversea.Framework.DataAccess;

namespace IPP.InventoryMgmt.JobV31.Dac
{
    public class VirtualRequestRunDA
    {
        public static List<VirtualRequestEntity> GetAuditedVirtualRequest(string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetAuditedVirtualRequest");
            command.SetParameterValue("@CompanyCode",companyCode);
            return command.ExecuteEntityList <VirtualRequestEntity>();
        }

        public static void LaunchVirtualRequest(VirtualRequestEntity request)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LaunchVirtualRequest");
            command.SetParameterValue("@StVirtualRequestSysNo",request.SysNo);
			command.SetParameterValue("@ProductSysNo",request.ProductSysNo);
			command.SetParameterValue("@AdjustVirtualQty",request.VirtualQty);
			command.SetParameterValue("@StockSysNo",request.StockSysNo);
			command.SetParameterValue("@CompanyCode",request.CompanyCode);
            command.SetParameterValue("@LanguageCode",request.LanguageCode);
            command.SetParameterValue("@StoreCompanyCode",request.StoreCompanyCode);
            command.ExecuteNonQuery();
        }

        public static void CloseRequestWithOutAdjustInventory(VirtualRequestEntity request)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CloseRequestWithOutAdjustInventory");

            command.SetParameterValue("@ProductSysNo",request.ProductSysNo);
            command.SetParameterValue("@StockSysNo",request.StockSysNo);
            command.SetParameterValue("@CompanyCode",request.CompanyCode);

            command.ExecuteNonQuery();
      
        }

        public static void CoverClosingRequest(int newRequesrtSysNo, int oldRequestSysNo, string companyCode, string storeCompanyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CoverClosingRequest");
            command.SetParameterValue("@NewRequsetSysNo", newRequesrtSysNo);
            command.SetParameterValue("@OldRequsetSysNo", oldRequestSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@StoreCompanyCode", storeCompanyCode);

            command.ExecuteNonQuery();
        }

    }
}
