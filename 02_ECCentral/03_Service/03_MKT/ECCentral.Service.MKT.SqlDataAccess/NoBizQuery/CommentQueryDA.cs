using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using System.Data;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ICommentQueryDA))]
    public class CommentQueryDA:ICommentQueryDA
    {
        /// <summary>
        /// 评分项定义查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryProductReviewScore(ReviewScoreItemQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo == null)
                pagingEntity = null;
            else
            {
                pagingEntity.SortField = filter.PageInfo.SortBy;
                pagingEntity.MaximumRows = filter.PageInfo.PageSize;
                pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Commnet_GetReviewScoreItemDetail");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "a.SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);
                
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "b.Category1Sysno", DbType.Int32, "@C1SysNo",
                    QueryConditionOperatorType.Equal, filter.C1SysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "b.Category2Sysno", DbType.Int32, "@C2SysNo",
                    QueryConditionOperatorType.Equal, filter.C2SysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "b.Category3Sysno", DbType.Int32, "@C3SysNo",
                    QueryConditionOperatorType.Equal, filter.C3SysNo);
                if (filter.SysNo > 0)
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.SysNo", DbType.Int32, "@SysNo",
                        QueryConditionOperatorType.Equal, filter.SysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.Status", DbType.String, "@Status",
                    QueryConditionOperatorType.Equal, filter.Status);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.Name", DbType.String, "@Name",
                    QueryConditionOperatorType.Like, filter.Name);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.CompanyCode", DbType.String, "@CompanyCode",
                    QueryConditionOperatorType.Equal, filter.CompanyCode);


                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var dt = cmd.ExecuteDataTable<ECCentral.BizEntity.MKT.ADStatus>("Status");
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 产品评论查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryProductReview(ProductReviewQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ProductReview_QueryProductReviewDetail");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "A.SysNo DESC"))
            {

                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.SysNo", DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, filter.SysNo);
                //顾客类型
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.CompanyCustomer", DbType.Int32, "@CustomerCategory", QueryConditionOperatorType.Equal, filter.CustomerCategory);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Title", DbType.String, "@Title", QueryConditionOperatorType.Like, filter.Title);
                //顾客
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CustomerSysNo", DbType.Int32, "@CustomerSysNo", QueryConditionOperatorType.Equal, filter.CustomerSysNo);
                //顾客ID
                //sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "CASE WHEN A.CustomerSysNo=0 THEN A.InUser ELSE D.CustomerID END =" + filter.CustomerID);

                if (filter.Operation != "0" && filter.Score != "0")
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.Score"+filter.Operation+filter.Score);

                if (filter.VendorType == 1)//商家ID
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "VD.VendorType=0");//中蛋特有
                else
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VD.SysNo", DbType.Int32, "@VendorType", QueryConditionOperatorType.Equal, filter.VendorType);
                if (filter.ProductGroupNo != 0)//商品组ID
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PC.ProductGroupSysno", DbType.Int32, "@GroupID", QueryConditionOperatorType.Equal, filter.ProductGroupNo);

                //是否商品组
                if (filter.IsByGroup)
                {
                    if (filter.ProductSysNo > 0)
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "B.SysNo in(SELECT ProductSysNo FROM [OverseaContentManagement].[dbo].[V_CM_AllProductInfo] WHERE  [ProductGroupSysno] IN (SELECT [ProductGroupSysno]  FROM [OverseaContentManagement].[dbo].[V_CM_AllProductInfo] WHERE  ProductID='" + filter.ProductID + "' AND ProductSysNo=" + filter.ProductSysNo + "))");
                }
                else
                {
                    if (!string.IsNullOrEmpty(filter.ProductID))
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.ProductID", DbType.String, "@ProductID", QueryConditionOperatorType.Equal, filter.ProductID);
                }
                //是否有用候选
                if (filter.MostUseFulCandidate != null)
                {
                    if (filter.MostUseFulCandidate == 0)
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.MostUseFul", DbType.Int32, "@MostUseFulCandidate", QueryConditionOperatorType.NotEqual, 1);
                    else if (filter.MostUseFulCandidate == 1)
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.MostUseFul", DbType.Int32, "@MostUseFulCandidate", QueryConditionOperatorType.Equal, filter.MostUseFulCandidate);
                }
                //是否最有用
                if (filter.MostUseFul != null)
                {
                    if (filter.MostUseFul == 0)
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.MostUseFul", DbType.Int32, "@MostUseFul", QueryConditionOperatorType.NotEqual, 2);
                    else if (filter.MostUseFul == 1)
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.MostUseFul", DbType.Int32, "@MostUseFul", QueryConditionOperatorType.Equal, 2);
                }

                //评论类型
                if (filter.ReviewType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.ReviewType", DbType.Int32, "@MostUseFul", QueryConditionOperatorType.Equal, (int)filter.ReviewType.Value);
                }


                //用户有用评价数
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.UsefulCount", DbType.Int32, "@UsefulCount", QueryConditionOperatorType.Equal, filter.UsefulCount);
                //评论状态
                if (!string.IsNullOrEmpty(filter.Status))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Status", DbType.String, "@Status", QueryConditionOperatorType.Equal, filter.Status.Substring(0, 1));
                    if (filter.Status.Length > 1)
                    {
                        if (filter.Status.Substring(2, 1) == "1")
                            sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.EditUser='System'");
                        else if (filter.Status.Substring(2, 1) == "2")
                            sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.EditUser<>'System'");
                    }
                }
                //置顶
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.IsTop", DbType.String, "@IsTop", QueryConditionOperatorType.Equal, filter.IsTop);
                //置底
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.IsBottom", DbType.String, "@IsBottom", QueryConditionOperatorType.Equal, filter.IsBottom);

                //是否精华
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.IsDigest", DbType.String, "@IsDigest", QueryConditionOperatorType.Equal, filter.IsDigest);


                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.InDate", DbType.DateTime, "@InDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.InDateFrom, filter.InDateTo);

                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.EditDate", DbType.DateTime, "@EditDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.EditDateFrom, filter.EditDateTo);
                //更新人
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.EditUser", DbType.String, "@EditUser", QueryConditionOperatorType.Like, filter.EditUser);
                //PM
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.PMUserSysNo", DbType.Int32, "@PMUserSysNo", QueryConditionOperatorType.Equal, filter.PMUserSysNo);
                //商品状态
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.Status", DbType.Int32, "@ProductStatus", QueryConditionOperatorType.Equal, filter.ProductStatus);

                //商品类别
                if (filter.Category3SysNo.HasValue)
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.C3SysNo", DbType.Int32, "@C3SysNo", QueryConditionOperatorType.Equal, filter.Category3SysNo);
                else
                {
                    string c3 = string.Empty;
                    if (filter.Category1SysNo.HasValue)
                        c3 += " AND Category1Sysno = " + filter.Category1SysNo;
                    if (filter.Category2SysNo.HasValue)
                        c3 += " AND Category2Sysno = " + filter.Category2SysNo;
                    if (!string.IsNullOrEmpty(c3))
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "B.C3SysNo IN (SELECT Category3Sysno FROM OverseaContentManagement.dbo.V_CM_CategoryInfo WHERE 1=1 AND CompanyCode =" + filter.CompanyCode + c3 + ")");
                }
                //CS处理状态
                //if (filter.ComplainStatus == null)
                //    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "CM.Status IS NOT NULL");
                //else
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CM.Status", DbType.Int32, "@CSProcessStatus", QueryConditionOperatorType.Equal, filter.ComplainStatus);
                //首页蛋友热评
                if (filter.IsIndexPageHotComment != null)
                {
                    if (filter.IsIndexPageHotComment == YNStatus.No)
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.SysNo NOT IN (SELECT ProductReviewSysno FROM OverseaECommerceManagement.dbo.ProductReview_Homepage with (nolock) WHERE Type = 'H')");
                    else if (filter.IsIndexPageHotComment == YNStatus.Yes)
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.SysNo IN (SELECT ProductReviewSysno FROM OverseaECommerceManagement.dbo.ProductReview_Homepage with (nolock) WHERE Type = 'H')");
                }
                //首页服务热线
                if (filter.IsIndexPageServiceHotComment != null)
                {
                    if (filter.IsIndexPageServiceHotComment == YNStatus.No)
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.SysNo NOT IN (SELECT ProductReviewSysno FROM OverseaECommerceManagement.dbo.ProductReview_Homepage with (nolock) WHERE Type = 'S')");
                    else if (filter.IsIndexPageServiceHotComment == YNStatus.Yes)
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.SysNo IN (SELECT ProductReviewSysno FROM OverseaECommerceManagement.dbo.ProductReview_Homepage with (nolock) WHERE Type = 'S')");
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
               

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("IsTop", typeof(ECCentral.BizEntity.MKT.YNStatus));
                enumList.Add("IsBottom", typeof(ECCentral.BizEntity.MKT.YNStatus));
                enumList.Add("MostUseFulCandidate", typeof(ECCentral.BizEntity.MKT.YNStatus));
                enumList.Add("MostUseFul", typeof(ECCentral.BizEntity.MKT.YNStatus));
                enumList.Add("IsDigest", typeof(ECCentral.BizEntity.MKT.YNStatus));
                enumList.Add("ComplainStatus", typeof(ECCentral.BizEntity.MKT.ReviewProcessStatus));//CS处理状态

                CodeNamePairColumnList pairList = new CodeNamePairColumnList();
                pairList.Add("Status", "MKT", "ReplyStatus");//评论状态
                var dt = cmd.ExecuteDataTable(enumList,pairList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 产品评论查询回复
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryProductReviewReply(ProductReviewReplyQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ProductReview_QueryProductReviewReplyList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "A.SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);

                //顾客类型
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.CompanyCustomer", DbType.Int32, "@CustomerCategory", QueryConditionOperatorType.Equal, filter.CustomerCategory);

                //回复关键字
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.Title", DbType.String, "@Content", QueryConditionOperatorType.Like, filter.Content);
                //顾客
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CustomerSysNo", DbType.Int32, "@CustomerSysNo", QueryConditionOperatorType.Equal, filter.CustomerSysNo);
                
                //评论状态
                if (!string.IsNullOrEmpty(filter.Status))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Status", DbType.String, "@Status", QueryConditionOperatorType.Equal, filter.Status.Substring(0, 1));
                    if (filter.Status.Length > 1)
                    {
                        if (filter.Status.Substring(2, 1) == "1")
                            sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.EditUser='System'");
                        else if (filter.Status.Substring(2, 1) == "2")
                            sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.EditUser<>'System'");
                    }
                }
                //评论回复类型 对应 ReplySource
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Type", DbType.String, "@Type", QueryConditionOperatorType.Equal, filter.Type);
                //回复后跟随语
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.NeedAdditionalText", DbType.String, "@NeedAdditionalText", QueryConditionOperatorType.Equal, filter.NeedAdditionalText);
                //编号查询暂时不存在
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.ReviewSysNo", DbType.Int32, "@ReviewSysNo", QueryConditionOperatorType.Equal, filter.ReviewSysNo);
                
                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.InDate", DbType.DateTime, "@InDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.InDateFrom, filter.InDateTo);
                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.EditDate", DbType.DateTime, "@EditDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.EditDateFrom, filter.EditDateTo);

                //更新人
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.EditUser", DbType.String, "@EditUser", QueryConditionOperatorType.Like, filter.EditUser);
                //商品状态
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.Status", DbType.Int32, "@ProductStatus", QueryConditionOperatorType.Equal, filter.ProductStatus);


                if (filter.VendorType == 1)//商家ID
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "VD.VendorType=0");//中蛋特有
                else
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VD.SysNo", DbType.Int32, "@VendorType", QueryConditionOperatorType.Equal, filter.VendorType);
                if (filter.ProductGroupNo != 0)//商品组ID
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PC.ProductGroupSysno", DbType.Int32, "@GroupID", QueryConditionOperatorType.Equal, filter.ProductGroupNo);

                //是否商品组
                if (filter.IsByGroup)
                {
                    if (filter.ProductSysNo > 0)
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "C.SysNo in(SELECT ProductSysNo FROM [OverseaContentManagement].[dbo].[V_CM_AllProductInfo] WHERE  [ProductGroupSysno] IN (SELECT [ProductGroupSysno]  FROM [OverseaContentManagement].[dbo].[V_CM_AllProductInfo] WHERE  ProductID='" + filter.ProductID + "' AND ProductSysNo=" + filter.ProductSysNo + "))");
                }
                else
                {
                    if (!string.IsNullOrEmpty(filter.ProductID))
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.ProductID", DbType.String, "@ProductID", QueryConditionOperatorType.Equal, filter.ProductID);
                }

                //PM
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PC.PMUserSysNo", DbType.Int32, "@PMUserSysNo", QueryConditionOperatorType.Equal, filter.PMUserSysNo);
                
                //商品类别
                if (filter.Category3SysNo.HasValue)
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.C3SysNo", DbType.Int32, "@C3SysNo", QueryConditionOperatorType.Equal, filter.Category3SysNo);
                else
                {
                    string c3 = string.Empty;

                    if (filter.Category1SysNo.HasValue)
                        c3 += " AND Category1Sysno = " + filter.Category1SysNo;
                    if (filter.Category2SysNo.HasValue)
                        c3 += " AND Category2Sysno = " + filter.Category2SysNo;
                    if (!string.IsNullOrEmpty(c3))
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "C.C3SysNo IN (SELECT Category3Sysno FROM OverseaContentManagement.dbo.V_CM_CategoryInfo WHERE 1=1" + c3 + ")");
                }

              

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                
                CodeNamePairColumnList pairList = new CodeNamePairColumnList();
                pairList.Add("Type", "MKT", "ReplySource");//回复类型
                pairList.Add("Status", "MKT", "ReplyStatus");//评论状态
                var dt = cmd.ExecuteDataTable(pairList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 产品咨询查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryProductConsult(ProductConsultQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ProductConsult_GetProductConsultDetailList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "A.SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);
                //中蛋故有
                if(filter.VendorType==1)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "VD.VendorType=0");
                else
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VD.SysNo", DbType.Int32, "@VendorType", QueryConditionOperatorType.Equal, filter.VendorType);

                if(filter.ProductGroupNo!=0)
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PC.ProductGroupSysno", DbType.Int32, "@GroupID", QueryConditionOperatorType.Equal, filter.ProductGroupNo);

                if (filter.IsByGroup)
                {
                    if (filter.ProductSysNo > 0)
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "B.SysNo in(SELECT ProductSysNo FROM [OverseaContentManagement].[dbo].[V_CM_AllProductInfo] WHERE  [ProductGroupSysno] IN (SELECT [ProductGroupSysno]  FROM [OverseaContentManagement].[dbo].[V_CM_AllProductInfo] WHERE  ProductID='" + filter.ProductID + "' AND ProductSysNo=" + filter.ProductSysNo + "))");
                }
                else
                {
                    if (!string.IsNullOrEmpty(filter.ProductID))
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.ProductSysNo IN (SELECT SysNo FROM ipp3.dbo.Product WITH (NOLOCK) WHERE ProductID = '" + filter.ProductID + "')");
                }

                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.InDate", DbType.DateTime, "@InDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.InDateFrom, filter.InDateTo);

                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.EditDate", DbType.DateTime, "@EditDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.EditDateFrom, filter.EditDateTo);
                //商品类别
                if (filter.Category1SysNo != null && filter.Category2SysNo != null && filter.Category3SysNo != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.ProductSysNo IN (SELECT SysNo FROM ipp3.dbo.Product WITH (NOLOCK) WHERE C3SysNo = " + filter.Category3SysNo + " AND CompanyCode=" + filter.CompanyCode + ")");
                else if (filter.Category1SysNo != null && filter.Category2SysNo != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.ProductSysNo IN (SELECT p.SysNo FROM ipp3.dbo.Product p WITH (NOLOCK),ipp3.dbo.Category3 c WITH (NOLOCK) WHERE p.C3SysNo = c.SysNo AND c.C2SysNo =" + filter.Category2SysNo + " AND P.CompanyCode=" + filter.CompanyCode + " AND C.CompanyCode=" + filter.CompanyCode + ")");
                else if (filter.Category1SysNo != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.ProductSysNo IN (SELECT p.SysNo FROM ipp3.dbo.Product p WITH (NOLOCK),ipp3.dbo.Category3 c3 WITH (NOLOCK),ipp3.dbo.Category2 c2 WITH (NOLOCK) WHERE p.C3SysNo = c3.SysNo  AND c3.C2SysNo = c2.SysNo AND c2.C1SysNo = " + filter.Category1SysNo + " AND P.companycode=" + filter.CompanyCode + " AND c2.companycode=" + filter.CompanyCode + " AND c3.companycode=" + filter.CompanyCode + ")");

                if (filter.PMUserSysNo != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.ProductSysNo IN (SELECT SysNo FROM ipp3.dbo.Product WITH (NOLOCK) WHERE PMUserSysNo = "+filter.PMUserSysNo+")");
                if(filter.ProductStatus!=null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.ProductSysNo IN (SELECT SysNo FROM ipp3.dbo.Product WITH (NOLOCK) WHERE Status = "+filter.ProductStatus+")");
                if (!string.IsNullOrEmpty(filter.Status))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Status", DbType.String, "@Status", QueryConditionOperatorType.Equal, filter.Status.Substring(0, 1));
                    if (filter.Status.Length>1)
                    {
                        if (filter.Status.Substring(2, 1) == "1")
                            sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.EditUser='System'");
                        else if (filter.Status.Substring(2, 1) == "2")
                            sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.EditUser<>'System'");
                    }
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Type", DbType.String, "@Type", QueryConditionOperatorType.Equal, filter.Type);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Content", DbType.String, "@Content", QueryConditionOperatorType.Like, filter.Content);
                if (filter.CustomerSysNo != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.CustomerSysNo=" + filter.CustomerSysNo + "");
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.EditUser", DbType.String, "@EditUser", QueryConditionOperatorType.Like, filter.EditUser);

                if (filter.CustomerCategory != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.CustomerSysNo IN ( SELECT SysNo FROM ipp3.dbo.Customer WITH (NOLOCK)  WHERE CompanyCustomer = "+filter.CustomerCategory+")");

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                cmd.CommandText = sqlBuilder.BuildQuerySql();


                CodeNamePairColumnList pairList = new CodeNamePairColumnList();
                pairList.Add("Status", "MKT", "ReplyStatus");//咨询状态
                //pairList.Add("ReferenceType", "MKT", "CommentsCategory"); 
                var dt = cmd.ExecuteDataTable( pairList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 产品咨询回复查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryProductConsultReply(ProductConsultReplyQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ProductConsult_GetProductConsultReplyDetailList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "A.SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);
                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.InDate", DbType.DateTime, "@InDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.InDateFrom, filter.InDateTo);
                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.EditDate", DbType.DateTime, "@EditDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.EditDateFrom, filter.EditDateTo);
               
                if (filter.VendorType == 1)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "VD.VendorType=0");
                else
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VD.SysNo", DbType.Int32, "@VendorType", QueryConditionOperatorType.Equal, filter.VendorType);

                if (filter.ProductGroupNo != 0)
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PC.ProductGroupSysno", DbType.Int32, "@GroupID", QueryConditionOperatorType.Equal, filter.ProductGroupNo);

                if (filter.IsByGroup)
                {
                    if (filter.ProductSysNo > 0)
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "B.SysNo in(SELECT ProductSysNo FROM [OverseaContentManagement].[dbo].[V_CM_AllProductInfo] WHERE  [ProductGroupSysno] IN (SELECT [ProductGroupSysno]  FROM [OverseaContentManagement].[dbo].[V_CM_AllProductInfo] WHERE  ProductID='" + filter.ProductID + "' AND ProductSysNo=" + filter.ProductSysNo + "))");
                }
                else
                {
                    if (!string.IsNullOrEmpty(filter.ProductID))
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.ConsultSysNo IN (SELECT G.SysNo FROM OverseaECommerceManagement.dbo.ProductConsult_Detail G WITH (NOLOCK),IPP3.dbo.Product C WITH (NOLOCK) WHERE G.ProductSysNo = C.SysNo AND C.ProductID = '" + filter.ProductID + "' AND G.CompanyCode=" + filter.CompanyCode + " AND C.CompanyCode=" + filter.CompanyCode + ")");
                }
 
                if (filter.Category1SysNo != null && filter.Category2SysNo != null && filter.Category3SysNo != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.ConsultSysNo IN (SELECT G.SysNo FROM OverseaECommerceManagement.dbo.ProductConsult_Detail G, ipp3.dbo.Product P WITH (NOLOCK) WHERE G.ProductSysNo = P.SysNo AND P.C3SysNo = " + filter.Category3SysNo + " AND G.CompanyCode=" + filter.CompanyCode + " AND P.CompanyCode=" + filter.CompanyCode + ")");
                else if (filter.Category1SysNo != null && filter.Category2SysNo != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.ConsultSysNo IN (SELECT G.SysNo FROM OverseaECommerceManagement.dbo.ProductConsult_Detail G, ipp3.dbo.Product P WITH (NOLOCK),ipp3.dbo.Category3 C WITH (NOLOCK) WHERE G.ProductSysNo = P.SysNo AND P.C3SysNo = C.SysNo AND C.C2SysNo =" + filter.Category2SysNo + " AND P.CompanyCode=" + filter.CompanyCode + " AND C.CompanyCode=" + filter.CompanyCode + ")");
                else if (filter.Category1SysNo != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.ConsultSysNo IN (SELECT G.SysNo FROM OverseaECommerceManagement.dbo.ProductConsult_Detail G, ipp3.dbo.Product P WITH (NOLOCK),ipp3.dbo.Category3 C3 WITH (NOLOCK),ipp3.dbo.Category2 C2 WITH (NOLOCK) WHERE G.ProductSysNo = P.SysNo AND P.C3SysNo = C3.SysNo  AND C3.C2SysNo = C2.SysNo AND C2.C1SysNo = " + filter.Category1SysNo + " AND P.companycode=" + filter.CompanyCode + " AND C2.companycode=" + filter.CompanyCode + " AND C3.companycode=" + filter.CompanyCode + ")");

                if (filter.PMUserSysNo != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.ConsultSysNo IN (SELECT G.SysNo FROM OverseaECommerceManagement.dbo.ProductConsult_Detail G, ipp3.dbo.Product C WITH (NOLOCK) WHERE G.ProductSysNo = C.SysNo AND C.PMUserSysNo = " + filter.PMUserSysNo + " AND G.CompanyCode=" + filter.CompanyCode + " AND C.CompanyCode=" + filter.CompanyCode + ")");
                if (filter.ProductStatus != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "pcd.ProductSysNo IN (SELECT G.ProductSysNo FROM OverseaECommerceManagement.dbo.ProductConsult_Detail G, ipp3.dbo.Product C WITH (NOLOCK) WHERE G.ProductSysNo = C.SysNo AND C.Status = " + filter.ProductStatus + " AND G.CompanyCode=" + filter.CompanyCode + " AND C.CompanyCode=" + filter.CompanyCode + ")");
                if (!string.IsNullOrEmpty(filter.Status))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Status", DbType.String, "@Status", QueryConditionOperatorType.Equal, filter.Status.Substring(0, 1));
                    if (filter.Status.Length > 1)
                    {
                        if (filter.Status.Substring(2, 1) == "1")
                            sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.EditUser='System'");
                        else if (filter.Status.Substring(2, 1) == "2")
                            sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.EditUser<>'System'");
                    }
                }
                if(filter.IsTop!=null)
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.IsTop", DbType.String, "@IsTop", QueryConditionOperatorType.Equal, filter.IsTop);

                if (filter.Type != null)
                {
                    if(filter.Type==ReplyVendor.YES)
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Type", DbType.String, "@Type", QueryConditionOperatorType.Equal, filter.Type);
                    else
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Type", DbType.String, "@Type", QueryConditionOperatorType.NotEqual, ReplyVendor.YES);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Content", DbType.String, "@Content", QueryConditionOperatorType.Like, filter.ReplyContent);
                if (filter.CustomerSysNo != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "((A.CustomerSysNo IN ( SELECT  C.SysNo FROM ipp3.dbo.Customer C WITH (NOLOCK) WHERE C.CustomerID like " + filter.CustomerSysNo + " )) or (A.CustomerSysNo = 0 and A.InUser like " + filter.CustomerSysNo + "))");
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.EditUser", DbType.String, "@EditUser", QueryConditionOperatorType.Like, filter.EditUser);

                if (filter.CustomerCategory != null)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.CustomerSysNo IN ( SELECT SysNo FROM ipp3.dbo.Customer WITH (NOLOCK)  WHERE CompanyCustomer = " + filter.CustomerCategory + ")");

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                cmd.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Type", typeof(ECCentral.BizEntity.MKT.ReplyVendor));
                enumList.Add("IsTop", typeof(ECCentral.BizEntity.MKT.YNStatus));
                CodeNamePairColumnList pairList = new CodeNamePairColumnList();
                pairList.Add("Status", "MKT", "ReplyStatus");//咨询状态
                var dt = cmd.ExecuteDataTable(enumList,pairList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
   
        /// <summary>
        /// 产品讨论查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryProductDiscuss(ProductDiscussQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ProductDiscuss_QueryProductDiscussList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "A.SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);
                if (filter.VendorType == 1)//商家ID
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "VD.VendorType=0");//中蛋特有
                else
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VD.SysNo", DbType.Int32, "@VendorType", QueryConditionOperatorType.Equal, filter.VendorType);
                if (filter.ProductGroupNo != 0)//商品组ID
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PC.ProductGroupSysno", DbType.Int32, "@GroupID", QueryConditionOperatorType.Equal, filter.ProductGroupNo);

                //是否商品组
                if (filter.IsByGroup)
                {
                    if (filter.ProductSysNo > 0)
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "B.SysNo in(SELECT ProductSysNo FROM [OverseaContentManagement].[dbo].[V_CM_AllProductInfo] WHERE  [ProductGroupSysno] IN (SELECT [ProductGroupSysno]  FROM [OverseaContentManagement].[dbo].[V_CM_AllProductInfo] WHERE  ProductID='" + filter.ProductID + "' AND ProductSysNo=" + filter.ProductSysNo + "))");
                }
                else
                {
                    if (!string.IsNullOrEmpty(filter.ProductID))
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.ProductID", DbType.String, "@ProductID", QueryConditionOperatorType.Equal, filter.ProductID);
                }

                //顾客类型
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.CompanyCustomer", DbType.Int32, "@CustomerCategory", QueryConditionOperatorType.Equal, filter.CustomerCategory);

                //顾客
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CustomerSysNo", DbType.Int32, "@CustomerSysNo", QueryConditionOperatorType.Equal, filter.CustomerSysNo);
                //顾客ID
                //sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "CASE WHEN A.CustomerSysNo=0 THEN A.InUser ELSE D.CustomerID END =" + filter.CustomerID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Title", DbType.String, "@Title", QueryConditionOperatorType.Like, filter.Title);
                 //讨论状态
                if (!string.IsNullOrEmpty(filter.Status))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Status", DbType.String, "@Status", QueryConditionOperatorType.Equal, filter.Status.Substring(0, 1));
                    if (filter.Status.Length > 1)
                    {
                        if (filter.Status.Substring(2, 1) == "1")
                            sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.EditUser='System'");
                        else if (filter.Status.Substring(2, 1) == "2")
                            sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.EditUser<>'System'");
                    }
                }

                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.InDate", DbType.DateTime, "@InDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.InDateFrom, filter.InDateTo);
                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.EditDate", DbType.DateTime, "@EditDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.EditDateFrom, filter.EditDateTo);

                //更新人
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.EditUser", DbType.String, "@EditUser", QueryConditionOperatorType.Like, filter.EditUser);
                //PM
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.PMUserSysNo", DbType.Int32, "@PMUserSysNo", QueryConditionOperatorType.Equal, filter.PMUserSysNo);
                //商品状态
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.Status", DbType.Int32, "@ProductStatus", QueryConditionOperatorType.Equal, filter.ProductStatus);



                //商品类别
                if (filter.Category3SysNo.HasValue)
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.C3SysNo", DbType.Int32, "@C3SysNo", QueryConditionOperatorType.Equal, filter.Category3SysNo);
                else
                {
                    string c3 = string.Empty;
                    if (filter.Category1SysNo.HasValue)
                        c3 += " AND Category1Sysno = " + filter.Category1SysNo;
                    if (filter.Category2SysNo.HasValue)
                        c3 += " AND Category2Sysno = " + filter.Category2SysNo;
                    if (!string.IsNullOrEmpty(c3))
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "B.C3SysNo IN (SELECT Category3Sysno FROM OverseaContentManagement.dbo.V_CM_CategoryInfo WHERE 1=1 AND CompanyCode =" + filter.CompanyCode + c3 + ")");
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);


                cmd.CommandText = sqlBuilder.BuildQuerySql();
                CodeNamePairColumnList pairList = new CodeNamePairColumnList();
               // pairList.Add("Type", "MKT", "ReplySource");//回复类型
                pairList.Add("Status", "MKT", "ReplyStatus");//评论状态
                var dt = cmd.ExecuteDataTable(pairList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 产品讨论回复查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryProductDiscussReply(ProductDiscussReplyQueryFilter filter, out int totalCount)
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
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ProductDiscuss_QueryProductDiscussReplyList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "A.SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);

                if (filter.VendorType == 1)//商家ID
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "VD.VendorType=0");//中蛋特有
                else
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VD.SysNo", DbType.Int32, "@VendorType", QueryConditionOperatorType.Equal, filter.VendorType);
                if (filter.ProductGroupNo != 0)//商品组ID
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PC.ProductGroupSysno", DbType.Int32, "@GroupID", QueryConditionOperatorType.Equal, filter.ProductGroupNo);

                //是否商品组
                if (filter.IsByGroup)
                {
                    if (filter.ProductSysNo > 0)
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "C.SysNo in(SELECT ProductSysNo FROM [OverseaContentManagement].[dbo].[V_CM_AllProductInfo] WHERE  [ProductGroupSysno] IN (SELECT [ProductGroupSysno]  FROM [OverseaContentManagement].[dbo].[V_CM_AllProductInfo] WHERE  ProductID='" + filter.ProductID + "' AND ProductSysNo=" + filter.ProductSysNo + "))");
                }
                else
                {
                    if (!string.IsNullOrEmpty(filter.ProductID))
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.ProductID", DbType.String, "@ProductID", QueryConditionOperatorType.Equal, filter.ProductID);
                }

                //顾客类型
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.CompanyCustomer", DbType.Int32, "@CustomerCategory", QueryConditionOperatorType.Equal, filter.CustomerCategory);

                //顾客
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CustomerSysNo", DbType.Int32, "@CustomerSysNo", QueryConditionOperatorType.Equal, filter.CustomerSysNo);
                //顾客ID
                //sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "CASE WHEN A.CustomerSysNo=0 THEN A.InUser ELSE D.CustomerID END =" + filter.CustomerID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Content", DbType.String, "@Content", QueryConditionOperatorType.Like, filter.Content);
                //讨论状态
                if (!string.IsNullOrEmpty(filter.Status))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Status", DbType.String, "@Status", QueryConditionOperatorType.Equal, filter.Status.Substring(0, 1));
                    if (filter.Status.Length > 1)
                    {
                        if (filter.Status.Substring(2, 1) == "1")
                            sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.EditUser='System'");
                        else if (filter.Status.Substring(2, 1) == "2")
                            sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.EditUser<>'System'");
                    }
                }

                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.InDate", DbType.DateTime, "@InDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.InDateFrom, filter.InDateTo);
                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.EditDate", DbType.DateTime, "@EditDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.EditDateFrom, filter.EditDateTo);

                //更新人
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.EditUser", DbType.String, "@EditUser", QueryConditionOperatorType.Like, filter.EditUser);
                //PM
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.PMUserSysNo", DbType.Int32, "@PMUserSysNo", QueryConditionOperatorType.Equal, filter.PMUserSysNo);
                //商品状态
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.Status", DbType.Int32, "@ProductStatus", QueryConditionOperatorType.Equal, filter.ProductStatus);



                //商品类别
                if (filter.Category3SysNo.HasValue)
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.C3SysNo", DbType.Int32, "@C3SysNo", QueryConditionOperatorType.Equal, filter.Category3SysNo);
                else
                {
                    string c3 = string.Empty;
                    if (filter.Category1SysNo.HasValue)
                        c3 += " AND Category1Sysno = " + filter.Category1SysNo;
                    if (filter.Category2SysNo.HasValue)
                        c3 += " AND Category2Sysno = " + filter.Category2SysNo;
                    if (!string.IsNullOrEmpty(c3))
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "C.C3SysNo IN (SELECT Category3Sysno FROM OverseaContentManagement.dbo.V_CM_CategoryInfo WHERE 1=1 AND CompanyCode =" + filter.CompanyCode + c3 + ")");
                }

               


                cmd.CommandText = sqlBuilder.BuildQuerySql();
                CodeNamePairColumnList pairList = new CodeNamePairColumnList();
                // pairList.Add("Type", "MKT", "ReplySource");//回复类型
                pairList.Add("Status", "MKT", "ReplyStatus");//评论状态
                var dt = cmd.ExecuteDataTable(pairList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
    
        /// <summary>
        /// 评论管理模式查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryRemarkModeList(RemarkModeQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("RemarkMode_GetRemarkModeList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "rm.SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,"rm.RemarkID = c.SysNo");


                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "rm.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "rm.RemarkType", DbType.String, "@RemarkType", QueryConditionOperatorType.Equal, filter.RemarkType);
                if (filter.Category1SysNo > 0 && filter.Category2SysNo > 0 && filter.Category3SysNo > 0)
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "rm.RemarkID", DbType.Int32, "@Category3SysNo", QueryConditionOperatorType.Equal, filter.Category3SysNo);
                else if (filter.Category1SysNo > 0 && filter.Category2SysNo > 0)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "rm.RemarkID IN (SELECT SysNo FROM ipp3.dbo.Category3 WHERE C2SysNo = " + filter.Category2SysNo.Value+ ")");
                else if (filter.Category1SysNo > 0 )
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "rm.RemarkID IN (SELECT c3.SysNo FROM IPP3.dbo.Category3 c3,ipp3.dbo.Category2 c2 WHERE c3.C2SysNo = c2.SysNo AND c2.C1SysNo = " + filter.Category1SysNo.Value + ")");

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.C3Name", DbType.String, "@C3Name", QueryConditionOperatorType.Like, filter.C3Name);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList list = new EnumColumnList();
                list.Add("RemarkType", typeof(ECCentral.BizEntity.MKT.RemarksType));
                list.Add("Status", typeof(ECCentral.BizEntity.MKT.RemarkTypeShow));

                var dt = cmd.ExecuteDataTable(list);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
    
        /// <summary>
        /// 留言管理的查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryLeaveWord(LeaveWordQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("LeaveWords_QueryLeaveWords");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "A.SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);

                if (filter.SysNo.HasValue)
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.SysNo", DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, filter.SysNo);
                else
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CustomerSysNo", DbType.Int32, "@CustomerSysNo", QueryConditionOperatorType.Equal, filter.CustomerSysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CustomerName", DbType.String, "@CustomerID", QueryConditionOperatorType.Equal, filter.CustomerID);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.SoSysno", DbType.Int32, "@SoSysno", QueryConditionOperatorType.Equal, filter.SOSysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.updateUserSysno", DbType.Int32, "@updateUserSysno", QueryConditionOperatorType.Equal, filter.UpdateUserSysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Subject", DbType.String, "@Subject", QueryConditionOperatorType.Like, filter.Subject);

                    sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.CreateTime", DbType.DateTime, "@CreateTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.CreateTimeFrom, filter.CreateTimeTo);
                    sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.UpdateTime", DbType.DateTime, "@UpdateTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.UpdateTimeFrom, filter.UpdateTimeTo);

                    if (!filter.OverTimeStatus.HasValue)
                    {
                        //没选处理状态，但选中了有效CASE
                        if(!filter.Status.HasValue&&filter.IsValidCase)//-999
                            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Status", DbType.Int32, "@Status", QueryConditionOperatorType.NotEqual, CommentProcessStatus.Invalid);
                        //else if(filter.Status==CommentProcessStatus.Invalid&&filter.IsValidCase)
                            //999对应的
                        else
                            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                    }
                    else
                    {
                        if (filter.Status == null)
                        {
                            sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, CommentProcessStatus.WaitHandling);
                            if(filter.OverTimeStatus==OverTimeStatus.Over3Day)
                                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "A.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, CommentProcessStatus.Handling);
                            sqlBuilder.ConditionConstructor.EndGroupCondition();
                        }
                        else
                        {
                            if(filter.Status==CommentProcessStatus.Invalid||filter.Status==CommentProcessStatus.Finish)//不明白的地方
                                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal,-999);
                            else if(!filter.Status.HasValue&&filter.IsValidCase)
                                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, CommentProcessStatus.Handling);
                            else
                                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                        }
                    }
                }
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                //EnumColumnList enumList = new EnumColumnList();
                //enumList.Add("Status", typeof(ECCentral.BizEntity.MKT.CommentProcessStatus));//前台展示状态
                //enumList.Add("OverTimeStatus", typeof(ECCentral.BizEntity.MKT.OverTimeStatus));

                var dt = cmd.ExecuteDataTable<ECCentral.BizEntity.MKT.CommentProcessStatus>("Status");
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
    
    
    }
}
