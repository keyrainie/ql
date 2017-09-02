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
    [VersionExport(typeof(ISaleGiftDA))]
    public class SaleGiftDA : ISaleGiftDA
    {
        public SaleGiftInfo Load(int? sysNo)
        {
            SaleGiftInfo info = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_LoadSaleGift");
            cmd.SetParameterValue("@SysNo", sysNo);
            DataSet ds = cmd.ExecuteDataSet();
            DataTable dtMaster = ds.Tables[0];
            if (dtMaster.Rows.Count == 0)
            {
                return info;
            }
            info = DataMapper.GetEntity<SaleGiftInfo>(dtMaster.Rows[0], (row, entity) =>
            {
                entity.Title = new LanguageContent(row["PromotionName"].ToString().Trim());
                entity.Description = new LanguageContent(row["PromotionDesc"] != null ? row["PromotionDesc"].ToString().Trim() : "");
                entity.OrderCondition = new PSOrderCondition();
                if (row["AmountLimit"] != null && !string.IsNullOrEmpty(row["AmountLimit"].ToString().Trim()))
                {
                    entity.OrderCondition.OrderMinAmount = Math.Round(decimal.Parse(row["AmountLimit"].ToString()), 2);
                }
                entity.IsGlobalProduct = row["IsGlobal"] != null ? (row["IsGlobal"].ToString().Trim() == "Y" ? true : false) : false;
            });

            info.ProductCondition = new List<SaleGift_RuleSetting>();
            DataTable dt2 = ds.Tables[1];
            if (dt2 != null && dt2.Rows.Count > 0)
            {
                info.ProductCondition = DataMapper.GetEntityList<SaleGift_RuleSetting, List<SaleGift_RuleSetting>>(dt2.Rows);
            }

            DataTable dt3 = ds.Tables[2];
            if (dt3 != null && dt3.Rows.Count > 0)
            {
                info.GiftItemList = DataMapper.GetEntityList<RelProductAndQty, List<RelProductAndQty>>(dt3.Rows);
            }
            return info;
        }

        public List<SaleGiftInfo> LoadAllRunSaleGiftList()
        {
            List<SaleGiftInfo> list = new List<SaleGiftInfo>();
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_LoadAllRunSaleGift");
            DataSet ds = cmd.ExecuteDataSet();

            DataTable dtMaster = ds.Tables[0];
            if (dtMaster == null || dtMaster.Rows.Count == 0)
            {
                return list;
            }
            list = DataMapper.GetEntityList<SaleGiftInfo, List<SaleGiftInfo>>(dtMaster.Rows);

            List<SaleGift_RuleSetting> saleRuleList = new List<SaleGift_RuleSetting>();
            DataTable dt2 = ds.Tables[1];
            if (dt2 != null && dt2.Rows.Count > 0)
            {
                saleRuleList = DataMapper.GetEntityList<SaleGift_RuleSetting, List<SaleGift_RuleSetting>>(dt2.Rows);
            }
            List<RelProductAndQty> giftRuleList = new List<RelProductAndQty>();
            DataTable dt3 = ds.Tables[2];
            if (dt3 != null && dt3.Rows.Count > 0)
            {
                giftRuleList = DataMapper.GetEntityList<RelProductAndQty, List<RelProductAndQty>>(dt3.Rows);
            }

            foreach (SaleGiftInfo info in list)
            {
                info.ProductCondition = new List<SaleGift_RuleSetting>();
                foreach (SaleGift_RuleSetting salerule in saleRuleList)
                {
                    if (salerule.PromotionSysNo == info.SysNo)
                    {
                        info.ProductCondition.Add(salerule);
                    }
                }

                info.GiftItemList = new List<RelProductAndQty>();
                foreach (RelProductAndQty giftrule in giftRuleList)
                {
                    if (giftrule.PromotionSysNo == info.SysNo)
                    {
                        info.GiftItemList.Add(giftrule);
                    }
                }
            }

            return list;
        }

        public int? CreateMaster(SaleGiftInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_CreateSaleGiftMaster");
            cmd.SetParameterValue("@PromotionName", info.Title.Content);
            cmd.SetParameterValue("@PromotionDesc", info.Description.Content);
            cmd.SetParameterValue("@Type", info.Type);
            cmd.SetParameterValue("@Status", SaleGiftStatus.Init);
            cmd.SetParameterValue("@BeginDate", info.BeginDate);
            cmd.SetParameterValue("@Enddate", info.EndDate);
            cmd.SetParameterValue("@AmountLimit", info.OrderCondition.OrderMinAmount == null ? 0 : info.OrderCondition.OrderMinAmount.Value);
            cmd.SetParameterValue("@PromotionLink", info.PromotionLink);
            cmd.SetParameterValue("@Memo", info.Memo);
            cmd.SetParameterValue("@InUser", info.InUser ?? "");
            cmd.SetParameterValue("@DisCountType", info.DisCountType);
            cmd.SetParameterValue("@IsGlobal", info.IsGlobalProduct.HasValue && info.IsGlobalProduct.Value ? "Y" : "N");
            cmd.SetParameterValue("@CompanyCode", info.CompanyCode);
            cmd.SetParameterValue("@VendorSysNo", info.VendorSysNo);
            cmd.ExecuteNonQuery();
            info.SysNo = (int)cmd.GetParameterValue("@SysNo");
            return info.SysNo;

        }

        public void UpdateMaster(SaleGiftInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_UpdateSaleGiftMaster");
            cmd.SetParameterValue("@SysNo", info.SysNo);
            cmd.SetParameterValue("@PromotionName", info.Title.Content);
            cmd.SetParameterValue("@PromotionDesc", info.Description.Content);
            cmd.SetParameterValue("@BeginDate", info.BeginDate);
            cmd.SetParameterValue("@Enddate", info.EndDate);
            cmd.SetParameterValue("@AmountLimit", info.OrderCondition.OrderMinAmount);
            cmd.SetParameterValue("@PromotionLink", info.PromotionLink);
            cmd.SetParameterValue("@Memo", info.Memo);
            cmd.SetParameterValue("@EditUser", info.EditUser);
            cmd.SetParameterValue("@DisCountType", info.DisCountType);
            cmd.SetParameterValue("@VendorSysNo", info.VendorSysNo);
            cmd.ExecuteNonQuery();
        }

        public List<SaleGiftInfo> GetGiftInfoListByProductSysNo(int productSysNo, SaleGiftStatus status)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_GetGiftInfoListByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@Status", status);

            return cmd.ExecuteEntityList<SaleGiftInfo>();
        }

        public List<SaleGiftInfo> GetGiftItemListByProductSysNo(int productSysNo, SaleGiftStatus status)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_GetGiftItemListByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@Status", status);

            return cmd.ExecuteEntityList<SaleGiftInfo>();
        }

        public void UpdateStatus(int sysNo, SaleGiftStatus status, string userName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_UpdateSaleGiftStatus");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@Status", status);
            cmd.SetParameterValue("@EditUser", userName);
            cmd.ExecuteNonQuery();
        }

        public void UpdateGiftIsGlobal(int promotionSysNo, bool isGlobal, string userName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_UpdateGiftIsGlobal");
            cmd.SetParameterValue("@PromotionSysNo", promotionSysNo);
            cmd.SetParameterValue("@IsGlobal", isGlobal ? "Y" : "N");
            cmd.SetParameterValue("@EditUser", userName);

            cmd.ExecuteNonQuery();
        }


        public void DeleteSaleRules(int promotionSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_DeleteSaleRules");
            cmd.SetParameterValue("@PromotionSysNo", promotionSysNo);
            cmd.ExecuteNonQuery();
        }

        public void CreateGloableSaleRules(int promotionSysNo, SaleGiftInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_CreateSaleRules");
            cmd.SetParameterValue("@PromotionSysNo", promotionSysNo);
            //cmd.SetParameterValue("@Type", info.Type);
            cmd.SetParameterValue("@Type", SaleGiftSaleRuleType.Brand);
            cmd.SetParameterValue("@C3SysNo", null);
            cmd.SetParameterValue("@BrandSysNo", null);
            cmd.SetParameterValue("@ProductSysNo", null);
            cmd.SetParameterValue("@BuyCount", null);
            cmd.SetParameterValue("@ComboType", info.GiftComboType);
            cmd.ExecuteNonQuery();
        }

        public void CreateSaleRules(int promotionSysNo, SaleGift_RuleSetting setting)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_CreateSaleRules");
            cmd.SetParameterValue("@PromotionSysNo", promotionSysNo);
            cmd.SetParameterValue("@Type", setting.Type);
            cmd.SetParameterValue("@C3SysNo", setting.RelC3 != null ? setting.RelC3.SysNo : null);
            cmd.SetParameterValue("@BrandSysNo", setting.RelBrand != null ? setting.RelBrand.SysNo : null);
            cmd.SetParameterValue("@ProductSysNo", setting.RelProduct != null ? setting.RelProduct.ProductSysNo : null);
            cmd.SetParameterValue("@BuyCount", setting.RelProduct != null ? setting.RelProduct.MinQty : null);
            cmd.SetParameterValue("@ComboType", setting.ComboType);
            cmd.ExecuteNonQuery();
        }

        public void DeleteGiftItemRules(int promotionSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_DeleteGiftRules");
            cmd.SetParameterValue("@PromotionSysNo", promotionSysNo);
            cmd.ExecuteNonQuery();
        }

        public void CreateGiftItemRules(int promotionSysNo, RelProductAndQty setting)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_CreateGiftRules");
            cmd.SetParameterValue("@PromotionSysNo", promotionSysNo);
            cmd.SetParameterValue("@ProductSysNo", setting.ProductSysNo);
            cmd.SetParameterValue("@Count", setting.Count);
            cmd.SetParameterValue("@Priority", setting.Priority);
            cmd.SetParameterValue("@PlusPrice", setting.PlusPrice);
            cmd.ExecuteNonQuery();
        }

        public void UpdateGiftItemCount(int promotionSysNo, SaleGiftGiftItemType giftComboType, int? itemGiftCount, string userName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_UpdateGiftItemCount");
            cmd.SetParameterValue("@PromotionSysNo", promotionSysNo);
            cmd.SetParameterValue("@GiftComboType", giftComboType);
            cmd.SetParameterValue("@ItemGiftCount", itemGiftCount);
            cmd.SetParameterValue("@InUser", userName);
            cmd.ExecuteNonQuery();
        }

        public bool CheckExistMultiSameGiftItem(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_CheckGiftRulesForVendor");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            int count = cmd.ExecuteScalar<int>();
            return count > 0;
        }

        public bool ProductIsGift(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_ProductIsGift");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            int count = cmd.ExecuteScalar<int>();
            return count > 0;
        }

        public bool CheckMarketIsActivity(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CheckMarketIsActivity");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            int count = cmd.ExecuteScalar<int>();
            return count > 0;
        }

        public List<ProductPromotionDiscountInfo> GetGiftAmount(int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("SaleGift_GetGiftAmount");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            var result = dc.ExecuteEntityList<ProductPromotionDiscountInfo>();
            if (result == null || result.Count == 0)
            {
                result = GetGiftAmountByGift(productSysNo);
            }
            return result;
        }

        private List<ProductPromotionDiscountInfo> GetGiftAmountByGift(int productSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetGiftAmountByGift");
            dc.SetParameterValue("@ProductSysNo", productSysNo);
            return dc.ExecuteEntityList<ProductPromotionDiscountInfo>();
        }
        public void SyncGiftStatus(int requestSysNo, SaleGiftStatus status)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("SaleGift_SyncGiftStatus");
            dataCommand.SetParameterValue("@RequestSysNo", requestSysNo);
            dataCommand.SetParameterValue("@Status", status);
            dataCommand.ExecuteNonQuery();
        }
        public List<RelVendor> GetGiftVendorList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_GetVendorList");
            return cmd.ExecuteEntityList<RelVendor>();
        }

        public int GetVendorSysNoByProductSysNo(int productsysno)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("SaleGift_VendorSysNoByProductSysNo");
            dataCommand.SetParameterValue("@ProductSysNo", productsysno);
            return dataCommand.ExecuteScalar<int>();
        }
    }
}
