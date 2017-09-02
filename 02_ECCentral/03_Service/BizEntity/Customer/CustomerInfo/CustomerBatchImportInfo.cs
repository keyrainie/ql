using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 用户批量导入信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class CustomerBatchImportInfo : IWebChannel
    {
        /// <summary>
        /// 用户来源
        /// </summary>
        [DataMember]
        public string FromLinkSource
        {
            get;
            set;
        }

        /// <summary>
        /// 模板类型
        /// </summary>
        [DataMember]
        public TemplateType? TemplateType
        {
            get;
            set;
        }

        /// <summary>
        /// 模板文件流
        /// </summary>
        [DataMember]
        public string ImportFilePath
        {
            get;
            set;
        }

        #region IWebChannel Members
        /// <summary>
        /// 所属渠道
        /// </summary>
        [DataMember]
        public Common.WebChannel WebChannel { get; set; }
        #endregion

        #region ICompany Members
        /// <summary>
        /// 所属公司
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }

        #endregion
    }
}