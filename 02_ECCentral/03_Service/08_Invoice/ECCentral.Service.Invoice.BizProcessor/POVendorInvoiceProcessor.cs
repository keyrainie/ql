using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(POVendorInvoiceProcessor))]
    public class POVendorInvoiceProcessor
    {
        private IPOVendorInvoiceDA poVendorDA = ObjectFactory<IPOVendorInvoiceDA>.Instance;
        private ICommonBizInteract createLog = ObjectFactory<ICommonBizInteract>.Instance;

        public POVendorInvoiceInfo Create(POVendorInvoiceInfo input)
        {
            POVendorInvoiceInfo entity = poVendorDA.Create(input);
            createLog.CreateOperationLog(GetMessageString("POVendorInvoice_Log_Create", ServiceContext.Current.UserSysNo, entity.SysNo)
                , BizLogType.PO_Vendor_Invoice_Add
                , entity.SysNo.Value
                , entity.CompanyCode);
            return entity;
        }

        public void Update(POVendorInvoiceInfo entity)
        {
            poVendorDA.Update(entity);
            createLog.CreateOperationLog(GetMessageString("POVendorInvoice_Log_Update", ServiceContext.Current.UserSysNo, entity.SysNo)
               , BizLogType.PO_Vendor_Invoice_Update
               , entity.SysNo.Value
               , entity.CompanyCode);
        }

        public void UnAbandon(int sysNo)
        {
            POVendorInvoiceInfo poVendor = poVendorDA.GetPOVendorInvoiceBySysNo(sysNo);
            if (poVendor == null)
            {
                ThrowBizException("POVendorInvoice_InvoiceNotFound", sysNo);
            }

            if (poVendor.Status != InvoiceStatus.Abandon)
            {
                ThrowBizException("POVendorInvoice_UnAbandon_StatusNotMatchAbandon");
            }
            poVendorDA.UpdateStatus(sysNo, InvoiceStatus.Origin);

            createLog.CreateOperationLog(GetMessageString("POVendorInvoice_Log_UnAbandon", ServiceContext.Current.UserSysNo, sysNo)
               , BizLogType.PO_Vendor_Invoice_Update
               , sysNo
               , poVendor.CompanyCode);
        }

        public void Abandon(int sysNo)
        {
            POVendorInvoiceInfo poVendor = poVendorDA.GetPOVendorInvoiceBySysNo(sysNo);
            if (poVendor == null)
            {
                ThrowBizException("POVendorInvoice_InvoiceNotFound", sysNo);
            }

            if (poVendor.Status != InvoiceStatus.Origin)
            {
                ThrowBizException("POVendorInvoice_Abandon_StatusNotMatchOrigin");
            }
            poVendorDA.UpdateStatus(sysNo, InvoiceStatus.Abandon);

            createLog.CreateOperationLog(GetMessageString("POVendorInvoice_Log_Abandon", ServiceContext.Current.UserSysNo, sysNo)
               , BizLogType.PO_Vendor_Invoice_Update
               , sysNo
               , poVendor.CompanyCode);
        }

        public void UnAudit(int sysNo)
        {
            POVendorInvoiceInfo poVendor = poVendorDA.GetPOVendorInvoiceBySysNo(sysNo);
            if (poVendor == null)
            {
                ThrowBizException("POVendorInvoice_InvoiceNotFound", sysNo);
            }

            if (poVendor.Status != InvoiceStatus.Audited)
            {
                ThrowBizException("POVendorInvoice_UnAudit_StatusNotMatchAudited");
            }
            poVendorDA.UpdateStatus(sysNo, InvoiceStatus.Origin);

            createLog.CreateOperationLog(GetMessageString("POVendorInvoice_Log_UnAudit", ServiceContext.Current.UserSysNo, sysNo)
               , BizLogType.PO_Vendor_Invoice_Update
               , sysNo
               , poVendor.CompanyCode);
        }

        public void Audit(int sysNo)
        {
            POVendorInvoiceInfo poVendor = poVendorDA.GetPOVendorInvoiceBySysNo(sysNo);
            if (poVendor == null)
            {
                ThrowBizException("POVendorInvoice_InvoiceNotFound", sysNo);
            }

            if (poVendor.Status != InvoiceStatus.Origin)
            {
                ThrowBizException("POVendorInvoice_Audit_StatusNotMatchOrigin");
            }
            poVendorDA.Audit(sysNo, InvoiceStatus.Audited);

            createLog.CreateOperationLog(GetMessageString("POVendorInvoice_Log_Audit", ServiceContext.Current.UserSysNo, sysNo)
               , BizLogType.PO_Vendor_Invoice_Update
               , sysNo
               , poVendor.CompanyCode);
        }

        #region Helper Methods

        private void ThrowBizException(string msgKeyName, params object[] args)
        {
            throw new BizException(GetMessageString(msgKeyName, args));
        }

        private string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.POVendorInvoice, msgKeyName), args);
        }

        #endregion Helper Methods
    }
}