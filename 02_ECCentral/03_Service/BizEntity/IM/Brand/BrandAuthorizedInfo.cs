using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
    [Serializable]
    [DataContract]
    public class BrandAuthorizedInfo:ICompany,ILanguage
    {
        [DataMember]
        public int Category1SysNo { get; set; }
        [DataMember]
        public int Category2SysNo { get; set; }
        [DataMember]
        public int Category3SysNo { get; set; }
        [DataMember]
        public string Category1Name { get; set; }
        [DataMember]
        public string Category2Name { get; set; }
        [DataMember]
        public string Category3Name { get; set; }
        [DataMember]
        public string ImageName { get; set; }
        [DataMember]
        public DateTime? StartActiveTime { get; set; }
        [DataMember]
        public DateTime? EndActiveTime { get; set; }
        /// <summary>
        /// 有效
        /// </summary>
        [DataMember]
        public bool AuthorizedAcive { get; set; }
        /// <summary>
        /// 无效
        /// </summary>
        [DataMember]
        public bool AuthorizedDeAcive { get; set; }

        /// <summary>
        /// 授权牌状态
        /// </summary>
        [DataMember]
        public AuthorizedStatus AuthorizedStatus { get; set; }

        [DataMember]
        public int BrandSysNo { get; set; }

        [DataMember]
        public int? ReferenceSysNo { get; set; }

        [DataMember]
        public int Type { get; set; }
        [DataMember]
        public int SysNo { get; set; }
        /// <summary>
        /// 是否已经存在该授权
        /// </summary>
        [DataMember]
        public bool IsExist { get; set; }

        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }

        [DataMember]
        public string LanguageCode
        {
            get;
            set;
        }

        [DataMember]
        public UserInfo User;
    }
}
