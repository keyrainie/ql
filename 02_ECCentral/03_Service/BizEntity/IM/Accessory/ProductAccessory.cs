using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品配件
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductAccessory : ICompany,ILanguage
    {
        /// <summary>
        /// 编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 配件信息
        /// </summary>
        [DataMember]
        public AccessoryInfo AccessoryInfo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public ProductAccessoryStatus Status { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public int Qty { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public LanguageContent Description { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [DataMember]
        public int Priority { get; set; }

        /// <summary>
        /// 公司编码
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }

        /// <summary>
        /// 语言编码
        /// </summary>
        [DataMember]
        public string LanguageCode { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public UserInfo OperationUser { get; set; }
    }
}
