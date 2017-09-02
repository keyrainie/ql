using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECommerce.Entity;
using ECommerce.Entity.Category;
using ECommerce.Entity.Store;
using ECommerce.Entity.Store.Filter;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess;
using ECommerce.Entity.Store.Vendor;

namespace ECommerce.DataAccess.Store
{
    public class StoreDA
    {
        /// <summary>
        /// 查询店铺导航
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<StoreNavigation> QueryStoreNavigationList(StorePageListQueryFilter queryFilter, int SellSysno)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryStoreNavigationList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, queryFilter, string.IsNullOrEmpty(queryFilter.SortFields) ? "s.[priority] asc" : queryFilter.SortFields))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.[SellerSysNo]", DbType.Int32, "@SellerSysNo", QueryConditionOperatorType.Equal, SellSysno);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.[status]", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, 1);

                command.CommandText = sqlBuilder.BuildQuerySql();
                List<StoreNavigation> resultList = command.ExecuteEntityList<StoreNavigation>();
                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                return new QueryResult<StoreNavigation>()
                {
                    PageInfo = new PageInfo() { PageIndex = queryFilter.PageIndex, PageSize = queryFilter.PageSize, TotalCount = totalCount, SortBy = queryFilter.SortFields },
                    ResultList = resultList
                };
            }
        }
        public static StorePage QueryStorePage(StorePageFilter filter)
        {
            CustomDataCommand cmd;
            if (filter.IsPreview)
            {
                //preview
                cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryStorePageWithPreview");
                using (var sqlBuild = new DynamicQuerySqlBuilder(cmd))
                {
                    sqlBuild.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "page.[SellerSysNo]", DbType.Int32, "@SellerSysNo",
                        QueryConditionOperatorType.Equal, filter.SellerSysNo);

                    sqlBuild.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "page.[SysNo]", DbType.Int32, "@SysNo",
                        QueryConditionOperatorType.Equal, filter.PublishPageSysNo);

                    sqlBuild.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "page.[PageTypeKey]", DbType.String, "@PageType",
                        QueryConditionOperatorType.Equal, filter.PageType);

                    cmd.CommandText = sqlBuild.BuildQuerySql();
                }
            }
            else
            {
                //not preview
                cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryStorePageWithPublish");
                #region build condition
                using (var sqlBuild = new DynamicQuerySqlBuilder(cmd))
                {
                    sqlBuild.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "p.[SellerSysNo]", DbType.Int32, "@SellerSysNo",
                        QueryConditionOperatorType.Equal, filter.SellerSysNo);

                    sqlBuild.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "p.[SysNo]", DbType.Int32, "@SysNo",
                        QueryConditionOperatorType.Equal, filter.PublishPageSysNo);

                    sqlBuild.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "P.[PageTypeKey]", DbType.String, "@PageType",
                        QueryConditionOperatorType.Equal, filter.PageType);

                    sqlBuild.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "p.[Status]", DbType.Int32, "@Status",
                        QueryConditionOperatorType.Equal, 1);

                    cmd.CommandText = sqlBuild.BuildQuerySql();
                }
                #endregion
            }


#if DEBUG
            Console.WriteLine("Query Store Page Command is : {0}", cmd.CommandText);
