using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Product;
using ECommerce.DataAccess.Product;
using ECommerce.Utility;
using ECommerce.Entity.Common;
using ECommerce.DataAccess.Common;
using System.Configuration;
using ECommerce.Entity;
using ECommerce.WebFramework.Mail;
using ECommerce.Enums;
using ECommerce.Entity.Product.Review;
using System.Web;
using System.Web.Caching;

namespace ECommerce.Facade.Product
{
    public class ReviewFacade
    {
        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public static Product_ReviewList GetProductReviewListByProductGroupSysNoForProduct(Product_ReviewQueryInfo queryInfo)
        {
            Product_ReviewList productReviewList = ReviewDA.GetProductReviewListByProductGroupSysNoForProduct(queryInfo);
            if (productReviewList != null && productReviewList.ProductReviewDetailList != null)
            {
                foreach (var productReviewDetail in productReviewList.ProductReviewDetailList)
                {
                    productReviewDetail.Cons = CommonFacade.SetCannotOnlineWordsMask(productReviewDetail.Cons);
                    productReviewDetail.Prons = CommonFacade.SetCannotOnlineWordsMask(productReviewDetail.Prons);
                    productReviewDetail.Service = CommonFacade.SetCannotOnlineWordsMask(productReviewDetail.Service);
                    if (productReviewDetail.ReplieList != null)
                    {
                        foreach (var reply in productReviewDetail.ReplieList)
                        {
                            reply.Content = CommonFacade.SetCannotOnlineWordsMask(reply.Content);
                        }
                    }
                }
            }
            return productReviewList;
        }

        /// <summary>
        /// 发表评论检查
        /// </summary>
        public static void CreateReviewPreCheck(Product_ReviewDetail reviewInfo)
        {
            if (reviewInfo.ProductSysNo <= 0)
            {
                throw new BusinessException("商品信息错误");
            }

            if (String.IsNullOrEmpty(reviewInfo.Title))
            {
                throw new BusinessException("评论标题不能为空！");
            }
            else if (reviewInfo.Title.Length > 40)
            {
                throw new BusinessException("标题长度不能大于40！");
            }

            if (String.IsNullOrEmpty(reviewInfo.Prons))
            {
                throw new BusinessException("评论优点不能为空！");
            }
            else if (reviewInfo.Prons.Length > 300)
            {
                throw new BusinessException("评论有点长度不能大于300！");
            }

            if (!String.IsNullOrEmpty(reviewInfo.Cons) && reviewInfo.Cons.Length > 300)
            {
                throw new BusinessException("评论缺点长度不能大于300！");
            }

            if (!String.IsNullOrEmpty(reviewInfo.Service) && reviewInfo.Service.Length > 300)
            {
                throw new BusinessException("评论服务长度不能大于300！");
            }

            //if (!String.IsNullOrEmpty(reviewInfo.Image))
            //{
            //    if (reviewInfo.Image.Split('|').Length > Int32.Parse(ResourceHelper.GetParam("WebUpload_ImageCountlimit_Rview")))
            //    {
            //        throw new BusinessException("FeedBackE0019");
            //    }
            //}
        }

        /// <summary>
        /// 发表评论 评分计算
        /// </summary>
        public static Product_ReviewDetail ReviewScoreCalculator(Product_ReviewDetail reviewInfo)
        {
            reviewInfo.Score = Convert.ToDecimal((((decimal)reviewInfo.Score1 + (decimal)reviewInfo.Score2 + (decimal)reviewInfo.Score3 + (decimal)reviewInfo.Score4) / 4).ToString("f1"));

            return reviewInfo;
        }

