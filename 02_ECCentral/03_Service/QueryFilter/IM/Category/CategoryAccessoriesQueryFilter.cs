using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
    public class CategoryAccessoriesQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 三级类
        /// </summary>
        public int? Category3SysNo { get; set; }

        /// <summary>
        /// 二级类
        /// </summary>
        public int? Category2SysNo { get; set; }

        /// <summary>
        /// 一级类
        /// </summary>
        public int? Category1SysNo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public CategoryAccessoriesStatus? Status { get; set; }

        /// <summary>
        /// 配件名称
        /// </summary>
        public string AccessoriesName { get; set; }

        /// <summary>
        /// 顺序
        /// </summary>
        public int? AccessoryOrder { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        public IsDefault? IsDefault { get; set; }
    }
}
