using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.QueryFilter.MKT.Promotion;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ISaleGiftQueryDA))]
    public class SaleGiftQueryDA : ISaleGiftQueryDA
    {

        public System.Data.DataTable QuerySaleGift(SaleGiftQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("SaleGift_QueryGift");


            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.WebChannelID", DbType.AnsiStringFixedLength,
                //    "@WebChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CompanyCode", DbType.AnsiStringFixedLength,
                    "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);


                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "B.BrandSysNo",
                    DbType.Int32, "@BrandSysNo", QueryConditionOperatorType.Equal, filter.BrandSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "D.BrandSysNo",
                    DbType.Int32, "@BrandSysNo", QueryConditionOperatorType.Equal, filter.BrandSysNo);
                sqlBuilder.ConditionConstructor.EndGroupCondition();

                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "B.C3SysNo",
                    DbType.Int32, "@Category3SysNo", QueryConditionOperatorType.Equal, filter.Category3SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "D.Category3SysNo",
                    DbType.Int32, "@Category3SysNo", QueryConditionOperatorType.Equal, filter.Category3SysNo);
                sqlBuilder.ConditionConstructor.EndGroupCondition();

                #region Jack.G.tang 2013-1-7 update Bug 95316
                /*修改原因:新增Category1和Category2的查询条件 
                 */
              
                if (filter.Category1SysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "C3.Category1SysNo",
                        DbType.Int32, "@Category1SysNo", QueryConditionOperatorType.Equal, filter.Category1SysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "Category3.Category1SysNo",
                        DbType.Int32, "@Category1SysNo", QueryConditionOperatorType.Equal, filter.Category1SysNo);
                    sqlBuilder.ConditionConstructor.EndGroupCondition();
                }

                if (filter.Category2SysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "C3.Category2SysNo",
                        DbType.Int32, "@Category2SysNo", QueryConditionOperatorType.Equal, filter.Category2SysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "Category3.Category2SysNo",
                        DbType.Int32, "@Category2SysNo", QueryConditionOperatorType.Equal, filter.Category2SysNo);
                    sqlBuilder.ConditionConstructor.EndGroupCondition();
                }

                #endregion

                if (filter.MasterProductSysNo.HasValue)
                {
                    ProductBasic pMaster = GetProductBasic(filter.MasterProductSysNo.Value);
                    sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "B.ProductSysNo",
                        DbType.Int32, "@MasterProductSysNo", QueryConditionOperatorType.Equal, filter.MasterProductSysNo);
                    if(pMaster.C3SysNo.HasValue)
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "B.C3SysNo",
                        DbType.Int32, "@MasterC3SysNo", QueryConditionOperatorType.Equal, pMaster.C3SysNo);
                    if(pMaster.BrandSysNo.HasValue)
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "B.BrandSysNo",
                        DbType.Int32, "@MasterBrandSysNo", QueryConditionOperatorType.Equal, pMaster.BrandSysNo);

                    sqlBuilder.ConditionConstructor.EndGroupCondition();
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.ProductSysNo",
                        DbType.Int32, "@GiftProductSysNo", QueryConditionOperatorType.Equal, filter.GiftProductSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.InUser",
                    DbType.String, "@PMUser", QueryConditionOperatorType.Equal, filter.PMUser);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.SysNo",
                    DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, filter.SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Status",
                    DbType.AnsiStringFixedLength, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                if (!string.IsNullOrEmpty(filter.PromotionName)) sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.PromotionName",
                     DbType.String, "@PromotionName", QueryConditionOperatorType.Like, filter.PromotionName);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Type",
                    DbType.AnsiStringFixedLength, "@Type", QueryConditionOperatorType.Equal, filter.Type);
                if (filter.ActivityDateFrom.HasValue) sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.BeginDate",
                    DbType.DateTime, "@BeginDate", QueryConditionOperatorType.MoreThanOrEqual, filter.ActivityDateFrom);
                if (filter.ActivityDateTo.HasValue) sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.EndDate",
                    DbType.DateTime, "@EndDate", QueryConditionOperatorType.LessThan, filter.ActivityDateTo.Value);

                if(filter.VendorSysNo>0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VendorSysNo", DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal, filter.VendorSysNo);
                }

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                DataSet ds = cmd.ExecuteDataSet();
                DataTable dt1 = ds.Tables[0];
                DataTable dt2 = ds.Tables[1];
                DataTable dt3 = ds.Tables[2];
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    foreach (DataRow row in dt1.Rows)
                    {
                        object status;
                        EnumCodeMapper.TryGetEnum(row["Status"], typeof(SaleGiftStatus), out status);
                        row["Status"] = status;
                        if (status != null)
                        {
                            row["StatusName"] = ((SaleGiftStatus)status).ToDisplayText();
                        }

                        object type;
                        EnumCodeMapper.TryGetEnum(row["Type"], typeof(SaleGiftType), out type);
                        row["Type"] = type;
                        if (type != null)
                        {
                            row["TypeName"] = ((SaleGiftType)type).ToDisplayText();
                        }

                        if (((SaleGiftType)type) == SaleGiftType.Full)
                        {
                            row["MasterProducts"] = "商品范围";
                        }
                        else
                        {
                            if (dt2 != null && dt2.Rows.Count > 0)
                            {
                                string masterProducts = "";
                                foreach (DataRow row2 in dt2.Rows)
                                {
                                    if (row2["SysNo"].ToString() == row["SysNo"].ToString())
                                    {
                                        masterProducts += row2["ProductID"].ToString() + Environment.NewLine;
                                    }
                                }
                                row["MasterProducts"] = masterProducts;
                            }
                        }

                        if (dt3 != null && dt3.Rows.Count > 0)
                        {
                            string giftProducts = "";
                            foreach (DataRow row3 in dt3.Rows)
                            {
                                if (row3["SysNo"].ToString() == row["SysNo"].ToString())
                                {
                                    giftProducts += row3["ProductID"].ToString() + Environment.NewLine;
                                }
                            }
                            row["GiftProducts"] = giftProducts;
                        }

                    }
                }

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt1;
            }
        }

        public DataTable GetValidVenderGifts(int productSysNo, out int totalCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_GetValidVenderGifts");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            DataTable dt = cmd.ExecuteDataTable();
            if (dt == null)
            {
                totalCount = 0;
            }
            else
            {
                totalCount = dt.Rows.Count;
            }
            return dt;
        }



        /// <summary>
        /// 根据主商品列表取得赠品列表
        /// </summary>
        /// <param name="giftBeginDate">赠品活动开始时间</param>
        /// <param name="giftEndDate">赠品活动结束时间</param>
        /// <param name="masterProductSysNo">赠品活动主商品列表</param>
        /// <returns></returns>
        public DataTable GetGiftItemByMasterProducts(DateTime giftBeginDate, DateTime giftEndDate, List<int> masterProductSysNo)
        {
            if (masterProductSysNo != null && masterProductSysNo.Count > 0)
            {
                CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("SaleGift_GetGiftsByMasterProducts");
                cmd.AddInputParameter("@GiftBenginTime", DbType.DateTime, giftBeginDate);
                cmd.AddInputParameter("@GiftEndTime", DbType.DateTime, giftEndDate);
                cmd.CommandText = cmd.CommandText.Replace("#MasterProductSysNo#", String.Join(",", masterProductSysNo));
                return cmd.ExecuteDataTable(new EnumColumnList { 
                { "Status", typeof(SaleGiftStatus) }, 
                { "GiftComboType", typeof(SaleGiftGiftItemType) },
                });
            }
            return null;
        }

        private ProductBasic GetProductBasic(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SaleGift_QueryItem");
            cmd.SetParameterValue("@ProductSysNo", sysNo);
            ProductBasic p = cmd.ExecuteEntity<ProductBasic>();
            return p;
        }


        public List<ProductItemInfo> GetSaleRules(int promotionSysNo, string companyCode)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("SaleGift_GetSaleRules");
            dataCommand.SetParameterValue("@PromotionSysNo", promotionSysNo);
            dataCommand.SetParameterValue("@CompanyCode", companyCode);
            return dataCommand.ExecuteEntityList<ProductItemInfo>();
        }

        public System.Data.DataTable QuerySaleGiftLog(SaleGiftLogQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QuerySaleGiftLog");


            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "A.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.ProductSysNo",
                        DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal, filter.ProductSysNo);

                cmd.CommandText = sqlBuilder.BuildQuerySql();


                DataTable dt = cmd.ExecuteDataTable("IsOnlineShow", typeof(GiftIsOnlineShow));

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
        
    }

    public class ProductBasic
    {
        public int SysNo { get; set; }
        public string ProductID { get; set; }
        public int? C3SysNo { get; set; }
        public int? BrandSysNo { get; set; }
    }
}