        /// <summary>
        /// 获取评论详情
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public static Product_ReviewDetail GetProductReviewInfo(Product_ReviewQueryInfo queryInfo)
        {
            Product_ReviewDetail productReviewDetail = ReviewDA.GetProductReviewInfo(queryInfo);
            if (productReviewDetail != null)
            {
                productReviewDetail.Cons = CommonFacade.SetCannotOnlineWordsMask(productReviewDetail.Cons);
                productReviewDetail.Prons = CommonFacade.SetCannotOnlineWordsMask(productReviewDetail.Prons);
                productReviewDetail.Service = CommonFacade.SetCannotOnlineWordsMask(productReviewDetail.Service);
                if (productReviewDetail.ReplieList != null)
                {
                    foreach (var reply in productReviewDetail.ReplieList)
                    {
                        reply.Content = CommonFacade.SetCannotOnlineWordsMask(reply.Content);
                    }
                }
            }
            return productReviewDetail;
        }

        /// <summary>
        /// 发表评论
        /// </summary>
        /// <param name="reviewInfo"></param>
        /// <returns></returns>
        public static bool CreateProductReview(Product_ReviewDetail reviewInfo)
        {
            string cacheKey = CommonFacade.GenerateKey("CreateProductReview", reviewInfo.CustomerInfo.SysNo.ToString(), reviewInfo.ProductSysNo.ToString());
            DateTime now = DateTime.Now;
            int nowTimePoint = now.Hour * 3600 + now.Minute * 60 + now.Second;
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                int preTimePoint = (int)HttpRuntime.Cache[cacheKey];
                if (nowTimePoint - preTimePoint < 60)
                {
                    throw new BusinessException("很抱歉，您发表评论的频率过快，请稍后再试。");
                }
            }
            else
            {
                HttpRuntime.Cache.Insert(cacheKey, 0, null, DateTime.Now.AddSeconds(CacheTime.Shortest), Cache.NoSlidingExpiration);
            }

            bool result = false;
            #region 二期修改不用此方法
            ////根据用户customer以及商品编号获取订单编号以及商品code
            //List<Product_TempSOinfo> tempsoinfo = ReviewDA.GetSoSysNoAndProductCode(reviewInfo.CustomerInfo.SysNo, reviewInfo.ProductSysNo);
            //if (tempsoinfo != null && tempsoinfo.Count > 0)
            //{
            //    CreateReviewPreCheck(reviewInfo);
            //    ReviewScoreCalculator(reviewInfo);
            //    //取消15天的检查
            //    //CheckCreateReviewByDays(reviewInfo.ProductSysNo, reviewInfo.CustomerInfo.SysNo);

            //    //检查当前订单号是否已评论
            //    if (reviewInfo.SOSysno != null && reviewInfo.SOSysno > 0)
            //    {
            //        Product_TempSOinfo temp = tempsoinfo.SingleOrDefault(p => p.SOSysNo == reviewInfo.SOSysno);
            //        if (temp != null)
            //        {
            //            bool isReview = ReviewDA.CheckReviewBySoSysNo(reviewInfo.SOSysno);
            //            if (isReview)
            //            {
            //                throw new BusinessException("当前订单已经参与过评论！");
            //            }
            //        }
            //        else
            //        {
            //            throw new BusinessException("须购买过此商品且订单已完成才能发表评论！");
            //        }
            //    }
            //    else
            //    {
            //        reviewInfo.SOSysno = tempsoinfo[0].SOSysNo;
            //    }
            //    //reviewInfo.SOSysno = tempsoinfo.SOSysNo;
            //    result = ReviewDA.CreateProductReview(reviewInfo);
            //}
            //else
            //{
            //    throw new BusinessException("须购买过此商品且订单已完成才能发表评论！");
            //}
            #endregion

