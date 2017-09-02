using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// Domain的商品分类信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductDepartmentCategory
    {        
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// Domain系统编号
        /// </summary>
        [DataMember]
        public int? ProductDomainSysNo { get; set; }

        /// <summary>
        /// 仅仅为了在UI那边绑定Category控件
        /// </summary>
        [DataMember]
        public int? C1SysNo { get; set; }

        /// <summary>
        /// 2级分类编号
        /// </summary>
        [DataMember]
        public int? C2SysNo { get; set; }

        /// <summary>
        /// 2级分类名称
        /// </summary>
        [DataMember]
        public string C2Name { get; set; }

        /// <summary>
        /// 品牌系统编号
        /// </summary>
        [DataMember]
        public int? BrandSysNo { get; set; }
        
        /// <summary>
        /// 品牌名称
        /// </summary>
        [DataMember]
        public string BrandName { get; set; }
        
        /// <summary>
        /// PM系统编号
        /// </summary>
        [DataMember]
        public int? PMSysNo { get; set; }

        /// <summary>
        /// PM名称
        /// </summary>
        [DataMember]
        public string PMName { get; set; }

        /// <summary>
        /// 备份PM名称列表
        /// </summary>
        [DataMember]
        public string BackupUserList { get; set; }
    }
}
