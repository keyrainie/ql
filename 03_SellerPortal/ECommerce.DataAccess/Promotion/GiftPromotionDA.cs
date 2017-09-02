using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECommerce.Entity.Common;
using ECommerce.Entity.Promotion;
using ECommerce.Enums;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Promotion
{
    public class GiftPromotionDA
    {
        public static QueryResult<GiftPromotionListQueryResult> QueryGiftPromotionActivityList(GiftPromotionListQueryFilter queryFilter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SaleGift_QueryGift");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, queryFilter, string.IsNullOrEmpty(queryFilter.SortFields) ? "SysNo DESC" : queryFilter.SortFields))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.VendorSysNo", DbType.Int32,
                 "@VendorSysNo", QueryConditionOperatorType.Equal, queryFilter.VendorSysNo);
                if (!string.IsNullOrEmpty(queryFilter.CompanyCode))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CompanyCode", DbType.AnsiStringFixedLength,
                   "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);
                }

                //主商品编号:
                if (!string.IsNullOrEmpty(queryFilter.MainProductSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.ProductSysNo",
                        DbType.Int32, "@MasterProductSysNo", QueryConditionOperatorType.Equal, queryFilter.MainProductSysNo);
                }
                //赠品编号:
                if (!string.IsNullOrEmpty(queryFilter.GiftProductSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.ProductSysNo",
               DbType.Int32, "@GiftProductSysNo", QueryConditionOperatorType.Equal, queryFilter.GiftProductSysNo);
                }
                if (!string.IsNullOrEmpty(queryFilter.ActivitySysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.SysNo",
                        DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, queryFilter.ActivitySysNo);
                }
                if (!string.IsNullOrEmpty(queryFilter.ActivityName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.PromotionName",
                     DbType.String, "@PromotionName", QueryConditionOperatorType.Like, queryFilter.ActivityName);
                }
                if (!string.IsNullOrEmpty(queryFilter.ActivityStatus))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Status",
                        DbType.AnsiStringFixedLength, "@Status", QueryConditionOperatorType.Equal, queryFilter.ActivityStatusEnumText);
                }
                if (!string.IsNullOrEmpty(queryFilter.ActivityType))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Type",
                        DbType.AnsiStringFixedLength, "@Type", QueryConditionOperatorType.Equal, queryFilter.ActivityTypeEnumText);
                }
                if (!string.IsNullOrEmpty(queryFilter.ActivityDateFrom))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.BeginDate",
                       DbType.DateTime, "@BeginDate", QueryConditionOperatorType.MoreThanOrEqual, Convert.ToDateTime(queryFilter.ActivityDateFrom));
                }
                if (!string.IsNullOrEmpty(queryFilter.ActivityDateTo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.EndDate",
                       DbType.DateTime, "@EndDate", QueryConditionOperatorType.LessThan, Convert.ToDateTime(queryFilter.ActivityDateTo).AddDays(1));
                }

                command.CommandText = sqlBuilder.BuildQuerySql();
                List<GiftPromotionListQueryResult> resultList = command.ExecuteEntityList<GiftPromotionListQueryResult>();

                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                return new QueryResult<GiftPromotionListQueryResult>() { PageInfo = new PageInfo() { PageIndex = queryFilter.PageIndex, PageSize = queryFilter.PageSize, TotalCount = totalCount, SortBy = queryFilter.SortFields }, ResultList = resultList };
            }
        }

        public static SalesGiftInfo LoadSalesGiftInfo(int sysNo)
        {
            SalesGiftInfo info = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_LoadSaleGift");
            cmd.SetParameterValue("@SysNo", sysNo);
            DataSet ds = cmd.ExecuteDataSet();
            DataTable dtMaster = ds.Tables[0];
            if (dtMaster.Rows.Count == 0)
            {
                return info;
            }
            info = DataMapper.GetEntity<SalesGiftInfo>(dtMaster.Rows[0], (row, entity) =>
            {
                entity.Title = row["PromotionName"].ToString().Trim();
                entity.Description = row["PromotionDesc"] != null ? row["PromotionDesc"].ToString().Trim() : "";
                //entity.OrderCondition = new PSOrderCondition();
                if (row["AmountLimit"] != null && !string.IsNullOrEmpty(row["AmountLimit"].ToString().Trim()))
                {
                    entity.OrderMinAmount = Math.Round(decimal.Parse(row["AmountLimit"].ToString()), 2);
                }
                entity.IsGlobalProduct = row["IsGlobal"] != null ? (row["IsGlobal"].ToString().Trim() == "Y" ? true : false) : false;
            });

            DataTable dt2 = ds.Tables[1];
            if (dt2 != null && dt2.Rows.Count > 0)
            {
                info.ProductRuleList = DataMapper.GetEntityList<SalesGiftMainProductRuleInfo, List<SalesGiftMainProductRuleInfo>>(dt2.Rows);
            }

            DataTable dt3 = ds.Tables[2];
            if (dt3 != null && dt3.Rows.Count > 0)
            {
                info.GiftRuleList = DataMapper.GetEntityList<SalesGiftProductRuleInfo, List<SalesGiftProductRuleInfo>>(dt3.Rows);
            }
            return info;
        }

        public static int SaveGiftPromotionMasterInfo(SalesGiftInfo info)
        {
            if (!info.SysNo.HasValue)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_CreateSaleGiftMaster");
                cmd.SetParameterValue("@PromotionName", info.Title);
                cmd.SetParameterValue("@PromotionDesc", info.Description);
                object typeCode = null;
                EnumCodeMapper.TryGetCode(info.Type.Value, out typeCode);
                cmd.SetParameterValue("@Type", typeCode.ToString());
                cmd.SetParameterValue("@Status", SaleGiftStatus.Origin);
                cmd.SetParameterValue("@BeginDate", info.BeginDate.Value);
                cmd.SetParameterValue("@Enddate", info.EndDate.Value);
                cmd.SetParameterValue("@AmountLimit", !info.OrderMinAmount.HasValue ? 0 : info.OrderMinAmount.Value);
                cmd.SetParameterValue("@PromotionLink", info.PromotionLink);
                cmd.SetParameterValue("@Memo", info.Memo);
                cmd.SetParameterValue("@InUser", info.InUserName ?? "");
                cmd.SetParameterValue("@DisCountType", SaleGiftDiscountBelongType.BelongMasterItem);
                cmd.SetParameterValue("@IsGlobal", info.IsGlobalProduct == true ? "Y" : "N");
                cmd.SetParameterValue("@CompanyCode", info.CompanyCode);
                cmd.SetParameterValue("@VendorSysNo", info.SellerSysNo);
                cmd.ExecuteNonQuery();
                info.SysNo = (int)cmd.GetParameterValue("@SysNo");
                return info.SysNo.Value;
            }
            else
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_UpdateSaleGiftMaster");
                cmd.SetParameterValue("@SysNo", info.SysNo);
                cmd.SetParameterValue("@PromotionName", info.Title);
                object typeCode = null;
                EnumCodeMapper.TryGetCode(info.Type.Value, out typeCode);
                cmd.SetParameterValue("@Type", typeCode);
                cmd.SetParameterValue("@PromotionDesc", info.Description);
                cmd.SetParameterValue("@BeginDate", info.BeginDate);
                cmd.SetParameterValue("@Enddate", info.EndDate.Value);
                cmd.SetParameterValue("@AmountLimit", !info.OrderMinAmount.HasValue ? 0 : info.OrderMinAmount.Value);
                cmd.SetParameterValue("@PromotionLink", info.PromotionLink);
                cmd.SetParameterValue("@Memo", info.Memo);
                cmd.SetParameterValue("@EditUser", info.EditUser);
                cmd.SetParameterValue("@DisCountType", SaleGiftDiscountBelongType.BelongMasterItem);
                cmd.SetParameterValue("@VendorSysNo", info.VendorSysNo);
                cmd.ExecuteNonQuery();
                return info.SysNo.Value;
            }
        }

        public static int GetVendorSysNoByProductSysNo(int productsysno)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("SaleGift_VendorSysNoByProductSysNo");
            dataCommand.SetParameterValue("@ProductSysNo", productsysno);
            return dataCommand.ExecuteScalar<int>();
        }

        public static void UpdateGiftPromotionStatus(int sysNo, SaleGiftStatus status, string userName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_UpdateSaleGiftStatus");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@Status", status);
            cmd.SetParameterValue("@EditUser", userName);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteSaleRules(int promotionSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_DeleteSaleRules");
            cmd.SetParameterValue("@PromotionSysNo", promotionSysNo);
            cmd.ExecuteNonQuery();
        }

        public static void CreateSaleRules(int promotionSysNo, SalesGiftMainProductRuleInfo setting)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_CreateSaleRules");
            cmd.SetParameterValue("@PromotionSysNo", promotionSysNo);
            cmd.SetParameterValue("@Type", setting.Type);
            cmd.SetParameterValue("@C3SysNo", setting.C3SysNo != null ? setting.C3SysNo : null);
            cmd.SetParameterValue("@BrandSysNo", setting.BrandSysNo != null ? setting.BrandSysNo : null);
            cmd.SetParameterValue("@ProductSysNo", setting.ProductSysNo != null ? setting.ProductSysNo : null);
            cmd.SetParameterValue("@BuyCount", setting.BuyCount != null ? setting.BuyCount : null);
            cmd.SetParameterValue("@ComboType", setting.ComboType);
            cmd.ExecuteNonQuery();
        }

        public static void UpdateGiftIsGlobal(int promotionSysNo, bool isGlobal, string userName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_UpdateGiftIsGlobal");
            cmd.SetParameterValue("@PromotionSysNo", promotionSysNo);
            cmd.SetParameterValue("@IsGlobal", isGlobal ? "Y" : "N");
            cmd.SetParameterValue("@EditUser", userName);

            cmd.ExecuteNonQuery();
        }

        public static void DeleteGiftItemRules(int promotionSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_DeleteGiftRules");
            cmd.SetParameterValue("@PromotionSysNo", promotionSysNo);
            cmd.ExecuteNonQuery();
        }

        public static void CreateGiftItemRules(int promotionSysNo, SalesGiftProductRuleInfo setting)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_CreateGiftRules");
            cmd.SetParameterValue("@PromotionSysNo", promotionSysNo);
            cmd.SetParameterValue("@ProductSysNo", setting.ProductSysNo);
            cmd.SetParameterValue("@Count", setting.Count);
            cmd.SetParameterValue("@Priority", setting.Priority);
            cmd.SetParameterValue("@PlusPrice", setting.PlusPrice);
            cmd.ExecuteNonQuery();
        }

        public static void UpdateGiftItemCount(int promotionSysNo, SaleGiftGiftItemType giftComboType, int? itemGiftCount, string userName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_UpdateGiftItemCount");
            cmd.SetParameterValue("@PromotionSysNo", promotionSysNo);
            cmd.SetParameterValue("@GiftComboType", giftComboType == SaleGiftGiftItemType.AssignGift ? "A" : "O");
            cmd.SetParameterValue("@ItemGiftCount", itemGiftCount);
            cmd.SetParameterValue("@InUser", userName);
            cmd.ExecuteNonQuery();
        }

        public static List<SalesGiftInfo> GetGiftItemListByProductSysNo(int productSysNo, SaleGiftStatus status)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_GetGiftItemListByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@Status", status);

            return cmd.ExecuteEntityList<SalesGiftInfo>();
        }

        public static void UpdateStatus(int sysNo, SaleGiftStatus status, string userName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_UpdateSaleGiftStatus");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@Status", status);
            cmd.SetParameterValue("@EditUser", userName);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据赠品编号判断此赠品活动是否在审核中，就绪中，运行中
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns>true:正在此三种状态中；false：</returns>
        public static bool CheckGiftItemListByProductSysNo(int productSysNo)
        {
            bool result = false;
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_CheckGiftItemListByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            int i = cmd.ExecuteScalar<Int32>();
            if (i > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
    }
}
