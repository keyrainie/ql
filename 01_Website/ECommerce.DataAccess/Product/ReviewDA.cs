using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility.DataAccess;
using ECommerce.Entity;
using ECommerce.Entity.Product;

using System.Data;
using ECommerce.Utility;
using ECommerce.Enums;
using ECommerce.Entity.Member;
using ECommerce.Entity.Product.Review;


namespace ECommerce.DataAccess.Product
{
    public class ReviewDA
    {
        private static void SetCommandDefaultParameters(DataCommand command)
        {
            command.SetParameterValue("@LanguageCode", "zh-cn");
            command.SetParameterValue("@CompanyCode", "8601");
            command.SetParameterValue("@StoreCompanyCode", "8601");
        }

        public static List<UICustomerReviewInfo> GetHomePageHotReview(int limitCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Review_GetHomePageHotReview");
            SetCommandDefaultParameters(cmd);
            cmd.SetParameterValue("@TopCount", limitCount);

            return cmd.ExecuteEntityList<UICustomerReviewInfo>();
        }

        public static List<UICustomerReviewInfo> GetGoodReview(int limitCount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Review_GetGoodReview");
            SetCommandDefaultParameters(cmd);
            cmd.SetParameterValue("@TopCount", limitCount);

            return cmd.ExecuteEntityList<UICustomerReviewInfo>();
        }

        /// <summary>
        /// 获取评论
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public static Product_ReviewList GetProductReviewListByProductGroupSysNoForProduct(Product_ReviewQueryInfo queryInfo)
        {

            Product_ReviewList reviews = new Product_ReviewList();
            List<Product_ReviewDetail> reviewDetailList = new List<Product_ReviewDetail>();
            List<Product_ReplyDetail> replyList = new List<Product_ReplyDetail>();

            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetProductReviewListByProductGroupSysNoForProduct");
            reviews.ProductReviewScore = GetProductReviewMaster(queryInfo.ProductSysNo);
            dataCommand.SetParameterValue("@ProductGroupSysNo", queryInfo.ProductGroupSysNo);
            string searchType = string.Empty;
            if (queryInfo.SearchType != null)
            {
                searchType = String.Join(",", queryInfo.SearchType.ConvertAll(x => (int)x).Distinct());
            }
            dataCommand.SetParameterValue("@SearchType", searchType);
            dataCommand.SetParameterValue("@PageSize", queryInfo.PagingInfo.PageSize);
            dataCommand.SetParameterValue("@NeedReplyCount", queryInfo.NeedReplyCount);
            dataCommand.SetParameterValue("@PageIndex", queryInfo.PagingInfo.PageIndex);
            SetCommandDefaultParameters(dataCommand);


            DataSet result = dataCommand.ExecuteDataSet();


            reviews.TotalCount0 = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount0"));
            reviews.TotalCount1 = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount1"));
            reviews.TotalCount2 = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount2"));
            reviews.TotalCount3 = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount3"));
            reviews.TotalCount4 = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount4"));
            reviews.TotalCount5 = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount5"));

            List<Product_ReplyDetail> webReplyList = null;
            if (result != null && result.Tables.Count > 0)
            {
                DataTable reviesTable = result.Tables[0];
                if (reviesTable.Rows != null && reviesTable.Rows.Count > 0)
                {
                    reviewDetailList = DataMapper.GetEntityList<Product_ReviewDetail, List<Product_ReviewDetail>>(reviesTable.Rows);
                    if (result.Tables.Count > 1)
                    {
                        if (queryInfo.NeedReplyCount > 0)
                        {
                            DataTable replysTable = result.Tables[1];
                            if (replysTable.Rows != null && replysTable.Rows.Count > 0)
                            {
                                replyList = DataMapper.GetEntityList<Product_ReplyDetail, List<Product_ReplyDetail>>(replysTable.Rows);

                            }
                            DataTable webReplysTable;
                            if (result.Tables.Count >= 3)
                            {
                                webReplysTable = result.Tables[2];
                                if (webReplysTable.Rows.Count > 0)
                                {
                                    webReplyList = DataMapper.GetEntityList<Product_ReplyDetail, List<Product_ReplyDetail>>(webReplysTable.Rows);
                                }
                            }

                            //if (webReplysTable.Rows != null && webReplysTable.Rows.Count > 0)
                            //{
                            //    webReplyList = DataMapper.GetEntityList<Product_ReplyDetail, List<Product_ReplyDetail>>(webReplysTable.Rows);
                            //}
                        }
                    }
                }
            }

            int totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            reviews.TotalCount = totalCount;

            if (reviewDetailList != null && reviewDetailList.Count > 0)
            {

                if (replyList != null && replyList.Count > 0)
                {
                    reviewDetailList.ForEach(delegate(Product_ReviewDetail info)
                    {
                        info.ReplieList = replyList.FindAll(delegate(Product_ReplyDetail temp)
                        {
                            return temp.ReviewSysNo == info.SysNo;
                        });
                    });
                }
            }

            if (reviewDetailList == null)
                reviewDetailList = new List<Product_ReviewDetail>();

            reviews.ProductReviewDetailList = new PagedResult<Product_ReviewDetail>(totalCount, queryInfo.PagingInfo.PageSize, queryInfo.PagingInfo.PageIndex, reviewDetailList);

            if (reviews.ProductReviewDetailList != null && reviews.ProductReviewDetailList.Count > 0)
            {

                //网友回复列别（top 5）
                if (webReplyList != null && webReplyList.Count > 0)
                    foreach (Product_ReviewDetail review in reviews.ProductReviewDetailList)
                    {
                        review.WebReplayList = webReplyList.FindAll(f => f.ReviewSysNo == review.SysNo && f.ReplyType == FeedbackReplyType.Web);
                    }

                StringBuilder sb = new StringBuilder();
                foreach (Product_ReviewDetail info in reviews.ProductReviewDetailList)
                    sb.Append(info.ProductSysNo.ToString()).Append(",");

                if (sb.Length > 1)
                    sb.Remove(sb.Length - 1, 1);

                //DataCommand dataCommand2 = DataCommandManager.GetDataCommand("GetProductGroupByProductSysnos");
                //dataCommand2.SetParameterValue("@productSysnos", sb.ToString());

                //List<GroupPropertyVendorInfo> groups = dataCommand2.ExecuteEntityList<GroupPropertyVendorInfo>();

                //if (groups != null && groups.Count > 0)
                //    foreach (Product_ReviewDetail info in reviews.ProductReviewDetailList)
                //        foreach (GroupPropertyVendorInfo g in groups)
                //            if (g.ProductSysNo == info.ProductSysNo)
                //            {
                //                info.ProductBaseInfo.GroupPropertyInfo = g.GroupPropertyInfo;
                //                info.ProductBaseInfo.VendorInfo = g.VendorInfo;
                //                break;
                //            }
            }
            return reviews;
        }


