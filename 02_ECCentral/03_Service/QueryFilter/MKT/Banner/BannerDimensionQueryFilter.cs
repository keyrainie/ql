using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT
{
    public class BannerDimensionQueryFilter
    {
        public PagingInfo PageInfo { get; set; }
        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public string ChannelID { get; set; }

        /// <summary>
        /// 页面类别
        /// </summary>
        public int? PageTypeID { get; set; }

        /// <summary>
        /// 广告尺寸编号
        /// </summary>
        public int? PositionID
        {
            get;
            set;
        }

        /// <summary>
        /// 广告尺寸名称
        /// </summary>
        public string PositionName
        {
            get;
            set;
        }
    }

    public class BannerFrameQueryFilter
    {

        /// <summary>
        /// 页面类别
        /// </summary>
        public int? PageType { get; set; }

        /// <summary>
        /// 广告尺寸编号
        /// </summary>
        public int? PositionID
        {
            get;
            set;
        }

    }
}
