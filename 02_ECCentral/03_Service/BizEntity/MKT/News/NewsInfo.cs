using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 新闻公告
    /// </summary>
    public class NewsInfo : IIdentity, IWebChannel
    {

        /// <summary>
        /// 标题
        /// </summary>
        public LanguageContent Title
        {
            get;
            set;
        }
        
        /// <summary>
        /// 副标题
        /// </summary>
        public string Subtitle
        {
            get;
            set;
        }

        /// <summary>
        /// 正文链接
        /// </summary>
        public string LinkUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 封面图片
        /// </summary>
        public string CoverImageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 正文内容
        /// </summary>
        public LanguageContent Content
        {
            get;
            set;
        }

        /// <summary>
        /// 新闻类型
        /// </summary>
        public int? NewsType
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public int? ReferenceSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpireDate
        {
            get;
            set;
        }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool? TopMost
        {
            get;
            set;
        }

        /// <summary>
        /// 是否飘红
        /// </summary>
        public bool? IsRed
        {
            get;
            set;
        }

        /// <summary>
        /// 是否允许评论
        /// </summary>
        public bool? EnableComment
        {
            get;
            set;
        }

        /// <summary>
        /// 顾客可评论级别
        /// </summary>
        public CustomerRank? EnableReplyRank
        {
            get;
            set;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public NewsStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 扩展生效
        /// </summary>
        public bool? Extendflag
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public string ShowStatusValue
        {
            get;
            set;
        }

        /// <summary>
        /// 包含页面ID
        /// </summary>
        public string ContainPageId
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
        /// 是否首页展示
        /// </summary>
        public bool? IsHomePageShow { get; set; }
        /// <summary>
        /// 是否在一级类别上展示
        /// </summary>
        public bool? IsC1Show { get; set; }
        /// <summary>
        /// 是否在二级类别上展示
        /// </summary>
        public bool? IsC2Show { get; set; }


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
        /// 兼容数据库
        /// </summary>
        public string PageShowInheritance
        {
            get
            {
                PageTypePresentationType type = PageTypeUtil.ResolvePresentationType(ModuleType.NewsAndBulletin, NewsType.ToString());
                switch (type)
                {
                    case PageTypePresentationType.Category1:
                        return string.Format("{0}100", IsHomePageShow.HasValue && IsHomePageShow.Value ? 1 : 0);
                    case PageTypePresentationType.Category2:
                        return string.Format("{0}{1}10", IsHomePageShow.HasValue && IsHomePageShow.Value ? 1 : 0, IsC1Show.HasValue && IsC1Show.Value ? 1 : 0);
                    case PageTypePresentationType.Category3:
                        return string.Format("{0}{1}{2}1", IsHomePageShow.HasValue && IsHomePageShow.Value ? 1 : 0, IsC1Show.HasValue && IsC1Show.Value ? 1 : 0, IsC2Show.HasValue && IsC2Show.Value ? 1 : 0);
                    default:
                        break;
                }
                return string.Empty;
            }
            set
            {
                PageTypePresentationType type = PageTypeUtil.ResolvePresentationType(ModuleType.NewsAndBulletin, NewsType.ToString());
                if (type == PageTypePresentationType.Category1 && value == "1100")
                {
                    IsHomePageShow = true;
                }
                else if (type == PageTypePresentationType.Category2)
                {
                    IsHomePageShow = value[0] == '1';
                    IsC1Show = value[1] == '1';
                }
                else if (type == PageTypePresentationType.Category3)
                {
                    IsHomePageShow = value[0] == '1';
                    IsC1Show = value[1] == '1';
                    IsC2Show = value[2] == '1';
                }

            }
        }

        /// <summary>
        /// 系统编号 
        /// </summary>
        public int? Priority
        {
            get;
            set;
        }
    }
}