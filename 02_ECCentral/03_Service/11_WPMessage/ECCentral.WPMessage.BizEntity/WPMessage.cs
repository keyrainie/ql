using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ECCentral.BizEntity;
using System.ComponentModel;

namespace ECCentral.WPMessage.BizEntity
{
    [Serializable]
    [DataContract]
    public class WPMessage
    {
        [DataMember]
        public int SysNo { get; set; }
        ///<summary>类别SysNo</summary>
        [DataMember]
        public int CategorySysNo { get; set; }
        ///<summary>业务数据的系统编号</summary>
        [DataMember]
        public string BizSysNo { get; set; }
        /// <summary>
        /// 对应的消息类型中的Url的参数，多个参数用"&"隔开
        /// </summary>
        [DataMember]
        public string Parameters { get; set; }
        ///<summary>
        ///消息的补充信息，比如商品相关的，可以填写商品ID；这个信息是可以帮助其它用户快速的定位到这条业务数据的。
        ///</summary>
        [DataMember]
        public string Memo { get; set; }
        ///<summary>0=待处理状态,1=处理中,2=处理完成</summary>
        [DataMember]
        public WPMessageStatus Status { get; set; }
        ///<summary>待处理项的创建时间</summary>
        [DataMember]
        public DateTime? CreateTime { get; set; }
        ///<summary>创建人</summary>
        [DataMember]
        public int CreateUserSysNo { get; set; }
        ///<summary>开始处理时间</summary>
        [DataMember]
        public DateTime? ProcessTime { get; set; }
        ///<summary>开始处理人</summary>
        [DataMember]
        public int ProcessUserSysNo { get; set; }
        ///<summary>完成处理时间</summary>
        [DataMember]
        public DateTime? CompletedTime { get; set; }
        ///<summary>完成处理人，这个完成处理人并不一定就是开始处理人</summary>
        [DataMember]
        public int CompletedUserSysNo { get; set; }
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCommonEnum")]
    public enum WPMessageStatus
    {
        //待处理状态,
        Waiting = 0,
        /// <summary>
        /// 处理中,
        /// </summary>
        Processing = 1,
        /// <summary>
        /// 处理完成
        /// </summary>
        Completed = 2
    }
}
