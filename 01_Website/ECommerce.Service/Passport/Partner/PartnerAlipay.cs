using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Passport;
using ECommerce.Utility;

namespace ECommerce.Facade.Passport.Partner
{
    /// <summary>
    /// 支付宝联合登录
    /// </summary>
    public class PartnerAlipay : Partners
    {

        /// <summary>
        /// 设置请求参数
        /// </summary>
        /// <param name="context">第三方登录上下文</param>
        public override void SetRequestParam(PartnerContext context)
        {
            context.RequestParam["_input_charset"] = context.PassportInfo.Parnter.Encoding;
            context.RequestParam["partner"]        = context.PassportInfo.Parnter.AppID;
            context.RequestParam["service"]        = "alipay.auth.authorize";
            context.RequestParam["target_service"] = "user.auth.quick.login";
            context.RequestParam["return_url"]     = SpecialUrlEncode(BuildActionUrl(context.PassportInfo.Base.BaseUrl, context.PassportInfo.Parnter.LoginBackUrl));

            context.RequestParam["sign_type"]      = "MD5";
            context.RequestParam["sign"]           = SignRequestData(context);        

        }

        /// <summary>
        /// 第三方登录回调验签
        /// </summary>
        /// <param name="context">第三方登录回调上下文</param>
        /// <returns></returns>
        public override bool BackVerifySign(PartnerBackContext context)
        {
            string localSign = SignReturnData(context);
            context.ActionType = PassportActionType.Accept;

            if (context.ResponseParam["is_success"] == "T")
            {
                if (context.ResponseParam.AllKeys.Contains("sign")
                    && context.ResponseParam["sign"] == localSign )
                {
                    if (VerfyNotifyID(context, context.ResponseParam["notify_id"]))
                    {
                        return true;
                    }
                    else
                    {
                        Logger.WriteLog(string.Format("验证Notify失败，notify_id：{0}"
                            , context.ResponseParam["notify_id"]), "PartnerAlipay", "PassportVerifySign");
                    }
                }
            }

            Logger.WriteLog(string.Format("验签失败，支付宝签名为：{0}，本地签名：{1}"
                , context.ResponseParam["sign"], localSign)
                , "PartnerAlipay"
                , "PassportVerifySign");
            
            return false;
        }

        /// <summary>
        /// 获取第三方登录回调用户信息
        /// </summary>
        /// <param name="context">第三方登录回调上下文</param>
        public override void GetResponseUserInfo(PartnerBackContext context)
        {
            string alipayUserId       = context.ResponseParam["user_id"];
            string alipayUserEmail    = context.ResponseParam["email"];
            string alipayUserRealName = context.ResponseParam["real_name"];
            alipayUserRealName        = string.IsNullOrEmpty(alipayUserRealName) ? alipayUserId : alipayUserRealName;

            context.CustomerID = string.Format("Alipay_{0}", alipayUserId);
            context.CustomerName = alipayUserRealName;
            if (alipayUserEmail.IndexOf('@') > 0)
            {
                context.Email = alipayUserEmail;
            }
            else
            {
                context.CellPhone = alipayUserEmail;
            }
            
            context.CustomerSouce = Enums.CustomerSourceType.AliPay;
        }

        /// <summary>
        /// Signs the request data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private string SignRequestData(PartnerContext context)
        {
            StringBuilder builder = new StringBuilder();
            if (context.RequestParam != null 
                && context.RequestParam.AllKeys != null
                && context.RequestParam.AllKeys.Length > 0)
            {
                List<string> allKeys = context.RequestParam.AllKeys.OrderBy(m => m).ToList();
                foreach (string key in allKeys)
                {
                    if (key.Equals("sign", StringComparison.CurrentCultureIgnoreCase)
                        || key.Equals("sign_type", StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }

                    builder.AppendFormat("{0}={1}&", key, context.RequestParam[key]);
                }
            }

            string sourceData = builder.ToString().TrimEnd('&');

            sourceData += string.Format("{0}", context.PassportInfo.Parnter.AppSecret);
            string signData = GetMD5(sourceData, context.PassportInfo.Parnter.Encoding);

            //Debug模式下记录相关信息至日志
            if (context.PassportInfo.Parnter.Debug.Equals("1"))
            {
                string sourceParam = BuildStringFromNameValueCollection(context.RequestParam);
                Logger.WriteLog(string.Format("原始值：{0}，签名明文：{1}，签名：{2}", sourceParam, sourceData, signData), "PartnerAlipay", "SignRequestData");
            }

            return signData;
        }

        /// <summary>
        /// Signs the return data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private string SignReturnData(PartnerBackContext context)
        {
            StringBuilder builder = new StringBuilder();
            if (context.ResponseParam != null
                && context.ResponseParam.AllKeys != null
                && context.ResponseParam.AllKeys.Length > 0)
            {
                List<string> allKeys = context.ResponseParam.AllKeys.OrderBy(m => m).ToList();
                foreach (string key in allKeys)
                {
                    if (key.Equals("sign", StringComparison.CurrentCultureIgnoreCase)
                        || key.Equals("sign_type", StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }

                    builder.AppendFormat("{0}={1}&", key, context.ResponseParam[key]);
                }
            }

            string sourceData = builder.ToString().TrimEnd('&');

            sourceData += string.Format("{0}", context.PassportInfo.Parnter.AppSecret);
            string signData = GetMD5(sourceData, context.PassportInfo.Parnter.Encoding);

            //Debug模式下记录相关信息至日志
            if (context.PassportInfo.Parnter.Debug.Equals("1"))
            {
                string sourceParam = BuildStringFromNameValueCollection(context.ResponseParam);
                Logger.WriteLog(string.Format("原始值：{0}，签名明文：{1}，签名：{2}", sourceParam, sourceData, signData), "PartnerAlipay", "PassportSignReturnData");
            }

            return signData;
        }

        /// <summary>
        /// Verfies the notify identifier.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="notifyID">The notify identifier.</param>
        /// <returns></returns>
        private bool VerfyNotifyID(PartnerBackContext context, string notifyID)
        {
            string notifyURL= context.PassportInfo.Parnter.LoginUrl 
                    + string.Format("?service=notify_verify&partner={0}&notify_id={1}"
                    ,context.PassportInfo.Parnter.AppID
                    ,notifyID);

            string result = Partners.HttpGetRequest(notifyURL,"utf-8");

            return result.ToLower().Trim() == "true";
        }
    }
}
