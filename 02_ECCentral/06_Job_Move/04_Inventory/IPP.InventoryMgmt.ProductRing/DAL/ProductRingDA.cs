using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECCentral.Job.Inventory.ProductRing.Components;
using Newegg.Oversea.Framework.DataAccess;
using ECCentral.BizEntity.Inventory;
using ECCentral.Job.Inventory.ProductRing.Model;

namespace ECCentral.Job.Inventory.ProductRing.DAL
{
    public static class ProductRingDA
    {
        public static List<ProductBatchInfo> QueryProductBatchModified()
        {
            DataCommand command = DataCommandManager.GetDataCommand("QueryProductBatchModified");
            command.SetParameterValue("@CompanyCode", Settings.CompanyCode);

            return command.ExecuteEntityList<ProductBatchInfo>();
        }

        public static int UpdateBatchInfo(string paramXml)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateBatchInfo");
            command.SetParameterValue("@Msg", paramXml);
            return command.ExecuteNonQuery();
        }
    }
}
