//************************************************************************
// 用户名				泰隆优选
// 系统名				商品管理员实体
// 子系统名		        商品管理员基本信息实体
// 作成者				Tom.H.Li
// 改版日				2012.4.24
// 改版内容				新建
//************************************************************************

using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品管理员信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductManagerInfo : IIdentity
    {
        /// <summary>
        /// 管辖分类
        /// </summary>
        [DataMember]
        public CategoryInfo CategoryInfo { get; set; }

        /// <summary>
        /// 管辖品牌
        /// </summary>
        [DataMember]
        public BrandInfo BrandInfo { get; set; }

        /// <summary>
        /// 管理员用户信息
        /// </summary>
        [DataMember]
        public UserInfo UserInfo { get; set; }

        /// <summary>
        /// 管理员状态
        /// </summary>
        [DataMember]
        public PMStatus Status { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// PM是否存在于PM Group
        /// </summary>
        [DataMember]
        public int IsExistGroup { get; set; }

        /// <summary>
        /// 备份PM
        /// </summary>
        [DataMember]
        public string BackupUserList { get; set; }

        /// <summary>
        /// 所属仓库
        /// </summary>
        [DataMember]
        public string WarehouseNumber { get; set; }

        /// <summary>
        /// 每单限额TL
        /// </summary>
        [DataMember]
        public double MaxAmtPerOrder
        {
            get;
            set;
        }

        /// <summary>
        /// 每天限额TL
        /// </summary>
        [DataMember]
        public double MaxAmtPerDay
        {
            get;
            set;
        }
        /// <summary>
        /// 每单限额PMD
        /// </summary>
        [DataMember]
        public double pMDMaxAmtPerOrder;

        [DataMember]
        public double PMDMaxAmtPerOrder
        {
            get;
            set;
        }
        /// <summary>
        /// 每天限额PMD
        /// </summary>
        [DataMember]
        public double PMDMaxAmtPerDay
        {
            get;
            set;
        }
        /// <summary>
        /// 移仓每单重量上限
        /// </summary>
        [DataMember]
        public double ITMaxWeightforPerOrder
        {
            get;
            set;
        }
        /// <summary>
        /// 移仓每天重量上限
        /// </summary>
        [DataMember]
        public double ITMaxWeightforPerDay
        {
            get;
            set;
        }
        /// <summary>
        /// 本月销售目标(税后)
        /// </summary>
        [DataMember]
        public double SaleTargetPerMonth
        {
            get;
            set;
        }
        /// <summary>
        /// 库存销售比率
        /// </summary>
        [DataMember]
        public double SaleRatePerMonth
        {
            get;
            set;
        }

        [DataMember]
        public bool IsSelected { get; set; }
    }
}
