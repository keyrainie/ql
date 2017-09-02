using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Invoice.IDataAccess;
using System.Data;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.NeweggCN.Invoice.SAP;
using System.Text.RegularExpressions;

namespace ECCentral.Service.Invoice.SqlDataAccess.NeweggCN
{
    [VersionExport(typeof(INECN_SyncSAPSalesDA))]
    public class NECN_SyncSAPSalesDA : INECN_SyncSAPSalesDA
    {
        public void DeleteSAPSales(int orderTypeSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteSAPSales");
            command.SetParameterValue("@OrderTypeSysNo", orderTypeSysNo);
            command.ExecuteNonQuery();

        }

        public void SyncSAPSales(SAPSalesInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SyncSAPSales");
            command.SetParameterValue<SAPSalesInfo>(entity);
            command.ExecuteNonQuery();
        }

    }
}
