using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IKeywordQueryDA))]
    public class KeywordQueryDA:IKeywordQueryDA
    {

        /// <summary>
        /// 查询中文词库
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QuerySegment(SegmentQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo == null)
            {
                pagingEntity = null;
            }
            else
            {
                pagingEntity.SortField = filter.PageInfo.SortBy;
                pagingEntity.MaximumRows = filter.PageInfo.PageSize;
                pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }


            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Segment_GetSegmentList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);
                
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Keywords", DbType.String, "@Keywords", QueryConditionOperatorType.Like, filter.Keywords);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,  "Status", DbType.String, "@Status", QueryConditionOperatorType.Equal, filter.Status);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                //CodeNamePairColumnList pairList = new CodeNamePairColumnList();
                //pairList.Add("Status", "MKT", "KeywordsStatus");
                //var dt = cmd.ExecuteDataTable(pairList);
                var dt = cmd.ExecuteDataTable<ECCentral.BizEntity.MKT.KeywordsStatus>("Status");
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
       
        /// <summary>
        /// 查询自动匹配关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QuerySearchedKeywords(SearchedKeywordsFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo == null)
            {
                pagingEntity = null;
            }
            else
            {
                pagingEntity.SortField = filter.PageInfo.SortBy;
                pagingEntity.MaximumRows = filter.PageInfo.PageSize;
                pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Keyword_QuerySearchKeyword");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);
                
                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "EditDate", DbType.DateTime, "@EditDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.InDateFrom, filter.InDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Keywords", DbType.String, "@Keywords", QueryConditionOperatorType.Like, filter.Keywords);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CreateUserType", DbType.Int32, "@CreateUserType", QueryConditionOperatorType.Like, filter.CreateUserType);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Status", DbType.String, "@Status", QueryConditionOperatorType.Equal, filter.Status);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "EditUser", DbType.String, "@EditUser", QueryConditionOperatorType.Equal, filter.EditUser);


                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var dt = cmd.ExecuteDataTable<ECCentral.BizEntity.MKT.ADStatus>("Status");
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
       
        /// <summary>
        /// 查询同义词
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryThesaurusKeywords(ThesaurusKeywordsQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo == null)
            {
                pagingEntity = null;
            }
            else
            {
                pagingEntity.SortField = filter.PageInfo.SortBy;
                pagingEntity.MaximumRows = filter.PageInfo.PageSize;
                pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }
            
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ThesaurusKeywords_QueryThesaurusKeywordsList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "TransactionNumber DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);
                
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "WordContent", DbType.String, "@Keywords", QueryConditionOperatorType.Like, filter.ThesaurusWords);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Type", DbType.String, "@Type", QueryConditionOperatorType.Equal, filter.Type);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Status", DbType.String, "@Status", QueryConditionOperatorType.Equal, filter.Status);

                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Status", typeof(ECCentral.BizEntity.MKT.ADTStatus));//前台展示状态
                enumList.Add("Type", typeof(ECCentral.BizEntity.MKT.ThesaurusWordsType));
                var dt = cmd.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 查询阻止词
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryStopWords(StopWordsQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo == null)
            {
                pagingEntity = null;
            }
            else
            {
                pagingEntity.SortField = filter.PageInfo.SortBy;
                pagingEntity.MaximumRows = filter.PageInfo.PageSize;
                pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("StopWord_GetStopWordList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "TransactionNumber DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);
                
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "Words", DbType.String, "@Keywords",
                    QueryConditionOperatorType.Like, filter.Keywords);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Status", DbType.String, "@Status", QueryConditionOperatorType.Equal, filter.Status);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var dt = cmd.ExecuteDataTable<ECCentral.BizEntity.MKT.ADTStatus>("Status");
                //var dt = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 查询跳转关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryAdvancedKeywords(AdvancedKeywordsQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo == null)
            {
                pagingEntity = null;
            }
            else
            {
                pagingEntity.SortField = filter.PageInfo.SortBy;
                pagingEntity.MaximumRows = filter.PageInfo.PageSize;
                pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Keywords_QueryAdvancedKeywordsList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Keywords", DbType.String, "@Keywords", QueryConditionOperatorType.Like, filter.Keywords);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "LinkUrl", DbType.String, "@LinkUrl", QueryConditionOperatorType.Equal, filter.LinkUrl);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Status", DbType.String, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "BeginDate", DbType.DateTime, "@BeginDate", QueryConditionOperatorType.MoreThanOrEqual, filter.BeginDate);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "EndDate", DbType.DateTime, "@EndDate", QueryConditionOperatorType.LessThanOrEqual, filter.EndDate);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Priority", DbType.Int32, "@Priority", QueryConditionOperatorType.Equal, string.IsNullOrEmpty(filter.Priority) ? null : filter.Priority);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode", DbType.String, "@CompanyCode",  QueryConditionOperatorType.Equal, filter.CompanyCode);
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Status", typeof(ECCentral.BizEntity.MKT.ADStatus));
                enumList.Add("AutoRedirectSwitch", typeof(ECCentral.BizEntity.MKT.NYNStatus));

                var dt = cmd.ExecuteDataTable(enumList);
                //var dt = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 查询分类关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryCategoryKeywords(CategoryKeywordsQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo == null)
            {
                pagingEntity = null;
            }
            else
            {
                pagingEntity.SortField = filter.PageInfo.SortBy;
                pagingEntity.MaximumRows = filter.PageInfo.PageSize;
                pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }


            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Keyword_QueryCategoryKeywordsList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "A.SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.Category1SysNo", DbType.String, "@C1SysNo", QueryConditionOperatorType.Equal, filter.Category1SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.Category2SysNo", DbType.String, "@C2SysNo", QueryConditionOperatorType.Equal, filter.Category2SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.Category3SysNo", DbType.String, "@C3SysNo", QueryConditionOperatorType.Equal, filter.Category3SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var dt = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));

                ECCentral.Service.MKT.IDataAccess.ICategoryKeywordsDA keywordDA = ObjectFactory<ECCentral.Service.MKT.IDataAccess.ICategoryKeywordsDA>.Instance;
                foreach (DataRow dr in dt.Rows)
                {
                    StringBuilder str=new StringBuilder();
                    List<string> list = keywordDA.GetKeywordsProperty(dr["PropertyKeywords"].ToString());
                    if (list != null)
                    {
                        foreach (string s in list)
                        {
                            str.Append(s).Append(' ');
                        }
                    }
                    dr["PropertyKeywords"] = str.ToString().TrimEnd(' ');
                }
                return dt;
            }
        }

        /// <summary>
        /// 查询关键字对应商品
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryKeyWordsForProduct(KeyWordsForProductQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo == null)
            {
                pagingEntity = null;
            }else{
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Keyword_QueryKeywordsForProduct");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "k.SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "k.Keywords", DbType.String, "@Keywords", QueryConditionOperatorType.Like, filter.Keywords);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "k.Priority", DbType.Int32, "@Priority", QueryConditionOperatorType.Equal, filter.Priority);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "k.Status", DbType.String, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "p.ProductID", DbType.String, "@ProductID", QueryConditionOperatorType.Equal, filter.ProductID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "k.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var dt = cmd.ExecuteDataTable<ECCentral.BizEntity.MKT.ADStatus>("Status");
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 查询产品页面关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryProductPageKeywords(ProductKeywordsQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo == null)
            {
                pagingEntity = null;
            }
            else
            {
                pagingEntity.SortField = filter.PageInfo.SortBy;
                pagingEntity.MaximumRows = filter.PageInfo.PageSize;
                pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }
            var cmd = DataCommandManager.GetDataCommand("ProductKeywords_QueryProductKeywordsList");
            cmd.SetParameterValue("@EditDateTo", filter.EditDateTo);
            cmd.SetParameterValue("@EditDateFrom", filter.EditDateFrom);
            cmd.SetParameterValue("@ProductMode", filter.ProductMode);
            cmd.SetParameterValue("@C1SysNo", filter.Category1SysNo);
            cmd.SetParameterValue("@C2SysNo", filter.Category2SysNo); 
            cmd.SetParameterValue("@C3SysNo", filter.Category3SysNo);
            cmd.SetParameterValue("@VendorName", filter.VendorName);
            cmd.SetParameterValue("@SelectedManufacturerSysNo", filter.SelectedManufacturerSysNo);
            cmd.SetParameterValue("@Keywords", filter.Keywords);
            cmd.SetParameterValue("@EditUser", filter.EditUser);
            cmd.SetParameterValue("@UserSysNo", filter.PMUserSysNo);
            cmd.SetParameterValue("@ProductID", filter.ProductID);
            cmd.SetParameterValue("@Status", filter.Status);
            cmd.SetParameterValue("@PropertyValueSysNo", filter.PropertyValueSysNo);
            cmd.SetParameterValue("@PropertySysNo", filter.PropertySysNo);
            cmd.SetParameterValue("@ManualInput", filter.InputValue);
            cmd.SetParameterValue("@StartNumber", pagingEntity.StartRowIndex);
            cmd.SetParameterValue("@EndNumber", pagingEntity.StartRowIndex + pagingEntity.MaximumRows);
            cmd.SetParameterValue("@SortField", pagingEntity.SortField);
            DataTable dt = new DataTable();
            dt = cmd.ExecuteDataTable();
            totalCount = (int)cmd.GetParameterValue("@TotalCount");
            return dt;
         }
    }
}
