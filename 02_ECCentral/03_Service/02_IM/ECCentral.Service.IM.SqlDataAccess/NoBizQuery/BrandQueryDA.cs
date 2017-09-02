//************************************************************************
// 用户名				泰隆优选
// 系统名				品牌管理
// 子系统名		        品牌管理NoBizQuery查询接口实现
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************
using System;
using System.Data;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;


namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IBrandQueryDA))]
    public class BrandQueryDA : IBrandQueryDA
    {

        /// <summary>
        /// 查询品牌
        /// </summary>
        /// <returns></returns>
        public virtual DataTable QueryBrand(BrandQueryFilter queryCriteria, out int totalCount)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetBrandListByQuery");
            dataCommand.SetParameterValue("@BrandName",string.IsNullOrEmpty(queryCriteria.BrandNameLocal)?null:string.Format("%{0}%", queryCriteria.BrandNameLocal));
            dataCommand.SetParameterValue("@ManufacturerName", string.IsNullOrEmpty(queryCriteria.ManufacturerName) ? null : string.Format("%{0}%", queryCriteria.ManufacturerName));
            dataCommand.SetParameterValue("@Status", queryCriteria.Status);
            dataCommand.SetParameterValue("@BrandSysNo", queryCriteria.BrandSysNo);
            dataCommand.SetParameterValue("@ManufacturerSysNo", queryCriteria.ManufacturerSysNo);
            dataCommand.SetParameterValue("@Category1SysNo", queryCriteria.Category1SysNo);
            dataCommand.SetParameterValue("@Category2SysNo", queryCriteria.Category2SysNo);
            dataCommand.SetParameterValue("@Category3SysNo", queryCriteria.Category3SysNo);
            dataCommand.SetParameterValue("@IsAuthorized", queryCriteria.IsAuthorized);
            dataCommand.SetParameterValue("@AuthorizedStatus", queryCriteria.AuthorizedStatus);
            dataCommand.SetParameterValue("@BrandStory", queryCriteria.IsBrandStory);
            dataCommand.SetParameterValue("@SortField", queryCriteria.PagingInfo.SortBy);
            dataCommand.SetParameterValue("@PageSize", queryCriteria.PagingInfo.PageSize);
            dataCommand.SetParameterValue("@PageCurrent", queryCriteria.PagingInfo.PageIndex);
            dataCommand.SetParameterValue("@Priority", queryCriteria.Priority == "" ? null : queryCriteria.Priority);
            EnumColumnList enumList = new EnumColumnList
                {
                    { "Status", typeof(ValidStatus) },
                 };
            DataTable dt = new DataTable();
            dt = dataCommand.ExecuteDataTable(enumList);
            totalCount =(int) dataCommand.GetParameterValue("@TotalCount");
            return dt;
             }



        public DataTable QueryBrandInfo(BrandQueryFilter queryCriteria, out int totalCount)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetBrandInfo");
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PagingInfo.SortBy,
                StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize,
                MaximumRows = queryCriteria.PagingInfo.PageSize
            };
            
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "C.SysNo DESC"))
            {
                if (queryCriteria.Status != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C.Status",
                        DbType.StringFixedLength, "@Status",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.Status.Value);
                }         
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "c.BrandName_Ch",
                       DbType.String, "@BrandName_Ch",
                       QueryConditionOperatorType.Like,
                       queryCriteria.BrandNameLocal);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "m.ManufacturerName",
                       DbType.String, "@ManufacturerName",
                       QueryConditionOperatorType.Like,
                       queryCriteria.ManufacturerName);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList enumList = new EnumColumnList
                {
                    { "Status", typeof(ValidStatus) },
                 };
                DataTable dt = new DataTable();
                dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = (int)dataCommand.GetParameterValue("@TotalCount");
                return dt;

            }            
           
        }
    }
}
