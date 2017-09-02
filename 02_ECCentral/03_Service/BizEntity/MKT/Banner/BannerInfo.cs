using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 广告对象
    /// </summary>
    public class BannerInfo
    {
        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 对应的渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 广告类型 ，比如Image,Flash等
        /// </summary>
        public BannerType BannerType { get; set; }

        /// <summary>
        /// 广告标题
        /// </summary>
        public string BannerTitle { get; set; }

        /// <summary>
        /// 广告文本
        /// </summary>
        public string BannerText
        {
            get;
            set;
        }

       /// <summary>
        /// 资源地址
       /// </summary>
        public string BannerResourceUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 资源地址
        /// </summary>
        public string BannerResourceUrl2
        {
            get;
            set;
        }

        /// <summary>
        /// 链接地址 
        /// </summary>
        public string BannerLink
        {
            get;
            set;
        }

        /// <summary>
        /// 广告脚本 
        /// </summary>
        public string BannerOnClick
        {
            get;
            set;
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public ADStatus? Status { get; set; }

        public int? BannerFrameSysNo { get; set; }
    }

    /// <summary>
    /// 广告模板
    /// </summary>
    public class BannerFrame
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string BannerFrameName
        {
            get;
            set;
        }

        /// <summary>
        /// 优先级
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int? PageType { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int? PositionID { get; set; }

        /// <summary>
        /// 模板内容
        /// </summary>
        public string BannerFrameText
        {
            get;
            set;
        }
        /// <summary>
        /// 模板预览
        /// </summary>
        public string BannerFrameView
        {
            get;
            set;
        }
        
    }
}
