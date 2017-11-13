using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.DataAccess.Product;
using ECommerce.Entity.Product;
using ECommerce.Utility;
using ECommerce.Entity.Common;
using ECommerce.DataAccess.Common;
using System.Configuration;
using ECommerce.Enums;
using ECommerce.WebFramework.Mail;
using ECommerce.Entity;
using System.Web;
using System.Web.Caching;

namespace ECommerce.Facade.Product
{
    public class MatchedTradingFacade
    {

        /// <summary>
        /// 获取撮合交易列表
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public static PagedResult<MatchedTradingInfo> GetProductDetailMatchedTradingList(MatchedTradingQueryInfo queryInfo)
        {
            PagedResult<MatchedTradingInfo> productConsultation = MatchedTradingDA.GetProductDetailMatchedTradingList(queryInfo);
            if (productConsultation != null)
            {
                foreach (var item in productConsultation)
                {
                    item.Content = CommonFacade.SetCannotOnlineWordsMask(item.Content);
                }
            }
            return productConsultation;
        }

        /// <summary>
        /// 发表撮合交易
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool CreateProductMatchedTrading(MatchedTradingInfo info)
        {
            string cacheKey = CommonFacade.GenerateKey("CreateProductMatchedTrading", info.CustomerSysNo.ToString(), info.ProductSysNo.ToString());
            DateTime now = DateTime.Now;
            int nowTimePoint = now.Hour * 3600 + now.Minute * 60 + now.Second;
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                int preTimePoint = (int)HttpRuntime.Cache[cacheKey];
                if (nowTimePoint - preTimePoint < 60)
                {
                    throw new BusinessException("很抱歉，您发表咨询的频率过快，请稍后再试。");
                }
            }
            else
            {
                HttpRuntime.Cache.Insert(cacheKey, 0, null, DateTime.Now.AddSeconds(CacheTime.Shortest), Cache.NoSlidingExpiration);
            }

            bool result = false;
            if (ConstValue.ProductMatchedTradingSwitch)
            {
                info.Status = "A";
            }
            else
            {
                info.Status = "O";
            }
            result = MatchedTradingDA.CreateProductMatchedTrading(info);

            if (result)
            {
                now = DateTime.Now;
                nowTimePoint = now.Hour * 3600 + now.Minute * 60 + now.Second;
                HttpRuntime.Cache[cacheKey] = nowTimePoint;
            }
            return result;
        }



        /// <summary>
        /// 发表撮合交易回复
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool CreateProductMatchedTradingReply(ProductMatchedTradingReplyInfo info, int productSysNo, string productName)
        {
            string cacheKey = CommonFacade.GenerateKey("CreateProductMatchedTradingReply", info.CustomerSysNo.ToString(), info.MatchedTradingSysNo.ToString());
            DateTime now = DateTime.Now;
            int nowTimePoint = now.Hour * 3600 + now.Minute * 60 + now.Second;
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                int preTimePoint = (int)HttpRuntime.Cache[cacheKey];
                if (nowTimePoint - preTimePoint < 60)
                {
                    throw new BusinessException("很抱歉，您发表咨询的频率过快，请稍后再试。");
                }
            }
            else
            {
                HttpRuntime.Cache.Insert(cacheKey, 0, null, DateTime.Now.AddSeconds(CacheTime.Shortest), Cache.NoSlidingExpiration);
            }


            bool result = false;
            result = MatchedTradingDA.CreateProductMatchedTradingReply(info);
            //确认不需要发邮件
            //if (result)
            //{
            //    SendMailConsultReply(info, productSysNo, productName);
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
        /// 撮合交易回复发送邮件
        /// </summary>
        /// <param name="info"></param>
        public static void SendMailMatchedTradingReply(ProductMatchedTradingReplyInfo info, int productSysNo, string productName)
        {
            AsyncEmail email = new AsyncEmail();
            email.MailAddress = info.CustomerInfo.Email;
            email.CustomerID = info.CustomerInfo.CustomerID;
            email.Status = (int)EmailStatus.NotSend;
            email.ImgBaseUrl = ConfigurationManager.AppSettings["CDNWebDomain"].ToString();

            string subject = string.Empty;
            email.MailBody = MailHelper.GetMailTemplateBody("ConsultReply", out subject);
            string productdetail = ConstValue.WebDomain + "/Product/Detail/" + productSysNo.ToString();

            MatchedTradingInfo consultInfo = MatchedTradingDA.GetProductMatchedTrading(info.MatchedTradingSysNo);
            email.MailSubject = subject;
            email.MailBody = email.MailBody.Replace("[ProducDetail]", productdetail)
                                            .Replace("[ProductName]", productName)
                                            .Replace("[ImgBaseUrl]", email.ImgBaseUrl)
                                            .Replace("[WebBaseUrl]", ConstValue.WebDomain)
                                            .Replace("[CurrentDateTime]", DateTime.Now.ToString("yyyy-MM-dd"))
                                            .Replace("[Content]", StringUtility.RemoveHtmlTag(consultInfo.Content))
                                            .Replace("[ReplyContent]", StringUtility.RemoveHtmlTag(info.Content));

            EmailDA.SendEmail(email);
        }

    }
}
