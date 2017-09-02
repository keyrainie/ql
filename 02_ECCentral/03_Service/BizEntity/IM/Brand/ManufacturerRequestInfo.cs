using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 厂商申请信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ManufacturerRequestInfo:ILanguage,ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }

        /// <summary>
        /// 生产商系统编号
        /// </summary>
        [DataMember]
        public int? ManufacturerSysNo { get; set; }

        /// <summary>
        /// 生产商名称
        /// </summary>
        [DataMember]
        public string ManufacturerName { get; set; }

        /// <summary>
        /// 生产商状态
        /// </summary>
        [DataMember]
        public ManufacturerStatus ManufacturerStatus { get; set; }
        /// <summary>
        /// 产品线
        /// </summary>
        [DataMember]
        public string ProductLine { get; set; }

        /// <summary>
        /// 申请理由
        /// </summary>
        [DataMember]
        public string Reasons { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public int? Status { get; set; }

        /// <summary>
        /// 其他名称
        /// </summary>
        [DataMember]
        public string ManufacturerBriefName { get; set; }


        /// <summary>
        /// 操作类型
        /// </summary>
        [DataMember]
        public int OperationType { get; set; }

        [DataMember]
        public string LanguageCode
        {
            get;
            set;
        }

        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }
    }
}
