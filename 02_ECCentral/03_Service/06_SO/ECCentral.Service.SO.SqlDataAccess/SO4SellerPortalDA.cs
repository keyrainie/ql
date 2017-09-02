using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System;
using System.Data;
using System.Text;
using ECCentral.QueryFilter.SO;
using System.Collections.Generic;

namespace ECCentral.Service.SO.SqlDataAccess
{
    [VersionExport(typeof(ISO4SellerPortalDA))]
    public class SO4SellerPortalDA : ISO4SellerPortalDA
    {
        #region ISO4SellerPortalDA Members

        public int UpdateSOSellerStatus(SalesOrderStatusEntity soStatusEntity, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_UpdateSOReceivingStatusAndSOStatus");
            command.SetParameterValue("@SOSysno", soStatusEntity.SOSysNo);
            command.SetParameterValue("@SOStatus", soStatusEntity.SOStatus);
            command.SetParameterValue("@ReceivingStatus", soStatusEntity.ReceivingStatus);
            command.SetParameterValue("@CompanyCode", companyCode);

            return command.ExecuteNonQuery();
        }

        public int UpdateSOMasterInvoiceNo(SOInvoicePrintedSalesOrder soInvoicePrintedSalesOrder, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSOMasterInvoiceNo");
            command.SetParameterValue("@SONumber", soInvoicePrintedSalesOrder.SONumber);
            command.SetParameterValue("@InvoiceNumber", soInvoicePrintedSalesOrder.InvoiceNumber);
            command.SetParameterValue("@CompanyCode", companyCode);

            return command.ExecuteNonQuery();
        }

        #endregion
    }
}
