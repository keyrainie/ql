using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 页面促销模板
    /// </summary>
    public class SaleAdvertisement: IIdentity, IWebChannel
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public WebChannel WebChannel
        {
            get;
            set;
        }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        /// <summary>
        /// 模板名称
        /// </summary>
        public LanguageContent Name { get; set; }

        /// <summary>
        /// 页面头部
        /// </summary>
        public string Header
        {
            get;
            set;
        }

        /// <summary>
        /// 页面底部
        /// </summary>
        public string Footer
        {
            get;
            set;
        }

        public string JumpAdvertising { get; set; }

        /// <summary>
        /// css路径
        /// </summary>
        public string CssPath
        {
            get;
            set;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public SaleAdvStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 是否分组显示
        /// </summary>
        public bool IsGroupByCategory
        {
            get;
            set;
        }

        /// <summary>
        /// 分组类型
        /// </summary>
        public GroupType? GroupType
        {
            get;
            set;
        }

        /// <summary>
        /// 是否展示评论
        /// </summary>
        public bool EnableComment
        {
            get;
            set;
        }

        /// <summary>
        /// 允许评论的会员级别
        /// </summary>
        public CustomerRank? EnableReplyRank
        {
            get;
            set;
        }

        /// <summary>
        /// 展示模式
        /// </summary>
        public ShowType? Type { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        public string IsHold { get; set; }

        /// <summary>
        /// 促销商品列表
        /// </summary>
        public List<SaleAdvertisementItem> Items { get; set; }

        /// <summary>
        /// 分组列表
        /// </summary>
        public List<SaleAdvertisementGroup> Groups { get; set; }
    }        
}