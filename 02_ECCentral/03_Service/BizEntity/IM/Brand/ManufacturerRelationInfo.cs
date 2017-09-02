//************************************************************************
// 用户名				泰隆优选
// 系统名				厂商关系管理实体
// 子系统名		        厂商同步信息实体
// 作成者				Roman.Z.Li
// 改版日				2016.9.22
// 改版内容				新建
//************************************************************************

using ECCentral.BizEntity.Common;
using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 厂商同步
    /// </summary>
    [Serializable]
    [DataContract]
    public class ManufacturerRelationInfo : IIdentity, ICompany, ILanguage
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 生产商本地化名称编号
        /// </summary>
        [DataMember]
        public int? LocalManufacturerSysNo { get; set; }

        /// <summary>
        /// Newegg
        /// </summary>
        [DataMember]
        public string NeweggManufacturer { get; set; }

        /// <summary>
        /// Amazon
        /// </summary>
        [DataMember]
        public string AmazonManufacturer { get; set; }

        /// <summary>
        /// EBay
        /// </summary>
        [DataMember]
        public string EBayManufacturer { get; set; }

        /// <summary>
        /// 其他
        /// </summary>
        [DataMember]
        public int? OtherManufacturerSysNo { get; set; }

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
