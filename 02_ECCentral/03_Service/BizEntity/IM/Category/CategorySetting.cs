using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ECCentral.BizEntity.IM.Category;
using System.Xml.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 基于类别的各类维护指标
    /// </summary>
    [Serializable]
    [DataContract]
    public class CategorySetting
    {
        /// <summary>
        /// 基于类别默认配件
        /// </summary>
        [DataMember]
        public List<CategoryAccessory> CategoryAccessories { get; set; }

        /// <summary>
        /// 基于类别默认属性
        /// </summary>
        [DataMember]
        public List<CategoryProperty> CategoryProperties { get; set; }

        /// <summary>
        /// 基于类别延保信息
        /// </summary>
        [DataMember]
        public List<CategoryExtendWarranty> CategoryExtendWarranty { get; set; }

        /// <summary>
        /// 初级毛利率
        /// </summary>
        [DataMember]
        public decimal PrimaryMargin { get; set; }

        /// <summary>
        /// 高级毛利率
        /// </summary>
        [DataMember]
        public decimal SeniorMargin { get; set; }

        /// <summary>
        /// 基本指标
        /// </summary>
        [DataMember]
        public CategoryBasic CategoryBasicInfo { get; set; }

        /// <summary>
        /// 毛利下限
        /// </summary>
        [DataMember]
        public CategoryMinMargin CategoryMinMarginInfo { get; set; }

        /// <summary>
        /// RMA信息
        /// </summary>
        [DataMember]
        public CategoryRMA CategoryRMAInfo { get; set; }

        /// <summary>
        /// 一级类名称
        /// </summary>
        [DataMember]
        public string C1Name { get; set; }

        /// <summary>
        /// 二级类名称
        /// </summary>
        [DataMember]
        public string C2Name { get; set; }

        /// <summary>
        /// 三级类名称
        /// </summary>
        [DataMember]
        public string C3Name { get; set; }

        /// <summary>
        /// 类别状态
        /// </summary>
        [DataMember]
        public CategoryStatus Status { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }
        
    }

    /// <summary>
    /// 基本信息指标
    /// </summary>
    public class CategoryBasic
    {
        /// <summary>
        /// 跌价风险
        /// </summary>
        public CheapenRisk CheapenRiskInfo { get; set; }

        /// <summary>
        /// 是否贵重物品
        /// </summary>
        public IsDefault IsValuableProduct { get; set; }

        /// <summary>
        /// 附件约束
        /// </summary>
        public IsDefault IsRequired { get; set; }

        /// <summary>
        /// 帐期选择
        /// </summary>
        public PayPeriodType PayPeriodTypeInfo { get; set; }

        /// <summary>
        /// 是否大货
        /// </summary>
        public IsLarge IsLargeInfo { get; set; }

        /// <summary>
        /// 缺货率
        /// </summary>
        public decimal OOSRate { get; set; }

        /// <summary>
        /// 缺货数量
        /// </summary>
        public int OOSQty { get; set; }

        /// <summary>
        /// 混合虚库率
        /// </summary>
        public decimal VirtualRate { get; set; }

        /// <summary>
        /// 纯虚库数量
        /// </summary>
        public int VirtualCount { get; set; }

        /// <summary>
        /// 天数
        /// </summary>
        public int SafetyInventoryDay { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int SafetyInventoryQty { get; set; }

        /// <summary>
        /// 额度
        /// </summary>
        public decimal? Quota { get; set; }

        /// <summary>
        /// 最低佣金限额
        /// </summary>
        public decimal MinCommission { get; set; }

        /// <summary>
        /// 日均销售量计算周期
        /// </summary>
        public int AvgDailySalesCycle { get; set; }

        /// <summary>
        /// 滞销库存天数信息
        /// </summary>
        public int InStockDays { get; set; }

        public int PropertySysNO { get; set; }
        /// <summary>
        /// 批量更新最低佣金用到的实体
        /// </summary>
        public CommissionInfo CommissionInfo { get; set; }
        /// <summary>
        /// 三级分类编号
        /// </summary>
        public int CategorySysNo { get; set; }
        public int Category1SysNo { get; set; }
        public int Category2SysNo { get; set; }
    }

    public class CommissionInfo
    {
        /// <summary>
        /// 商品状态
        /// </summary>
        public ProductStatus? ProductStatus { get; set; }
        /// <summary>
        /// PMSysNo
        /// </summary>
        public int? PMSysNo { get; set; }

        /// <summary>
        /// 等于;不等于
        /// </summary>
        public Comparison? Comparison { get; set; }
        /// <summary>
        /// 生产商SysNo
        /// </summary>
        public int ManufacturerSysNo { get; set; }

        public CategoryType? CategoryType { get; set; }
    }
    /// <summary>
    /// 毛利率指标
    /// </summary>  
    [KnownType(typeof(MinMarginKPI))]
    [KnownType(typeof(Dictionary<MinMarginDays, MinMarginKPI>))]
    public class CategoryMinMargin
    {
        /// <summary>
        /// 最小毛利率
        /// </summary>       

        public Dictionary<MinMarginDays, MinMarginKPI> Margin { get; set; }

        /// <summary>
        /// 三级分类编号
        /// </summary>
   
        public int CategorySysNo { get; set; }
    }

    /// <summary>
    /// RMA信息指标
    /// </summary>
    public class CategoryRMA
    {
        /// <summary>
        /// 保修天数
        /// </summary>
        public int WarrantyDays { get; set; }

        /// <summary>
        /// RMA 率标准
        /// </summary>
        public double RMARateStandard { get; set; }


        /// <summary>
        /// 三级分类编号
        /// </summary>
        public int CategorySysNo { get; set; }
    }

    /// <summary>
    /// 指标数据
    /// </summary>
 
    public class MinMarginKPI
    {
        /// <summary>
        /// 滞销库存时间
        /// </summary>  
      
        public MinMarginDays MinMarginDays { get; set; }

        /// <summary>
        /// 最低毛利率
        /// </summary> 
      
        public decimal MinMargin { get; set; }

        /// <summary>
        /// 最高毛利率
        /// </summary>
        public decimal MaxMargin { get; set; }
    }
}
