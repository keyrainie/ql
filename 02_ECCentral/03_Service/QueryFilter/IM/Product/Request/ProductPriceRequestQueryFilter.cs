//************************************************************************
// 用户名				泰隆优选
// 系统名				商品价格申请单据
// 子系统名		        商品价格申请单据查询条件
// 作成者				Tom.H.Li
// 改版日				2012.4.26
// 改版内容				新建
//************************************************************************

using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM.Product.Request
{
    public class ProductPriceRequestQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 一级分类
        /// </summary>
        public int? Category1 { get; set; }

        /// <summary>
        /// 二级分类
        /// </summary>
        public int? Category2 { get; set; }

        /// <summary>
        /// 三级分类
        /// </summary>
        public int? Category3 { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        public ProductStatus? ProductStatus { get; set; }

        /// <summary>
        /// 生产商SysNO
        /// </summary>
        public int? ManufacturerSysNo { get; set; }

        /// <summary>
        /// 生产商SysNO
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public ProductPriceRequestStatus? RequestStatus { get; set; }

        /// <summary>
        /// 价格申请类型
        /// </summary>
        public int PriceApplyType { get; set; }

        /// <summary>
        /// PM权限 0:
        /// </summary>
        public int PMRole { get; set; }

        /// <summary>
        /// 比较符
        /// </summary>
        public Comparison ComparisonOperators { get; set; }

        /// <summary>
        /// 查询审核类型
        /// </summary>
        public QueryProductPriceRequestAuditType? AuditType { get; set; }

    }
}
