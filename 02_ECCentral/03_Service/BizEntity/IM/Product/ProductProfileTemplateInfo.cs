using System;
using System.Runtime.Serialization;


namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 模板查询
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductProfileTemplateInfo
    {
        /// <summary>
        /// 编号
        /// </summary>
        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 模板类型
        /// </summary>
        [DataMember]
        public string TemplateType
        {
            get;
            set;
        }

        /// <summary>
        /// 模板名称
        /// </summary>
        [DataMember]
        public string TemplateName
        {
            get;
            set;
        }

        /// <summary>
        /// 模板描述
        /// </summary>
         [DataMember]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 模板值
        /// </summary>
       [DataMember]
        public string TemplateValue
        {
            get;
            set;
        }

        /// <summary>
        /// 用户ID
        /// </summary>
         [DataMember]
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// 优先级
        /// </summary>
        [DataMember]
        public int Priority
        {
            get;
            set;
        }

        /// <summary>
        /// 查询模板外部SysNo
        /// </summary>
        [DataMember]
        public int? ReferenceSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 公司ID
        /// </summary>
        [DataMember]
        public string CompanyCode
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

    }
}
