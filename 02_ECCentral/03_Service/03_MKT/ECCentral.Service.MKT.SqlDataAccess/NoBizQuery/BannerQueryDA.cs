using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IBannerQueryDA))]
    public class BannerQueryDA : IBannerQueryDA
    {
        public DataSet Query(BannerQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Banner_GetBanners");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "BI.SysNo DESC"))
            {
                //页面类型
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "BD.PageType",
                    DbType.Int32,
                    "@PageType",
                    QueryConditionOperatorType.Equal,
                    filter.PageType);
                //页面编号
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                     "BL.PageID", DbType.Int32, "@PageID", QueryConditionOperatorType.Equal,
                                     filter.PageID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "BD.PositionID", DbType.Int32, "@PositionID",
                    QueryConditionOperatorType.Equal, filter.PositionID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "BI.BannerType", DbType.AnsiStringFixedLength, "@BannerType",
                    QueryConditionOperatorType.Equal, filter.BannerType);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "BI.BannerTitle", DbType.String, "@BannerTitle",
                    QueryConditionOperatorType.Like, filter.BannerTitle);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "BL.Status", DbType.String, "@Status",
                    QueryConditionOperatorType.Equal, filter.Status);

                if (filter.BeginDateFrom.HasValue && filter.BeginDateTo.HasValue && filter.BeginDateFrom.Equals(filter.BeginDateTo)) filter.BeginDateTo = filter.BeginDateTo.Value.AddDays(1);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "BL.BeginDate", DbType.DateTime, "@BeginDateFrom",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     filter.BeginDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "BL.BeginDate", DbType.DateTime, "@BeginDateTo",
                     QueryConditionOperatorType.LessThan,
                     filter.BeginDateTo);

                if (filter.EndDateFrom.HasValue && filter.EndDateTo.HasValue && filter.EndDateFrom.Equals(filter.EndDateTo)) filter.EndDateTo = filter.EndDateTo.Value.AddDays(1);
               
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "BL.EndDate", DbType.DateTime, "@EndDateFrom",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     filter.EndDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "BL.EndDate", DbType.DateTime, "@EndDateTo",
                     QueryConditionOperatorType.LessThan,
                     filter.EndDateTo);

                //主要投放区域查询
                if (!string.IsNullOrEmpty(filter.AreaShow))
                {
                    var areaSysNos = filter.AreaShow.Split(",".ToCharArray(),  StringSplitOptions.RemoveEmptyEntries);
                    for (var i = 0; i < areaSysNos.Length;i++ )
                    {
                        string paramName = "@AreaShow" + i.ToString();
                        sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, null, QueryConditionOperatorType.Exist, "SELECT TOP 1 1 FROM OverseaECommerceManagement.dbo.AreaRelation_Website WITH(NOLOCK) WHERE AreaSysNo = " + paramName + " AND RefSysNo=BL.SysNo AND [Type]='B' AND [Status]='A'");
                        cmd.AddInputParameter(paramName, DbType.Int32, areaSysNos[i]);
                    }
                }

                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "BI.CompanyCode",
                   DbType.AnsiStringFixedLength,
                   "@CompanyCode",
                   QueryConditionOperatorType.Equal,
                 filter.CompanyCode);
                //TODO:添加渠道过滤条件

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var ds = cmd.ExecuteDataSet();
                //转换广告信息DataTable的枚举列
                EnumColumnList enumConfig = new EnumColumnList();
                enumConfig.Add("Status", typeof(ADStatus));
                enumConfig.Add("BannerType", typeof(BannerType));
                cmd.ConvertEnumColumn(ds.Tables[0], enumConfig);

                cmd.ConvertCodeNamePairColumn(ds.Tables[1], 1, "MKT", string.Format("Area-{0}-{1}-NewsAdv", filter.CompanyCode, filter.ChannelID ?? "1"));

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return ds;
            }
        }

        public List<BannerDimension> GetBannerDimensions(BannerDimensionQueryFilter filter)
        {
            var cmd = DataCommandManager.GetDataCommand("Banner_GetBannerDimensions");
            cmd.SetParameterValue<BannerDimensionQueryFilter>(filter);

            return cmd.ExecuteEntityList<BannerDimension>();
        }

        public List<BannerFrame> GetBannerFrame(BannerFrameQueryFilter filter)
        {
            var cmd = DataCommandManager.GetDataCommand("Banner_GetBannerFrame");
            cmd.SetParameterValue("@PageType", filter.PageType);
            cmd.SetParameterValue("@PositionID", filter.PositionID);
            return cmd.ExecuteEntityList<BannerFrame>();
        }
    }
}