        /// <summary>
        /// 获取品论详情
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public static Product_ReviewDetail GetProductReviewInfo(Product_ReviewQueryInfo queryInfo)
        {


            DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductReview_GetProductReviewInfoBySysNo");
            dataCommand.SetParameterValue("@SysNo", queryInfo.ReviewSysNo);
            dataCommand.SetParameterValue("@PageSize", queryInfo.PagingInfo.PageSize);
            dataCommand.SetParameterValue("@PageIndex", queryInfo.PagingInfo.PageIndex);
            SetCommandDefaultParameters(dataCommand);
            DataSet result = dataCommand.ExecuteDataSet();
            DataTable masterTable = result.Tables[0];
            Product_ReviewDetail reviewInfo = new Product_ReviewDetail();
            if (masterTable.Rows != null && masterTable.Rows.Count > 0)
            {
                reviewInfo = DataMapper.GetEntity<Product_ReviewDetail>(masterTable.Rows[0]);
                Product_ReviewMaster masterscore = GetProductReviewMaster(reviewInfo.ProductSysNo);
                if (masterscore != null && reviewInfo != null)
                {
                    reviewInfo.AvgScore = masterscore.AvgScore;

                    reviewInfo.ReviewCount = masterscore.ReviewCount;
                }
                DataTable itemTable = result.Tables[1];
                if (itemTable.Rows != null && itemTable.Rows.Count > 0)
                {
                    DataRow[] itemRows = itemTable.Select("ReviewSysNo=" + reviewInfo.SysNo);
                    DataTable newdt = new DataTable();
                    newdt = itemTable.Clone();
                    foreach (DataRow row in itemRows)
                    {
                        newdt.ImportRow(row);
                    }
                    if (newdt != null && newdt.Rows.Count > 0)
                    {
                        List<Product_ReplyDetail> replyList = DataMapper.GetEntityList<Product_ReplyDetail, List<Product_ReplyDetail>>(newdt.Rows);
                        int totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                        int webTotalCount = Convert.ToInt32(dataCommand.GetParameterValue("@WebTotalCount"));
                        int pageIndex = queryInfo.PagingInfo.PageIndex;
                        if ((pageIndex * queryInfo.PagingInfo.PageSize) > totalCount)
                        {
                            if (totalCount != 0 && (totalCount % queryInfo.PagingInfo.PageSize) == 0)
                            {
                                pageIndex = (int)(totalCount / queryInfo.PagingInfo.PageSize);
                            }
                            else
                            {
                                pageIndex = (int)(totalCount / queryInfo.PagingInfo.PageSize) + 1;
                            }
                        }
                        //网友回复
                        reviewInfo.Replies = new PagedResult<Product_ReplyDetail>(webTotalCount, queryInfo.PagingInfo.PageSize, pageIndex, replyList.FindAll(f => f.ReplyType == FeedbackReplyType.Web));

                        //厂商和买家回复
                        reviewInfo.ReplieList =
                            replyList.FindAll(f => f.ReplyType == FeedbackReplyType.Manufacturer || f.ReplyType == FeedbackReplyType.Newegg);
                        //reviewInfo.ReplieList.Sort((a, b) => b.ReplyType.CompareTo(a.ReplyType));


                    }
                }

                if (result.Tables.Count > 2)
                {
                    DataTable scoreNameTable = result.Tables[2];
                    if (scoreNameTable.Rows != null && scoreNameTable.Rows.Count > 0 && reviewInfo != null)
                    {
                        reviewInfo.ScoreNameList = new List<string>();
                        foreach (DataRow row in scoreNameTable.Rows)
                        {
                            reviewInfo.ScoreNameList.Add(row["Name"].ToString());
                        }
                    }
                }

            }
            return reviewInfo;
        }


