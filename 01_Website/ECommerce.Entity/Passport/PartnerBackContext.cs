using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECommerce.Entity.Member;
using ECommerce.Enums;

namespace ECommerce.Entity.Passport
{
    /// <summary>
    /// 第三方登录回调上下文
    /// </summary>
    [Serializable]
    [DataContract]
    public class PartnerBackContext
    {
        /// <summary>
        /// 第三方标识
        /// </summary>
        [DataMember]
        public string PartnerIdentify { get; set; }

        /// <summary>
        /// 回调参数
        /// </summary>
        [DataMember]
        public NameValueCollection ResponseParam { get; set; }

        /// <summary>
        /// 第三方配置
        /// </summary>
        [DataMember]
        public PassportSetting PassportInfo { get; set; }

        /// <summary>
        /// 用户来源
        /// </summary>
        [DataMember]
        public CustomerSourceType CustomerSouce { get; set; }

        /// <summary>
        /// 登录后返回的地址
        /// </summary>
        [DataMember]
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public string CustomerID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [DataMember]
        public string CustomerName { get; set; }

        /// <summary>
        /// 电子邮件
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// CellPhone
        /// </summary>
        [DataMember]
        public string CellPhone { get; set; }

        /// <summary>
        /// 是否认证
        /// </summary>
        [DataMember]
        public string IsAuth { get; set; }

        private PassportActionType actionType;
        /// <summary>
        /// 动作类型
        /// </summary>
        [DataMember]
        public PassportActionType ActionType
        {
            get { return actionType; }
            set { actionType = value; }
        }
    }

    /// <summary>
    /// 第三方登录回调处理结果
    /// </summary>
    [Serializable]
    [DataContract]
    public class PartnerBackResult
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        [DataMember]
        public CustomerInfo Customer { get; set; }
        /// <summary>
        /// 返回地址
        /// </summary>
        [DataMember]
        public string ReturnUrl { get; set; }
        /// <summary>
        /// 动作类型
        /// </summary>
        [DataMember]
        public PassportActionType ActionType { get; set; }
    }

    public enum PassportActionType
    {
        /// <summary>
        /// 适用于需要进行两步回调的登录认证，第一次回调用于获取AccessCode，第二次回调用于获取AcceccToken
        /// 请在第一次回调时将ActionType设置为Logon
        /// </summary>
        Logon,
        /// <summary>
        /// 适用于不需要进行两步回调的登录认证，请在回调完成后将ActionType设置为Accept
        /// </summary>
        Accept
    }
}
