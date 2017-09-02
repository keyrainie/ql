//************************************************************************
// 用户名				泰隆优选
// 系统名				商品管理员组
// 子系统名		        商品管理员组查询条件
// 作成者				Tom.H.Li
// 改版日				2012.4.24
// 改版内容				新建
//************************************************************************

using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM.ProductManager
{
    public class ProductManagerGroupQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 商品管理员组名称
        /// </summary>
        public string PMGroupName { get; set; }

        /// <summary>
        /// 商品管理员组状态
        /// </summary>
        public PMGroupStatus? Status { get; set; }
    }
}
