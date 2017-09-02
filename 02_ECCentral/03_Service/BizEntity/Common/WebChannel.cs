using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 渠道信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class WebChannel : IIdentity
    {
        #region IIdentity Members

        /// <summary>
        /// 渠道系统编号
        /// </summary>
        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }

        #endregion IIdentity Members

        /// <summary>
        /// 渠道编号
        /// </summary>
        [DataMember]
        public string ChannelID
        {
            get;
            set;
        }

        /// <summary>
        /// 渠道名称
        /// </summary>
        [DataMember]
        public string ChannelName { get; set; }

        /// <summary>
        /// 渠道类型
        /// </summary>
        [DataMember]
        public WebChannelType ChannelType { get; set; }

        public WebChannel()
        {
            ChannelType = WebChannelType.InternalChennel;
        }

        //public string CompanyCode { get; set; }
    }
}