            //根据用户customer以及商品编号获取-----该用户所有订单编号以及商品code
            List<Product_TempSOinfo> tempsoinfo = ReviewDA.GetCustomerSoSysNoAndProductCode(reviewInfo.CustomerInfo.SysNo, reviewInfo.ProductSysNo);
            if (tempsoinfo != null && tempsoinfo.Count > 0)
            {
                CreateReviewPreCheck(reviewInfo);
                ReviewScoreCalculator(reviewInfo);
                Product_TempSOinfo temp = tempsoinfo.SingleOrDefault(p => p.SOSysNo == reviewInfo.SOSysno);
                if (temp != null)
                {
                    //检查当前订单号是否已经完成两次评论
                    int num = ReviewDA.CheckReviewedBySoSysNo(temp.SOSysNo, reviewInfo.ProductSysNo);
                    if (num > 1)
                    {
                        throw new BusinessException("当前订单已经参与过两次评论！");
                    }
                    else
                    {
                        result = ReviewDA.CreateProductReview(reviewInfo);
                    }
                }
                else
                {
                    throw new BusinessException("须购买过此商品且订单已完成才能发表评论！");
                }
            }
            else
            {
                throw new BusinessException("须购买过此商品且订单已完成才能发表评论！");
            }
            if (result)
            {
                now = DateTime.Now;
                nowTimePoint = now.Hour * 3600 + now.Minute * 60 + now.Second;
                HttpRuntime.Cache[cacheKey] = nowTimePoint;
            }

            return result;
        }

        /// <summary>
        /// 15天检查
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="customerSysNo"></param>
        public static void CheckCreateReviewByDays(int productSysNo, int customerSysNo)
        {
            DateTime resutl = ReviewDA.CheckCreateReviewByDays(productSysNo, customerSysNo);
            DateTime now = DateTime.Now;
            if (resutl != null)
            {
                if (resutl.AddDays(15) > now)
                {
                    throw new BusinessException("15天内购买同一商品仅可发表一次！");
                }
            }
        }

        /// <summary>
        /// 发表回复权限
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="reviewSysNo"></param>
        /// <returns></returns>
        public static Product_ReplyAuthorityInfo GetReplyAuthorityInfo(int customerSysNo, int reviewSysNo)
        {
            return ReviewDA.GetReplyAuthorityInfo(customerSysNo, reviewSysNo);
        }

        /// <summary>
        /// 发表回复
        /// </summary>
        /// <param name="replyInfo"></param>
        /// <returns></returns>
        public static bool CreateProductReviewReply(Product_ReplyDetail replyInfo, int SoSysNo)
        {
            string cacheKey = CommonFacade.GenerateKey("CreateProductReviewReply", replyInfo.Customer.SysNo.ToString(), replyInfo.ReviewSysNo.ToString());
            DateTime now = DateTime.Now;
            int nowTimePoint = now.Hour * 3600 + now.Minute * 60 + now.Second;
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                int preTimePoint = (int)HttpRuntime.Cache[cacheKey];
                if (nowTimePoint - preTimePoint < 60)
                {
                    throw new BusinessException("很抱歉，您发表评论的频率过快，请稍后再试。");
                }
            }
            else
            {
                HttpRuntime.Cache.Insert(cacheKey, 0, null, DateTime.Now.AddSeconds(CacheTime.Shortest), Cache.NoSlidingExpiration);
            }

