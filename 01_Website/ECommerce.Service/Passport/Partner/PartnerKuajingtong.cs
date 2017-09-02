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
    public class PartnerKuajingtong : Partners
    {
        public override void SetRequestParam(PartnerContext context)
        {
            context.RequestParam["method"] = HttpUtility.UrlEncode("sso.checkLogin");
            context.RequestParam["version"] = HttpUtility.UrlEncode("1.0");
            context.RequestParam["appId"] = HttpUtility.UrlEncode(context.PassportInfo.Parnter.AppID);
            context.RequestParam["timestamp"] = GetNowTime2Timestamp();
            context.RequestParam["nonce"] = (new Random()).Next(100000).ToString();
            context.RequestParam["back_url"] = SpecialUrlEncode(BuildActionUrl(context.PassportInfo.Base.BaseUrl, context.PassportInfo.Parnter.LoginBackUrl));
            context.RequestParam["need_login"] = "1";
            context.RequestParam["client_state"] = SpecialUrlEncode(context.ReturnUrl);
            context.RequestParam["sign"] = SignData(context);
        }

        public override bool BackVerifySign(PartnerBackContext context)
        {
            NameValueCollection param = new NameValueCollection();
            param["err_code"] = context.ResponseParam.AllKeys.Contains("err_code") ? context.ResponseParam["err_code"] : "";
            if (!string.IsNullOrWhiteSpace(param["err_code"]) && !param["err_code"].Equals("0"))
            {
                param["err_msg"] = context.ResponseParam.AllKeys.Contains("err_msg") ? context.ResponseParam["err_msg"] : "";
                if (!string.IsNullOrWhiteSpace(param["err_msg"]))
                {
                    param["err_msg"] = SpecialUrlEncode(param["err_msg"]);
                }
            }
            param["login_status"] = context.ResponseParam.AllKeys.Contains("login_status") ? context.ResponseParam["login_status"] : "";
            param["kjt_user_id"] = context.ResponseParam.AllKeys.Contains("kjt_user_id") ? context.ResponseParam["kjt_user_id"] : "";
            param["client_state"] = context.ResponseParam.AllKeys.Contains("client_state") ? context.ResponseParam["client_state"] : "";
            param["sign"] = context.ResponseParam.AllKeys.Contains("sign") ? context.ResponseParam["sign"] : "";
            if (!string.IsNullOrWhiteSpace(param["client_state"]))
            {
                context.ReturnUrl = param["client_state"];
                param["client_state"] = SpecialUrlEncode(param["client_state"]);
            }
            context.ResponseParam = param;

            string localSign = SignData(context);
            if (context.ResponseParam.AllKeys.Contains("sign")
                && context.ResponseParam["sign"].ToLower().Trim().Equals(localSign.ToLower().Trim()))
            {
                return true;
            }

            Logger.WriteLog(string.Format("验签失败，跨境通签名为：{0}，本地签名：{1}", context.ResponseParam["sign"], localSign), "PassportKuajingtong", "PassportVerifySign");
            return false;
        }

        public override void GetResponseUserInfo(PartnerBackContext context)
        {
            int rdm = (new Random()).Next(100000);
            string kuajingtongUserID = context.ResponseParam["kjt_user_id"];

            NameValueCollection param = new NameValueCollection();
            param["method"] = HttpUtility.UrlEncode("user.getInfo");
            param["version"] = HttpUtility.UrlEncode("1.0");
            param["appId"] = HttpUtility.UrlEncode(context.PassportInfo.Parnter.AppID);
            param["timestamp"] = GetNowTime2Timestamp();
            param["nonce"] = rdm.ToString();
            param["kjt_user_id"] = kuajingtongUserID;
            context.ResponseParam = param;

            string requestParam = string.Format("method={0}&version={1}&appId={2}&timestamp={3}&nonce={4}&kjt_user_id={5}&sign={6}",
                HttpUtility.UrlEncode("user.getInfo"), HttpUtility.UrlEncode("1.0"),
                HttpUtility.UrlEncode(context.PassportInfo.Parnter.AppID),
                GetNowTime2Timestamp(), rdm, 
                kuajingtongUserID, SignData(context));
            string requestUrl = string.Format("{0}?{1}", context.PassportInfo.Parnter.GetUserInfoUrl, requestParam);
            string responseData = HttpGetRequest(requestUrl, context.PassportInfo.Parnter.Encoding);
            KuajingtongReturnData entity = SerializationUtility.JsonDeserialize<KuajingtongReturnData>(responseData);
            if (entity == null || entity.err_code.Equals("1") || !entity.err_code.Equals("0"))
            {
                Logger.WriteLog(string.Format("获取跨境通用户信息失败，原始数据：{0}", responseData), "PassportKuajingtong", "GetUserInfo");
                throw new BusinessException("用户不存在！");
            }

            context.CustomerID = string.Format("Kuajingtong_{0}", entity.id);
            context.CustomerName = entity.username;
            context.Email = entity.email;
            context.IsAuth = entity.is_real_auth;
            context.CustomerSouce = Enums.CustomerSourceType.Kuajingtong;
        }

        private string SignData(PartnerContext context)
        {
            StringBuilder builder = new StringBuilder();
            if (context.RequestParam != null && context.RequestParam.AllKeys != null
                && context.RequestParam.AllKeys.Length > 0)
            {
                List<string> allKeys = context.RequestParam.AllKeys.OrderBy(m => m).ToList();
                foreach (string key in allKeys)
                {
                    builder.AppendFormat("{0}={1}&", key, context.RequestParam[key]);
                }
            }
            string sourceData = builder.ToString().TrimEnd('&');
            sourceData += string.Format("&{0}", context.PassportInfo.Parnter.AppSecret);
            string signData = GetMD5(sourceData, context.PassportInfo.Parnter.Encoding);

            //Debug模式下记录相关信息至日志
            if (context.PassportInfo.Parnter.Debug.Equals("1"))
            {
                string sourceParam = BuildStringFromNameValueCollection(context.RequestParam);
                Logger.WriteLog(string.Format("原始值：{0}，签名明文：{1}，签名：{2}", sourceParam, sourceData, signData), "PassportKuajingtong", "PassportSignData");
            }

            return signData;
        }

        private string SignData(PartnerBackContext context)
        {
            StringBuilder builder = new StringBuilder();
            if (context.ResponseParam != null && context.ResponseParam.AllKeys != null
                && context.ResponseParam.AllKeys.Length > 0)
            {
                List<string> allKeys = context.ResponseParam.AllKeys.OrderBy(m => m).ToList();
                foreach (string key in allKeys)
                {
                    if (!key.Equals("sign"))
                        builder.AppendFormat("{0}={1}&", key, context.ResponseParam[key]);
                }
            }
            string sourceData = builder.ToString().TrimEnd('&');
            sourceData += string.Format("&{0}", context.PassportInfo.Parnter.AppSecret);
            string signData = GetMD5(sourceData, context.PassportInfo.Parnter.Encoding);

            //Debug模式下记录相关信息至日志
            if (context.PassportInfo.Parnter.Debug.Equals("1"))
            {
                string sourceParam = BuildStringFromNameValueCollection(context.ResponseParam);
                Logger.WriteLog(string.Format("原始值：{0}，签名明文：{1}，签名：{2}", sourceParam, sourceData, signData), "PassportKuajingtong", "PassportBackSignData");
            }

            return signData;
        }
    }

    [Serializable]
    [DataContract]
    public class KuajingtongReturnData
    {
        [DataMember]
        public string err_code { get; set; }
        [DataMember]
        public string err_msg { get; set; }
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string username { get; set; }
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string mobile { get; set; }
        [DataMember]
        public string is_real_auth { get; set; }
        [DataMember]
        public string is_email_auth { get; set; }
    }
}
