using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ECommerce.Enums;

namespace ECommerce.Entity.Promotion
{
    /// <summary>
    /// 组合销售
    /// </summary>
    [Serializable]
    [DataContract]
    public class ComboInfo  
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
        public string SaleRuleName { get; set; }

        /// <summary>
        /// 是否显示标题
        /// </summary>
        [DataMember]
        public bool IsShowName { get; set; }

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
        /// 公司编号
        /// </summary>
        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        } 
        #endregion 

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
        public int  ComboSysNo { get; set; }
        /// <summary>
        /// 商品系统编号
        /// </summary>
        [DataMember]
        public int  ProductSysNo { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        [DataMember]
        public int  Quantity { get; set; }
        /// <summary>
        /// 商品的折扣
        /// </summary>
        [DataMember]
        public decimal  Discount { get; set; }
        /// <summary>
        /// 是否为主商品
        /// </summary>
        [DataMember]
        public bool  IsMasterItemB { get; set; }
        
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
        /// 商品图片
        /// </summary>
        [DataMember]
        public string DefaultImage { get; set; }
        
        /// <summary>
        /// 商品当前价格
        /// </summary>
        [DataMember]
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        [DataMember]
        public decimal TariffRate { get; set; }

    }
}
