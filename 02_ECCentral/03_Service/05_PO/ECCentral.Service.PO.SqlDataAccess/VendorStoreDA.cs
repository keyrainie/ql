using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO.Vendor;
using IPP.Oversea.CN.POASNMgmt.DataAccess;

namespace ECCentral.Service.PO.SqlDataAccess
{
    [VersionExport(typeof(IVendorStoreDA))]
    public class VendorStoreDA : IVendorStoreDA
    {
        public VendorStoreInfo Create(VendorStoreInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateVendorStore");
            command.SetParameterValue(entity);

            command.ExecuteNonQuery();

            entity.SysNo = Convert.ToInt32(command.GetParameterValue("@SysNo"));
            return entity;
        }

        public VendorStoreInfo Update(VendorStoreInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateVendorStore");
            command.SetParameterValue(entity);

            command.ExecuteNonQuery();

            return Load(entity.SysNo.Value);
        }

        public VendorStoreInfo Load(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadVendorStoreBySysNo");
            command.SetParameterValue("@SysNo", sysNo);
            return command.ExecuteEntity<VendorStoreInfo>();
        }

        public bool CheckVendorStoreNameExists(int sysNo, string name, int vendorSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckVendorStoreNameExists");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@Name", name);
            command.SetParameterValue("@VendorSysNo", vendorSysNo);

            int result = command.ExecuteScalar<int>();

            return result > 0;
        }

        public List<VendorStoreInfo> GetVendorStoreInfoList(int vendorSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorStoreInfoList");
            command.SetParameterValue("@VendorSysNo", vendorSysNo);
            return command.ExecuteEntityList<VendorStoreInfo>();
        }


        public void UpdateCommissionRuleTemplate(BizEntity.PO.Vendor.CommissionRuleTemplateInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateCommissionRuleTemplate");
            command.SetParameterValue<CommissionRuleTemplateInfo>(info);
            command.ExecuteNonQuery();
        }


        public void SetCommissionRuleStatus(string sysnos, CommissionRuleStatus commissionRuleStatus)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SetCommissionRuleStatus");
            command.SetParameterValue("@Status", commissionRuleStatus);
            command.CommandText = command.CommandText.Replace("#sysnos#", sysnos);
            command.ExecuteNonQuery();
        }

        public BizEntity.PO.Vendor.CommissionRuleTemplateInfo GetCommissionRuleTemplateInfo(int sysno)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetCommissionRuleTemplateInfo");
            command.SetParameterValue("@SysNo", sysno);
            return command.ExecuteEntity<CommissionRuleTemplateInfo>();
        }

        public int CreateStorePageHeader(StorePageHeader info)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("CreateStorePageHeader");
            command.SetParameterValue(info);
            return command.ExecuteNonQuery();
            
        }

        public int CreateStorePageInfo(StorePageInfo info)
        {
            var command = DataCommandManager.GetDataCommand("CreateStorePageInfo");
            command.SetParameterValue(info);
            command.ExecuteNonQuery();
            return Convert.ToInt32(command.GetParameterValue("@SysNo"));
        }

        public int CreatePublishedStorePageInfo(PublishedStorePageInfo info)
        {
            var command = DataCommandManager.GetDataCommand("CreatePublishedStorePageInfo");
            command.SetParameterValue(info);
            command.ExecuteNonQuery();
            return Convert.ToInt32(command.GetParameterValue("@SysNo"));
        }

        public List<StorePageInfo> GetStorePageInfoListBySeller(int sellerSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetStorePageInfoListBySeller");
            command.SetParameterValue("@SellerSysNo", sellerSysNo);
            return command.ExecuteEntityList<StorePageInfo>();
        }

        public List<PublishedStorePageInfo> GetPublishedStorePageInfoListBySeller(int sellerSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetPublishedStorePageInfoListBySeller");
            command.SetParameterValue("@SellerSysNo", sellerSysNo);
            return command.ExecuteEntityList<PublishedStorePageInfo>();
        }

        public VendorPresetContent GetVendorPresetContent()
        {
            var command = DataCommandManager.GetDataCommand("GetVendorPresetContent");
            string configString = Convert.ToString(command.ExecuteScalar());
            if (string.IsNullOrWhiteSpace(configString))
                return null;

            var data = SerializationUtility.XmlDeserialize<VendorPresetContent>(configString);
            data.StorePageInfos.ForEach(page =>
            {
                var pageTemplate = GetStorePageTemplate(page.TemplateKey, page.PageTypeKey);
                if (pageTemplate != null)
                {
                    page.DataValue = pageTemplate.DataValue;
                }
            });
            return data;
        }

        private PageTemplateInfo GetStorePageTemplate(string key, string pageTypeKey)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetStorePageTemplate");
            cmd.SetParameterValue("@Key", key);
            cmd.SetParameterValue("@PageTypeKey", pageTypeKey);
            PageTemplateInfo result = cmd.ExecuteEntity<PageTemplateInfo>();
            return result;
        }

        public StorePageHeader GetStorePageHeaderBySeller(int sellerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPageHeaderBySellerSysNo");
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            StorePageHeader result = cmd.ExecuteEntity<StorePageHeader>();
            return result;
        }

        /// <summary>
        /// 创建品牌报备
        /// </summary>
        public int InsertStoreBrandFiling(StoreBrandFilingInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertStoreBrandFiling");
            cmd.SetParameterValue<StoreBrandFilingInfo>(entity);
            cmd.ExecuteNonQuery();
            int sysNo = (int)cmd.GetParameterValue("@SysNo");
            return sysNo;
        }

        /// <summary>
        /// 更新品牌报备
        /// </summary>
        public void UpdateStoreBrandFiling(StoreBrandFilingInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateStoreBrandFiling");
            cmd.SetParameterValue<StoreBrandFilingInfo>(entity);
            cmd.ExecuteNonQuery();
        }

        public void DeleteStoreBrandFiling(int sellerSysNo, int brandSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteStoreBrandFiling");
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            cmd.SetParameterValue("@BrandSysNo", brandSysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 商家的品牌商检编号种子递增，返回新的序列号
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <returns></returns>
        public int IncrementStoreBrandInspectionSeed(int sellerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IncrementStoreBrandInspectionSeed");
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            return cmd.ExecuteScalar<int>();
        }

        /// <summary>
        /// 商家商检编号种子递增，返回新的序列号
        /// </summary>
        public int IncrementStoreInspectionSeed(int sellerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IncrementStoreInspectionSeed");
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            return cmd.ExecuteScalar<int>();
        }

        /// <summary>
        /// 写入商家商检号信息到店铺基本信息表。
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <param name="storeInspectionNo"></param>
        /// <param name="userName"></param>
        public void WriteStoreInspectionNo(int sellerSysNo, string storeInspectionNo, string userName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("WriteStoreInspectionNo");
            cmd.SetParameterValue("@InspectionNo", storeInspectionNo);
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            cmd.SetParameterValue("@UserName", userName);
            cmd.ExecuteNonQuery();
        }
    }
}
