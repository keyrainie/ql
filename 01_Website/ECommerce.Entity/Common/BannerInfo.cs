using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity
{
    /// <summary>
    /// 广告信息实体类
    /// </summary>
    public class BannerInfo
    {
        /// <summary>
        /// 获取或设置系统编号
        /// </summary>
        public int SysNo { get; set; }
        /// <summary>
        /// 获取或设置广告标题
        /// </summary>
        public string BannerTitle { get; set; }
        /// <summary>
        /// 获取或设置广告文字信息
        /// </summary>
        public string BannerText { get; set; }
        /// <summary>
        /// 获取或设置广告资源文件URL
        /// </summary>
        public string BannerResourceUrl { get; set; }
        /// <summary>
        /// 获取或设置广告对应链接
        /// </summary>
        public string BannerLink { get; set; }

        /// <summary>
        /// 相关标签
        /// </summary>
        public string RelativeTags { get; set; }

        /// <summary>
        /// 获取或设置广告开始时间
        /// </summary>
        public DateTime BeginDate { get; set; }
        /// <summary>
        /// 获取或设置广告结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 获取或设置广告宽度
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 获取或设置广告高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 位置ID
        /// </summary>
        public BannerPosition PositionID { get; set; }

        /// <summary>
        /// 获取或设置广告类型
        /// </summary>
        public string BannerType { get; set; }
        /// <summary>
        /// 获取或设置广告描述
        /// </summary>
        public string Description { get; set; }
    }
}
