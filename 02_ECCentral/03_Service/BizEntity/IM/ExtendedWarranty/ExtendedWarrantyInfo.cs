using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 延保服务信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ExtendedWarrantyInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 延保服务编号
        /// </summary>
        [DataMember]
        public int ExtendedWarrantyID { get; set; }

        /// <summary>
        /// 延保服务支持的商品类别
        /// </summary>
        [DataMember]
        public CategoryInfo ProductCategory { get; set; }

        /// <summary>
        /// 延保服务支持的商品品牌
        /// </summary>
        [DataMember]
        public BrandInfo ProductBrand { get; set; }

        /// <summary>
        /// 延保服务限定的商品价格下限
        /// </summary>
        [DataMember]
        public decimal MinUnitPrice { get; set; }

        /// <summary>
        /// 延保服务限定的商品价格上限
        /// </summary>
        [DataMember]
        public decimal MaxUnitPrice { get; set; }

        /// <summary>
        /// 延保服务年限
        /// </summary>
        [DataMember]
        public decimal ServiceYears { get; set; }

        /// <summary>
        /// 延保服务价格
        /// </summary>
        [DataMember]
        public decimal ServiceUnitPrice { get; set; }

        /// <summary>
        /// 延保服务成本
        /// </summary>
        [DataMember]
        public decimal ServiceCost { get; set; }

        /// <summary>
        /// 延保服务状态
        /// </summary>
        [DataMember]
        public ValidStatus ServiceStatus { get; set; }

    }
}
