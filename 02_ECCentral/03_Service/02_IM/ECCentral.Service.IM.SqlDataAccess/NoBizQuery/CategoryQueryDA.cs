using System;
using System.Collections.Generic;
using System.Data;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ICategoryQueryDA))]
    public class CategoryQueryDA : ICategoryQueryDA
    {
        #region ICategoryQueryDA Members

        public List<CategoryInfo> QueryCategory1(CategoryQueryFilter queryFilter)
        {
            var dataCommand = DataCommandManager.GetDataCommand("QueryCategory1");
            dataCommand.SetParameterValue("@CompanyCode", queryFilter.CompanyCode);
            DataTable dt = dataCommand.ExecuteDataTable();
            return BuildCategoryEntityList(dt);
        }

        public List<CategoryInfo> QueryCategory2(CategoryQueryFilter queryFilter)
        {
            var dataCommand = DataCommandManager.GetDataCommand("QueryCategory2");
            dataCommand.SetParameterValue("@Category1SysNumber", queryFilter.Category1SysNo.HasValue ? queryFilter.Category1SysNo.Value : 0);
            dataCommand.SetParameterValue("@CompanyCode", queryFilter.CompanyCode);
            DataTable dt = dataCommand.ExecuteDataTable();
            return BuildCategoryEntityList(dt);
        }

        public List<CategoryInfo> QueryCategory3(CategoryQueryFilter queryFilter)
        {
            var dataCommand = DataCommandManager.GetDataCommand("QueryCategory3");
            dataCommand.SetParameterValue("@Category2SysNumber", queryFilter.Category2SysNo.HasValue ? queryFilter.Category2SysNo.Value : 0);
            dataCommand.SetParameterValue("@CompanyCode", queryFilter.CompanyCode);
            DataTable dt = dataCommand.ExecuteDataTable();
            return BuildCategoryEntityList(dt);
        }

        public List<CategoryInfo> QueryAllCategory2(CategoryQueryFilter queryFilter)
        {
            var dataCommand = DataCommandManager.GetDataCommand("QueryAllCategory2");
            dataCommand.SetParameterValue("@CompanyCode", queryFilter.CompanyCode);
            DataTable dt = dataCommand.ExecuteDataTable();
            return BuildCategoryEntityList(dt);
        }

        public List<CategoryInfo> QueryAllCategory3(CategoryQueryFilter queryFilter)
        {
            var dataCommand = DataCommandManager.GetDataCommand("QueryAllCategory3");
            dataCommand.SetParameterValue("@CompanyCode", queryFilter.CompanyCode);
            DataTable dt = dataCommand.ExecuteDataTable();
            return BuildCategoryEntityList(dt);
        }

        /// <summary>
        /// 查询类别数据
        /// 本方法不区分1，2，3类别
        /// 目前供IM类别控件使用
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public List<CategoryInfo> QueryAllPrimaryCategory(CategoryQueryFilter queryFilter)
        {
            var dataCommand = DataCommandManager.GetDataCommand("QueryAllPrimaryCategory");
            dataCommand.SetParameterValue("@CompanyCode", "8601");
            DataTable dt = dataCommand.ExecuteDataTable();
            return BuildCategoryEntityList(dt);
        }

        #endregion

        /// <summary>
        /// 构建EntityList（多语言支持).
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<CategoryInfo> BuildCategoryEntityList(DataTable dt)
        {
            List<CategoryInfo> returnList = new List<CategoryInfo>();
            foreach (DataRow dr in dt.Rows)
            {
                CategoryInfo cateInfo = new CategoryInfo()
                {
                    SysNo = int.Parse(dr["SysNo"].ToString()),
                    ParentSysNumber = dr["ParentSysNumber"] == null || dr["ParentSysNumber"] == DBNull.Value ? (int?)null : int.Parse(dr["ParentSysNumber"].ToString()),
                    Status = dr["Status"].ToString() == "-1" ? CategoryStatus.DeActive : CategoryStatus.Active,
                    C3Code = dt.Columns.Contains("C3Code") == true ?( dr["C3Code"] == null || dr["C3Code"] == DBNull.Value ? string.Empty : dr["C3Code"].ToString()):string.Empty
                };
                cateInfo.CategoryName = new LanguageContent(dr["CategoryName"].ToString());
                returnList.Add(cateInfo);
            }
            return returnList;
        }

        #region ICategoryDA Members
        public virtual DataTable QueryCategory(CategoryQueryFilter queryCriteria, out int totalCount)
        {

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCategory");
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PagingInfo.SortBy,
                StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize,
                MaximumRows = queryCriteria.PagingInfo.PageSize
            };

            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "SysNo DESC"))
            {
                if (!String.IsNullOrEmpty(queryCriteria.CategoryName))
                {
                    dataCommand.AddInputParameter("@CategoryName", DbType.String, queryCriteria.CategoryName);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C3Name",
                        DbType.String, "@CategoryName",
                        QueryConditionOperatorType.Like,
                        queryCriteria.CategoryName);
                }
                if (queryCriteria.Status != null)
                {
                    dataCommand.AddInputParameter("@Status", DbType.String, queryCriteria.Status);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "Status",
                        DbType.String, "@Status",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.Status);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = dataCommand.ExecuteDataTable("CategoryStatus", typeof(CategoryStatus));
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }

        }


        #endregion
    }
}
