using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 广告位置
    /// </summary>
    public class BannerLocation : IIdentity, IWebChannel
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 对应的渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }

        /// <summary>
        /// 是否扩展生效
        /// </summary>
        public bool IsExtendValid { get; set; }

        /// <summary>
        /// 广告显示页面的编号
        /// </summary>
        public int? PageID { get; set; }

        /// <summary>
        ///广告的尺寸系统编号
        /// </summary>
        public int BannerDimensionSysNo { get; set; }

        /// <summary>
        /// 广告尺寸
        /// </summary>
        public BannerDimension BannerDimension { get; set; }

        /// <summary>
        /// 广告比率（%）
        /// </summary>
        public int? Ratio { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate
        {
            get;
            set;
        }

        /// <summary>
        /// 相关Tags 
        /// </summary>
        public string RelativeTags
        {
            get;
            set;
        }

        /// <summary>
        /// 优先级
        /// </summary>
        public int? Priority
        {
            get;
            set;
        }

        /// <summary>
        /// 排除PageID 
        /// </summary>
        public string ExceptPageID
        {
            get;
            set;
        }

        /// <summary>
        /// 主要投放区域
        /// </summary>
        public List<int> AreaShow
        {
            get;
            set;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public ADStatus? Status { get; set; }

        /// <summary>
        /// 广告信息系统编号
        /// </summary>
        public int BannerInfoSysNo { get; set; }
        
        /// <summary>
        /// 广告信息
        /// </summary>
        public BannerInfo Infos { get; set; }
    }
}