        /// <summary>
        /// 检查是否可以发表评论
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="customerSysno"></param>
        /// <param name="soSysno"></param>
        /// <returns></returns>
        public static bool CheckProductRemark(string productCode, int customerSysno, int soSysno, int reviewType)
        {

            DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductReview_CheckProductRemark");
            dataCommand.SetParameterValue("@ProductCode", productCode);
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysno);
            dataCommand.SetParameterValue("@SOSysNo", soSysno);
            dataCommand.SetParameterValue("@ReviewType", reviewType);
            SetCommandDefaultParameters(dataCommand);
            return dataCommand.ExecuteScalar<int>() > 0;

        }

        /// <summary>
        /// 发表回复权限
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public static Product_ReplyAuthorityInfo GetReplyAuthorityInfo(int customerSysNo, int reviewSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductReviewReply_GetReplyAuthorityInfo");
            dataCommand.SetParameterValue("@ReviewSysNo", reviewSysNo);
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            SetCommandDefaultParameters(dataCommand);
            return dataCommand.ExecuteEntity<Product_ReplyAuthorityInfo>();
        }

        /// <summary>
        /// 发表回复 
        /// </summary>
        /// <param name="replyInfo"></param>
        /// <returns></returns>
        public static bool CreateProductReviewReply(Product_ReplyDetail replyInfo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("CreateProductReviewReply");
            dataCommand.SetParameterValue("@ReviewSysNo", replyInfo.ReviewSysNo);
            dataCommand.SetParameterValue("@CustomerSysNo", replyInfo.Customer.SysNo);
            dataCommand.SetParameterValue("@Content", replyInfo.Content);
            SetCommandDefaultParameters(dataCommand);
            return dataCommand.ExecuteNonQuery() > 0;
        }


