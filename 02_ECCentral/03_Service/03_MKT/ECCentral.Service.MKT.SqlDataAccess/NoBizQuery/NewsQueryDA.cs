using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.MKT;
using System.Data;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(INewsQueryDA))]
    public class NewsQueryDA : INewsQueryDA
    {
        public DataSet QueryNews(NewsInfoQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetNewsList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "a.SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);
                if (filter.SysNo.HasValue && filter.SysNo.Value > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.SysNo", DbType.Int32, "@SysNo",
                        QueryConditionOperatorType.Equal, filter.SysNo);
                }
                else
                {

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "Title", DbType.String, "@Title",
                        QueryConditionOperatorType.Like, filter.Title);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "Subtitle", DbType.String, "@Subtitle",
                        QueryConditionOperatorType.Like, filter.Subtitle);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "TopMost", DbType.Int32, "@TopMost",
                        QueryConditionOperatorType.Equal, filter.IsSetTop);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "CreateUserSysNo", DbType.Int32, "@CreateUserSysNo",
                        QueryConditionOperatorType.Equal, filter.InUser);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "CreateDate", DbType.DateTime, "@CreateDateTo",
                         QueryConditionOperatorType.LessThanOrEqual,
                         filter.InDateFromTo);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "CreateDate", DbType.DateTime, "@CreateDateFrom",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        filter.InDateFrom);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "NewsType", DbType.Int32, "@NewsType",
                        QueryConditionOperatorType.Equal, filter.NewsType);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                      "ReferenceSysNo", DbType.Int32, "@ReferenceSysNo",
                      QueryConditionOperatorType.Equal, filter.ReferenceSysNo);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.Status", DbType.Int32, "@Status",
                        QueryConditionOperatorType.Equal, filter.Status);


                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "EnableComment", DbType.Int32, "@EnableComment",
                        QueryConditionOperatorType.Equal, filter.IsShow);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "IsRed", DbType.Int32, "@IsRed",
                        QueryConditionOperatorType.Equal, filter.IsRed);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.CompanyCode", DbType.String, "@CompanyCode",
                    QueryConditionOperatorType.Equal, filter.CompanyCode);
                if (!string.IsNullOrEmpty(filter.SelectedArea))
                {
                    foreach (string area in filter.SelectedArea.Split(','))
                    {
                        sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, null, QueryConditionOperatorType.Exist, "SELECT TOP 1 1 FROM OverseaECommerceManagement.dbo.AreaRelation_Website WITH(NOLOCK) WHERE AreaSysNo = " + area + " AND RefSysNo=A.SysNo AND [Type]='N' AND [Status]='A'");
                    }
                }

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var ds = cmd.ExecuteDataSet();
                EnumColumnList enumConfigNews = new EnumColumnList();
                enumConfigNews.Add(13, typeof(NewsStatus));
                enumConfigNews.Add(15, typeof(CustomerRank));
                cmd.ConvertEnumColumn(ds.Tables[0], enumConfigNews);
                cmd.ConvertCodeNamePairColumn(ds.Tables[1], 0, "MKT", string.Format("Area-{0}-{1}-NewsAdv", filter.CompanyCode, filter.ChannelID ?? "0"));
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return ds;
            }
        }

        /// <summary>
        /// 查询公告及促销评论
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryNewsAdvReply(NewsAdvReplyQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("News_QueryNewsAdvReplyList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "N.SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);

                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "N.CreateDate", DbType.DateTime, "@CreateDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.InDateFrom, filter.InDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "N.CustomerSysNo", DbType.Int32, "@CustomerSysNo", QueryConditionOperatorType.Equal, filter.CustomerSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "N.LastEditUserSysNo", DbType.Int32, "@LastEditUserSysNo", QueryConditionOperatorType.Like, filter.LastEditUserSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "N.ReferenceType", DbType.Int32, "@ReferenceType", QueryConditionOperatorType.Equal, filter.ReferenceType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "N.ReferenceSysNo", DbType.Int32, "@ReferenceSysNo", QueryConditionOperatorType.Equal, filter.ReferenceSysNo);
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CI.LastEditUserID", DbType.String, "@LastEditUserID", QueryConditionOperatorType.Like, filter.LastEditUserID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SI.DisplayName", DbType.String, "@LastEditUserID", QueryConditionOperatorType.Like, filter.LastEditUserID);

                if (filter.IsUploadImage != null)
                {
                    if (filter.IsUploadImage == ECCentral.BizEntity.MKT.NYNStatus.Yes)
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "N.Image is not null");
                    //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,  "N.Image", DbType.Int32, "@Image", QueryConditionOperatorType.IsNotNull, "");
                    else
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "N.Image is null");
                    //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "N.[Image]", DbType.String, "@Image", filter.IsUploadImage == ECCentral.BizEntity.MKT.NYNStatus.Yes?QueryConditionOperatorType.IsNotNull:QueryConditionOperatorType.IsNull,);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "N.ReplyContent", DbType.String, "@ReplyContent", QueryConditionOperatorType.Like, filter.ReplyContent);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "N.SysNo", DbType.Int32, "@SysNo", QueryConditionOperatorType.Like, filter.SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "N.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "N.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Status", typeof(ECCentral.BizEntity.MKT.NewsAdvReplyStatus));//前台展示状态
                enumList.Add("ReplyHasReplied", typeof(ECCentral.BizEntity.MKT.NYNStatus));//是否存在回复
                CodeNamePairColumnList pairList = new CodeNamePairColumnList();
                pairList.Add("ReferenceType", "MKT", "CommentsCategory"); //ReferenceType评论类型

                var dt = cmd.ExecuteDataTable(enumList, pairList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                //if (dt != null && dt.Rows.Count > 0)
                //{
                //    foreach (DataRow dr in dt.Rows)
                //    {
                //        if (!string.IsNullOrEmpty(dr["Image"].ToString()))
                //        {
                //            dr["Image"] = "http://c3.neweggimages.com.cn/CImages/Promotion/P80/" + dr["Image"].ToString().Replace(",", ",http://c3.neweggimages.com.cn/CImages/Promotion/P80/");
                //            dr["LinkImage"] = "http://c3.neweggimages.com.cn/CImages/Promotion/Original/" + dr["Image"].ToString().Replace(",", ",http://c3.neweggimages.com.cn/CImages/Promotion/Original/");
                //        }
                //    }
                //}
                return dt;
            }
        }
    }
}
