using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
    public class CategoryExtendWarrantyQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 一级类
        /// </summary>
        public int C1SysNo { get; set; }

        /// <summary>
        /// 二级类
        /// </summary>
        public int C2SysNo { get; set; }

        /// <summary>
        /// 三级类
        /// </summary>
        public int C3SysNo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public CategoryExtendWarrantyStatus Status { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 延保编号
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        ///  延保年限
        /// </summary>
        public CategoryExtendWarrantyYears Years { get; set; }

        /// <summary>
        /// 价格下限
        /// </summary>
        public decimal MinUnitPrice { get; set; }

        /// <summary>
        /// 价格上限
        /// </summary>
        public decimal MaxUnitPrice { get; set; }

        /// <summary>
        /// 延保价格
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 延保成本
        /// </summary>
        public decimal Cost { get; set; }


        /// <summary>
        ///  是否前台展示
        /// </summary>
        public BooleanEnum IsECSelected { get; set; }
    }

    public class CategoryExtendWarrantyDisuseBrandQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }
        /// <summary>
        /// 一级类
        /// </summary>
        public int C1SysNo { get; set; }

        /// <summary>
        /// 二级类
        /// </summary>
        public int C2SysNo { get; set; }

        /// <summary>
        /// 三级类
        /// </summary>
        public int C3SysNo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public CategoryExtendWarrantyDisuseBrandStatus Status { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }
       
    }
}
