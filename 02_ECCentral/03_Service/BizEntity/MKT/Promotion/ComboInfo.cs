using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.MKT 
{
    /// <summary>
    /// 组合销售
    /// </summary>
    [Serializable]
    [DataContract]
    public class ComboInfo : IIdentity, IWebChannel, ICompany 
    {
        public ComboInfo()
        {
            this.Items = new List<ComboItem>();
        }

        #region 基础
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [DataMember]
        public LanguageContent Name { get; set; }

        /// <summary>
        /// 当前状态:无效 -1,有效 0,待审核 1
        /// </summary>
        [DataMember]
        public ComboStatus? Status { get; set; }

        /// <summary>
        /// 目标状态
        /// </summary>
        [DataMember]
        public ComboStatus? TargetStatus { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        [DataMember]
        public string InUser { get; set; }
        /// <summary>
        /// 创建账户系统编号
        /// </summary>
        [DataMember]
        public int? CreateUserSysNo { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [DataMember]
        public ComboType SaleRuleType { get; set; }
        
        /// <summary>
        /// 显示优先级
        /// </summary>
        [DataMember]
        public int? Priority { get; set; }

        /// <summary>
        /// 前台是否显示标题
        /// </summary>
        [DataMember]
        public bool? IsShowName { get; set; }

        /// <summary>
        /// 引用系统编号
        /// </summary>
        [DataMember]
        public int? ReferenceSysNo { get; set; }
        /// <summary>
        /// 应用类型
        /// </summary>
        [DataMember]
        public int? ReferenceType { get; set; }


        /// <summary>
        /// 审核人 
        /// </summary>
        [DataMember]
        public string AuditUser { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        [DataMember]
        public DateTime? AuditDate { get; set; }
        /// <summary>
        /// 最后一次编辑用户
        /// </summary>
        [DataMember]
        public string EditUser { get; set; }
        /// <summary>
        /// 编辑时间
        /// </summary>
        [DataMember]
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        [DataMember]
        public WebChannel WebChannel
        {
            get;
            set;
        }

        /// <summary>
        /// 公司编号
        /// </summary>
        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }

        #endregion
        [DataMember]
        public int RequestSysNo { get; set; }
        /// <summary>
        /// 申请理由
        /// </summary>
       [DataMember]
        public string Reason { get; set; }
        /// <summary>
        /// 商品清单
        /// </summary>
        [DataMember]
        public List<ComboItem> Items { get; set; }

    }
    /// <summary>
    /// 套餐商品信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ComboItem
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }
        /// <summary>
        /// 套餐系统编号
        /// </summary>
        [DataMember]
        public int? ComboSysNo { get; set; }
        /// <summary>
        /// 商品系统编号
        /// </summary>
        [DataMember]
        public int? ProductSysNo { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        [DataMember]
        public int? Quantity { get; set; }
        /// <summary>
        /// 商品的折扣
        /// </summary>
        [DataMember]
        public decimal? Discount { get; set; }
        /// <summary>
        /// 是否为主商品
        /// </summary>
        [DataMember]
        public bool? IsMasterItemB { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 用于计算和显示，产品编号
        /// </summary>
        [DataMember]
        public string ProductID { get; set; }
        /// <summary>
        /// 用于计算和显示，产品名称
        /// </summary>
        [DataMember]
        public string ProductName { get; set; }
        /// <summary>
        /// 生产商系统编号
        /// </summary>
        [DataMember]
        public int? MerchantSysNo { get; set; }
        /// <summary>
        /// 生产商名称
        /// </summary>
        [DataMember]
        public string MerchantName { get; set; }
        /// <summary>
        /// 商品当前价格
        /// </summary>
        [DataMember]
        public decimal? ProductCurrentPrice { get; set; }
        /// <summary>
        /// 商品成本价格
        /// </summary>
        [DataMember]
        public decimal? ProductUnitCost { get; set; }
        /// <summary>
        /// 上哦赠送积分
        /// </summary>
        [DataMember]
        public int? ProductPoint { get; set; }
    }
}
