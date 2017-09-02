using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Passport
{
    /// <summary>
    /// 第三方登录上下文
    /// </summary>
    [Serializable]
    [DataContract]
    public class PartnerContext
    {
        /// <summary>
        /// 第三方标识
        /// </summary>
        [DataMember]
        public string PartnerIdentify { get; set; }

        /// <summary>
        /// 登录后返回的地址
        /// </summary>
        [DataMember]
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        [DataMember]
        public NameValueCollection RequestParam { get; set; }

        /// <summary>
        /// 第三方配置
        /// </summary>
        [DataMember]
        public PassportSetting PassportInfo { get; set; }
    }

    /// <summary>
    /// 第三方配置
    /// </summary>
    [Serializable]
    [DataContract]
    public class PassportSetting
    {
        /// <summary>
        /// 基础公共配置
        /// </summary>
        [DataMember]
        public PassportBase Base { get; set; }

        /// <summary>
        /// 第三方具体配置
        /// </summary>
        [DataMember]
        public PassportParnter Parnter { get; set; }
    }

    /// <summary>
    /// 基础公共配置
    /// </summary>
    [Serializable]
    [DataContract]
    public class PassportBase
    {
        /// <summary>
        /// 基地址
        /// </summary>
        [DataMember]
        public string BaseUrl { get; set; }
    }

    /// <summary>
    /// 第三方具体配置
    /// </summary>
    [Serializable]
    [DataContract]
    public class PassportParnter
    {
        /// <summary>
        /// 第三方标识
        /// </summary>
        [DataMember]
        public string PartnerIdentify { get; set; }

        /// <summary>
        /// 第三方名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        [DataMember]
        public string RequestType { get; set; }

        /// <summary>
        /// 处理程序
        /// </summary>
        [DataMember]
        public string PartnerProcessor { get; set; }

        /// <summary>
        /// 登录地址
        /// </summary>
        [DataMember]
        public string LoginUrl { get; set; }

        /// <summary>
        /// 获取AccessToken地址
        /// </summary>
        [DataMember]
        public string AccessTokenUrl { get; set; }

        /// <summary>
        /// 获取OpenID地址
        /// </summary>
        [DataMember]
        public string OpenIDUrl { get; set; }

        /// <summary>
        /// 登录回调地址
        /// </summary>
        [DataMember]
        public string LoginBackUrl { get; set; }

        /// <summary>
        /// 获取用户信息地址
        /// </summary>
        [DataMember]
        public string GetUserInfoUrl { get; set; }

        /// <summary>
        /// 本站在第三方平台的唯一ID
        /// </summary>
        [DataMember]
        public string AppID { get; set; }

        /// <summary>
        /// 本站在第三方平台唯一ID的的密钥
        /// </summary>
        [DataMember]
        public string AppSecret { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [DataMember]
        public string Encoding { get; set; }

        /// <summary>
        /// 是否debug，0-否；1-是
        /// </summary>
        [DataMember]
        public string Debug { get; set; }

        #region CustomProperty1至CustomProperty5为各第三方的自定义配置
        [DataMember]
        public string CustomProperty1 { get; set; }
        [DataMember]
        public string CustomProperty2 { get; set; }
        [DataMember]
        public string CustomProperty3 { get; set; }
        [DataMember]
        public string CustomProperty4 { get; set; }
        [DataMember]
        public string CustomProperty5 { get; set; }
        #endregion
    }
}
