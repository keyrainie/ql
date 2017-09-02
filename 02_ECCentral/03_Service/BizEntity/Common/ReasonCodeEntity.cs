using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    ///  ReasonCode信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ReasonCodeEntity : ICompany, IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public System.Int32? SysNo { get; set; }
        
        /// <summary>
        /// 编号
        /// </summary>
        [DataMember]
        public System.String ReasonCodeID { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        [DataMember]
        public System.Int32? ReasonCodeType { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public System.String Description { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        [DataMember]
        public System.Int32? ParentNodeSysNo { get; set; }

        public ReasonCodeEntity ParentNode { get; set; }
        
        /// <summary>
        /// 节点等级
        /// </summary>
        [DataMember]
        public System.Int32? NodeLevel { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public System.Int32? Status { get; set; }
        
        /// <summary>
        /// 使用语言
        /// </summary>
        [DataMember]
        public System.String LanguageCode { get; set; }
        
        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public System.String InUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public System.DateTime InDate { get; set; }
        
        /// <summary>
        /// 编辑人
        /// </summary>
        [DataMember]
        public System.String EditUser { get; set; }
        
        /// <summary>
        /// 编辑时间
        /// </summary>
        [DataMember]
        public System.DateTime? EditDate { get; set; }
        
        /// <summary>
        /// 所属公司
        /// </summary>
        [DataMember]
        public System.String CompanyCode { get; set; }
        
        /// <summary>
        /// 所属渠道
        /// </summary>
        [DataMember]
        public int? WebChannel { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        [DataMember]
        public string Path
        {
            get;
            set;
        }
    }
}
