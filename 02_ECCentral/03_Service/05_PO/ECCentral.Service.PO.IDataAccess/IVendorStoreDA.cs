using System.Collections.Generic;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.PO.Vendor;

namespace ECCentral.Service.PO.IDataAccess
{
    public interface IVendorStoreDA
    {
        VendorStoreInfo Create(VendorStoreInfo entity);

        VendorStoreInfo Update(VendorStoreInfo entity);

        bool CheckVendorStoreNameExists(int sysNo, string name, int vendorSysNo);

        List<VendorStoreInfo> GetVendorStoreInfoList(int vendorSysNo);

        void UpdateCommissionRuleTemplate(CommissionRuleTemplateInfo info);


        void SetCommissionRuleStatus(string p, CommissionRuleStatus commissionRuleStatus);

        CommissionRuleTemplateInfo GetCommissionRuleTemplateInfo(int sysno);


        int CreateStorePageHeader(StorePageHeader info);
        int CreateStorePageInfo(StorePageInfo info);
        int CreatePublishedStorePageInfo(PublishedStorePageInfo info);
        List<StorePageInfo> GetStorePageInfoListBySeller(int sellerSysNo);
        List<PublishedStorePageInfo> GetPublishedStorePageInfoListBySeller(int sellerSysNo);
        VendorPresetContent GetVendorPresetContent();
        StorePageHeader GetStorePageHeaderBySeller(int sellerSysNo);
        int InsertStoreBrandFiling(StoreBrandFilingInfo entity);
        void UpdateStoreBrandFiling(StoreBrandFilingInfo entity);
        void DeleteStoreBrandFiling(int sellerSysNo, int brandSysNo);
        int IncrementStoreBrandInspectionSeed(int sellerSysNo);
        int IncrementStoreInspectionSeed(int sellerSysNo);
        void WriteStoreInspectionNo(int sellerSysNo, string storeInspectionNo, string userName);
    }
}