using Nesoft.ECWeb.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.MemberService
{
    public class StoreNavigationModel : EntityBase
    {
        /// <summary>
        ///系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        ///导航链接URL
        /// </summary>
        public string LinkUrl
        {
            get;
            set;
        }

        /// <summary>
        ///导航内容
        /// </summary>
        public string Content
        {
            get;
            set;
        }

        /// <summary>
        ///【枚举】状态，0=无效的，1=有效的
        /// </summary>
        public int? Status
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
    }
}