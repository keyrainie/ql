using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.BizProcessor;
using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.PO.AppService
{
    [VersionExport(typeof(VendorRefundAppService))]
    public class VendorRefundAppService
    {

        #region [Fields]
        private VendorRefundProcessor m_VendorRefundProcessor;
        private IInvoiceBizInteract m_InvoiceBizInteract;

        public VendorRefundProcessor VendorRefundProcessor
        {
            get
            {
                if (null == m_VendorRefundProcessor)
                {
                    m_VendorRefundProcessor = ObjectFactory<VendorRefundProcessor>.Instance;
                }
                return m_VendorRefundProcessor;
            }
        }

        public IInvoiceBizInteract InvoiceBizInteract
        {
            get
            {
                if (null == m_InvoiceBizInteract)
                {
                    m_InvoiceBizInteract = ObjectFactory<IInvoiceBizInteract>.Instance;
                }
                return m_InvoiceBizInteract;
            }
        }
        #endregion

        public VendorRefundInfo LoadVendorRefundInfo(int refundSysNo)
        {
            return VendorRefundProcessor.LoadVendorRefundInfo(refundSysNo);
        }

        public VendorRefundInfo ApproveVendorRefundInfo(VendorRefundInfo refundInfo)
        {
            //获取当前操作人所在组 （PM,PMD,PMCC），根据级别来判断：
            switch (refundInfo.UserRole)
            {
                case "PM":
                    return VendorRefundProcessor.PMApproveVendorRefund(refundInfo);
                case "PMD":
                    return VendorRefundProcessor.PMDApproveVendorRefund(refundInfo);
                case "PMCC":
                    return VendorRefundProcessor.PMCCApproveVendorRefund(refundInfo);
                default:
                    return null;
            }
        }

        public VendorRefundInfo RejectVendorRefundInfo(VendorRefundInfo refundInfo)
        {
            //获取当前操作人所在组 （PM,PMD,PMCC），根据级别来判断：
            switch (refundInfo.UserRole)
            {
                case "PM":
                    return VendorRefundProcessor.PMReject(refundInfo);
                case "PMD":
                    return VendorRefundProcessor.PMDReject(refundInfo);
                case "PMCC":
                    return VendorRefundProcessor.PMCCReject(refundInfo);
                default:
                    return null;
            }
        }

        public VendorRefundInfo UpdateVendorRefundInfo(VendorRefundInfo refundInfo)
        {
            return VendorRefundProcessor.UpdateVendorRefund(refundInfo);
        }

        public decimal GetVendorPayBalanceByVendorSysNo(int vendorSysNo)
        {
            return InvoiceBizInteract.GetVendorPayBalanceByVendorSysNo(vendorSysNo);
        }

        public VendorRefundInfo SubmitToPMCC(VendorRefundInfo entity)
        {
            return VendorRefundProcessor.SubmitToPMCC(entity);
        }
    }
}