            bool result = ReviewDA.CreateProductReviewReply(replyInfo);
            //确认不需要发邮件
            //if (result)
            //{
            //    SendMailReviewReply(replyInfo, SoSysNo);
            //}
            if (result)
            {
                now = DateTime.Now;
                nowTimePoint = now.Hour * 3600 + now.Minute * 60 + now.Second;
                HttpRuntime.Cache[cacheKey] = nowTimePoint;
            }
            return result;
        }

        /// <summary>
        /// 得到用户评论评分
        /// </summary>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public static Product_ReviewMaster GetProductReviewMaster(int productSysNo)
        {
            return ReviewDA.GetProductReviewMaster(productSysNo);
        }

        public static int BuildReviewCssIndex(decimal reviewScore)
        {
            int reviewCssIndex = 10;
            if (reviewScore > 3.2M)
            {
                if (reviewScore >= 3.3M && reviewScore <= 3.7M)
                {
                    reviewCssIndex = 7;
                }
                else if (reviewScore >= 3.8M && reviewScore <= 4.2M)
                {
                    reviewCssIndex = 8;
                }
                else if (reviewScore >= 4.3M && reviewScore <= 4.7M)
                {
                    reviewCssIndex = 9;
                }
                else
                {
                    reviewCssIndex = 10;
                }
            }
            //往小的搜索
            else
            {
                if (reviewScore >= 2.8M && reviewScore <= 3.2M)
                {
                    reviewCssIndex = 6;
                }
                else if (reviewScore >= 2.3M && reviewScore <= 2.7M)
                {
                    reviewCssIndex = 5;
                }
                else if (reviewScore >= 1.8M && reviewScore <= 2.2M)
                {
                    reviewCssIndex = 4;
                }
                else if (reviewScore >= 1.3M && reviewScore <= 1.7M)
                {
                    reviewCssIndex = 3;
                }
                else if (reviewScore >= 0.8M && reviewScore <= 1.2M)
                {
                    reviewCssIndex = 2;
                }
                else if (reviewScore >= 0.1M && reviewScore <= 0.7M)
                {
                    reviewCssIndex = 1;
                }
                else
                {
                    reviewCssIndex = 0;
                }
            }
            return reviewCssIndex;
        }

        /// <summary>
        /// 检查是否可以发表评论
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="customerSysno"></param>
        /// <param name="soSysno"></param>
        /// <param name="reviewType"></param>
        /// <returns></returns>
        public static bool CheckProductRemark(string productCode, int customerSysno, int soSysno, int reviewType)
        {
            return ReviewDA.CheckProductRemark(productCode, customerSysno, soSysno, reviewType);
        }

        /// <summary>
        /// 评论回复发邮件
        /// </summary>
        public static void SendMailReviewReply(Product_ReplyDetail replyInfo, int sosysno)
        {
            AsyncEmail email = new AsyncEmail();
            email.MailAddress = replyInfo.Customer.Email;
            email.CustomerID = replyInfo.Customer.CustomerID;
            email.Status = (int)EmailStatus.NotSend;
            email.ImgBaseUrl = ConfigurationManager.AppSettings["CDNWebDomain"].ToString();
            string subject = string.Empty;
            email.MailBody = MailHelper.GetMailTemplateBody("ReviewReply", out subject);
            email.MailSubject = subject;
            email.MailBody = email.MailBody.Replace("[SOSysNo]", sosysno.ToString())
                                   .Replace("[ImgBaseUrl]", email.ImgBaseUrl)
                                   .Replace("[WebBaseUrl]", ConstValue.WebDomain)
                                   .Replace("[CurrentDateTime]", DateTime.Now.ToString("yyyy-MM-dd"))
                                   .Replace("[ReplyContent]", StringUtility.RemoveHtmlTag(replyInfo.Content));


            EmailDA.SendEmail(email);
        }

        /// <summary>
        /// 用户对评论投票
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="reviewSysNo"></param>
        /// <param name="isUsefull"></param>
        /// <returns></returns>
        public static bool UpdateReviewVote(int customerSysNo, int reviewSysNo, int isUsefull)
        {
            bool success = ReviewDA.CheckCustomerCanReviewVote(reviewSysNo, customerSysNo, ConstValue.LanguageCode, ConstValue.CompanyCode, ConstValue.StoreCompanyCode);
            if (success)
            {
                ReviewDA.UpdateProductReviewVote(reviewSysNo, customerSysNo, isUsefull, ConstValue.LanguageCode, ConstValue.CompanyCode, ConstValue.StoreCompanyCode);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 查询用户可以评论的订单商品列表
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="pageInfo"></param>
        /// <returns></returns>
        public static QueryResult<Product_NoReviewOrderMaster> QueryCustomerNoReviewOrderProducts(int customerSysNo, int pageIndex, int pageSize)
        {
            if (pageIndex < 0)
            {
                pageIndex = 0;
            }
            return ReviewDA.QueryCustomerNoReviewOrderProducts(customerSysNo, pageIndex, pageSize);
        }
    }
}
