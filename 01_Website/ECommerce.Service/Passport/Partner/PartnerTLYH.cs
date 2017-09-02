using ECommerce.Entity.Passport;
using ECommerce.Enums;
using ECommerce.Utility;
using SolrNet.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace ECommerce.Facade.Passport.Partner
{
    /// <summary>
    /// 泰隆银行登陆
    /// </summary>
    public class PartnerTLYH : Partners
    {
        public override void SetRequestParam(Entity.Passport.PartnerContext context)
        {
            //第一次request
            string requestParam = string.Format("trxCode={0}&redirect_uri={1}",
               context.PassportInfo.Parnter.CustomProperty1,
               SpecialUrlEncode(BuildActionUrl(context.PassportInfo.Base.BaseUrl, context.PassportInfo.Parnter.LoginBackUrl)));
            Logger.WriteLog(string.Format("返回地址：{0}", SpecialUrlEncode(BuildActionUrl(context.PassportInfo.Base.BaseUrl, context.PassportInfo.Parnter.LoginBackUrl))), "PassportTLYH", "GetLoginUrl");
            string url = string.Format("{0}/{1}?{2}",
                context.PassportInfo.Parnter.LoginUrl,
                context.PassportInfo.Parnter.CustomProperty2,
                requestParam);
            string responseData = HttpGetRequest(url, context.PassportInfo.Parnter.Encoding);
            //Debug模式下记录相关信息至日志
            if (context.PassportInfo.Parnter.Debug.Equals("1"))
            {
                Logger.WriteLog(string.Format("获取第一次request响应数据：{0}", responseData), "PassportTLYH", "GetLoginUrl");
            }
            TLYHLoginInfo loginInfo = SerializationUtility.JsonDeserialize<TLYHLoginInfo>(responseData);
            if (loginInfo == null)
            {
                throw new BusinessException("登录失败！");
            }
            //第二次post
            context.RequestParam["uniteLoginCode"] = loginInfo.cd.uniteLoginVerifyCode;
            context.RequestParam["authorizedSerial"] = loginInfo.cd.authorizedSerial;
            context.RequestParam["redirect_uri"] = SpecialUrlEncode(BuildActionUrl(context.PassportInfo.Base.BaseUrl, context.PassportInfo.Parnter.LoginBackUrl));// context.ReturnUrl;
            context.PassportInfo.Parnter.LoginUrl += loginInfo.cd.uniteLoginUri;
        }


        public override bool BackVerifySign(Entity.Passport.PartnerBackContext context)
        {
            context.ActionType = PassportActionType.Accept;
            return true;
        }

        public override void GetResponseUserInfo(Entity.Passport.PartnerBackContext context)
        {
            Random r = new Random();
            string tlyhUserId = context.ResponseParam["customerId"];
            context.CustomerID = string.Format("TLYH_{0}", tlyhUserId);
            context.Email = context.ResponseParam["email"];
            context.CellPhone = context.ResponseParam["customerMobile"];
            context.CustomerName = string.Format("泰隆客户_{0}", r.Next(10000000, 99999999));
            context.CustomerSouce = Enums.CustomerSourceType.TLYH;
        }
    }

    [Serializable]
    [DataContract]
    public class TLYHLoginInfo
    {
        [DataMember]
        public string ec { get; set; }
        [DataMember]
        public string em { get; set; }
        [DataMember]
        public TLYHLoginInfoCD cd { get; set; }
    }

    [Serializable]
    [DataContract]
    public class TLYHLoginInfoCD
    {
        [DataMember]
        public string uniteLoginVerifyCode { get; set; }
        [DataMember]
        public string authorizedSerial { get; set; }
        [DataMember]
        public string uniteLoginUri { get; set; }
    }

}
