using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using ECommerce.Entity.Passport;
using ECommerce.Enums;
using ECommerce.Utility;
using SolrNet.Utils;

namespace ECommerce.Facade.Passport.Partner
{
    /// <summary>
    /// QQ联合登录
    /// </summary>
    public class PartnerTencentQQ : Partners
    {
        /// <summary>
        /// 设置请求参数
        /// </summary>
        /// <param name="context">第三方登录上下文</param>
        public override void SetRequestParam(PartnerContext context)
        {
            context.RequestParam["response_type"] = "code";
            context.RequestParam["client_id"] = context.PassportInfo.Parnter.AppID;
            context.RequestParam["redirect_uri"] = SpecialUrlEncode(BuildActionUrl(context.PassportInfo.Base.BaseUrl, context.PassportInfo.Parnter.LoginBackUrl));
            context.RequestParam["state"] = context.PassportInfo.Parnter.CustomProperty1;
            context.RequestParam["timestamp"] = GetNowTime2Timestamp();
        }

        /// <summary>
        /// 第三方登录回调验签
        /// </summary>
        /// <param name="context">第三方登录回调上下文</param>
        /// <returns></returns>
        public override bool BackVerifySign(PartnerBackContext context)
        {
            context.ActionType = PassportActionType.Accept;
            NameValueCollection param = new NameValueCollection();
            param["code"] = context.ResponseParam.AllKeys.Contains("code") ? context.ResponseParam["code"] : "";
            param["state"] = context.ResponseParam.AllKeys.Contains("state") ? context.ResponseParam["state"] : "";
            context.ResponseParam = param;

            //如果url中有code参数，说明是获取code请求的回调，需要验证state值，防止CSRF攻击
            if (!String.IsNullOrWhiteSpace(param["code"]))
            {
                string localState = context.PassportInfo.Parnter.CustomProperty1;
                if (String.Equals(localState, param["state"], StringComparison.CurrentCultureIgnoreCase))
                {                    
                    return true;
                }
                Logger.WriteLog(string.Format("验签失败，QQ状态为：{0}，本地状态：{1}", context.ResponseParam["state"], localState), "PassportTencentQQ", "PassportVerifySign");
                return false;
            }
            return false;
        }


        /// <summary>
        /// 获取第三方登录回调用户信息
        /// </summary>
        /// <param name="context">第三方登录回调上下文</param>
        /// <exception cref="ECommerce.Utility.BusinessException">
        /// 登录失败！
        /// or
        /// </exception>
        public override void GetResponseUserInfo(PartnerBackContext context)
        {
            //第一步，获取AccessToken
            NameValueCollection data = new NameValueCollection();
            data.Add("client_id", context.PassportInfo.Parnter.AppID);
            data.Add("client_secret", context.PassportInfo.Parnter.AppSecret);
            data.Add("grant_type", "authorization_code");
            data.Add("code", context.ResponseParam["code"]);
            data.Add("redirect_uri", SpecialUrlEncode(BuildActionUrl(context.PassportInfo.Base.BaseUrl, context.PassportInfo.Parnter.LoginBackUrl)));

            string tokenResult = string.Empty;
            string tokenURL = string.Format("{0}?{1}", context.PassportInfo.Parnter.AccessTokenUrl, BuildStringFromNameValueCollection(data));
            
            HttpHelper.Get(tokenURL, out tokenResult);

            //Debug模式下记录相关信息至日志
            if (context.PassportInfo.Parnter.Debug.Equals("1"))
            {
                Logger.WriteLog(string.Format("获取AccessToken响应数据：{0}", tokenResult), "PassportTencentQQ", "AccessToken");
            }

            //"access_token=7A16EECB7F4904A1D7ACF851E4B283C7&expires_in=7776000&refresh_token=BC5FC164D418609ED26DDE16F8DA5EFF"
            NameValueCollection nvc = HttpUtility.ParseQueryString(tokenResult);
            context.ResponseParam["access_token"] = nvc["access_token"];

            //第二步，用AccessToken获取OpenID

            string returnData = string.Empty;
            string opidURL　= string.Format("{0}?access_token={1}", context.PassportInfo.Parnter.OpenIDUrl, context.ResponseParam["access_token"]);
            
            HttpHelper.Get(opidURL, out returnData);

            //Debug模式下记录相关信息至日志
            if (context.PassportInfo.Parnter.Debug.Equals("1"))
            {
                Logger.WriteLog(string.Format("获取OpenID响应数据：{0}", returnData), "PassportTencentQQ", "GetOpenID");
            }

            //PC网站接入时，获取到用户OpenID，返回包如下：
            //callback( {"client_id":"YOUR_APPID","openid":"YOUR_OPENID"} ); 
            string pattern = "callback\\(\\s*{\\s*\"client_id\"\\s*:\\s*\"(?<client_id>\\w+)\"\\s*,\\s*\"openid\"\\s*:\\s*\"(?<openid>\\w+)\"\\s*}\\s*\\)";
            var match = Regex.Match(returnData, pattern);
            string client_id = string.Empty;
            string openid = string.Empty;
            if (match != null)
            {
                client_id = match.Groups["client_id"].Value;
                openid = match.Groups["openid"].Value;
            }
            else
            {
                Logger.WriteLog(string.Format("获取OpenID失败，{0}", returnData), "PassportTencentQQ", "GetOpenID");
                throw new BusinessException("登录失败！");
            }

            //第三步，获取用户信息
            string requestParam = string.Format("access_token={0}&oauth_consumer_key={1}&openid={2}",
                  context.ResponseParam["access_token"],
                  context.PassportInfo.Parnter.AppID,
                  openid
               );

            string userInfoURL = string.Format("{0}?{1}", context.PassportInfo.Parnter.GetUserInfoUrl, requestParam);
            string responseData = string.Empty;
            
            HttpHelper.Get(userInfoURL, out responseData);

            //Debug模式下记录相关信息至日志
            if (context.PassportInfo.Parnter.Debug.Equals("1"))
            {
                Logger.WriteLog(string.Format("获取User响应数据：{0}", responseData), "PassportTencentQQ", "GetUserInfo");
            }

            TencentQQUserInfo userInfo = SerializationUtility.JsonDeserialize<TencentQQUserInfo>(responseData);
            if (userInfo.ret != 0)
            {
                throw new BusinessException(string.Format("登录失败！ {0}", userInfo.msg));
            }

            context.CustomerID = string.Format("TencentQQ_{0}", openid);
            context.CustomerName = userInfo.nickname;
            context.CustomerSouce = CustomerSourceType.TencentQQ;
        }
    }

    /*
     完整定义参见http://wiki.open.qq.com/wiki/website/get_user_info
     */
    [Serializable]
    [DataContract]
    public class TencentQQUserInfo
    {
        [DataMember]
        public int ret { get; set; }
        [DataMember]
        public string msg { get; set; }
        [DataMember]
        public string nickname { get; set; }
    }
}
