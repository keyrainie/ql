using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    [VersionExport(typeof(IPOVendorInvoiceDA))]
    public class POVendorInvoiceDA : IPOVendorInvoiceDA
    {
        #region IPOVendorInvoiceDA Members

        public POVendorInvoiceInfo Create(POVendorInvoiceInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertPOVendorInvoice");
            command.SetParameterValue(entity);
            entity.SysNo = Convert.ToInt32(command.ExecuteScalar());

            return entity;
        }

        public void Update(POVendorInvoiceInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePOVendorInvoice");
            command.SetParameterValue(entity);
            command.ExecuteNonQuery();
        }

        public POVendorInvoiceInfo GetPOVendorInvoiceBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPOVendorInvoiceBySysNo");
            command.SetParameterValue("@SysNo", sysNo);
            return command.ExecuteEntity<POVendorInvoiceInfo>();
        }

        public void UpdateStatus(int sysNo, InvoiceStatus invoiceStatus)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePOVendorInvoiceStatus");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@Status", invoiceStatus);
            command.ExecuteNonQuery();
        }

        public void Audit(int sysNo, InvoiceStatus invoiceStatus)
        {
            DataCommand command = DataCommandManager.GetDataCommand("AuditPOVendorInvoice");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@Status", invoiceStatus);
            command.SetParameterValueAsCurrentUserSysNo("@AuditUserSysNo");
            command.ExecuteNonQuery();
        }

        #endregion
    }
}
