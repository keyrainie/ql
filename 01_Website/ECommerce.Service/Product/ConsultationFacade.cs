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
    public class ConsultationFacade
    {
        /// <summary>
        /// 三分钟检查
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public static ConsultationInfo CheckProductConsultInfo(int customerSysNo)
        {
            return ConsultationDA.CheckProductConsultInfo(customerSysNo);
        }
        /// <summary>
        /// 获取咨询
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public static ConsultationInfo GetConsultListBySysNo(ConsultQueryInfo queryInfo)
        {
            ConsultationInfo consultationInfo = ConsultationDA.GetConsultListBySysNo(queryInfo);
            if (consultationInfo != null)
            {
                consultationInfo.Content = CommonFacade.SetCannotOnlineWordsMask(consultationInfo.Content);
                if (consultationInfo.PagedReplyList != null)
                {
                    foreach (var reply in consultationInfo.PagedReplyList)
                    {
                        reply.Content = CommonFacade.SetCannotOnlineWordsMask(reply.Content);
                    }
                }
            }
            return consultationInfo;
        }
        /// <summary>
        /// 获取咨询列表
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public static PagedResult<ConsultationInfo> GetProductDetailConsultList(ConsultQueryInfo queryInfo)
        {
            PagedResult<ConsultationInfo> productConsultation = ConsultationDA.GetProductDetailConsultList(queryInfo);
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
        /// 发表咨询
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool CreateProductConsult(ConsultationInfo info)
        {
            string cacheKey = CommonFacade.GenerateKey("CreateProductConsult", info.CustomerSysNo.ToString(), info.ProductSysNo.ToString());
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
            if (ConstValue.ProductConsultSwitch)
            {
                info.Status = "A";
            }
            else
            {
                info.Status = "O";
            }
            result = ConsultationDA.CreateProductConsult(info);

            if (result)
            {
                now = DateTime.Now;
                nowTimePoint = now.Hour * 3600 + now.Minute * 60 + now.Second;
                HttpRuntime.Cache[cacheKey] = nowTimePoint;
            }
            return result;
        }



        /// <summary>
        /// 发表咨询回复
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool CreateProductConsultReply(ProductConsultReplyInfo info, int productSysNo, string productName)
        {
            string cacheKey = CommonFacade.GenerateKey("CreateProductConsultReply", info.CustomerSysNo.ToString(), info.ConsultSysNo.ToString());
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
            result = ConsultationDA.CreateProductConsultReply(info);
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
        /// 咨询回复发送邮件
        /// </summary>
        /// <param name="info"></param>
        public static void SendMailConsultReply(ProductConsultReplyInfo info, int productSysNo, string productName)
        {
            AsyncEmail email = new AsyncEmail();
            email.MailAddress = info.CustomerInfo.Email;
            email.CustomerID = info.CustomerInfo.CustomerID;
            email.Status = (int)EmailStatus.NotSend;
            email.ImgBaseUrl = ConfigurationManager.AppSettings["CDNWebDomain"].ToString();

            string subject = string.Empty;
            email.MailBody = MailHelper.GetMailTemplateBody("ConsultReply", out subject);
            string productdetail = ConstValue.WebDomain + "/Product/Detail/" + productSysNo.ToString();

            ConsultationInfo consultInfo = ConsultationDA.GetProductConsult(info.ConsultSysNo);
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