        /// <summary>
        /// 发表评论
        /// </summary>
        /// <param name="reviewInfo"></param>
        /// <returns></returns>
        public static bool CreateProductReview(Product_ReviewDetail reviewInfo)
        {
            bool result = false;
            DataCommand dataCommand = DataCommandManager.GetDataCommand("CreateProductReview");
            dataCommand.SetParameterValue("@ProductSysNo", reviewInfo.ProductSysNo);
            dataCommand.SetParameterValue("@CustomerSysNo", reviewInfo.CustomerInfo.SysNo);
            dataCommand.SetParameterValue("@Title", reviewInfo.Title);
            dataCommand.SetParameterValue("@Prons", reviewInfo.Prons);
            dataCommand.SetParameterValue("@Cons", reviewInfo.Cons);
            dataCommand.SetParameterValue("@Service", reviewInfo.Service);
            dataCommand.SetParameterValue("@Score", reviewInfo.Score);
            dataCommand.SetParameterValue("@Image", reviewInfo.Image);
            dataCommand.SetParameterValue("@Score1", reviewInfo.Score1);
            dataCommand.SetParameterValue("@Score2", reviewInfo.Score2);
            dataCommand.SetParameterValue("@Score3", reviewInfo.Score3);
            dataCommand.SetParameterValue("@Score4", reviewInfo.Score4);
            dataCommand.SetParameterValue("@ReviewType", reviewInfo.ReviewType);
            dataCommand.SetParameterValue("@SOSysNo", reviewInfo.SOSysno);
            SetCommandDefaultParameters(dataCommand);
            result = dataCommand.ExecuteNonQuery() > 0;

            return result;
        }


        /// <summary>
        /// 得到位用户评论评分
        /// </summary>
        /// <param name="reviewQueryInfo"></param>
        /// <returns></returns>
        public static Product_ReviewMaster GetProductReviewMaster(int productSysNo)
        {
            Product_ReviewMaster reviewMasterInfo = new Product_ReviewMaster();
            DataCommand dataCommand = DataCommandManager.GetDataCommand("ProductReview_GetProductReviewMaster");
            dataCommand.SetParameterValue("@ProductSysNo", productSysNo);
            SetCommandDefaultParameters(dataCommand);
            DataSet result = dataCommand.ExecuteDataSet();
            if (result != null && result.Tables.Count > 0)
            {
                DataTable reviewMasterInfoTable = result.Tables[0];
                if (reviewMasterInfoTable.Rows != null && reviewMasterInfoTable.Rows.Count > 0)
                {
                    reviewMasterInfo = DataMapper.GetEntity<Product_ReviewMaster>(reviewMasterInfoTable.Rows[0]);
                }

                if (result.Tables.Count > 1)
                {
                    DataTable scoreNameTable = result.Tables[1];
                    if (scoreNameTable.Rows != null && scoreNameTable.Rows.Count > 0 && reviewMasterInfo != null)
                    {
                        reviewMasterInfo.ScoreNameList = new List<string>();
                        foreach (DataRow row in scoreNameTable.Rows)
                        {
                            reviewMasterInfo.ScoreNameList.Add(row["Name"].ToString());
                        }
                    }
                }

                if (result.Tables.Count > 2)
                {
                    DataTable customerListTable = result.Tables[2];
                    if (customerListTable.Rows != null && customerListTable.Rows.Count > 0 && reviewMasterInfo != null)
                    {
                        reviewMasterInfo.CustomerList = DataMapper.GetEntityList<CustomerInfo, List<CustomerInfo>>(customerListTable.Rows);
                    }
                }
            }

            return reviewMasterInfo;
        }

        /// <summary>
        /// 根据用户customer以及商品编号获取订单编号以及商品code
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static List<Product_TempSOinfo> GetSoSysNoAndProductCode(int customerSysNo, int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSoSysNoAndProductCode");
            cmd.SetParameterValue("@customersysno", customerSysNo);
            cmd.SetParameterValue("@productSysNo", productSysNo);
            return cmd.ExecuteEntityList<Product_TempSOinfo>();
        }

