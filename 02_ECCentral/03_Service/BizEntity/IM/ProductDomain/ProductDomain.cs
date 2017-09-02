using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品内控管理
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductDomain : IIdentity,ICompany
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ProductDomain()
        {
            DepartmentMerchandiserSysNoList = new List<int?>();
            DepartmentCategoryList = new List<ProductDepartmentCategory>();
        }

        /// <summary>
        /// Domain名称
        /// </summary>
        [DataMember]
        public LanguageContent ProductDomainName { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 公司编码
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }
        
        /// <summary>
        /// PM Leader编号
        /// </summary>
        [DataMember]
        public int? ProductDomainLeaderUserSysNo { get; set; }             

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public ValidStatus? Status { get; set; }

        /// <summary>
        /// Domain下的类别列表
        /// </summary>
        [DataMember]
        public List<ProductDepartmentCategory> DepartmentCategoryList { get; set; }

        /// <summary>
        /// Merchandiser编号列表
        /// </summary>
        [DataMember]
        public List<int?> DepartmentMerchandiserSysNoList { get; set; }
    }
}
