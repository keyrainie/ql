//************************************************************************
// 用户名				泰隆优选
// 系统名				商品管理员
// 子系统名		        商品管理员查询条件
// 作成者				Tom.H.Li
// 改版日				2012.4.24
// 改版内容				新建
//************************************************************************

using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Common;

namespace ECCentral.QueryFilter.IM
{
    public class ProductManagerQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        ///用户账户
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// PM组名
        /// </summary>
        public string PMGroupName
        {
            get;
            set;
        }

        /// <summary>
        /// PM管理员状态
        /// </summary>
        public PMStatus? Status { get; set; }

        public string PMQueryType { get; set; }

        public string CompanyCode { get; set; }

    }
}