        /// <summary>
        /// 根据用户customer以及商品编号获取-----该用户所有订单编号以及商品code
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static List<Product_TempSOinfo> GetCustomerSoSysNoAndProductCode(int customerSysNo, int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCustomerSoSysNoAndProductCode");
            cmd.SetParameterValue("@customersysno", customerSysNo);
            cmd.SetParameterValue("@productSysNo", productSysNo);
            return cmd.ExecuteEntityList<Product_TempSOinfo>();
        }
        /// <summary>
        /// 判断用户是否可以投票
        /// </summary>
        /// <param name="reviewSysNo"></param>
        /// <param name="cutomerSysNo"></param>
        /// <param name="LanguageCode"></param>
        /// <param name="CompanyCode"></param>
        /// <param name="StoreCompanyCode"></param>
        /// <returns></returns>
        public static bool CheckCustomerCanReviewVote(int reviewSysNo, int cutomerSysNo
                                                    , string LanguageCode, string CompanyCode, string StoreCompanyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Review_CheckProductReviewVote");
            cmd.SetParameterValue("@ReviewSysNo", reviewSysNo);
            cmd.SetParameterValue("@CustomerSysNo", cutomerSysNo);
            cmd.SetParameterValue("@LanguageCode", LanguageCode);
            cmd.SetParameterValue("@CompanyCode", CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", StoreCompanyCode);
            if (cmd.ExecuteScalar<int>() > 0)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 对评论投票
        /// </summary>
        /// <param name="reviewSysNo"></param>
        /// <param name="cutomerSysNo"></param>
        /// <param name="isUsefull"></param>
        /// <param name="LanguageCode"></param>
        /// <param name="CompanyCode"></param>
        /// <param name="StoreCompanyCode"></param>
        public static void UpdateProductReviewVote(int reviewSysNo, int cutomerSysNo, int isUsefull
                                                   , string LanguageCode, string CompanyCode, string StoreCompanyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Review_UpdateProductReviewVote");
            cmd.SetParameterValue("@ReviewSysNo", reviewSysNo);
            cmd.SetParameterValue("@CustomerSysNo", cutomerSysNo);
            cmd.SetParameterValue("@IsUsefull", isUsefull);
            cmd.SetParameterValue("@LanguageCode", LanguageCode);
            cmd.SetParameterValue("@CompanyCode", CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", StoreCompanyCode);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 15天购买同一商品只能发表一次
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public static DateTime CheckCreateReviewByDays(int productSysNo, int customerSysNo)
        {
            DateTime result = new DateTime();
            DataCommand cmd = DataCommandManager.GetDataCommand("CheckCreateReviewByDays");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);

            result = cmd.ExecuteScalar<DateTime>();

            return result;
        }



        /// <summary>
        /// 检查订单是否发表过评论，若已发表，则不能再发表
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="customerSysNo"></param>
        /// <returns>true:已发表；false：未发表(不限状态)</returns>
        public static bool CheckReviewBySoSysNo(int SOSysno)
        {
            bool result = false;
            DataCommand cmd = DataCommandManager.GetDataCommand("CheckReviewBySoSysNo");
            cmd.SetParameterValue("@SOSysno", SOSysno);

            int i = cmd.ExecuteScalar<Int32>();
            if (i > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 检查订单是否发表过几次评论，若已发表1次以上，则不能再发表
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="customerSysNo"></param>
        /// <returns>0从来就没有评论过；1评论了一次，还可以评论一次；2不能在进行评论</returns>
        public static int CheckReviewedBySoSysNo(int sosysno, int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CheckReviewdDBySoSysNo");
            cmd.SetParameterValue("@SOSysno", sosysno);
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.ExecuteDataSet();
            var totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
            return totalCount;
        }

        public static QueryResult<Product_NoReviewOrderMaster> QueryCustomerNoReviewOrderProducts(int customerSysNo, int pageIndex, int pageSize)
        {
            QueryResult<Product_NoReviewOrderMaster> result = new QueryResult<Product_NoReviewOrderMaster>();

            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                StartRowIndex = pageIndex * pageSize,
                MaximumRows = pageSize
            };

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCustomerNoReviewOrderProducts");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, pagingInfo, "SoSysno DESC"))
            {
                command.SetParameterValue("@CustomerSysNo", customerSysNo);
                command.CommandText = sqlBuilder.BuildQuerySql();

                var ds = command.ExecuteDataSet();
                if (ds != null && ds.Tables != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        result.ResultList = DataMapper.GetEntityList<Product_NoReviewOrderMaster, List<Product_NoReviewOrderMaster>>(ds.Tables[0].Rows);
                    }
                    if (ds.Tables.Count > 1)
                    {
                        List<Product_ReviewSimpleProductInfo> products = DataMapper.GetEntityList<Product_ReviewSimpleProductInfo, List<Product_ReviewSimpleProductInfo>>(ds.Tables[1].Rows);
                        if (result.ResultList != null && products != null)
                        {
                            foreach (var item in result.ResultList)
                            {
                                item.NoReviewOrderProducts = products.FindAll(x => x.SOSysNo == item.SOSysNo);
                            }
                        }
                    }
                }
                result.PageInfo = new PageInfo()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"))
                };

                return result;
            }
        }
    }
}
