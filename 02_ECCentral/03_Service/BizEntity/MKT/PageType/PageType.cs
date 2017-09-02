using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT.PageType
{
    /// <summary>
    /// 页面类型
    /// </summary>
    public class PageType:IIdentity,IWebChannel
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }
        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 渠道编号
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }

        /// <summary>
        /// 页面类型编号
        /// </summary>
        public int PageTypeID { get; set; }

        /// <summary>
        /// 页面类型名称
        /// </summary>
        public string PageTypeName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ADStatus Status { get; set; }

        public string InUser { get; set; }

        public string EditUser { get; set; }
    }
}
