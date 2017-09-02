using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IECCategoryQueryDA))]
    public class ECCategoryQueryDA : IECCategoryQueryDA
    {
        public List<ECCategory> GetAllECCategory1(string companyCode, string channelID)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ECCategory_GetECCategory1");
            dc.SetParameterValue("@CompanyCode", companyCode);
            dc.SetParameterValue("@ChannelID", channelID);

            return dc.ExecuteEntityList<ECCategory>();
        }

        public List<ECCategory> GetAllECCategory2(string companyCode, string channelID)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ECCategory_GetECCategory2");
            dc.SetParameterValue("@CompanyCode", companyCode);
            dc.SetParameterValue("@ChannelID", channelID);

            return dc.ExecuteEntityList<ECCategory>();
        }

        public List<ECCategory> GetAllECCategory3(string companyCode, string channelID)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ECCategory_GetECCategory3");
            dc.SetParameterValue("@CompanyCode", companyCode);
            dc.SetParameterValue("@ChannelID", channelID);

            return dc.ExecuteEntityList<ECCategory>();
        }

        public DataTable Query(ECCategoryQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ECCategory_Query");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "A.SysNo DESC"))
            {
                //1级分类
                sqlBuilder.ConditionConstructor.AddCondition(
                  QueryConditionRelationType.AND,
                  "R3.EC_CategorySysno",
                  DbType.AnsiStringFixedLength,
                  "@C1SysNo",
                  QueryConditionOperatorType.Equal,
                filter.C1SysNo);
                //2级分类
                sqlBuilder.ConditionConstructor.AddCondition(
                  QueryConditionRelationType.AND,
                  "R2.EC_CategorySysno",
                  DbType.AnsiStringFixedLength,
                  "@C2SysNo",
                  QueryConditionOperatorType.Equal,
                filter.C2SysNo);

                //3级分类
                sqlBuilder.ConditionConstructor.AddCondition(
                  QueryConditionRelationType.AND,
                  "A.SysNo",
                  DbType.AnsiStringFixedLength,
                  "@C3SysNo",
                  QueryConditionOperatorType.Equal,
                filter.C3SysNo);

                //状态
                sqlBuilder.ConditionConstructor.AddCondition(
                  QueryConditionRelationType.AND,
                  "A.Status",
                  DbType.AnsiStringFixedLength,
                  "@Status",
                  QueryConditionOperatorType.Equal,
                filter.Status);


                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "A.CompanyCode",
                   DbType.AnsiStringFixedLength,
                   "@CompanyCode",
                   QueryConditionOperatorType.Equal,
                 filter.CompanyCode);
                //TODO:添加渠道过滤条件

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var dt = cmd.ExecuteDataTable();
                EnumColumnList enumConfig = new EnumColumnList();
                enumConfig.Add("Status", typeof(ADStatus));
                enumConfig.Add("PromotionStatus", typeof(FeatureType));
                enumConfig.Add("Level", typeof(ECCategoryLevel));
                enumConfig.Add("IsParentCategoryShow", typeof(YNStatus));
                cmd.ConvertEnumColumn(dt, enumConfig);

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 查询前台分类可用的父类
        /// </summary>
        /// <param name="sysNo">父级level</param>
        /// <returns>可用父类列表</returns>
        public List<ECCategory> GetECCategoryParents(ECCategoryLevel ecCategoryLevel)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ECCategory_LoadParentList");
            cmd.SetParameterValue("@Level", ecCategoryLevel);

            return cmd.ExecuteEntityList<ECCategory>();
        }

        /// <summary>
        /// 查询前台分类的父类系统编号列表
        /// </summary>
        /// <param name="sysNo">前台分类系统编号</param>
        /// <returns>父类系统编号列表</returns>
        public List<int> GetECCategoryCurrentParentSysNos(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ECCategory_LoadParentSysNoList");
            cmd.SetParameterValue("@SysNo", sysNo);
            return cmd.ExecuteFirstColumn<int>();
        }

        /// <summary>
        /// 查询前台分类可用的子类
        /// </summary>
        /// <param name="sysNo">子级level</param>
        /// <returns>可用子类列表</returns>
        public List<ECCategory> GetECCategoryChildren(ECCategoryLevel ecCategoryLevel)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ECCategory_GetECCategoryChildren");
            cmd.SetParameterValue("@Level", ecCategoryLevel);

            return cmd.ExecuteEntityList<ECCategory>();
        }

        /// <summary>
        /// 查询前台分类的子类系统编号列表
        /// </summary>
        /// <param name="sysNo">前台分类系统编号</param>
        /// <param name="sysNo">前台分类层级系统编号</param>
        /// <returns>子类系统编号列表</returns>
        public List<int> GetECCategoryCurrentChildSysNos(int sysNo, int rSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ECCategory_GetECCategoryCurrentChildSysNos");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@RSysNo", rSysNo);
            return cmd.ExecuteFirstColumn<int>();
        }

        public List<ECCategory> GetECCategoryTree(ECCategoryQueryFilter filter)
        {
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ECCategory_GetECCategoryTree");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, null, "A.SysNo DESC"))
            {
                //状态
                sqlBuilder.ConditionConstructor.AddCondition(
                  QueryConditionRelationType.AND,
                  "A.Status",
                  DbType.AnsiStringFixedLength,
                  "@Status",
                  QueryConditionOperatorType.Equal,
                filter.Status);


                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "A.CompanyCode",
                   DbType.AnsiStringFixedLength,
                   "@CompanyCode",
                   QueryConditionOperatorType.Equal,
                 filter.CompanyCode);
                //TODO:添加渠道过滤条件

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                return cmd.ExecuteEntityList<ECCategory>();
            }

        }

        public DataTable QueryECCategoryMapping(ECCategoryMappingQueryFilter filter, out int totalCount)
        {
            var pagingInfo = new PagingInfoEntity
            {
                SortField = filter.PagingInfo.SortBy,
                StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize,
                MaximumRows = filter.PagingInfo.PageSize
            };
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("MKT_QueryECCategoryProductMapping");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingInfo, "m.SysNo DESC"))
            {
                //状态
                sqlBuilder.ConditionConstructor.AddCondition(
                  QueryConditionRelationType.AND,
                  "m.ECCategorySysNo",
                  DbType.Int32,
                  "@ECCategorySysNo",
                  QueryConditionOperatorType.Equal,
                filter.ECCategorySysNo);

                //只查询有效的
                sqlBuilder.ConditionConstructor.AddCondition(
                  QueryConditionRelationType.AND,
                  "m.Status",
                  DbType.Int32,
                  "@Status",
                  QueryConditionOperatorType.Equal,
                0);

                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "m.CompanyCode",
                   DbType.AnsiStringFixedLength,
                   "@CompanyCode",
                   QueryConditionOperatorType.Equal,
                 filter.CompanyCode);                

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList list = new EnumColumnList();
                list.Add("ProductStatus", typeof(ProductStatus));

                var dt = cmd.ExecuteDataTable(list);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        #region 管理前台类别关系

        public List<CategoryInfo> QueryCategory1(CategoryQueryFilter queryFilter)
        {
            var dataCommand = DataCommandManager.GetDataCommand("QueryECCCategory1");
            dataCommand.SetParameterValue("@CompanyCode", queryFilter.CompanyCode);
            DataTable dt = dataCommand.ExecuteDataTable();
            return BuildCategoryEntityList(dt);
        }


        public List<CategoryInfo> QueryAllCategory2(CategoryQueryFilter queryFilter)
        {
            var dataCommand = DataCommandManager.GetDataCommand("QueryAllECCCategory2");
            dataCommand.SetParameterValue("@CompanyCode", queryFilter.CompanyCode);
            DataTable dt = dataCommand.ExecuteDataTable();
            return BuildCategoryEntityList(dt);
        }


        public List<CategoryInfo> QueryAllCategory3(CategoryQueryFilter queryFilter)
        {
            var dataCommand = DataCommandManager.GetDataCommand("QueryAllECCCategory3");
            dataCommand.SetParameterValue("@CompanyCode", queryFilter.CompanyCode);
            DataTable dt = dataCommand.ExecuteDataTable();
            return BuildCategoryEntityList(dt);
        }

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
                    C3Code = dt.Columns.Contains("C3Code") == true ? (dr["C3Code"] == null || dr["C3Code"] == DBNull.Value ? string.Empty : dr["C3Code"].ToString()) : string.Empty
                };
                cateInfo.CategoryName = new LanguageContent(dr["CategoryName"].ToString());
                returnList.Add(cateInfo);
            }
            return returnList;
        }

        public DataTable QueryECCCategory(ECCManageCategoryQueryFilter query, out int totalCount)
        {
            DataCommand cmd = null;
            switch (query.Type)
            {
                case ECCCategoryManagerType.ECCCategoryType1:
                    cmd = DataCommandManager.GetDataCommand("GetECCManageCategory1List");
                    break;
                case ECCCategoryManagerType.ECCCategoryType2:
                    cmd = DataCommandManager.GetDataCommand("GetECCManageCategory2List");
                    cmd.SetParameterValue("@Category1SysNo", query.Category1SysNo);
                    break;
                case ECCCategoryManagerType.ECCCategoryType3:
                    cmd = DataCommandManager.GetDataCommand("GetECCManageCategory3List");
                    cmd.SetParameterValue("@Category1SysNo", query.Category1SysNo);
                    cmd.SetParameterValue("@Category2SysNo", query.Category2SysNo);
                    break;
                default:
                    cmd = DataCommandManager.GetDataCommand("GetCategory1List");
                    break;
            }

            cmd.SetParameterValue("@PageCurrent", query.PagingInfo.PageIndex);
            cmd.SetParameterValue("@PageSize", query.PagingInfo.PageSize);
            cmd.SetParameterValue("@SortField", query.PagingInfo.SortBy);
            cmd.SetParameterValue("@Status", query.Status);
            cmd.SetParameterValue("@CategoryName", query.CategoryName);
            EnumColumnList enumList = new EnumColumnList
                {
                    { "CategoryStatus", typeof(ECCCategoryManagerStatus) },
                  
                   
                };
            DataTable dt = new DataTable();
            dt = cmd.ExecuteDataTable(enumList);
            totalCount = (int)cmd.GetParameterValue("@TotalCount");
            return dt;
        }
        #endregion
    }
}