#endif
            var result = cmd.ExecuteEntity<StorePage>();
            if (result == null) return null;

            var page = SerializationUtility.JsonDeserialize2<StorePage>(result.DataValue);
            page.SysNo = result.SysNo.Value;
            page.SellerSysNo = filter.SellerSysNo.Value;


            var pageTheme = QueryStorePageTheme(page.StorePageTemplate.StorePageThemeSysNo);
            if (pageTheme != null)
            {
                page.StorePageTemplate.StorePageThemeCssUrl = pageTheme.CssResUrl;
            }

            page.StorePageType = QueryStorePageType(page.StorePageType.Key);

            return page;
        }

        public static string QueryStorePageHeader(int sellerSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("QueryStorePageHeader");
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            return cmd.ExecuteFirstColumn<string>().FirstOrDefault();
        }

        public static StorePageTheme QueryStorePageTheme(int themeSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("QueryStorePageTheme");
            cmd.SetParameterValue("@SysNo", themeSysNo);
            return cmd.ExecuteEntity<StorePageTheme>();
        }

        public static StorePageType QueryStorePageType(string key)
        {
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryStorePageType");
            cmd.AddInputParameter("@key", DbType.String, key);
            return cmd.ExecuteEntity<StorePageType>();
        }

        public static List<RecommendProduct> QueryStoreNewRecommendProduct(
          int MerchantSysNo,
          string CategoryCode,
          int count,
          string languageCode,
          string companyCode)
        {
            var cmd = DataCommandManager.GetDataCommand("QueryStoreNewRecommendProduct");
            cmd.SetParameterValue("@MerchantSysNo", MerchantSysNo);
            cmd.SetParameterValue("@CategoryCode", CategoryCode);
            cmd.SetParameterValue("@Count", count);
            cmd.SetParameterValue("@LanguageCode", languageCode);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<RecommendProduct>();
        }
        public static List<RecommendProduct> QueryWeekRankingForCategoryCode(
          int MerchantSysNo,
          string CategoryCode
          )
        {
            var cmd = DataCommandManager.GetDataCommand("QueryWeekRankingForCategoryCode");
            cmd.SetParameterValue("@MerchantSysNo", MerchantSysNo);
            cmd.SetParameterValue("@CategoryCode", CategoryCode);
            return cmd.ExecuteEntityList<RecommendProduct>();
        }


        public static List<RecommendProduct> QuerySuperSpecialProductForCategoryCode(int MerchantSysNo, string CategoryCode, string languageCode,
          string companyCode)
        {
            var cmd = DataCommandManager.GetDataCommand("QuerySuperSpecialProductForCategoryCode");
            cmd.SetParameterValue("@MerchantSysNo", MerchantSysNo);
            cmd.SetParameterValue("@CategoryCode", CategoryCode);
            cmd.SetParameterValue("@languageCode", languageCode);
            cmd.SetParameterValue("@companyCode", companyCode);
            return cmd.ExecuteEntityList<RecommendProduct>();
        }



        public static StoreBasicInfo QueryStoreBasicInfo(int sellerSysNo)
        {
            //var cmd = DataCommandManager.GetDataCommand("QueryStoreBasicInfo");
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryStoreBasicInfo");
            cmd.AddInputParameter("@SellerSysNo", DbType.Int32, sellerSysNo);
            return cmd.ExecuteEntity<StoreBasicInfo>();
        }

        public static StoreDomainPage GetStoreIndexPageBySubDomain(string subdomain)
        {
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetStoreIndexPageBySubDomain");
            cmd.SetParameterValue("@SecondDomain", subdomain);
            return cmd.ExecuteEntity<StoreDomainPage>();
        }

        public static List<StoreDomainPage> GetAllStoreDomainHomePageList()
        {
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetAllStoreDomainHomePageList");
            return cmd.ExecuteEntityList<StoreDomainPage>();
        }
        /// <summary>
        /// 检查商家是否存在
        /// </summary>
        /// <param name="vendorName"></param>
        /// <returns></returns>
        public static bool CheckExistsVendor(string vendorName)
        {
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("CheckExistsVendor");
            cmd.SetParameterValue("@VendorName", vendorName);
            return cmd.ExecuteScalar<int>() > 0;
        }
        /// <summary>
        /// 创建商家
        /// </summary>
        /// <param name="vendorBasicInfo">商家基本信息</param>
        public static void CreateVendor(VendorBasicInfo vendorBasicInfo)
        {
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("CreateVendor");
            cmd.SetParameterValue<VendorBasicInfo>(vendorBasicInfo);
            cmd.ExecuteNonQuery();
        }
    }
}
