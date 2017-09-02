using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.BizProcessor;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.PO.AppService
{
    [VersionExport(typeof(VendorStoreAppService))]
    public class VendorStoreAppService
    {
        public VendorStoreInfo Create(VendorStoreInfo entity)
        {
            return ObjectFactory<VendorStoreProcessor>.Instance.Create(entity);
        }

        public VendorStoreInfo Update(VendorStoreInfo entity)
        {
            return ObjectFactory<VendorStoreProcessor>.Instance.Update(entity);
        }

        public void UpdateCommissionRuleTemplate(BizEntity.PO.Vendor.CommissionRuleTemplateInfo info)
        {
            ObjectFactory<VendorStoreProcessor>.Instance.UpdateCommissionRuleTemplate(info);
        }

        public BizEntity.PO.Vendor.CommissionRuleTemplateInfo GetCommissionRuleTemplateInfo(int sysno)
        {
        return     ObjectFactory<VendorStoreProcessor>.Instance.GetCommissionRuleTemplateInfo(sysno);
        }

        public void BatchSetCommissionRuleStatus(string sysnos, CommissionRuleStatus commissionRuleStatus)
        {
            ObjectFactory<VendorStoreProcessor>.Instance.BatchSetCommissionRuleStatus(sysnos, commissionRuleStatus);
        }

        public List<VendorAgentInfo> GetVendorBrandFilingList(int vendorId)
        {
            return ObjectFactory<VendorProcessor>.Instance.LoadVendorInfo(vendorId).VendorAgentInfoList;
        }

        public void SecondDomainAuditThrough(int SysNo)
        {
            ObjectFactory<VendorStoreProcessor>.Instance.SecondDomainAuditThrough(SysNo);
        }

        public void SecondDomainAuditThroughNot(int SysNo)
        {
            ObjectFactory<VendorStoreProcessor>.Instance.SecondDomainAuditThroughNot(SysNo);
        }
    }
}
