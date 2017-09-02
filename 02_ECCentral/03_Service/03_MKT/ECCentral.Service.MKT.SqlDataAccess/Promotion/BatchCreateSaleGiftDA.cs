using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.Promotion;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity;

namespace ECCentral.Service.MKT.SqlDataAccess.Promotion
{
    [VersionExport(typeof(IBatchCreateSaleGiftDA))]
    public class BatchCreateSaleGiftDA : IBatchCreateSaleGiftDA
    {

        #region IBatchCreateSaleGiftDA Members

        public int CheckGiftRulesForVendor(int? productSysNo, bool isVendor, string CompanyCode)
        {
            DataCommand dataCommand = dataCommand = DataCommandManager.GetDataCommand("BatchCreateSaleGift_CheckGiftRulesForVendor");

            dataCommand.SetParameterValue("@ProductSysNo", productSysNo.Value);
            dataCommand.SetParameterValue("@IsVendor",isVendor?1:0);
            dataCommand.SetParameterValue("@CompanyCode", CompanyCode);

            return dataCommand.ExecuteScalar<int>();
        }

        public void CreateGiftRules(ProductItemInfo productItem, BatchCreateGiftRuleInfo ruleInfo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("SaleGift_CreateGiftRules");
            dataCommand.SetParameterValue("@PromotionSysNo", ruleInfo.PromotionSysNo);
            dataCommand.SetParameterValue("@ProductSysNo", productItem.ProductSysNo);
            dataCommand.SetParameterValue("@Count", productItem.HandselQty);
            dataCommand.SetParameterValue("@Priority", productItem.Priority);
            dataCommand.ExecuteNonQuery();
        }

        public void UpdateItemGiftCouontGiftRules(int promotionSysNo, int? count, SaleGiftGiftItemType isGiftPool, string companyCode, string username, int special)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("SaleGift_UpdateItemGiftCountPromotions");
            dataCommand.SetParameterValue("@PromotionSysNo", promotionSysNo);
            dataCommand.SetParameterValue("@IsSpecial", special);
            dataCommand.SetParameterValue("@GiftComboType", isGiftPool);
            dataCommand.SetParameterValue("@ItemGiftCount", count);
            dataCommand.SetParameterValue("@InUser", username);
            dataCommand.SetParameterValue("@CompanyCode", companyCode);
            dataCommand.ExecuteNonQuery();
        }

       

        public void DeleteSaleRules(string promotionSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("BatchCreateSaleGift_DeleteSaleRules");
            dataCommand.SetParameterValue("@PromotionSysNo", promotionSysNo);
            dataCommand.ExecuteNonQuery();
        }

        public int CheckIsVendorGift(int? productSysNo, string companyCode)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("SaleGift_CheckIsVendorGift");
            dataCommand.SetParameterValue("@ProductSysNo", productSysNo);
            dataCommand.SetParameterValue("@CompanyCode", companyCode);
            return dataCommand.ExecuteScalar<int>();
        }

       

        public void CreateSaleRules(BatchCreateSaleGiftSaleRuleInfo info, ProductItemInfo entity)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("SaleGift_CreateSaleRules");
            dataCommand.SetParameterValue("@PromotionSysNo", info.PromotionSysNo);
            dataCommand.SetParameterValue("@Type", "I");
            dataCommand.SetParameterValue("@ComboType","A");
            dataCommand.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            dataCommand.SetParameterValue("@C3SysNo", entity.C3SysNo);
            dataCommand.SetParameterValue("@BrandSysNo", entity.BrandSysNo);
            dataCommand.SetParameterValue("@BuyCount", entity.HandselQty == 0 ? null : entity.HandselQty);
            dataCommand.ExecuteNonQuery();
        }

        public void UpdateGiftIsGlobal(int promotionSysNo, string isGlobal, string companyCode, string user)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("BatchCreateSaleGift_UpdateGiftIsGlobal");
            dataCommand.SetParameterValue("@PromotionSysNo", promotionSysNo);
            dataCommand.SetParameterValue("@IsGlobal", isGlobal);
            dataCommand.SetParameterValue("@CompanyCode", companyCode);
            dataCommand.SetParameterValue("@EditUser", user);
            dataCommand.ExecuteNonQuery();
        }

        #endregion
    }
}
