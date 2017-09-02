using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 系统用户信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class UserInfo : IIdentity
    {
        /// <summary>
        /// 用户账号
        /// </summary>
        [DataMember]
        public string UserID
        {
            get;
            set;
        }

        /// <summary>
        /// 用户名
        /// </summary>
        [DataMember]
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// 显示用户名称
        /// </summary>
        [DataMember]
        public string UserDisplayName
        {
            get;
            set;
        }
        /// <summary>
        /// 显示用户名称
        /// </summary>
        [DataMember]
        public string EmailAddress
        {
            get;
            set;
        }

        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }

        #endregion IIdentity Members

        #region 附加数据载体
        
        public string CompanyCode
        {
            get;
            set;
        }

        public string Domain
        {
            get;
            set;
        }

        #endregion
    }
}