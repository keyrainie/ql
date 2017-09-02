using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 促销分组信息
    /// </summary>
    public class SaleAdvertisementGroup
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 页面促销模板编号
        /// </summary>
        public int? SaleAdvSysNo { get; set; }

        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 定时开始时间
        /// </summary>
        public DateTime? ShowStartDate { get; set; }

        /// <summary>
        /// 定时结束时间
        /// </summary>
        public DateTime? ShowEndDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ADStatus? Status { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// 分组更多链接
        /// </summary>
        public string OtherGroupLink { get; set; }

        /// <summary>
        /// 定位锚ID
        /// </summary>
        public string GroupIDForAnchor { get; set; }

        /// <summary>
        /// 分组Banner
        /// </summary>
        public string GroupBannerHTML { get; set; }

        /// <summary>
        /// 组广告图片地址
        /// </summary>
        public string GroupImgResourceAddr { get; set; }

        /// <summary>
        /// 组广告图片链接
        /// </summary>
        public string GroupImgResourceLink { get; set; }

        /// <summary>
        /// 边框色
        /// </summary>
        public string BorderColor { get; set; }

        /// <summary>
        /// 标题字体色
        /// </summary>
        public string TitleForeColor { get; set; }

        /// <summary>
        /// 标题背景色
        /// </summary>
        public string TitleBackColor { get; set; }

        /// <summary>
        /// 推荐方式
        /// </summary>
        public RecommendType? RecommendType { get; set; }

        /// <summary>
        /// 所有分组适用
        /// </summary>
        public bool AllGroup { get; set; }

        /// <summary>
        /// 显示商品个数
        /// </summary>
        public int? ItemsCount { get; set; }

        public string InUser { get; set; }

        public DateTime? InDate { get; set; }

        public string EditUser { get; set; }

        public DateTime? EditDate { get; set; }
    }
}
