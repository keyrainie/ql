using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using ECommerce.Entity.Passport;
using ECommerce.Utility;

namespace ECommerce.Facade.Passport.Partner
{
    /// <summary>
    /// 新浪微博联合登录
    /// </summary>
    public class PartnerSinaWeibo : Partners
    {
        public override void SetRequestParam(PartnerContext context)
        {
            context.RequestParam["client_id"] = context.PassportInfo.Parnter.AppID;
            context.RequestParam["redirect_uri"] = SpecialUrlEncode(BuildActionUrl(context.PassportInfo.Base.BaseUrl, context.PassportInfo.Parnter.LoginBackUrl));
            context.RequestParam["state"] = context.PassportInfo.Parnter.CustomProperty1;
            context.RequestParam["timestamp"] = GetNowTime2Timestamp();
        }

        public override bool BackVerifySign(PartnerBackContext context)
        {
            NameValueCollection param = new NameValueCollection();
            param["code"] = context.ResponseParam.AllKeys.Contains("code") ? context.ResponseParam["code"] : "";
            param["state"] = context.ResponseParam.AllKeys.Contains("state") ? context.ResponseParam["state"] : "";
            context.ResponseParam = param;
            context.ActionType = PassportActionType.Accept;

            string localState = context.PassportInfo.Parnter.CustomProperty1;
            if (String.Equals(localState, param["state"], StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            Logger.WriteLog(string.Format("验签失败，新浪微博状态为：{0}，本地状态：{1}", context.ResponseParam["state"], localState), "PassportSinaWeibo", "PassportVerifySign");
            return false;
        }

        public override void GetResponseUserInfo(PartnerBackContext context)
        {
            //第一步，获取AccessToken
            NameValueCollection postData = new NameValueCollection();
            postData.Add("client_id", context.PassportInfo.Parnter.AppID);
            postData.Add("client_secret", context.PassportInfo.Parnter.AppSecret);
            postData.Add("grant_type", "authorization_code");
            postData.Add("code", context.ResponseParam["code"]);
            postData.Add("redirect_uri", SpecialUrlEncode(BuildActionUrl(context.PassportInfo.Base.BaseUrl, context.PassportInfo.Parnter.LoginBackUrl)));
            string responseData = HttpPostRequest(context.PassportInfo.Parnter.AccessTokenUrl
                            , BuildStringFromNameValueCollection(postData)
                            , "application/x-www-form-urlencoded"
                            , context.PassportInfo.Parnter.Encoding);

            //Debug模式下记录相关信息至日志
            if (context.PassportInfo.Parnter.Debug.Equals("1"))
            {
                Logger.WriteLog(string.Format("获取AccessToken响应数据：{0}", responseData), "PassportSinaWeibo", "GetAccessToken");
            }

            SinaWeiboAccessToken returnData = SerializationUtility.JsonDeserialize<SinaWeiboAccessToken>(responseData);
            if (returnData == null)
            {
                throw new BusinessException("登录失败！");
            }

            //第二步，获取用户信息
            string requestParam = string.Format("access_token={0}&uid={1}",
               returnData.access_token,
               returnData.uid
                );
            responseData = HttpGetRequest(string.Format("{0}?{1}", context.PassportInfo.Parnter.GetUserInfoUrl, requestParam)
                , context.PassportInfo.Parnter.Encoding);

            //Debug模式下记录相关信息至日志
            if (context.PassportInfo.Parnter.Debug.Equals("1"))
            {
                Logger.WriteLog(string.Format("获取User响应数据：{0}", responseData), "PassportSinaWeibo", "GetUserInfo");
            }
            SinaWeiboUserInfo userInfo = SerializationUtility.JsonDeserialize<SinaWeiboUserInfo>(responseData);

            context.CustomerID = string.Format("SinaWeibo_{0}", userInfo.id);
            context.CustomerName = userInfo.screen_name;
            context.CustomerSouce = Enums.CustomerSourceType.Sina;
        }
    }

    [Serializable]
    [DataContract]
    public class SinaWeiboAccessToken
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string expires_in { get; set; }
        [DataMember]
        public string remind_in { get; set; }
        [DataMember]
        public string uid { get; set; }
    }

    /*
     完整定义参见http://open.weibo.com/wiki/2/users/show
     */
    [Serializable]
    [DataContract]
    public class SinaWeiboUserInfo
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string screen_name { get; set; }
        [DataMember]
        public string name { get; set; }
    }
}
