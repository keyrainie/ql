using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 产品推荐对象
    /// </summary>
    public class ProductRecommendInfo : IIdentity, IWebChannel
    {
        public ProductRecommendInfo()
        {
            Location = new ProductRecommendLocation();
        }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public Common.WebChannel WebChannel
        {
            get;
            set;
        }


        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int ProductSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID
        {
            get;
            set;
        }

        /// <summary>
        /// 显示优先级
        /// </summary>
        public int Priority
        {
            get;
            set;
        }

        /// <summary>
        /// 生效开始时间
        /// </summary>
        public DateTime? BeginDate
        {
            get;
            set;
        }

        /// <summary>
        /// 失效结束日期
        /// </summary>
        public DateTime? EndDate
        {
            get;
            set;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public ADStatus Status
        {
            get;
            set;
        }

        /// <summary>
        /// 推荐商品上要显示的小图标，比如"新品，"惊爆价"等小图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 推荐位置信息
        /// </summary>
        public ProductRecommendLocation Location { get; set; }

        /// <summary>
        /// 是否扩展生效,业务含义：
        /// 如果PageID是前台三级分类，那么需要找到其对应的后台三级分类，然后将推荐信息扩展到该后台三级分类对应的其它前台三级分类
        /// </summary>
        public bool IsExtendValid
        {
            get;
            set;
        }
    }
}