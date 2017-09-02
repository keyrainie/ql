using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 帮助主题
    /// </summary>
    public class HelpTopic : IIdentity, IWebChannel
    {
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
        /// 帮助主题名称
        /// </summary>
        public string Title
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
        /// 帮助主题描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 帮助内容(输入Html代码 )
        /// </summary>
        public string Content
        {
            get;
            set;
        }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keywords
        {
            get;
            set;
        }

        /// <summary>
        /// 帮助主题状态
        /// </summary>
        public ADStatus Status
        {
            get;
            set;
        }

        /// <summary>
        /// 标识类型，比如New,Hot等
        /// </summary>
        public FeatureType? Type
        {
            get;
            set;
        }

        /// <summary>
        /// 是否在分类页面显示
        /// </summary>
        public bool ShowInCategory
        {
            get;
            set;
        }

        /// <summary>
        /// 相关帮助内容系统编号
        /// </summary>
        public string RelatedSysNoList
        {
            get;
            set;
        }

        /// <summary>
        /// 帮助分类系统编号
        /// </summary>
        public int CategorySysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 帮助内容链接
        /// </summary>
        public string Link
        {
            get;
            set;
        }
    }
}