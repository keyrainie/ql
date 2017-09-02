using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.SO;

namespace ECCentral.BizEntity.MKT 
{
    /// <summary>
    /// 活动的商品范围条件
    /// </summary>
    public class PSProductCondition
    {
         
        /// <summary>
        /// 商品分类范围设置信息
        /// </summary>
        public RelCategory3 RelCategories { get; set; }

        /// <summary>
        /// 品牌范围设置信息
        /// </summary>
        public RelBrand RelBrands { get; set; }

        
        /// <summary>
        /// 商品范围设置信息
        /// </summary>
        public RelProduct RelProducts { get; set; }

        /// <summary>
        /// 限定商家集合
        /// </summary>
        public List<RelVendor> ListRelVendor { get; set; }
    }

    /// <summary>
    /// 限定商家实体
    /// </summary>
    public class RelVendor
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int VendorSysNo { get; set; }
        /// <summary>
        /// 商家名称
        /// </summary>
        public string VendorName { get; set; }
    }

    /// <summary>
    /// 商品分类范围设置信息
    /// </summary>
    public class RelCategory3  
    {
        /// <summary>
        /// 是否包含关系
        /// </summary>
        public bool? IsIncludeRelation { get; set; }
        /// <summary>
        /// 类别列表
        /// </summary>
        public List<SimpleObject> CategoryList { get; set; }
         
    }
        

    /// <summary>
    /// 品牌范围设置信息
    /// </summary>
    public class RelBrand  
    {
        /// <summary>
        /// 是否包含关系
        /// </summary>
        public bool? IsIncludeRelation { get; set; }
        /// <summary>
        /// 类别列表
        /// </summary>
        public List<SimpleObject> BrandList { get; set; }
         
    }

   

    /// <summary>
    /// 商品范围设置信息
    /// </summary>
    public class RelProduct  
    {
        /// <summary>
        /// 是否包含关系
        /// </summary>
        public bool? IsIncludeRelation { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        public List<RelProductAndQty> ProductList { get; set; }
         

    }
    /// <summary>
    /// 商品编码及数量设置
    /// </summary>
    public class RelProductAndQty
    {
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        ///<summary>
        /// 门槛数量，最少要达到多少的数量
        /// </summary>
        public int? MinQty { get; set; }    


        /**** 下面的都是用来显示的 *****/
        public int? PromotionSysNo { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 可用库存
        /// </summary>
        public int? AvailableQty { get; set; }
         
        /// <summary>
        /// 代销库存
        /// </summary>
        public int? ConsignQty { get; set; }
         
        /// <summary>
        /// 虚拟库存
        /// </summary>
        public int? VirtualQty { get; set; }
         
        /// <summary>
        /// 成本
        /// </summary>
        public decimal? UnitCost { get; set; }
        /// <summary>
        /// 售价
        /// </summary>
        public decimal? CurrentPrice { get; set; }

        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal? GrossMarginRate { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// 商家
        /// </summary>
        public int? VendorSysNo { get; set; }

        /// <summary>
        /// 加购价
        /// </summary>
        public decimal PlusPrice { get; set; }
          
    }
    /// <summary>
    /// 品牌和3级类别DTO
    /// </summary>
    public class BrandC3
    {
        /// <summary>
        /// 品牌系统编号
        /// </summary>
        public int? BrandSysNo{get;set;}
        /// <summary>
        /// 3级类别系统编号
        /// </summary>
        public int? C3SysNo{get;set;}
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName{get;set;}
        /// <summary>
        /// 3级类别名称
        /// </summary>
        public string C3Name {get;set;}
    }
}
