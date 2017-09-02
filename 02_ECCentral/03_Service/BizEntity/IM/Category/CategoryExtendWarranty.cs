using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;
namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 类别延保信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class CategoryExtendWarranty : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 三级分类
        /// </summary>
        [DataMember]
        public CategoryInfo CategoryInfo { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        [DataMember]
        public BrandInfo Brand { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public CategoryExtendWarrantyStatus Status { get; set; }

        /// <summary>
        /// 延保编号
        /// </summary>
        [DataMember]
        public string ProductCode { get; set; }

        /// <summary>
        ///  延保年限
        /// </summary>
        [DataMember]
        public CategoryExtendWarrantyYears Years { get; set; }

        /// <summary>
        /// 价格下限
        /// </summary>
        [DataMember]
        public decimal MinUnitPrice { get; set; }

        /// <summary>
        /// 价格上限
        /// </summary>
        [DataMember]
        public decimal MaxUnitPrice { get; set; }

        /// <summary>
        /// 延保价格
        /// </summary>
        [DataMember]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 延保成本
        /// </summary>
        [DataMember]
        public decimal Cost { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [DataMember]
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public UserInfo InUser { get; set; }

        /// <summary>
        ///  是否前台展示
        /// </summary>
        [DataMember]
        public BooleanEnum IsECSelected { get; set; }

    }

    /// <summary>
    /// 类别延保信息
    /// </summary>
    public class CategoryExtendWarrantyDisuseBrand : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 三级分类
        /// </summary>
        public CategoryInfo CategoryInfo { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public BrandInfo Brand { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public CategoryExtendWarrantyDisuseBrandStatus Status { get; set; }
       
        /// <summary>
        /// 更新人
        /// </summary>
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public UserInfo InUser { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string LanguageCode { get; set; }

    }
}
