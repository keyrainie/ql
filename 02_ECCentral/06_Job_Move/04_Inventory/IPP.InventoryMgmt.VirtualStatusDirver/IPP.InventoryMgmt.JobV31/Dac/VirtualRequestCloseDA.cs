using System.Collections.Generic;
using IPP.InventoryMgmt.JobV31.BusinessEntities;
using Newegg.Oversea.Framework.DataAccess;

namespace IPP.InventoryMgmt.JobV31.Dac
{
    public class VirtualRequestCloseDA
    {
        public static List<VirtualRequestEntity> GetClosingVirtualRequest(string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetClosingVirtualRequest");
            command.SetParameterValue("@CompanyCode", companyCode);
            return command.ExecuteEntityList<VirtualRequestEntity>();
        }


        public static int CloseVirtualRequest(int requestSysNo, int status, int isAdjustVirtualQty, string inUser, string companyCode, string storeCompanyCode, string languageCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CloseVirtualRequest");

            command.SetParameterValue("@StVirtualRequestSysNo", requestSysNo);
            command.SetParameterValue("@Status", status);
            command.SetParameterValue("@IsAdjustVirtualQty", isAdjustVirtualQty);
            command.SetParameterValue("@InUser", inUser);
            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@StoreCompanyCode", storeCompanyCode);
            command.SetParameterValue("@LanguageCode", languageCode);
            command.ExecuteNonQuery();

            int result = (int)command.GetParameterValue("@ReturnValue");

            return result;
        }
    }
}
