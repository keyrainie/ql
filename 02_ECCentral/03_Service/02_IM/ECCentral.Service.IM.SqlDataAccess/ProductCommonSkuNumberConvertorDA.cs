using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IProductCommonSkuNumberConvertorDA))]
    public class ProductCommonSkuNumberConvertorDA : IProductCommonSkuNumberConvertorDA
    {

        public DataTable GetCommonSkuNumbersByProductIDs(List<string> list)
        {
            
            string listString = "";
            foreach (string item in list)
            {
                listString = listString + "'" + item + "',";
            }
            listString = listString.Substring(0, listString.Length - 1);
            DataCommand command = DataCommandManager.GetDataCommand("Item_GetCommonSkuNumbersByProductIDs");

            command.SetParameterValue("@ProductIDList", listString);

           return command.ExecuteDataTable();

        }
        public DataTable GetProductIDsByCommonSkuNumbers(List<string> list)
        {
            string listString = "";
            foreach (string item in list)
            {
                listString = listString + "'" + item + "',";
            }
            listString = listString.Substring(0, listString.Length - 1);
            DataCommand command = DataCommandManager.GetDataCommand("Item_GetProductIDsByCommonSkuNumbers");

            command.SetParameterValue("@CommonSkuNumberList", listString);

            return command.ExecuteDataTable();
        }
    }
}
