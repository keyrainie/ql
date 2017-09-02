using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class BannerQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public string ChannelID { get; set; }

        /// <summary>
        /// 页面类型
        /// </summary>
        public int? PageType
        {
            get;
            set;
        }

        /// <summary>
        /// 页面编号
        /// </summary>
        public int? PageID
        {
            get;
            set;
        }

        /// <summary>
        ///广告尺寸系统编号
        /// </summary>
        public int? PositionID
        {
            get;
            set;
        }

        /// <summary>
        /// 广告信息状态
        /// </summary>
        public ADStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 开始时间区间从
        /// </summary>
        public DateTime? BeginDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 开始时间区间到
        /// </summary>
        public DateTime? BeginDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// 结束时间区间从
        /// </summary>
        public DateTime? EndDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 结束时间区间到
        /// </summary>
        public DateTime? EndDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// 广告标题
        /// </summary>
        public string BannerTitle
        {
            get;
            set;
        }

        public BannerType? BannerType { get; set; }

        /// <summary>
        /// 主要投放区域,用逗号分隔的区域系统编号
        /// </summary>
        public string AreaShow
        {
            get;
            set;
        }
    }
}
