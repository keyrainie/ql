using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECommerce.Entity.Common;
using ECommerce.Entity.Product;
using ECommerce.Enums;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Product
{
    /// <summary>
    /// 
    /// </summary>
    public static class ProductReviewDA
    {

        /// <summary>
        /// 产品评论查询
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="totalCount">The total count.</param>
        /// <returns></returns>
        public static List<ProductReviewQueryBasicInfo> QueryProductReviewBasicInfoList(ProductReviewQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.SortFields;
            pagingEntity.MaximumRows = filter.PageSize;
            pagingEntity.StartRowIndex = filter.PageIndex * filter.PageSize;

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("ProductReview_QueryProductReviewDetail");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingEntity, string.IsNullOrEmpty(pagingEntity.SortField) ? pagingEntity.SortField : "A.SysNo DESC"))
            {
                if (filter.SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND
                        , "A.SysNo"
                        , DbType.Int32
                        , "@SysNo"
                        , QueryConditionOperatorType.Equal
                        , filter.SysNo);
                }

                if (filter.CustomerCategory.HasValue)
                {
                    //顾客类型
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND
                        , "D.CompanyCustomer"
                        , DbType.Int32
                        , "@CustomerCategory"
                        , QueryConditionOperatorType.Equal
                        , filter.CustomerCategory);
                }

                if (!string.IsNullOrEmpty(filter.Title))
                {
                    dataCommand.AddInputParameter("@Title", DbType.String, filter.Title);
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND
                        , "A.Title"
                        , DbType.String
                        , "@Title", QueryConditionOperatorType.Like
                        , filter.Title);
                }
                if (filter.CustomerSysNo.HasValue)
                {
                    //顾客
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND
                        , "A.CustomerSysNo"
                        , DbType.Int32, "@CustomerSysNo"
                        , QueryConditionOperatorType.Equal
                        , filter.CustomerSysNo);
                }

                if (!string.IsNullOrEmpty(filter.Operation)
                    && !string.IsNullOrEmpty(filter.Score)
                    && filter.Operation != "0" && filter.Score != "0")
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(
                        QueryConditionRelationType.AND
                        , "A.Score" + filter.Operation + filter.Score);
                }
                if (filter.VendorType.HasValue)
                {
                    if (filter.VendorType == 1)//商家ID
                    {
                        sqlBuilder.ConditionConstructor.AddCustomCondition(
                            QueryConditionRelationType.AND
                            , "VD.VendorType=0");//中蛋特有
                    }
                    else
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                            , "VD.SysNo"
                            , DbType.Int32
                            , "@VendorType"
                            , QueryConditionOperatorType.Equal
                            , filter.VendorType);
                    }
                }

                if (filter.ProductGroupNo != 0)//商品组ID
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                       QueryConditionRelationType.AND
                       , "PC.ProductGroupSysno"
                       , DbType.Int32
                       , "@GroupID"
                       , QueryConditionOperatorType.Equal
                       , filter.ProductGroupNo);
                }

                //是否商品组
                if (filter.IsByGroup)
                {
                    if (filter.ProductSysNo > 0)
                    {
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "B.SysNo in(SELECT ProductSysNo FROM [OverseaContentManagement].[dbo].[V_CM_AllProductInfo] WHERE  [ProductGroupSysno] IN (SELECT [ProductGroupSysno]  FROM [OverseaContentManagement].[dbo].[V_CM_AllProductInfo] WHERE  ProductID='" + filter.ProductID + "' AND ProductSysNo=" + filter.ProductSysNo + "))");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(filter.ProductID))
                    {
                        dataCommand.AddInputParameter("@ProductID", DbType.String, filter.ProductID);
                        sqlBuilder.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND
                            , "B.ProductID"
                            , DbType.String
                            , "@ProductID"
                            , QueryConditionOperatorType.Like
                            , filter.ProductID);
                    }

                    if (!string.IsNullOrEmpty(filter.ProductName))
                    {
                        dataCommand.AddInputParameter("@ProductName", DbType.String, filter.ProductName);
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "B.ProductName",
                            DbType.String, 
                            "@ProductName",
                            QueryConditionOperatorType.Like,
                            filter.ProductName);
                    }
                }

                //是否有用候选
                if (filter.MostUseFulCandidate != null)
                {
                    if (filter.MostUseFulCandidate == 0)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND
                            , "A.MostUseFul"
                            , DbType.Int32
                            , "@MostUseFulCandidate"
                            , QueryConditionOperatorType.NotEqual
                            , 1);
                    }
                    else if (filter.MostUseFulCandidate == 1)
                        sqlBuilder.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND
                            , "A.MostUseFul"
                            , DbType.Int32
                            , "@MostUseFulCandidate"
                            , QueryConditionOperatorType.Equal
                            , filter.MostUseFulCandidate);
                }

                //是否最有用
                if (filter.MostUseFul != null)
                {
                    if (filter.MostUseFul == 0)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND
                            , "A.MostUseFul", DbType.Int32
                            , "@MostUseFul"
                            , QueryConditionOperatorType.NotEqual
                            , 2);
                    }
                    else if (filter.MostUseFul == 1)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND
                            , "A.MostUseFul"
                            , DbType.Int32
                            , "@MostUseFul"
                            , QueryConditionOperatorType.Equal
                            , 2);
                    }
                }

                //评论类型
                if (filter.ReviewType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND
                        , "A.ReviewType"
                        , DbType.Int32, "@MostUseFul"
                        , QueryConditionOperatorType.Equal
                        , (int)filter.ReviewType.Value);
                }

                if (filter.UsefulCount.HasValue)
                {
                    //用户有用评价数
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND
                        , "A.UsefulCount"
                        , DbType.Int32
                        , "@UsefulCount"
                        , QueryConditionOperatorType.Equal
                        , filter.UsefulCount);
                }

                //评论状态
                if (!string.IsNullOrEmpty(filter.Status))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND
                        , "A.Status"
                        , DbType.String, "@Status"
                        , QueryConditionOperatorType.Equal
                        , filter.Status.Substring(0, 1));
                    if (filter.Status.Length > 1)
                    {
                        if (filter.Status.Substring(2, 1) == "1")
                        {
                            sqlBuilder.ConditionConstructor.AddCustomCondition(
                                QueryConditionRelationType.AND
                                , "A.EditUser='System'");
                        }
                        else if (filter.Status.Substring(2, 1) == "2")
                            sqlBuilder.ConditionConstructor.AddCustomCondition(
                                QueryConditionRelationType.AND
                                , "A.EditUser<>'System'");
                    }
                }

                if (filter.SellerSysNo.HasValue)
                {
                    dataCommand.AddInputParameter("@SellerSysNo", DbType.String, filter.SellerSysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "B.MerchantSysNo",
                        DbType.String, "@SellerSysNo",
                        QueryConditionOperatorType.Equal,
                        filter.SellerSysNo);
                }
                if (filter.IsTop.HasValue)
                {
                    //置顶
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND
                        , "A.IsTop"
                        , DbType.String
                        , "@IsTop"
                        , QueryConditionOperatorType.Equal
                        , filter.IsTop);
                }

                if (filter.IsBottom.HasValue)
                {
                    //置底
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND
                        , "A.IsBottom"
                        , DbType.String, "@IsBottom"
                        , QueryConditionOperatorType.Equal
                        , filter.IsBottom);
                }

                if (filter.IsDigest.HasValue)
                {
                    //是否精华
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND
                        , "A.IsDigest"
                        , DbType.String
                        , "@IsDigest"
                        , QueryConditionOperatorType.Equal
                        , filter.IsDigest);
                }

                //sqlBuilder.ConditionConstructor.AddBetweenCondition(
                //    QueryConditionRelationType.AND
                //    , "A.InDate"
                //    , DbType.DateTime
                //    , "@InDate"
                //    , QueryConditionOperatorType.MoreThanOrEqual
                //    , QueryConditionOperatorType
                //    .LessThanOrEqual
                //    , filter.InDateFrom
                //    , filter.InDateTo);

                if (filter.InDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                        QueryConditionRelationType.AND,
                                        "P.InDate",
                                        DbType.DateTime,
                                        "@InDateFrom",
                                        QueryConditionOperatorType.MoreThanOrEqual,
                                        filter.InDateFrom
                                   );
                }

                if (filter.InDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                  QueryConditionRelationType.AND,
                                  "P.InDate",
                                  DbType.DateTime,
                                  "@InDateTo",
                                  QueryConditionOperatorType.LessThan,
                                  filter.InDateTo
                             );
                }



                //sqlBuilder.ConditionConstructor.AddBetweenCondition(
                //    QueryConditionRelationType.AND
                //    , "A.EditDate"
                //    , DbType.DateTime
                //    , "@EditDate"
                //    , QueryConditionOperatorType.MoreThanOrEqual
                //    , QueryConditionOperatorType.LessThanOrEqual
                //    , filter.EditDateFrom, filter.EditDateTo);


                if (filter.EditDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                        QueryConditionRelationType.AND,
                                        "P.EditDate",
                                        DbType.DateTime,
                                        "@EditDateFrom",
                                        QueryConditionOperatorType.MoreThanOrEqual,
                                        filter.InDateFrom
                                   );
                }

                if (filter.EditDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                  QueryConditionRelationType.AND,
                                  "P.InDate",
                                  DbType.DateTime,
                                  "@EditDateTo",
                                  QueryConditionOperatorType.LessThan,
                                  filter.EditDateTo
                             );
                }

                if (!string.IsNullOrEmpty(filter.EditUser))
                {
                    //更新人
                    dataCommand.AddInputParameter("@EditUser", DbType.String, filter.EditUser);

                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND
                        , "A.EditUser"
                        , DbType.String, "@EditUser"
                        , QueryConditionOperatorType.Like
                        , filter.EditUser);
                }

                if (filter.PMUserSysNo.HasValue)
                {
                    //PM
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND
                        , "B.PMUserSysNo"
                        , DbType.Int32
                        , "@PMUserSysNo"
                        , QueryConditionOperatorType.Equal
                        , filter.PMUserSysNo);
                }

                if (filter.ProductStatus.HasValue)
                {
                    //商品状态
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND
                        , "B.Status"
                        , DbType.Int32, "@ProductStatus"
                        , QueryConditionOperatorType.Equal
                        , filter.ProductStatus);
                }

                //商品类别
                if (filter.Category3SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND
                        , "B.C3SysNo"
                        , DbType.Int32
                        , "@C3SysNo"
                        , QueryConditionOperatorType.Equal
                        , filter.Category3SysNo);
                }
                else
                {
                    string c3 = string.Empty;
                    if (filter.Category1SysNo.HasValue)
                    {
                        c3 += " AND Category1Sysno = " + filter.Category1SysNo;
                    }
                    if (filter.Category2SysNo.HasValue)
                    {
                        c3 += " AND Category2Sysno = " + filter.Category2SysNo;
                    }
                    if (!string.IsNullOrEmpty(c3))
                    {
                        sqlBuilder.ConditionConstructor.AddCustomCondition(
                            QueryConditionRelationType.AND
                            , "B.C3SysNo IN (SELECT Category3Sysno FROM OverseaContentManagement.dbo.V_CM_CategoryInfo WHERE 1=1 AND CompanyCode =" + filter.CompanyCode + c3 + ")");
                    }
                }
                //CS处理状态
                //if (filter.ComplainStatus == null)
                //    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "CM.Status IS NOT NULL");
                //else
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CM.Status", DbType.Int32, "@CSProcessStatus", QueryConditionOperatorType.Equal, filter.ComplainStatus);
                //首页蛋友热评
                if (filter.IsIndexPageHotComment != null)
                {
                    if (filter.IsIndexPageHotComment == CommonYesOrNo.No)
                    {
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.SysNo NOT IN (SELECT ProductReviewSysno FROM OverseaECommerceManagement.dbo.ProductReview_Homepage with (nolock) WHERE Type = 'H')");
                    }
                    else if (filter.IsIndexPageHotComment == CommonYesOrNo.Yes)
                    {
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.SysNo IN (SELECT ProductReviewSysno FROM OverseaECommerceManagement.dbo.ProductReview_Homepage with (nolock) WHERE Type = 'H')");
                    }
                }

                //首页服务热线
                if (filter.IsIndexPageServiceHotComment != null)
                {
                    if (filter.IsIndexPageServiceHotComment == CommonYesOrNo.No)
                    {
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.SysNo NOT IN (SELECT ProductReviewSysno FROM OverseaECommerceManagement.dbo.ProductReview_Homepage with (nolock) WHERE Type = 'S')");
                    }
                    else if (filter.IsIndexPageServiceHotComment == CommonYesOrNo.Yes)
                    {
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "A.SysNo IN (SELECT ProductReviewSysno FROM OverseaECommerceManagement.dbo.ProductReview_Homepage with (nolock) WHERE Type = 'S')");
                    }
                }

                if (!string.IsNullOrEmpty(filter.CompanyCode))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                        , "A.CompanyCode"
                        , DbType.String
                        , "@CompanyCode"
                        , QueryConditionOperatorType.Equal
                        , filter.CompanyCode);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                //EnumColumnList enumList = new EnumColumnList();
                //enumList.Add("IsTop", typeof(CommonYesOrNo));
                //enumList.Add("IsBottom", typeof(CommonYesOrNo));
                //enumList.Add("MostUseFulCandidate", typeof(CommonYesOrNo));
                //enumList.Add("MostUseFul", typeof(CommonYesOrNo));
                //enumList.Add("IsDigest", typeof(CommonYesOrNo));
                //enumList.Add("ComplainStatus", typeof(ReviewProcessStatus));

                //CodeNamePairColumnList pairList = new CodeNamePairColumnList();
                //pairList.Add("Status", "MKT", "ReplyStatus");//评论状态

                //DataTable dataTable = dataCommand.ExecuteDataTable(enumList, pairList);




                //List<ProductReviewQueryBasicInfo> list = new List<ProductReviewQueryBasicInfo>();

                //foreach (DataRow row in dataTable.Rows)
                //{
                //    list.Add(DataMapper.GetEntity<ProductReviewQueryBasicInfo>(row));
                //}

                List<ProductReviewQueryBasicInfo> list = dataCommand.ExecuteEntityList<ProductReviewQueryBasicInfo>();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

                return list;
            }
        }

        /// <summary>
        /// 产品评论的批量审核状态
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="status">The status.</param>
        public static void BatchSetProductReviewStatus(List<int> items, string status, string currentUser)
        {
            StringBuilder message = new StringBuilder();
            foreach (var i in items)
            {
                message.Append(i.ToString() + ",");
            }

            DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductReview_BatchUpdateProductReviewStatus");

            dataCommand.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            dataCommand.SetParameterValue("@Status", status);
            dataCommand.SetParameterValue("@EditUser", currentUser);
            dataCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates the product review.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static ProductReviewInfo CreateProductReview(ProductReviewInfo entity)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductReview_CreateProductReview");

            dataCommand.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            dataCommand.SetParameterValue("@CustomerSysNo", entity.CustomerSysNo);
            dataCommand.SetParameterValue("@Service", entity.Service);
            dataCommand.SetParameterValue("@Title", entity.Title);
            dataCommand.SetParameterValue("@Prons", entity.Prons);
            dataCommand.SetParameterValue("@Cons", entity.Cons);
            dataCommand.SetParameterValue("@Score1", entity.Score1);
            dataCommand.SetParameterValue("@Score2", entity.Score2);
            dataCommand.SetParameterValue("@Score3", entity.Score3);
            dataCommand.SetParameterValue("@Score4", entity.Score4);
            dataCommand.SetParameterValue("@Score", entity.Score);
            dataCommand.SetParameterValueAsCurrentUserSysNo("@InUser");

            entity.SysNo = dataCommand.ExecuteScalar<int>();

            return entity;
        }

        /// <summary>
        /// 更新评论相关的信息
        /// </summary>
        /// <param name="item">The item.</param>
        public static void UpdateProductReview(ProductReviewInfo item)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductReview_UpdateProductReview");

            dataCommand.SetParameterValue<ProductReviewInfo>(item);

            dataCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新评论之后更新Homepage中的记录
        /// </summary>
        /// <param name="sysNo">The system no.</param>
        /// <param name="type">The type.</param>
        public static void UpdateHomepageForProductReview(int sysNo, string type)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductReview_UpdateHomepageSetting");

            dataCommand.SetParameterValue("@SysNo", sysNo);
            dataCommand.SetParameterValue("@Type", type);
            dataCommand.SetParameterValueAsCurrentUserAcct("EditUser");

            dataCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新评论之后删除Homepage中的记录
        /// </summary>
        /// <param name="sysNo">The system no.</param>
        /// <param name="type">The type.</param>
        public static void DeleteHomepageForProductReview(int sysNo, string type)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductReview_DeleteHomepageSetting");

            dataCommand.SetParameterValue("@SysNo", sysNo);
            dataCommand.SetParameterValue("@Type", type);
            dataCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除评论相关图片
        /// </summary>
        /// <param name="image">The image.</param>
        public static void DeleteProductReviewImage(string image)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductReview_DeleteProductReviewImage");

            string[] param = image.Split('!');
            dataCommand.SetParameterValue("@Image", param[0]);
            dataCommand.SetParameterValue("@SysNo", param[1]);

            dataCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据评论编号，加载相应的评论
        /// </summary>
        /// <param name="sysNo">The system no.</param>
        /// <returns></returns>
        public static ProductReviewInfo LoadProductReview(int sysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductReview_GetProductReviewInfo");

            dataCommand.SetParameterValue("@SysNo", sysNo);

            return dataCommand.ExecuteEntity<ProductReviewInfo>();
        }

        /// <summary>
        /// 产品评论查询回复
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="totalCount">The total count.</param>
        /// <returns></returns>
        public static DataTable QueryProductReviewReply(ProductReviewReplyQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.SortFields;
            pagingEntity.MaximumRows = filter.PageSize;
            pagingEntity.StartRowIndex = filter.PageIndex * filter.PageSize;

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("ProductReview_QueryProductReviewReplyList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingEntity, "A.SysNo DESC"))
            {

                //顾客类型
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND
                    , "D.CompanyCustomer"
                    , DbType.Int32
                    , "@CustomerCategory"
                    , QueryConditionOperatorType.Equal
                    , filter.CustomerCategory);

                //回复关键字
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND
                    , "B.Title"
                    , DbType.String
                    , "@Content"
                    , QueryConditionOperatorType.Like
                    , filter.Content);

                //顾客
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND
                    , "A.CustomerSysNo"
                    , DbType.Int32
                    , "@CustomerSysNo"
                    , QueryConditionOperatorType.Equal
                    , filter.CustomerSysNo);

                //评论状态
                if (!string.IsNullOrEmpty(filter.Status))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND
                        , "A.Status", DbType.String, "@Status"
                        , QueryConditionOperatorType.Equal
                        , filter.Status.Substring(0, 1));
                    if (filter.Status.Length > 1)
                    {
                        if (filter.Status.Substring(2, 1) == "1")
                        {
                            sqlBuilder.ConditionConstructor.AddCustomCondition(
                                QueryConditionRelationType.AND
                                , "A.EditUser='System'");
                        }
                        else if (filter.Status.Substring(2, 1) == "2")
                        {
                            sqlBuilder.ConditionConstructor.AddCustomCondition(
                                QueryConditionRelationType.AND
                                , "A.EditUser<>'System'");
                        }
                    }
                }
                //评论回复类型 对应 ReplySource
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND
                    , "A.Type"
                    , DbType.String
                    , "@Type"
                    , QueryConditionOperatorType.Equal
                    , filter.Type);
                //回复后跟随语
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.NeedAdditionalText", DbType.String, "@NeedAdditionalText", QueryConditionOperatorType.Equal, filter.NeedAdditionalText);
                //编号查询暂时不存在
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.ReviewSysNo", DbType.Int32, "@ReviewSysNo", QueryConditionOperatorType.Equal, filter.ReviewSysNo);

                sqlBuilder.ConditionConstructor.AddBetweenCondition(
                    QueryConditionRelationType.AND
                    , "A.InDate"
                    , DbType.DateTime
                    , "@InDate"
                    , QueryConditionOperatorType.MoreThanOrEqual
                    , QueryConditionOperatorType.LessThanOrEqual
                    , filter.InDateFrom
                    , filter.InDateTo);

                sqlBuilder.ConditionConstructor.AddBetweenCondition(
                    QueryConditionRelationType.AND
                    , "A.EditDate"
                    , DbType.DateTime
                    , "@EditDate"
                    , QueryConditionOperatorType.MoreThanOrEqual
                    , QueryConditionOperatorType.LessThanOrEqual
                    , filter.EditDateFrom
                    , filter.EditDateTo);

                //更新人
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND
                    , "A.EditUser"
                    , DbType.String, "@EditUser"
                    , QueryConditionOperatorType.Like
                    , filter.EditUser);

                //商品状态
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND
                    , "C.Status", DbType.Int32
                    , "@ProductStatus"
                    , QueryConditionOperatorType.Equal
                    , filter.ProductStatus);


                if (filter.VendorType == 1)//商家ID
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(
                        QueryConditionRelationType.AND
                        , "VD.VendorType=0");//中蛋特有
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND
                        , "VD.SysNo"
                        , DbType.Int32
                        , "@VendorType"
                        , QueryConditionOperatorType.Equal
                        , filter.VendorType);
                }
                if (filter.ProductGroupNo != 0)//商品组ID
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND
                        , "PC.ProductGroupSysno"
                        , DbType.Int32, "@GroupID"
                        , QueryConditionOperatorType.Equal
                        , filter.ProductGroupNo);

                //是否商品组
                if (filter.IsByGroup)
                {
                    if (filter.ProductSysNo > 0)
                    {
                        sqlBuilder.ConditionConstructor.AddCustomCondition(
                            QueryConditionRelationType.AND
                            , "C.SysNo in(SELECT ProductSysNo FROM [OverseaContentManagement].[dbo].[V_CM_AllProductInfo] WHERE  [ProductGroupSysno] IN (SELECT [ProductGroupSysno]  FROM [OverseaContentManagement].[dbo].[V_CM_AllProductInfo] WHERE  ProductID='" + filter.ProductID + "' AND ProductSysNo=" + filter.ProductSysNo + "))");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(filter.ProductID))
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND
                            , "C.ProductID", DbType.String, "@ProductID"
                            , QueryConditionOperatorType.Equal
                            , filter.ProductID);
                    }
                }

                //PM
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "PC.PMUserSysNo", DbType.Int32
                    , "@PMUserSysNo"
                    , QueryConditionOperatorType.Equal
                    , filter.PMUserSysNo);

                //商品类别
                if (filter.Category3SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                        , "C.C3SysNo"
                        , DbType.Int32
                        , "@C3SysNo"
                        , QueryConditionOperatorType.Equal
                        , filter.Category3SysNo);
                }
                else
                {
                    string c3 = string.Empty;

                    if (filter.Category1SysNo.HasValue)
                    {
                        c3 += " AND Category1Sysno = " + filter.Category1SysNo;
                    }
                    if (filter.Category2SysNo.HasValue)
                    {
                        c3 += " AND Category2Sysno = " + filter.Category2SysNo;
                    }
                    if (!string.IsNullOrEmpty(c3))
                    {
                        sqlBuilder.ConditionConstructor.AddCustomCondition(
                            QueryConditionRelationType.AND
                            , "C.C3SysNo IN (SELECT Category3Sysno FROM OverseaContentManagement.dbo.V_CM_CategoryInfo WHERE 1=1" + c3 + ")");
                    }
                }


                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                CodeNamePairColumnList pairList = new CodeNamePairColumnList();

                pairList.Add("Type", "MKT", "ReplySource");//回复类型
                pairList.Add("Status", "MKT", "ReplyStatus");//评论状态

                DataTable dataTable = dataCommand.ExecuteDataTable(pairList);

                List<ProductReviewQueryBasicInfo> list = new List<ProductReviewQueryBasicInfo>();

                foreach (DataRow row in dataTable.Rows)
                {
                    list.Add(DataMapper.GetEntity<ProductReviewQueryBasicInfo>(row));
                }

                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

                return dataTable;
            }
        }

        /// <summary>
        /// 根据讨论论编号，加载相应的讨论回复
        /// </summary>
        /// <param name="sysNo">The system no.</param>
        /// <returns></returns>
        public static List<ProductReviewReplyInfo> GetProductReviewReplyList(int sysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductReview_GetProductReviewReplyList");
            dataCommand.SetParameterValue("@ReviewSysNo", sysNo);

            CodeNamePairColumnList pairList = new CodeNamePairColumnList();

            pairList.Add("Status", "MKT", "ReplyStatus");//评论状态
            pairList.Add("Type", "MKT", "ReplySource");//回复类型

            DataTable dataTable = dataCommand.ExecuteDataTable(pairList);

            List<ProductReviewReplyInfo> list = new List<ProductReviewReplyInfo>();

            foreach (DataRow row in dataTable.Rows)
            {
                list.Add(DataMapper.GetEntity<ProductReviewReplyInfo>(row));
            }

            return list;
        }

        /// <summary>
        /// 根据评论编号加载相应的厂商评论回复
        /// </summary>
        /// <param name="sysNo">The system no.</param>
        /// <returns></returns>
        public static List<ProductReviewReplyInfo> GetProductReviewFactoryReply(int sysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductReview_GetProductReviewFactoryReply");

            dataCommand.SetParameterValue("@ReviewSysNo", sysNo);

            CodeNamePairColumnList pairList = new CodeNamePairColumnList();
            //pairList.Add("Status", "MKT", "ReplyStatus");//评论状态
            pairList.Add("Type", "MKT", "ReplySource");//回复类型
            pairList.Add("Status", "MKT", "FactoryReplyStatus");

            DataTable dataTable = dataCommand.ExecuteDataTable(pairList);

            List<ProductReviewReplyInfo> list = new List<ProductReviewReplyInfo>();

            foreach (DataRow row in dataTable.Rows)
            {
                list.Add(DataMapper.GetEntity<ProductReviewReplyInfo>(row));
            }

            return list;
        }

        /// <summary>
        /// 批量设置产品评论的状态
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="status">The status.</param>
        public static void BatchSetProductReviewReplyStatus(List<int> items, string status)
        {
            using (ITransaction transaction = TransactionManager.Create())
            {
                foreach (int sysNo in items)
                {
                    DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductReview_UpdateProductReviewStatusForUpdateReplyStatus");
                    dataCommand.SetParameterValue("@SysNo", sysNo);
                    dataCommand.SetParameterValue("@Status", status);
                    dataCommand.SetParameterValueAsCurrentUserAcct("EditUser");

                    dataCommand.ExecuteNonQuery();
                }

                transaction.Complete();
            }
        }

        /// <summary>
        /// 添加产品评论回复:添加产品评论回复有3种方式：
        /// 1.	网友回复，需通过审核才展示。
        /// 2.	厂商回复（通过Seller Portal），需通过审核才展示。
        /// 3.	IPP系统中回复，默认直接展示。
        /// </summary>
        /// <param name="item">The item.</param>
        public static void AddProductReviewReply(ProductReviewReplyInfo item)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductReview_CreateProductReviewReply");

            dataCommand.SetParameterValue<ProductReviewReplyInfo>(item);

            dataCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 厂商回复的批量发布与拒绝
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static int UpdateProductReviewVendorReplyStatus(ProductReviewReplyInfo item)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductReview_UpdateVendorStatus");

            dataCommand.SetParameterValue("@SysNo", item.SysNo.Value);
            dataCommand.SetParameterValue("@ReviewSysNo", item.ReviewSysNo.Value);
            dataCommand.SetParameterValue("@Content", item.Content);
            dataCommand.SetParameterValue("@Status", item.Status);
            dataCommand.SetParameterValue("@CompanyCode", item.CompanyCode);
            dataCommand.SetParameterValueAsCurrentUserAcct("EditUser");

            dataCommand.ExecuteNonQuery();

            return Convert.ToInt32(dataCommand.GetParameterValue("@IsSuccess"));
        }

    }
}
